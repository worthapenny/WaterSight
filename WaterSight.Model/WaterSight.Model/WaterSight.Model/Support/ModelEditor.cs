using Haestad.Domain;
using Haestad.Support.Library;
using Haestad.Support.Support;
using Haestad.Support.User;
using WaterSight.Model.Domain;
using WaterSight.Model.Extensions;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Haestad.Domain.ModelingObjects;
using WaterSight.Model.Sensors;

namespace WaterSight.Model.Support;

public enum Direction
{
    E = 0,
    NNE = 22,
    NE = 45,
    ENE = 67,
    N = 90,
    NNW = 112,
    NW = 135,
    WNW = 157,
    W = 180,
    WSW = 202,
    SW = 225,
    SSW = 247,
    S = 270,
    SSE = 292,
    SE = 315,
    ESE = 337,
}

public class ModelEditor
{
    #region Constructor
    public ModelEditor(IWaterModel waterModel)
    {
        WaterModel = waterModel;
    }
    #endregion

    #region Public Methods
    public async Task<bool> CreateSCADElementsAsync(List<Sensor> sensors, double distance)
    {
        SCADAElments = WaterModel.Network.SCADAElements.Elements(ElementStateType.Active);

        foreach (var sensor in sensors)
        {
            var element = sensor.NetworkElement;

            if (element == null)
                Log.Warning($"Network element is null, without which SCADAElement can't be created.");
            else
            {
                // check if SCADA element is already there
                var scadaElementCheck = SCADAElments.Where(s =>
                    s.Input.TargetElement != null
                    && s.Input.TargetElement.Id == element.Id
                    && s.Input.TargetAttribute == sensor.TargetAttribute);

                if (scadaElementCheck.Any()) // already exists
                {
                    var se = scadaElementCheck.First();
                    Log.Information($"SCADA element {se} targeting {se.Input.TargetAttribute} already exist. SKIPPED creating a new one for {sensor}");
                }
                else // does not exists
                {
                    _ = await CreateSCADElementAsync(sensor, element, distance);

                    //if (element is IPointNodeInput)
                    //    _ = await CreateSCADElementsForPointNodeAsync(sensor, element);

                    //else if (element is IBaseLinkInput)
                    //    _ = await CreateSCADElementsForLink(sensor, element);

                    //else
                    //    Log.Warning($"Unsupported element type. {element}. Type: {element.WaterElementType}");
                }


            }
        }


        return true;
    }
    /*public void AddConnectionToMSAccess(DataSourceConnectionOptions options)
    {

        //if (!File.Exists(options.DataSourceFilePath))
        //{
        //    Log.Error($"Datasouce path is invalid. Path: {options.DataSourceFilePath}");
        //    return;
        //}

        // Add an item in the SCADASignals
        var supportElementManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource);
        var id = supportElementManager.Add();
        var supportElement = supportElementManager.Element(id);
        supportElement.Label = "Local MS Access";


        //var scadaAdapter = new ScadaAdapter(WaterModel.DomainDataSet);
        //var wrappedScadaDataSource = scadaAdapter.GetOrCreateAndOpenScadaDataSource(id, TraceMessageHandler.Instance);


        //var activeEngineName = WaterModel.DomainDataSet.DomainDataSetType().GetMainNumericalEngineTypeName();
        //var activeEngine = WaterModel.DomainDataSet.NumericalEngine(activeEngineName);
        //var scadaConnection = activeEngine.ResultDataConnection as IScadaDataConnection;
        //if (scadaConnection == null && activeEngine is ICompositeNumericalEngine)
        //{
        //    IResultDataConnection rdc = ((ICompositeNumericalEngine)activeEngine).GetActiveNumericalEngine(WaterModel.ActiveScenario.Id).ResultDataConnection;
        //    scadaConnection = (IScadaDataConnection)rdc;
        //}


        //var adapter = new ScadaSourceAdapter(scadaConnection.GetScadaDataReader(), WaterModel.DomainDataSet, id);






        //var logWriter = new SCADALogWriter(options.LogFilePath, true);
        //var connectionManager = new ScadaConnectionManager(logWriter);
        //var sourceManager = new ScadaSourceManager(logWriter, connectionManager);
        
        

        //var accessConnection = connectionManager.CreateNewAccess2007Connection(options.DataSourceFilePath, "Access2007");
        //connectionManager.Add(accessConnection);
        //var message = "";
        //var connectionOK = accessConnection.TestConnection(out message);
        //if (connectionOK != ConnectionCheckResults.ConnectionOK)
        //{
        //    Log.Error($"Test connection to {options.DataSourceFilePath} failed. Message: {message}");
        //    return;
        //}
        //if (connectionOK == ConnectionCheckResults.ConnectionOK)
        //    Log.Information($"Connection successful to the souce: {options.DataSourceFilePath}");


        //object[] tables = connectionManager.GetTables(accessConnection);
        //Log.Debug($"Number of tables found: {tables.Length} from file: {options.DataSourceFilePath}");

        //var workingTableCheck = tables.Where(t => t.ToString() == options.DataSourceTableName);
        //if (!workingTableCheck.Any())
        //{
        //    Log.Error($"Given table name {options.DataSourceTableName} could not be found in the data souce {options.DataSourceFilePath}");
        //    return;
        //}

        //var workingTable = workingTableCheck.First().ToString();
        //object[] columns = connectionManager.GetTableColumns(accessConnection, workingTable);
        //Log.Debug($"Number of columns found: {columns.Length} from table: {workingTable}");

        //var tagFieldExists = columns.Contains(options.DataSourceTableTagFieldName);
        //var dateTimeFieldExits = columns.Contains(options.DataSourceTableDateTimeFieldName);
        //var valueFieldExits = columns.Contains(options.DataSourceTableValueFieldName);

        //if (!tagFieldExists)
        //{
        //    Log.Error($"Given tag field {options.DataSourceTableTagFieldName} doesn't exits. Exiting.");
        //    return;
        //}
        //if (!dateTimeFieldExits)
        //{
        //    Log.Error($"Given datetime field {options.DataSourceTableDateTimeFieldName} doesn't exits. Exiting.");
        //    return;
        //}
        //if (!valueFieldExits)
        //{
        //    Log.Error($"Given value field {options.DataSourceTableValueFieldName} doesn't exits. Exiting.");
        //    return;
        //}

        //var dataSourceManager = new ScadaSourceManager(logWriter, connectionManager);
        //var dataSource = (IScadaDatabaseSource)dataSourceManager.CreateNewSCADASource(
        //    conn: accessConnection,
        //    tableName: workingTable,
        //    signalNameField: options.DataSourceTableTagFieldName,
        //    valueNameField: options.DataSourceTableValueFieldName,
        //    dateTimeField: options.DataSourceTableDateTimeFieldName
        //    );
        //dataSource.Name = "Access";
        //dataSource.ColumnPerSignal = false;




        //var fields = supportElement.SupportedFields();
        //foreach (var field in fields)
        //{
        //    if(field.Name == "DatabaseType_Datasource")
        //    {
        //        ((IEditField)field).SetValue(supportElement.Id, dataSource);
        //    }
        //}
        ////var field = (IEditField)supportElement.Manager.ModelingElementField("DatabaseType_Datasource");
        ////field.SetValue(supportElement.Id, dataSource);



        //var signalNames = dataSource.ReadSignalsPresentInSource();
        //Log.Information($"Number of signals found: {signalNames.Count}");


        //var mappingManager = new SignalMappingManager(logWriter, dataSourceManager);
        //foreach (var signalName in signalNames)
        //{
        //    var signal = new Signal(signalName.ToString(), dataSource);
        //    var mapping = new SignalMapping(signal.Name, dataSource, true, false, 1); // not sure what element id to provide?
        //    mappingManager.Add(mapping);
        //    mappingManager.AddSignalToHistoricalSignalList(mapping);
        //}

        //dataSource.Close();
    }*/
    public void MapSCADASignals(List<string> tags)
    {

    }

    public bool MapSCADASignal(IWaterElement element, SCADATargetAttribute attribute, string tag, int dataSoucrceId)
    {
        var success = true;
        var seCheck = element.ConnectedSCADAElements(WaterModel)
            .Where(se => se.Input.TargetElement.Id == element.Id
                && se.Input.TargetAttribute == attribute);
        if (!seCheck.Any())
        {
            Log.Error($"No connected SCADA Element found for {element} with attribute {attribute}");
            return false;
        }

        var se = seCheck.First().Input;

        // Check if SCADA signal is there already
        var dataSource = WaterModel.Components.SCADASignals(dataSoucrceId);
        var ssCheck = dataSource.Elements()
                .Where(ss => ss.SignalLabel == tag);

        if (ssCheck.Any())
            se.HistoricalSignal = ssCheck.First();
        else
        {
            var ss = dataSource.Create();
            ss.SignalLabel = tag;
            ss.Label = tag;
            se.HistoricalSignal = ss;
        }

        Log.Debug($"Mapped SCADA Signal. element = {element.IdLabel()} to = {se.HistoricalSignal.IdLabel()} for attribute = {attribute.ToString()}");
        return success;
    }
    public async Task<ISCADAElement> CreateSCADElementAsync(Sensor sensor, IWaterElement element, double distance)
    {
        try
        {
            var newSE = WaterModel.Network.SCADAElements.Create(
            label: sensor.Label,
            point: await GetLocationAsync(sensor, element, distance),
            targetElement: element,
            scadaTargetAttribute: sensor.TargetAttribute);

            return newSE;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while creating SCADA elements for {element}. Sensor: {sensor}");
            return null;
        }
    }
    public void DeleteDuplicateSCADAElements()
    {
        var scadaElements = WaterModel.Network.SCADAElements.Elements(ElementStateType.Active);

        var distinctSEIds = scadaElements
            .GroupBy(se => new { se.Label, se.Input.TargetElement.Id, se.Input.TargetAttribute })
            .Select(g => g.First())
            .Select(se => se.Id)
            .ToList();

        Log.Information($"'{distinctSEIds.Count}' of '{scadaElements.Count}' are unique.");

        var seCheck = scadaElements.Where(se => !distinctSEIds.Contains(se.Id));

        if (seCheck.Any())
        {
            foreach (var se in seCheck)
            {
                se.Delete();
                Log.Information($"{se.IdLabel()} got deleted");
            }
        }
    }
    #endregion

    #region Private Methods


    //private async Task<bool> CreateSCADElementsForPointNodeAsync(Sensor sensor, IWaterNetworkElement element)
    //{
    //    var newSE = WaterModel.Network.SCADAElements.Create(
    //        label: sensor.Label,
    //        point: await GetLocationAsync(sensor, element),
    //        targetElement: element,
    //        scadaTargetAttribute: sensor.TargetAttribute);

    //    return true;
    //}


    private async Task<GeometryPoint> GetLocationAsync(Sensor sensor, IWaterElement element, double distance)
    {
        //var point =  await Task.Run(() =>
        //{
            // Calculate Slope
            var slope = 0.0;
            if (element is IBaseLinkInput)
            {
                slope = (element as IBaseLinkInput).SlopeAngle(out _);
            }
            else if (element is ITank || element is IReservoir)
            {
                slope = 0;
            }
            else if (element is IPointNodeInput)
            {
                var connectedLinks = element.ConnectedAdjacentElements(WaterModel);
                if (connectedLinks.Any())
                {

                    var slopeFirst = (connectedLinks.First() as IBaseLinkInput).SlopeAngle(out _);
                    var slopeSecond = (connectedLinks.Last() as IBaseLinkInput).SlopeAngle(out _);
                    slope = (slopeFirst + slopeSecond) / 2;
                }
            }

            //Log.Debug($"Slope for {element} is: {slope}");
            Log.Debug($"Slope for {element.Id}: {element.Label} is: {slope} radian, {slope * 180 / Math.PI} degree");

            var location = new GeometryPoint();

            if (element is IBaseLinkInput)
                location = GetLocationForPipe(slope, sensor, element, distance);

            if (element is IPointNodeInput)
                location = GetLocationForJunctionOrHydrant(slope, sensor, element, distance);

            //else if (element is IJunction || element is IHydrant)
            //    location = GetLocationForJunctionOrHydrant(slope, sensor, element);

            //else if (element is IBaseValveInput)
            //    location = GetLocationForValves(slope, sensor, element);

            //else if (element is IPump)
            //    location = GetLocationForPump(slope, sensor, element);

            //else if (element is ITank)
            //    location = GetLocatoinForTank(slope, sensor, element);

            //else if (element is IReservoir)
            //    location = GetLocationForReservoir(slope, sensor, element);

            Log.Debug($"Location for {sensor} is: {location}");
            return location;
        //});

        //return point;
    }


    /*
     * PIPE
     *             Flow (on positive side)
     *                        |
     *           o-------------------------o  -> 
     *                        |
     *             Status (on negative side)            
     */
    private GeometryPoint GetLocationForPipe(double slope, Sensor sensor, IWaterElement element, double distance)
    {
        var direction = (int)GetDirection(sensor) * Math.PI / 180;
        var pipe = element as IPipe;
        var midPoint = MathLibrary.GetPointAtDistanceIntoPolyline(pipe.Input.GetPoints().ToArray(), pipe.Input.Length / 2, out _);
        var point = GetCoordinateAtDistanceAndAngle(midPoint, distance, direction, slope);

        return point;
    }


    /*        
     *             
     *  JUNCTION / HYDRANT    
     *            Pressure (on positive side)
     *                        |
     *           o------------O------------o  ->     
     *                        |
     *                  Concentration
     */
    private GeometryPoint GetLocationForJunctionOrHydrant(double slope, Sensor sensor, IWaterElement element, double distance)
    {
        var direction = (int)GetDirection(sensor) * Math.PI / 180;
        var node = element as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, direction, slope);

        return point;
    }

    /*           
     *  PUMP    
     *            S.P.       Flow      D.P.
     *                        |
     *           o------------P------------o  ->    
     *                        |
     *            Speed     Status      Power      
     */
    //private GeometryPoint GetLocationForPump(double slope, Sensor sensor, IWaterNetworkElement element, double distance)
    //{
    //    var direction = GetDirection(sensor.SensorType);
    //    var node = element as IPointNodeInput;
    //    var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, direction, Math.Tan(slope));

    //    return point;
    //}

    /*            
     *  VALVE    
     *            US.P.      Flow      DS.P.
     *                        |
     *           o------------V------------o  ->    
     *                        |
     *                      Status  
     */
    //private GeometryPoint GetLocationForValves(double slope, Sensor sensor, IWaterNetworkElement element, double distance)
    //{
    //    var direction = GetDirection(sensor.SensorType);
    //    var node = element as IPointNodeInput;
    //    var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, direction, Math.Tan(slope));

    //    return point;
    //}

    /*           
     *  TANK    
     *                       HGL    Level
     *                        |  
     *           o------------T------------o  ->    
     *                        |
     *                                 
     */
    //private GeometryPoint GetLocatoinForTank(double slope, Sensor sensor, IWaterNetworkElement element, double distance)
    //{
    //    var direction = GetDirection(sensor.SensorType);
    //    var node = element as IPointNodeInput;
    //    var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, direction, Math.Tan(slope));

    //    return point;
    //}


    /*            
     *  RESERVOIR             
     *                       HGL
     *                        |
     *                        R------------o  ->    
     *                        |
     *                                             
     */
    //private GeometryPoint GetLocationForReservoir(double slope, Sensor sensor, IWaterNetworkElement element, double distance)
    //{
    //    var direction = GetDirection(sensor.SensorType);
    //    var node = element as IPointNodeInput;
    //    var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, direction, Math.Tan(slope));

    //    return point;
    //}

    private Direction GetDirection(Sensor sensor)
    {
        switch (sensor.SensorType)
        {
            case SensorType.Pressure:
                var direction = Direction.N;
                if (sensor.IsDirectional && sensor.IsDirectionOutward)
                    direction = Direction.NE;
                if (sensor.IsDirectional && !sensor.IsDirectionOutward)
                    direction = Direction.NW;
                return direction;

            case SensorType.Flow:
                return Direction.N;

            case SensorType.Level:
                return Direction.NE;

            case SensorType.Power:
                return Direction.SE;

            case SensorType.Concentration:
                return Direction.S;

            case SensorType.Status:
                return Direction.S;

            case SensorType.HydraulicGrade:
                return Direction.N;

            case SensorType.PumpSpeed:
                return Direction.SW;

            case SensorType.pH:
                return Direction.E;

            default:
                return Direction.W;
        }
    }

    private GeometryPoint GetCoordinateAtDistanceAndAngle(
        GeometryPoint from,
        double distance,
        double radianAngle,
        double slopeAngle = 0.0)
    {
        return new GeometryPoint(
            x: from.X + Math.Cos(radianAngle + slopeAngle) * distance,
            y: from.Y + Math.Sin(radianAngle + slopeAngle) * distance);
    }
    #endregion

    #region Private Properties
    private IWaterModel WaterModel { get; }
    private List<ISCADAElement> SCADAElments { get; set; } = new List<ISCADAElement>();
    #endregion
}
