using CsvHelper;
using CsvHelper.Configuration;
using Haestad.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WaterSight.Model.Support.Data;
using Niraula.Extensions.Water.Sensors;
using Niraula.Extensions.Water.Support.TSD;
using Niraula.Extensions;
using Niraula.Extensions.Water;
using Niraula.Extensions.Water.Scada;
using Niraula.Extensions.Library;
using Niraula.Extensions.Water.Support;
using OpenFlows.Water.Domain.ModelingElements.Components;


namespace WaterSight.Model.Generator.Data;

public class OutputManager
{
    #region Constructor
    public OutputManager(
        IWaterModel waterModel,
        SCADADataGeneratorOptions options,
        Randomizer randomizer,
        List<Sensor> autoSearchedSensors)
    {
        WaterModel = waterModel;
        Options = options;
        Randomizer = randomizer;

        CombinedSensors = autoSearchedSensors.Select(s => new TamperedSensor(
            waterModel: waterModel,
            sensor: s,
            randomizer: randomizer,
            meterError: options.MeterError)).ToList(); ;


        ScadaConnect = new ScadaConnect(WaterModel);
        if (options.Output.TsdOutputOptions.Enable)
        {
            if (options.Output.TsdOutputOptions.OutputDir is null)
                Log.Warning("The SCADA output data path is not defined.");

            else if (!Directory.Exists(options.Output.TsdOutputOptions.OutputDir))
                Log.Warning($"Given SCADA data output directory is not valid. Path: {options.Output.TsdOutputOptions.OutputDir}");
        }


        if (options.Output.BillingDataOutputOptions.Enable)
        {
            if (options.Output.BillingDataOutputOptions.OutputDir is null)
                Log.Warning("The Billing data path is not defined.");

            else if (!Directory.Exists(options.Output.BillingDataOutputOptions.OutputDir))
                Log.Warning($"Given billing data directory is not valid. Path: {options.Output.TsdOutputOptions.OutputDir}");
        }


    }
    #endregion

    #region Public Methods

    #region SCADA Data Generation Related
    public bool PrepareSCADADataOutputSQLiteFile()
    {
        var success = false;

        // Copy the the Empty sqlite file to output dir
        var emptySqliteCreated = CreateSQLiteOutputFileForSCADAData(
            Options.Output.TsdOutputOptions.OutputFullFilePath);

        if (!emptySqliteCreated)
        {
            Log.Fatal($"Failed to create empty sqlite file to export SCADA data");
            return success;
        }
        else
        {
            success = true;
            Log.Debug($"Empty SQLite file created. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");
        }


        // Create necessary tables
        var tablesCreated = CreateSCADADataOutputTable();
        if (tablesCreated)
        {
            Log.Debug($"Tables are created in SQLite file. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");

            var indexCreated = CreateSCADADataOutputTableIndex();
            if (indexCreated)
                Log.Debug($"Table index is created in SQLite file. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");
            else
                Log.Error($"Failed to create table index.");

            // If it failed to create index that's still OK
            success = true;
        }
        else
        {
            Log.Fatal($"Failed to create tables in SQL file. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");
            success = false;
        }

        Log.Information($"Created SQLite file. Successfully: {success}");
        LogLibrary.Separate_Dot();

        return success;
    }

    public void DeleteSCADADataSourcesFromModel()
    {
        Log.Debug($"About to delete SCADA Signal sources");

        WaterModel
            .Components
            .SCADADataSources(WaterModel)
            .ForEach(s => s.Delete());
        
        Log.Information($"  SCADA Elements are deleted.");
    }

    public void DeleteSCADAElementsFromModel()
    {
        Log.Debug($"About to delete SCADA Elements");

        WaterModel
            .Network
            .SCADAElements
            .Elements()
            .ForEach(se => se.Delete());

        Log.Information($"  SCADA Elements are deleted.");
    }


    /// <summary>
    /// Combine the auto detected sensors based on facilities
    /// with the sensors given by the user
    /// </summary>
    public void CombineSCADADataRecordSensors()
    {
        Log.Information($"Auto searched sensors count: {CombinedSensors.Count}");
        Log.Information($"User selected sensors count: {Options.SensorElements.Count}");

        // if auto search sensor is in the list of user sensor node
        // then update the property of the auto sensors
        foreach (var userSensor in Options.SensorElements)
        {
            var dynamicUserSensorCheck = CombinedSensors
                .Where(s => s.NetworkElement.Id == userSensor.Id
                    && s.TargetAttribute == userSensor.TargetAttribute);

            TamperedSensor sensor;
            // match found [sensor selected by user exists in the autoFound serns],
            // update the properties
            if (dynamicUserSensorCheck.Any())
            {
                sensor = dynamicUserSensorCheck.First();
                sensor.OutagePercentChance = userSensor.OutagePercentChance;
                sensor.OutageDurationHours = userSensor.OutageDurationHours;
                sensor.OutageDurationPercentVariability = userSensor.OutageDurationPercentVariability;
                sensor.Randomizer = Randomizer;
                sensor.MeterError = Options.MeterError;
            }

            // user listed sensors which are not in the CombinedSensors
            // add those
            else
            {
                sensor = new TamperedSensor(
                    waterModel: WaterModel,
                    sensor: new Sensor(
                        waterModel: WaterModel,
                        sensorType: GetSensorType(userSensor.TargetAttribute),
                        networkElement: (IWaterElement)WaterModel.Element(userSensor.Id),
                        originElement: (IWaterElement)WaterModel.Element(userSensor.Id),
                        targetAttribute: userSensor.TargetAttribute
                        ),
                    randomizer: Randomizer,
                    meterError: new MeterError());

                Log.Verbose($"Given element node {userSensor.Id} of {userSensor.TargetAttribute.ToString()} type is added to the sensor list. Sensor: {sensor}");
                CombinedSensors.Add(sensor);
            }

        }


        Log.Information($"After combining the auto and user selected sensors, total count is: {CombinedSensors.Count}.");
    }

    public void AddUnitsToTagName()
    {
        // update the tag based on user options
        if (Options.Output.TsdOutputOptions.TagContainsUnit)
        {
            foreach (var sensor in CombinedSensors)
            {
                if (!string.IsNullOrEmpty(sensor.Unit))
                    sensor.AppendToTag($"_{sensor.Unit}");
            }

            Log.Information($"Tags are appended with unit");
        }
        Log.Information($"Based on user selection, tags remain the same, no unit is appended.");
    }

    public bool WriteSensorsToFile<T>(List<T> sensors, string jsonFilePath) where T : TamperedSensor
    {
        bool success = false;
        try
        {
            File.WriteAllText(
                path: jsonFilePath,
                contents: JsonConvert.SerializeObject(sensors, Formatting.Indented));

            Log.Information($"Wrote '{sensors.Count}' sensors data to JSON format, path: {jsonFilePath}");
            success = true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while writing the json content to a file, path: {jsonFilePath}");
#if DEBUG
            Debugger.Break();
#endif
            throw ex;
        }

        return success;
    }

    #region Read / Write TSDs
    public List<TimeSeriesData> ReadTimeSeriesDataFromSQLiteFile()
    {
        throw new NotImplementedException();
    }

    public List<TimeSeriesData> ReadTimeSeriesDataFromCsvFile()
    {
        Log.Debug($"About to read TimeSeriesData from a CSV file...");
        var timer = Stopwatch.StartNew();

        var tsds = new List<TimeSeriesData>();

        try
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var reader = new StreamReader(Options.Output.TsdOutputOptions.OutputFullFilePath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                tsds = csv.GetRecords<TimeSeriesData>().ToList();
            }

        }
        catch (Exception ex)
        {
            Debugger.Break();
            Log.Error(ex, $"...while reading the TSDs from a CSV file. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");
        }

        Log.Information($"Done reading '{tsds.Count}' rows of TimeSeriesData, Time taken: {timer.Elapsed}. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");

        LogLibrary.Separate_Dot();
        return tsds;
    }

    public bool WriteTimeSeriesDataToCSV(List<TimeSeriesData> tsds, bool hasHeader)
    {
        var success = false;
        var timer = Stopwatch.StartNew();

        try
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeader,
            };

            using (var writer = new StreamWriter(Options.Output.TsdOutputOptions.OutputFullFilePath, true, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(tsds);
            }
            success = true;

            Log.Information($"Wrote {tsds.Count} rows of SCADA data to output file, time taken: {timer.Elapsed}. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");

        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Log.Error(ex, $"...while exporting the results to CSV file");
            success = false;
        }

        LogLibrary.Separate_Dot();
        return success;
    }

    public bool WriteTimeSeriesDataToSQLite(List<TimeSeriesData> tsds)
    {
        var success = false;
        var timer = Stopwatch.StartNew();
        var sb = new StringBuilder();

        try
        {
            if (tsds != null &&
                tsds.Count > 0)
            {
                Log.Debug($"About to write SCADA data to output file. Rows: {tsds.Count}");

                var scadaOptions = Options.Output.TsdOutputOptions;
                using (var conn = new SQLiteConnection(Options.Output.TsdOutputOptions.OutputFullFilePath))
                {
                    // Note: SQLite file has a limit to 500 INSERT at a time
                    // There seems to some workaround using UNION/UNIONALL.
                    // However breaking into small chunk is fast enough
                    var tsdsChunks = tsds.ChunkBy(499);
                    foreach (var tsdsChunk in tsdsChunks)
                    {
                        try
                        {
                            // build the sql query
                            var insertCmd = @$"INSERT INTO {scadaOptions.TableName} 
                                        ({scadaOptions.TsdTable.TimestampColumnName},
                                         {scadaOptions.TsdTable.TagColumnName},                                         
                                         {scadaOptions.TsdTable.ValueColumnName}) 
                                        VALUES ";

                            sb = new StringBuilder(insertCmd);


                            foreach (var tsd in tsdsChunk)
                                sb.AppendLine($"({tsd.ToCSV()}),");

                            // replace the command
                            sb.Length = sb.Length - 3; // (\n \r ,)
                            sb.Append(";");

                            conn.Open();
                            using (var transaction = conn.BeginTransaction())
                            {
                                // run the query
                                var cmd = new SQLiteCommand(sb.ToString(), conn);
                                cmd.ExecuteNonQuery();

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debugger.Break();
                            Log.Error(ex, $"...while exporting the results to SQLite file");
                            success = false;
                        }
                    }
                    success = true;
                }

                Log.Information($"Wrote {tsds.Count} rows of SCADA data to output file, time taken: {timer.Elapsed}. Path: {Options.Output.TsdOutputOptions.OutputFullFilePath}");
            }
            else
            {
                Log.Warning($"SCADA data is null or empty");
            }

            timer.Stop();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Log.Error(ex, $"...while exporting the results to SQLite file");
            success = false;
        }

        LogLibrary.Separate_Dot();
        return success;
    }


    #endregion


    /// <summary>
    /// In the SCADASignals Dialog (of WTRG)
    /// Add DataSource (link to the generated SQLite file)
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool AddGeneratedDataSourceToSCADASignals(out IElement element)
    {
        var success = false;
        var dataSourceLabel = $"{Options.Output.TsdOutputOptions.OutputFileType.ToString()}DataSource";
        element = null;

        if (Options.Output.TsdOutputOptions.AddGeneratedDataSourceToSCADASignals)
        {
            success = ScadaConnect.ConnectToNewSQLiteDataSource(dataSourceLabel, Options.Output.TsdOutputOptions, out element);
            success = success & element.Id > 0;
        }

        LogLibrary.Separate_Dot();
        return success;
    }

    public List<ISCADAElement> CreateSCADAElements<T>(IElement dataSource, List<T> sensors) where T : Sensor
    {
        var elements = new List<ISCADAElement>();
        if (Options.Output.TsdOutputOptions.CreateSCADAElements)
        {
            elements = ScadaConnect.CreateSCADAElements(
                 sensors: sensors,
                 dataSource: dataSource,
                 distance: Options.Output.TsdOutputOptions.SCADAElementLocationOffset);

            LogLibrary.Separate_Dot();
        }
        return elements;
    }

    #endregion


    #region Related to Billing Data
    public bool WriteMeterDataToCSVFile(List<CustomerMeterData> meterData)
    {
        var success = false;
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(CustomerMeterData.CsvHeader);
            meterData.ForEach(d => sb.AppendLine(d.ToCsv()));

            File.WriteAllText(
                path: Options.Output.BillingDataOutputOptions.OutputFullFilePath,
                contents: sb.ToString());

            Log.Information($"Wrote '{meterData.Count}' meter data to CSV format, path: {Options.Output.BillingDataOutputOptions.OutputFullFilePath}");
            success = true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"while writing the meter data to file. Path: {Options.Output.BillingDataOutputOptions.OutputFullFilePath}");
#if DEBUG
            Debugger.Break();
#endif

            throw ex;
        }

        LogLibrary.Separate_Dot();
        return success;
    }
    #endregion

    #endregion

    #region Private Methods



    #region SQLite Creation Related
    private bool CreateSQLiteOutputFileForSCADAData(string fullFilePath)
    {
        var success = false;

        var stopWatch = Stopwatch.StartNew();
        Log.Debug($"About to extract empty sqlite file...");

        var fileInfo = new FileInfo(fullFilePath);
        if (!fileInfo.Directory.Exists)
            throw new DirectoryNotFoundException(fileInfo.Directory.FullName);

        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Empty.sqlite"));

        try
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var file = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }

            Log.Information($"Empty sqlite file extracted and saved to path: {fullFilePath}");
            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"... while extracting sqlite file from resources";
            Log.Error(ex, message);
        }

        stopWatch.Stop();
        Log.Information($"Sqlite file obtained in {stopWatch.Elapsed}, path: {fullFilePath}");


        return success;
    }

    private bool CreateSCADADataOutputTable()
    {
        var success = false;
        var scadaOptions = Options.Output.TsdOutputOptions;
        var createSql = $@"CREATE TABLE IF NOT EXISTS '{scadaOptions.TableName}' 
                    ('{scadaOptions.TsdTable.TagColumnName}' TEXT,                     
                     '{scadaOptions.TsdTable.TimestampColumnName}' DATETIME,
                     '{scadaOptions.TsdTable.ValueColumnName}' REAL);";

        var indexSql = $@"CREATE INDEX IF NOT EXISTS 'IX_{scadaOptions.TableName}' ON '{scadaOptions.TableName}'
                    ('{scadaOptions.TsdTable.TagColumnName}' ASC,
	                 '{scadaOptions.TsdTable.TimestampColumnName}' DESC);";

        try
        {
            using (var conn = new SQLiteConnection(Options.Output.TsdOutputOptions.OutputFullFilePath))
            {
                conn.Open();
                var cmd = new SQLiteCommand(createSql, conn);
                cmd.ExecuteNonQuery();
            }

            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...when creating a table in sqite file. Create Query: {createSql}";
            Log.Error(ex, message);
        }

        return success;
    }
    private bool CreateSCADADataOutputTableIndex()
    {
        var success = false;
        var scadaOptions = Options.Output.TsdOutputOptions;

        var indexSql = $@"CREATE INDEX IF NOT EXISTS 'IX_{scadaOptions.TableName}' ON '{scadaOptions.TableName}'
                    ('{scadaOptions.TsdTable.TagColumnName}' ASC,
	                 '{scadaOptions.TsdTable.TimestampColumnName}' DESC);";

        try
        {
            using (var conn = new SQLiteConnection(Options.Output.TsdOutputOptions.OutputFullFilePath))
            {
                conn.Open();
                var cmd = new SQLiteCommand(indexSql, conn);
                cmd.ExecuteNonQuery();
            }

            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...when creating a table in sqite file. Create Index Query: {indexSql}";
            Log.Error(ex, message);
        }
        return success;
    }
    #endregion


    private WaterSightSensorType GetSensorType(SCADATargetAttribute targetAttribute)
    {
        switch (targetAttribute)
        {
            case SCADATargetAttribute.RelativeClosure:
                return WaterSightSensorType.Other;

            case SCADATargetAttribute.ConstituentConcentration:
                return WaterSightSensorType.Concentration;

            case SCADATargetAttribute.PressureNodeDemand:
            case SCADATargetAttribute.FCValveSetting:
            case SCADATargetAttribute.Discharge:
                return WaterSightSensorType.Flow;

            case SCADATargetAttribute.ValveStatus:
            case SCADATargetAttribute.PumpStatus:
            case SCADATargetAttribute.PipeStatus:
                return WaterSightSensorType.Status;

            case SCADATargetAttribute.TankLevel:
                return WaterSightSensorType.Level;

            case SCADATargetAttribute.Pressure:
            case SCADATargetAttribute.PressureValveSetting:
            case SCADATargetAttribute.PressureOut:
            case SCADATargetAttribute.PressureIn:
                return WaterSightSensorType.Pressure;

            case SCADATargetAttribute.HydraulicGrade:
            case SCADATargetAttribute.HydraulicGradeOut:
            case SCADATargetAttribute.HydraulicGradeIn:
                return WaterSightSensorType.HydraulicGrade;

            case SCADATargetAttribute.PumpSetting:
                return WaterSightSensorType.PumpSpeed;

            case SCADATargetAttribute.TCValveSetting:
                return WaterSightSensorType.Other;

            case SCADATargetAttribute.WirePower:
                return WaterSightSensorType.Power;

            case SCADATargetAttribute.UnAssigned:
                return WaterSightSensorType.Other;
        }

        return WaterSightSensorType.Other;
    }



    /*private bool GetExporter(object outputLocationItem, string exporterName, out SimpleTableExporter exporter)
    {
        Logger.Log($"Initializing {exporterName} Exporter");
        // Note: The outputLocationItem is the object automatically created by the XML schema "choice".
        exporter = null;

        if (outputLocationItem is UserOptionsOutputScadaOutputLocation_CSV)
        {
            exporter = new SimpleTableExporter_CSV(Logger, exporterName);
            UserOptionsOutputScadaOutputLocation_CSV outputLocation = (UserOptionsOutputScadaOutputLocation_CSV)outputLocationItem;
            ((SimpleTableExporter_CSV)exporter).SetDestination(outputLocation.filePath);
        }
        else if (outputLocationItem is UserOptionsOutputScadaOutputLocation_XLSX)
        {
            exporter = new SimpleTableExporter_XLSX(Logger, exporterName);
            UserOptionsOutputScadaOutputLocation_XLSX outputLocation = (UserOptionsOutputScadaOutputLocation_XLSX)outputLocationItem;
            ((SimpleTableExporter_XLSX)exporter).SetDestination(outputLocation.filePath);
        }
        else if (outputLocationItem is UserOptionsOutputScadaOutputLocation_SQL_Server)
        {
            exporter = new SimpleTableExporter_SQLServer(Logger, exporterName);
            UserOptionsOutputScadaOutputLocation_SQL_Server outputLocation = (UserOptionsOutputScadaOutputLocation_SQL_Server)outputLocationItem;
            ((SimpleTableExporter_SQLServer)exporter).SetDestination(outputLocation.datasource, outputLocation.database, outputLocation.userId, outputLocation.password);
        }
        else if (outputLocationItem is UserOptionsOutputScadaOutputLocation_Access)
        {
            exporter = new SimpleTableExporter_Access(Logger, exporterName);
            UserOptionsOutputScadaOutputLocation_Access outputLocation = (UserOptionsOutputScadaOutputLocation_Access)outputLocationItem;
            ((SimpleTableExporter_Access)exporter).SetDestination(outputLocation.filePath);
        }
        else if (outputLocationItem is UserOptionsOutputScadaOutputLocation_Sqlite)
        {
            exporter = new SimpleTableExporter_Sqlite(Logger, exporterName);
            UserOptionsOutputScadaOutputLocation_Sqlite outputLocation = (UserOptionsOutputScadaOutputLocation_Sqlite)outputLocationItem;
            if (!((SimpleTableExporter_Sqlite)exporter).SetDestination(outputLocation.filePath))
                return false;
        }
        else
        {
            Logger.Log($"Export type defined in User Options for {exporterName} Exporter is not yet supported.");
            return false;
        }

        if (!exporter.DestinationConnectionIsValid)
        {
            Logger.Log($"Destination connection for {exporterName} Exporter is not valid.");
            return false;
        }

        return true;
    }
    private bool AddColumnsToTable(DataTable table, Dictionary<string, Type> columns)
    {
        foreach (string columnName in columns.Keys)
        {
            try
            {
                table.Columns.Add(columnName, columns[columnName]);
            }
            catch (DuplicateNameException e)
            {
                Logger.LogException($"Duplicate column names detected for table {table.TableName}: {columnName}.", e);
                return false;
            }
        }
        return true;
    }

    private bool ExportTable(SimpleTableExporter exporter, DataTable table, UserOptionsOutputScadaExistingRecordStrategy userOptionsStrategy)
    {
        Logger.Log($"Exporting {exporter.ExporterName} Data.");

        SimpleTableExporterStrategy strategy;
        if (userOptionsStrategy == UserOptionsOutputScadaExistingRecordStrategy.AppendToExistingRecords)
            strategy = SimpleTableExporterStrategy.AppendToExistingRecords;
        else if (userOptionsStrategy == UserOptionsOutputScadaExistingRecordStrategy.DropAllExistingRecords)
            strategy = SimpleTableExporterStrategy.DropAllExistingRecords;
        else
            throw new NotImplementedException();

        if (!exporter.ExportTable(table, strategy))
            return false;

        return true;
    }*/
    #endregion

    #region Public Properties
    public List<TamperedSensor> CombinedSensors { get; set; } = new List<TamperedSensor>();
    public SCADADataGeneratorOptions Options { get; }


    //public DataTable ScadaResults { get; set; }
    //public List<TsdTable> SCADAData { get; set; } = new List<TsdTable>();

    public DataTable CustomerBillResults { get; set; }
    #endregion

    #region Private Properties
    private IWaterModel WaterModel { get; }
    private Randomizer Randomizer { get; }
    private ScadaConnect ScadaConnect { get; }

    /*    private UserOptions UserOptions { get; }
        private SimpleTableExporter ScadaExporter { get; set; }
        private SimpleTableExporter CustomerBillExporter { get; set; }*/
    #endregion
}
