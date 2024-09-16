//using Haestad.Domain;
//using Haestad.SCADA.Domain;
//using Haestad.SCADA.Domain.Application;
//using Haestad.SCADA.Domain.Support;
//using OpenFlows.Domain.ModelingElements;
//using OpenFlows.Water.Domain;
//using OpenFlows.Water.Domain.ModelingElements.Components;
//using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using WaterSight.Model.Extensions;
//using WaterSight.Model.Generator;
//using WaterSight.Model.Sensors;

//namespace WaterSight.Model.Domain.Scada;

//public class ScadaConnect
//{
//    #region Constructor
//    public ScadaConnect(IWaterModel waterModel)
//    {
//        WaterModel = waterModel;
//    }
//    #endregion

//    #region Public Methods
//    public bool ConnectToNewSQLiteDataSource(string dataSourceLabel, TsdOutputOptions options, out IElement element)
//    {
//        if (options.OutputFileType != SupportedFileType.SQLite)
//            throw new ArgumentException($"Unsupported external data source format '{options.OutputFileType.ToString()}'.");

//        if (!File.Exists(options.OutputFullFilePath))
//            throw new FileNotFoundException(options.OutputFullFilePath);

//        Log.Debug($"About to add connection to SCADA data source, path: {options.OutputFullFilePath}");


//        var success = false;
//        var timer = Stopwatch.StartNew();
//        element = null;

//        try
//        {
//            var sqliteFilePath = options.OutputFullFilePath;
//            var tableName = options.TableName;
//            var tagColName = options.TsdTable.TagColumnName;
//            var valueColName = options.TsdTable.ValueColumnName;
//            var dateTimeColName = options.TsdTable.TimestampColumnName;

//            // Create the DataSource item
//            var dataSource = WaterModel.Components.SCADADataSource(WaterModel);
//            dataSource.Add(dataSourceLabel);
//            element = dataSource;

//            // Set the DataSource properties
//            dataSource.DataSourceType = DatabaseDataSourceType.OdbcConnection;
//            dataSource.UseConnectionString = true;
//            dataSource.ConnectionString = @$"DRIVER=SQLite3 ODBC Driver;Database={sqliteFilePath};";
//            dataSource.QueryDateTimeDelimiter = "'";
//            dataSource.DataFilePath = sqliteFilePath; // Set this at the last

//            dataSource.TableName = tableName;
//            dataSource.SourceFormat = DatabaseDataSourceFormat.OneValuePerRow;
//            dataSource.SignalColumnName = tagColName;
//            dataSource.ValueColumnName = valueColName;
//            dataSource.TimestampColumnName = dateTimeColName;
//            dataSource.QuestionableColumnName = string.Empty;
//            dataSource.IsHistorical = true;
//            dataSource.TimeToleranceBackwardInMinutes = 15;
//            dataSource.TimeToleranceForwardInMinutes = 15;
//            dataSource.IsZeroGoodQuality = false;


//            dataSource.UseCustomizedSQL = true;
//            dataSource.AvailableSignalsSQLQuery = @$"SELECT DISTINCT [{tagColName}] FROM [{tableName}]";
//            dataSource.SignalDataSQLQueryOVPR = @$"SELECT [{tagColName}],[{valueColName}],[{dateTimeColName}] FROM [{tableName}] WHERE [{tagColName}] IN (@requestedsignals) AND ([{dateTimeColName}]>=@startdatetime(""yyyy-MM-dd HH:mm:ss"")) AND ([{dateTimeColName}]<=@enddatetime(""yyyy-MM-dd HH:mm:ss""))";
//            dataSource.DateTimeRangeSQLQuery = $@"SELECT Min([{dateTimeColName}]),Max([{dateTimeColName}]) FROM [{tableName}]";

//            if (options.TransformPumpRawData)
//            {
//                dataSource.PumpTransformType = TransformType.SubstituteTransform;
//                dataSource.PumpOnOperator = RelationalOperator.Equal;
//                dataSource.PumpOnRawSignalValue = options.PumpOnValue;

//                Log.Information($"Pump Transformation applied. Type: '{dataSource.PumpTransformType}', Operator: '{dataSource.PumpOnOperator}', Value: '{dataSource.PumpOnRawSignalValue}'");
//            }
//            else
//                Log.Debug($"Skipped pump data transformation (as per the user options)");

//            if (options.TransformValveRawData)
//            {
//                dataSource.ValveTransformType = TransformType.SubstituteTransform;
//                dataSource.ValveActiveOperator = RelationalOperator.Equal;
//                dataSource.ValveActiveValue = options.ValveActiveValue;
//                dataSource.ValveClosedOperator = RelationalOperator.Equal;
//                dataSource.ValveClosedValue = options.ValveClosedValue;

//                Log.Information($"Valve Transformation applied: Type: '{dataSource.ValveTransformType}', A. Operator: '{dataSource.ValveActiveOperator}', A. Value: '{dataSource.ValveActiveValue}', C. Operator: '{dataSource.ValveClosedOperator}', C. Value: '{dataSource.ValveClosedValue}'");
//            }
//            else
//                Log.Debug($"Skipped valve data transformation (as per the user options)");


//            // Required to save the Transformation related changes
//            dataSource.CommitChanges();
//            Log.Debug($"Modification to data source {dataSource.IdLabel} is completed");
//            Log.Debug(new string('-', 100));


//            // Test Connection
//            success = dataSource.TestConnection() == TestConnectionResult.ConnectionOK;
//            if (success)
//            {
//                Log.Information($"Connection to data source is successful.");
//                var signalCount = dataSource.LoadSCADATags();
//                Log.Information($"'{signalCount}' tags are loaded from external source");
//            }

           

//            Log.Information($"New Data Source {dataSource.IdLabel} is added. Time taken: {timer.Elapsed}. Path: {options.OutputFullFilePath}");
//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex, $"...while connecting to external data source: Path: {options.OutputFullFilePath}");
//        }

//        return success;
//    }


//    public List<ISCADAElement> CreateSCADAElements<T>(List<T> sensors, string dataSourceLabel, double distance = 10) where T : Sensor
//    {
//        Log.Debug($"About to create SCADAElements");
//        var timer = Stopwatch.StartNew();

//        var scadaElements = new List<ISCADAElement>();
//        var counter = 0;
//        foreach (Sensor sensor in sensors)
//        {
//            var existingSEs = sensor.NetworkElement.ConnectedSCADAElements(WaterModel);
//            var existingSEsCheck = existingSEs.Where(se =>
//                se.Input.HistoricalSignal?.SignalLabel == sensor.TagName
//                && se.Input.TargetAttribute == sensor.TargetAttribute);

//            if (!existingSEsCheck.Any())
//            {
//                var se = sensor.CreateSCADAElement(dataSourceLabel, distance);
                
//                scadaElements.Add(se);
//                counter++;
//            }
//            else
//            {
//                Log.Information($"Skipped creating SCADAElements for {sensor} as there is already one [tag and target attribute are the same]");
//            }
//        }

//        Log.Information($"Created {counter}/[{sensors.Count}] SCADAElements. Time taken: {timer.Elapsed}");
//        timer.Stop();

//        return scadaElements;
//    }
//    public Dictionary<IBaseValveInput, bool> CreateValveStatusDerivedSignal(string scadaDataSourceLabel, List<IBaseValveInput> valves, string valveActiveValue, string valveClosedValue)
//    {
//        var createdSignals = new Dictionary<IBaseValveInput, bool>();

//        // Make sure there are data sources
//        var scadaDataSources = WaterModel.Components.SCADADataSources(WaterModel);
//        if (!scadaDataSources.Any())
//        {
//            Log.Information($"No SCADA Data source found");
//            return createdSignals;
//        }

//        // Make sure given data source exists
//        var scadaSourceTest = scadaDataSources.Where(s => s.Label == scadaDataSourceLabel);
//        if (!scadaSourceTest.Any())
//        {
//            Log.Warning($"No SCADA data source found under '{scadaDataSourceLabel}' name. Names are: {string.Join(", ", scadaDataSources.Select(s => s.IdLabel))}");
//            return createdSignals;
//        }

//        var scadaDataSource = scadaSourceTest.First();
//        var scadaSignals = WaterModel.Components.SCADASignals(scadaDataSource.Id);

//        foreach (var valve in valves)
//        {
//            var valveElement = valve as IWaterElement;
//            // Make sure pump has SCADA Elements connected to it
//            var vavleScadaElements = valveElement.ConnectedSCADAElements(WaterModel);
//            if (!vavleScadaElements.Any())
//            {
//                Log.Warning($"No SCADA Element is connected to give '{valveElement.IdLabel}' valve.");
//                createdSignals.Add(valve, false);
//                continue;
//            }

//            // Make sure vavle has status type mapped
//            var valveStatusScadaElementCheck = vavleScadaElements.Where(s => s.Input.TargetAttribute == SCADATargetAttribute.ValveStatus);
//            if (!valveStatusScadaElementCheck.Any())
//            {
//                Log.Warning($"Given valve '{valveElement.IdLabel}' has no Status signal mapped.");
//                createdSignals.Add(valve, false);
//                continue;
//            }

//            // Make sure the historical tag is mapped/filled in
//            var valveStatusSignal = valveStatusScadaElementCheck.First().Input.HistoricalSignal;
//            if (valveStatusSignal == null)
//            {
//                Log.Warning($"Given SCADA Element '{valveStatusSignal.IdLabel}' that is mapped to valve status of valve'{valveElement.IdLabel}' is invalid/null.");
//                createdSignals.Add(valve, false);
//                continue;
//            }

//            // If the mapped signal is of derived type, skip (only create derived signal on raw signal)
//            if (valveStatusSignal.IsDerived)
//            {
//                Log.Information($"Given status signal is already a derived signal, hence skipped creating derived signal based on a derived signal");
//                createdSignals.Add(valve, false);
//                continue;
//            }


//            // Create derived Signal
//            var formula = $"iif(<{valveStatusSignal.SignalLabel}> = {valveActiveValue}, 0, iif(<{valveStatusSignal.SignalLabel}> = {valveClosedValue}, 2, 1))";
//            var derivedSignal = scadaSignals.CreateFormulaSignal(
//                    label: $"D. {valveElement.Label} STS",
//                    signalLabel: valveStatusSignal.SignalLabel,
//                    formula: formula);
//            Log.Debug($"Derived signal '{derivedSignal.IdLabel}' crated with folumla = '{formula}'");

//            // Assign the signal to the SCADAElement
//            valveStatusScadaElementCheck.First().Input.HistoricalSignal = derivedSignal;
//            Log.Information($"Vavlve status derived signal '{derivedSignal.IdLabel}' created on valve '{valveElement.IdLabel}' mapped to SCADA Element '{valveStatusScadaElementCheck.First().IdLabel}'. Formula = '{formula}'");

//            createdSignals.Add(valve, true);
//        }


//        return createdSignals;
//    }
//    public Dictionary<IPump, bool> CreatePumpStatusDerivedSignal(string scadaDataSourceLabel, List<IPump> pumps, string pumpOnValue)
//    {
//        var createdSignals = new Dictionary<IPump, bool>();

//        // Make sure there are data sources
//        var scadaDataSources = WaterModel.Components.SCADADataSources(WaterModel);
//        if (!scadaDataSources.Any())
//        {
//            Log.Information($"No SCADA Data source found");
//            return createdSignals;
//        }

//        // Make sure given data source exists
//        var scadaSourceTest = scadaDataSources.Where(s => s.Label == scadaDataSourceLabel);
//        if (!scadaSourceTest.Any())
//        {
//            Log.Warning($"No SCADA data source found under '{scadaDataSourceLabel}' name. Names are: {string.Join(", ", scadaDataSources.Select(s => s.IdLabel))}");
//            return createdSignals;
//        }

//        var scadaDataSource = scadaSourceTest.First();
//        var scadaSignals = WaterModel.Components.SCADASignals(scadaDataSource.Id);


//        foreach (var pump in pumps)
//        {
//            // Make sure pump has SCADA Elements connected to it
//            var pumpScadaElements = pump.ConnectedSCADAElements(WaterModel);
//            if (!pumpScadaElements.Any())
//            {
//                Log.Warning($"No SCADA Element is connected to give '{pump.IdLabel}' pump.");
//                createdSignals.Add(pump, false);
//                continue;
//            }

//            // Make sure pump has status type mapped
//            var pumpStatusScadaElementCheck = pumpScadaElements.Where(s => s.Input.TargetAttribute == SCADATargetAttribute.PumpStatus);
//            if (!pumpStatusScadaElementCheck.Any())
//            {
//                Log.Warning($"Given pump '{pump.IdLabel}' has no Status signal mapped.");
//                createdSignals.Add(pump, false);
//                continue;
//            }

//            // Make sure the historical tag is mapped/filled in
//            var pumpStatusSignal = pumpStatusScadaElementCheck.First().Input.HistoricalSignal;
//            if (pumpStatusSignal == null)
//            {
//                Log.Warning($"Given SCADA Element '{pumpStatusSignal.IdLabel}' that is mapped to pump status of pump '{pump.IdLabel}' is invalid/null.");
//                createdSignals.Add(pump, false);
//                continue;
//            }


//            // If the mapped signal is of derived type, skip (only create derived signal on raw signal)
//            if (pumpStatusSignal.IsDerived)
//            {
//                Log.Information($"Given status signal is already a derived signal, hence skipped creating derived signal based on a derived signal");
//                createdSignals.Add(pump, false);
//                continue;
//            }

//            // Create derived Signal
//            var formula = $"iif(<{pumpStatusSignal.SignalLabel}> = {pumpOnValue},0,1)";
//            var derivedSignal = scadaSignals.CreateFormulaSignal(
//                    label: $"D. {pump.Label} STS",
//                    signalLabel: pumpStatusSignal.SignalLabel,
//                    formula: formula);
//            Log.Debug($"Derived signal '{derivedSignal.IdLabel}' crated with folumla = '{formula}'");

//            // Assign the signal to the SCADAElement
//            pumpStatusScadaElementCheck.First().Input.HistoricalSignal = derivedSignal;
//            Log.Information($"Pump status derived signal '{derivedSignal.IdLabel}' created on pump '{pump.IdLabel}' mapped to SCADA Element '{pumpStatusScadaElementCheck.First().IdLabel}'. Formula = '{formula}'");

//            createdSignals.Add(pump, true);
//        }

//        return createdSignals;
//    }
//    #endregion

//    #region Public Properties
//    #endregion

//    #region Private Properties
//    private IWaterModel WaterModel { get; }
//    #endregion

//}