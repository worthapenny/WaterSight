using Haestad.Domain.ModelingObjects.Water;
using Haestad.Support.Library;
using Haestad.Support.Support;
using Newtonsoft.Json;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WaterSight.Model.Domain.Scada.Support;
using WaterSight.Model.Extensions;
using WaterSight.Model.Generator;
using WaterSight.Model.Generator.Data;
using WaterSight.Model.Support.Data;

namespace WaterSight.Model.Sensors;

#region Enum
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

public enum SensorType
{
    Pressure,
    Flow,
    Level,
    Power,
    Concentration,
    Status,
    HydraulicGrade,
    PumpSpeed,
    pH,
    Other,
}

#endregion

[DebuggerDisplay("{ToString()}")]
public class TamperedSensor : Sensor
{
    #region Constructor
    public TamperedSensor()
    {
    }
    public TamperedSensor(IWaterModel waterModel, Sensor sensor, Randomizer randomizer, MeterError meterError)
        : this(waterModel, sensor, randomizer, meterError, 0, 0, 0)
    {
    }
    public TamperedSensor(IWaterModel waterModel, Sensor sensor, Randomizer randomizer, MeterError meterError, double outagePercentChance, double outageDurationHours, double outageDurationPercentVariability)
        : base(waterModel, sensor.SensorType, sensor.NetworkElement, sensor.OriginElement, sensor.TargetAttribute)
    {
        Randomizer = randomizer;
        MeterError = meterError;
        OutagePercentChance = outagePercentChance;
        OutageDurationHours = outageDurationHours;
        OutageDurationPercentVariability = outageDurationPercentVariability;

        Label = sensor.Label;
        TagName = DropIllegalCharacters(sensor.TagName);
        Unit = sensor.Unit;
        IsDirectional = sensor.IsDirectional;
        IsDirectionOutward = sensor.IsDirectionOutward;

    }
    #endregion

    #region Public Methods
    public List<TimeSeriesData> TamperedResults(double timeStepHours)
    {
        var outage = false;
        var duration = 0.0;
        var maxDuration = 0.0;
        var tsdValues = new List<TimeSeriesData>();

        foreach (var result in Results())
        {
            if (!outage)
            {
                result.Value = GetRandomizedResult(result.Value);
                tsdValues.Add(result);

                if (Randomizer.Random.NextDouble() < (OutagePercentChance / 100))
                {
                    outage = true;
                    duration = 0;
                    maxDuration = OutageDurationHours * Randomizer.RandomBetween(100 - OutageDurationPercentVariability, 100 + OutageDurationPercentVariability) / 100;
                }
            }
            else
            {
                duration += timeStepHours;
                if (duration > maxDuration)
                    outage = false;
            }
        }

        return tsdValues;
    }

    #endregion

    #region Private Methods
    private double? GetRandomizedResult(double? value)
    {
        // TODO <sjac>: Round values to specified precision.
        // TODO <sjac>: Introduce meter error as user-defined percentage.
        // TODO <sjac>: Add user option for randomly shifting timesteps +/- some number of seconds.
        var percentError = GetPercentError();
        return value + Randomizer.RandomBetween(-percentError, percentError, true) * (double)value;
    }

    private double GetPercentError()
    {
        return TargetAttribute switch
        {
            SCADATargetAttribute.TankLevel
                or SCADATargetAttribute.HydraulicGrade
                or SCADATargetAttribute.HydraulicGradeOut
                or SCADATargetAttribute.HydraulicGradeIn
                    => MeterError.LevelMeterErrorPercent / 100,

            SCADATargetAttribute.Pressure
                or SCADATargetAttribute.PressureNodeDemand
                or SCADATargetAttribute.PressureValveSetting
                or SCADATargetAttribute.PressureOut
                or SCADATargetAttribute.PressureIn
                    => MeterError.PressureMeterErrorPercent / 100,

            SCADATargetAttribute.Discharge => MeterError.FlowMeterErrorPercent / 100,

            SCADATargetAttribute.WirePower => MeterError.PowerMeterErrorPercent / 100,

            _ => 0,
        };
    }
    #endregion

    #region Public Methods
    public double OutagePercentChance { get; set; } = 0;
    public double OutageDurationHours { get; set; } = 0;
    public double OutageDurationPercentVariability { get; set; } = 0;

    [JsonIgnore]
    public Randomizer Randomizer { get; set; }
    public MeterError MeterError { get; set; }
    #endregion

}


[DebuggerDisplay("{ToString()}")]
public class Sensor
{
    #region Constructor
    public Sensor() { }
    public Sensor(IWaterModel waterModel, SensorType sensorType)
        : this()
    {
        WaterModel = waterModel;
        SensorType = sensorType;
    }


    public Sensor(
        IWaterModel waterModel,
        SensorType sensorType,
        IWaterElement networkElement,
        IWaterElement originElement,
        SCADATargetAttribute targetAttribute)
        : this(waterModel, sensorType)
    {
        NetworkElement = networkElement;
        OriginElement = originElement ?? networkElement;
        TargetAttribute = targetAttribute;

        //if (networkElement is IPointNodeInput)
        //{
        //    var point = (networkElement as IPointNodeInput).GetPoint();
        //    Location = new PointF((float)point.X, (float)point.Y);
        //}
        //else if (networkElement is IBaseLinkInput)
        //{
        //    var point = (networkElement as IBaseLinkInput).MidPoint();
        //    Location = new PointF((float)point.X, (float)point.Y);
        //}

        //Location = GetSCADAElementLocation(waterModel, )

        UpdateTagName();
        UpdateLabel();

        //TagName = GetTagName();
        //Label = GetLabel();
    }

    #endregion

    #region Public Methods
    public void UpdateProperties(IWaterModel waterModel, int networkElementId, int originElementId)
    {
        WaterModel = waterModel;
        NetworkElement = (IWaterElement)waterModel.Element(networkElementId);
        OriginElement = (IWaterElement)waterModel.Element(originElementId);
        UpdateLabel();
    }
    public List<TimeSeriesData> Results()
    {
        var results = new List<TimeSeriesData>();
        var timeStepsInSeconds = WaterModel.ActiveScenario.TimeStepsInSeconds;
        var simStartDate = WaterModel.ActiveScenario.Options.SimulationStartDate;
        var timesInDateTime = timeStepsInSeconds.Select(t => simStartDate + TimeSpan.FromSeconds(t)).ToList();
        //var timesInDateTime = timeStepsInSeconds.Select(t => WaterModel.ActiveScenario.TimeStepToDateTime(t)).ToList();
        double?[] values;

        try
        {
            // Gather the model results
            switch (TargetAttribute)
            {
                case SCADATargetAttribute.RelativeClosure:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException($"{this}");

                case SCADATargetAttribute.ConstituentConcentration:
                    values = (NetworkElement as IWaterQualityResults).Concentrations();
                    break;

                case SCADATargetAttribute.PressureNodeDemand:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException($"{this}");

                case SCADATargetAttribute.ValveStatus:
                    values = (NetworkElement as IBaseValveResults).CalculatedStatuses()
                        .Select(d =>
                            d == (int)ValveSettingEnum.ValveActiveType
                                ? (double?)1
                                : d == (int)ValveSettingEnum.ValveClosedType
                                    ? (double?)0
                                    : (double?)2).ToArray();

                    break;

                case SCADATargetAttribute.PumpStatus:
                    values = (NetworkElement as IBasePumpResults).CalculatedPumpStatuses()
                        .Select(d => d == PumpStatusEnum.PumpOnType ? (double?)1 : (double?)0).ToArray();
                    break;

                case SCADATargetAttribute.PipeStatus:
                    values = (NetworkElement as IPipeResults).CalculatedStatuses()
                        .Select(d => d == (int)PipeStatusEnum.OpenType ? (double?)1 : (double?)0).ToArray();
                    break;

                case SCADATargetAttribute.TankLevel:
                    values = (NetworkElement as ITankResults).Levels();
                    break;

                case SCADATargetAttribute.Pressure:
                    // Note: Since all pressures are measured from junction
                    // it is safe to assume that the Network node is a junction
                    if (NetworkElement is IJunctionResults)
                        values = (NetworkElement as IJunctionResults).Pressures();
                    //else if (NetworkElement is IPumpResults)
                    //    values = (NetworkElement as IPumpResults).DischargePressures();
                    //else if (NetworkElement is IBaseValvesResults)
                    //    values = (NetworkElement as IBaseValvesResults).ToPressures();
                    else
                        values = null;

                    break;

                case SCADATargetAttribute.HydraulicGrade:
                    if (NetworkElement is ITank)
                        values = (NetworkElement as ITankResults).HydraulicGrades();
                    else
                        values = (NetworkElement as IReservoirResults).HydraulicGrades();
                    break;

                case SCADATargetAttribute.PumpSetting:
                    values = (NetworkElement as IPumpResults).CalculatedRelativeSpeedFactors();
                    break;

                case SCADATargetAttribute.PressureValveSetting:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException($"{this}");

                case SCADATargetAttribute.TCValveSetting:
                    values = (NetworkElement as IThrottleControlValveResults).CalculatedSettings();
                    break;

                case SCADATargetAttribute.FCValveSetting:
                    values = (NetworkElement as IFlowControlValveResults).CalculatedFlowSettings();
                    break;

                case SCADATargetAttribute.PressureOut:
                    values = (NetworkElement as IBaseValveResults).ToPressures();
                    break;

                case SCADATargetAttribute.PressureIn:
                    values = (NetworkElement as IBaseValveResults).FromPressures();
                    break;

                case SCADATargetAttribute.HydraulicGradeOut:
                    values = (NetworkElement as IBaseValveResults).ToHydraulicGrades();
                    break;

                case SCADATargetAttribute.HydraulicGradeIn:
                    values = (NetworkElement as IBaseValveResults).FromHydraulicGrades();
                    break;

                case SCADATargetAttribute.Discharge:
                    if (NetworkElement is IPipeResults)
                        values = (NetworkElement as IPipeResults).Flows();
                    else if (NetworkElement is IBaseValveResults)
                        values = (NetworkElement as IBaseValveResults).Flows();
                    else
                        values = null;

                    // Make sure the flow values are absolute/positive
                    if (values != null)
                        values = values.Select(v => Math.Abs(v.Value) as double?).ToArray();
                    break;

                case SCADATargetAttribute.WirePower:
                    values = (NetworkElement as IPumpResults).WirePowers();
                    break;

                case SCADATargetAttribute.UnAssigned:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException($"{this}");

                default:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException($"{this}");
            }

            // Create the time series data
            if (values != null)
            {
                if (values.Length != timesInDateTime.Count)
                {
                    Debugger.Break();
                    Log.Fatal($"The array size '{values.Length}' of result values and the size '{timesInDateTime.Count}' of times are not same!");
                }

                for (int i = 0; i < timesInDateTime.Count; i++)
                    results.Add(new TimeSeriesData(timesInDateTime[i], values[i], TagName));
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Log.Error(ex, $"...while getting the results for {this}");
        }

        return results;
    }
    public void UpdateTagName()
    {
        var label = DropIllegalCharacters(OriginElement.Label); 
        TagName = $"{label}_{GetTagNameSuffix()}";
    }


    public string DropIllegalCharacters(string text)
    {
        if (text == null) return text;
        return new string(text.Where(c => !_illegalTagCharacters.Contains(c)).ToArray());
    }
    public void AppendToTag(string text)
    {
        TagName += text;
    }
    public void UpdateLabel()
    {
        Label = $"{OriginElement.Label}{GetTagLabelSuffix()}";
    }
    //public void UpdateLabel()
    //{
    //    Label = GetLabel();
    //}
    public ISCADAElement CreateSCADAElement(ISCADASignal historicalSignal, double distance)
    {
        LocationOffset = distance;
        var location = GetSCADAElementLocation();
        if(location == null)
        {
            Log.Warning($"No location information found. {this}");
        }

        var se = WaterModel.Network.SCADAElements.Create();
        se.Label = Label;
        se.Input.SetPoint(location.Value);
        se.Input.TargetElement = NetworkElement;
        se.Input.TargetAttribute = TargetAttribute;
        se.Input.RealtimeSignal = null;
        se.Input.HistoricalSignal = historicalSignal;


        if (historicalSignal == null)
            Log.Warning($"Created SCADAElement with no historical signal. Label: {Label}, NetworkElement: {NetworkElement.IdLabel()}, OriginElement: {OriginElement.IdLabel()}");
        else
            Log.Verbose($"Created SE. {se.IdLabel()}");
        return se;
    }
    public ISCADAElement CreateSCADAElement(string dataSourceLabel, double distance)
    {
        ISCADASignal signal = null;
        if (string.IsNullOrEmpty(dataSourceLabel))
            return CreateSCADAElement(signal, distance);

        var dataSources = WaterModel.Components.SCADADataSources(WaterModel);
        var dataSourceCheck = dataSources.Where(s => s.Label == dataSourceLabel);

        if (dataSourceCheck.Any())
        {
            var signalCheck = WaterModel.Components
                .SCADASignals(dataSourceCheck.First().Id).Elements()
                .Where(s => s.SignalLabel == TagName);

            signal = signalCheck.Any() ? signalCheck.First() : null;

            if (signal == null)
                Log.Warning($"Given data source '{dataSourceLabel}', does not contain tag of '{TagName}'");

        }
        else
        {
            Log.Warning($"Data source not found. Given: '{dataSourceLabel}'");
        }

        return CreateSCADAElement(signal, distance);
    }
    #endregion

    #region Private Methods

    #region  Location per element type related

    private GeometryPoint? GetSCADAElementLocation()
    {
        var slopeRad = 0.0;

        if (WaterModel == null) return null;

        if (NetworkElement is IBaseLinkInput)
        {
            slopeRad = (NetworkElement as IBaseLinkInput).SlopeAngle(out _);
        }
        else if (NetworkElement is IBaseDirectedNodeInput)
        {
            var directedNode = (NetworkElement as IBaseDirectedNodeInput);
            var downstreamLink = directedNode.DownstreamLink;
            slopeRad = (downstreamLink as IBaseLinkInput).SlopeAngle(out _);

            // when the downstream-pipe's start node is not same as the directed node
            // we need to adjust the angle
            if ((directedNode as IElement).Id != (downstreamLink as IPipeInput).StartNode.Id)
                slopeRad += Math.PI;
        }
        else if (NetworkElement is ITank || NetworkElement is IReservoir)
        {
            slopeRad = 0;
        }
        else if (NetworkElement is IPointNodeInput)
        {
            var connectedLinks = NetworkElement.ConnectedAdjacentElements(WaterModel );
            if (connectedLinks.Any())
            {

                var slopeFirst = (connectedLinks.First() as IBaseLinkInput).SlopeAngle(out _);
                var slopeSecond = (connectedLinks.Last() as IBaseLinkInput).SlopeAngle(out _);
                slopeRad = (slopeFirst + slopeSecond) / 2;
            }
        }

        Log.Verbose($"Slope for {NetworkElement.IdLabel()} is: {slopeRad} radian, {slopeRad * 180 / Math.PI} degree");

        var location = new GeometryPoint();

        if (NetworkElement is IBaseLinkInput)
            location = GetLocationForPipe(slopeRad, LocationOffset);

        else if (NetworkElement is IJunction || NetworkElement is IHydrant)
            location = GetLocationForJunctionOrHydrant(slopeRad, LocationOffset);


        else if (NetworkElement is IBaseValveInput)
            location = GetLocationForValves(slopeRad, LocationOffset);

        else if (NetworkElement is IPump)
            location = GetLocationForPump(slopeRad, LocationOffset);

        else if (NetworkElement is ITank)
            location = GetLocationForTank(slopeRad, LocationOffset);

        else if (NetworkElement is IReservoir)
            location = GetLocationForReservoir(slopeRad, LocationOffset);

        return location;
    }
    /*
     * PIPE
     *             Flow (on positive side)
     *                        |
     *           o-------------------------o  -> 
     *                        |
     *             Status (on negative side)            
     */
    private GeometryPoint GetLocationForPipe(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var pipe = NetworkElement as IPipe;
        var midPoint = MathLibrary.GetPointAtDistanceIntoPolyline(pipe.Input.GetPoints().ToArray(), pipe.Input.Length / 2, out _);
        var point = GetCoordinateAtDistanceAndAngle(midPoint, distance, directionRad, slope);

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
    private GeometryPoint GetLocationForJunctionOrHydrant(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var node = NetworkElement as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, directionRad, slope);

        return point;
    }

    /*           
     *  PUMP    
     *            S.P.                 D.P.
     *                        
     *           o------------P------------o  ->    
     *                        |
     *            Speed     Status      Power      
     */
    private GeometryPoint GetLocationForPump(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var node = NetworkElement as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, directionRad, slope);

        return point;
    }

    /*            
     *  VALVE    
     *            US.P.      Flow      DS.P.
     *                        |
     *           o------------V------------o  ->    
     *                        |
     *                      Status  
     */
    private GeometryPoint GetLocationForValves(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var node = NetworkElement as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, directionRad, slope);
        Log.Verbose($"Directed Angle: {(int)GetDirection()}, Rad: {directionRad}");
        return point;
    }

    /*           
     *  TANK    
     *                       HGL    Level
     *                        |  
     *           o------------T------------o  ->    
     *                        |
     *                                 
     */
    private GeometryPoint GetLocationForTank(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var node = NetworkElement as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, directionRad, slope);

        return point;
    }


    /*            
     *  RESERVOIR             
     *                       HGL
     *                        |
     *                        R------------o  ->    
     *                        |
     *                                             
     */
    private GeometryPoint GetLocationForReservoir(double slope, double distance)
    {
        var directionRad = (int)GetDirection() * Math.PI / 180;
        var node = NetworkElement as IPointNodeInput;
        var point = GetCoordinateAtDistanceAndAngle(node.GetPoint(), distance, directionRad, slope);

        return point;
    }


    private Direction GetDirection()
    {
        switch (SensorType)
        {
            case SensorType.Pressure:
                var direction = Direction.N;
                if (IsDirectional && IsDirectionOutward)
                    direction = Direction.NE;
                if (IsDirectional && !IsDirectionOutward)
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
        Log.Verbose($"Final angle in rad: {radianAngle + slopeAngle}");

        return new GeometryPoint(
            x: from.X + Math.Cos(radianAngle + slopeAngle) * distance,
            y: from.Y + Math.Sin(radianAngle + slopeAngle) * distance);
    }


    #endregion

    private string GetSensorUnit(SensorType sensorType)
    {
        if (WaterModel == null) return string.Empty;

        switch (sensorType)
        {
            case SensorType.Pressure:
                return WaterModel.Units.NetworkUnits.Junction.PressureUnit.ShortLabel;

            case SensorType.Flow:
                return WaterModel.Units.NetworkUnits.Pipe.FlowUnit.ShortLabel;

            case SensorType.Level:
                return WaterModel.Units.NetworkUnits.Tank.LevelUnit.ShortLabel;

            case SensorType.Power:
                return WaterModel.Units.NetworkUnits.Pump.PowerUnit.ShortLabel;

            case SensorType.Concentration:
                return WaterModel.Units.NetworkUnits.Junction.ConcentrationUnit.ShortLabel;

            case SensorType.Status:
                return string.Empty;

            case SensorType.HydraulicGrade:
                return WaterModel.Units.NetworkUnits.Reservoir.HydraulicGradeUnit.ShortLabel;

            case SensorType.PumpSpeed:
                return string.Empty;

            case SensorType.pH:
                return string.Empty;

            default:
#if DEBUG
                Debugger.Break();
#endif
                return string.Empty;
        }
    }

    private string GetTagLabelSuffix(bool shortName = true)
    {
        var label = "";
        switch (SensorType)
        {
            case SensorType.Pressure:
                {
                    label = shortName ? "Psr" : "Pressure";
                    if (IsDirectional && IsDirectionOutward)
                        label = $"Out_{label}";
                    if (IsDirectional && !IsDirectionOutward)
                        label = $"In_{label}";
                }
                break;

            case SensorType.Flow:
                label = shortName ? "Flo" : "Flow";
                if (IsDirectional && IsDirectionOutward)
                    label = $"Out_{label}";
                if (IsDirectional && !IsDirectionOutward)
                    label = $"In_{label}";
                break;

            case SensorType.Level:
                label = shortName ? "Lvl" : "Level";
                break;

            case SensorType.Power:
                label = shortName ? "Pwr" : "Power";
                break;

            case SensorType.Concentration:
                label = shortName ? "Conc." : "Conc.";
                break;

            case SensorType.Status:
                label = shortName ? "Sts" : "Status";
                break;

            case SensorType.HydraulicGrade:
                label = shortName ? "Hgl" : "H. Grade";
                break;

            case SensorType.PumpSpeed:
                label = shortName ? "Spd" : "Speed";
                break;

            case SensorType.pH:
                label = shortName ? "pH" : "PH";
                break;

            default:
#if DEBUG
                Debugger.Break();
#endif
                label = shortName ? "Unk" : "Unknown";
                break;
        }

        return $"_{label}";
    }

    private string GetTagNameSuffix()
    {
        switch (SensorType)
        {
            case SensorType.Pressure:
                var psr = "PSR";
                if (IsDirectional && IsDirectionOutward)
                    psr = "OUT_PSR";
                if (IsDirectional && !IsDirectionOutward)
                    psr = "IN_PSR";
                return psr;

            case SensorType.Flow:
                var flow = "FLO";
                if (IsDirectional && IsDirectionOutward)
                    flow = "OUT_FLO";
                if (IsDirectional && !IsDirectionOutward)
                    flow = "IN_FLO";
                return flow;

            case SensorType.Level:
                return "LVL";

            case SensorType.Power:
                return "PWR";

            case SensorType.Concentration:
                return "CNC";

            case SensorType.Status:
                return "STS";

            case SensorType.HydraulicGrade:
                return "HGL";

            case SensorType.PumpSpeed:
                return "SPD";

            case SensorType.pH:
                return "PH";

            default:
#if DEBUG
                Debugger.Break();
#endif
                return "UNK";
        }
    }
    #endregion

    #region Overridden methods
    public override string ToString()
    {
        return $"[{SensorType}] {Label} Tag = {TagName}, Tgt = {TargetAttribute}, On = {NetworkElement.IdLabel()}";
    }
    #endregion

    #region Properties

    [JsonIgnore]
    public IWaterModel WaterModel { get; private set; }

    public SensorType SensorType { get; set; }
    public string TagName { get; set; }
    public GeometryPointEx Location
    {
        get => _location ??= new GeometryPointEx(GetSCADAElementLocation());
        set => _location = value;
    }
    public string Label { get; set; }
    public string Unit
    {
        get => _unit ??= GetSensorUnit(SensorType);
        set => _unit = value;
    }
    public bool IsDirectional { get; set; } = false;
    public bool IsDirectionOutward
    {
        get { return _IsDirectionOutwards; }
        set
        {
            _IsDirectionOutwards = value;
            if (WaterModel != null)
                UpdateLabel();
        }
    }
    public SCADATargetAttribute TargetAttribute { get; set; } = SCADATargetAttribute.UnAssigned;

    //public PointF Location { get; set; } = new PointF();

    public double? ReferenceElevation
    {
        get => OriginElement is IPhysicalNodeElementInput
            ? (OriginElement as IPhysicalNodeElementInput).Elevation
            : null;
        set
        {
            if (OriginElement != null && OriginElement is IPhysicalNodeElementInput)
                (OriginElement as IPhysicalNodeElementInput).Elevation = value ?? -9999;
        }
    }

    public int? NetworkElementId { get => _networkElementId ??= NetworkElement.Id; set => _networkElementId = value; }
    public int? OriginElementId { get => _originElementId ??= OriginElement.Id; set => _originElementId = value; }

    [JsonIgnore]
    public IWaterElement NetworkElement { get; private set; }

    [JsonIgnore]
    public IWaterElement OriginElement { get; private set; }

    public double LocationOffset { get; set; } = 10.0;
    #endregion

    #region Fields
    bool _IsDirectionOutwards = false;
    string _unit;
    int? _networkElementId;
    int? _originElementId;
    GeometryPointEx _location;

    string _illegalTagCharacters = "\"'#י";
    #endregion

}

