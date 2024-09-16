using Haestad.Domain;
using Haestad.Network;
using Haestad.NetworkBuilder.Water;
using Haestad.SCADA.Forms.Domain.Datasources;
using Haestad.Support.Support;
using Haestad.Support.Units;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using WaterSight.Excel.Customer;
using WaterSight.Excel.Pump;
using WaterSight.Excel.Sensor;
using WaterSight.Excel.Tank;
using WaterSight.Excel.Zone;
using WaterSight.Web.Sensors;
using WaterSight.Web.Support;

namespace WaterSight.Support;

public class SetupExcel
{
    #region Singleton Pattern
    private static readonly SetupExcel instance = new SetupExcel();

    // Explicit static constructor to tell C# compiler
    // not to mark type as before field init
    static SetupExcel() { }
    private SetupExcel() { }
    public static SetupExcel Instance => instance;
    #endregion

    #region Sensor Methods
    /// <summary>
    /// Update/Overwrite? Sensors tab in Excel based on SCADAElements in the model
    /// </summary>
    public async Task<bool> WriteSensorsAsync(
        IWaterModel waterModel,
        string sensorsExcelFilePath,
        string timeZoneId,
        int readFrequency = 5,
        int transmitFrequency = 5,
        string? udxTagsGroupsFieldName = null,
        Unit? coordinateUnit = null)
    {
        var xlSensors = new SensorsXlSheet(sensorsExcelFilePath);
        var scadaElements = waterModel
                                .Network
                                .SCADAElements
                                .Elements(ElementStateType.All)
                                .Where(se => se.Input.HistoricalSignal != null);

        // Elevation unit
        var elevationUnit = waterModel.Network.Junctions.InputFields.FieldByLabel("Elevation").Unit.ShortLabel;

        var udxTagGroupsFieldValues = new Dictionary<int, string>();
        if (udxTagsGroupsFieldName != null)
        {
            udxTagGroupsFieldValues = (Dictionary<int, string>)waterModel.Network
                .SCADAElements.InputFields.FieldByLabel(udxTagsGroupsFieldName).Field.GetValues();
        }

        // Change the coordinate/geometry unit if needed
        if (coordinateUnit != null)
            waterModel.Units.NetworkUnits.Junction.GeometryUnit.SetUnit(coordinateUnit);



        foreach (var scadaElement in scadaElements)
        {
            Log.Debug($"About to work with: {scadaElement}");
            var xlSensor = new SensorItem();
            xlSensors.SensorItemsList.Add(xlSensor);
            var seInput = scadaElement.Input;

            xlSensor.SensorTag = seInput.HistoricalSignal.TagForWaterSight();
            xlSensor.DisplayName = scadaElement.Label.StartsWith("SE-") ? scadaElement.Label.Substring(3) : scadaElement.Label;
            xlSensor.Type = GetSensorType(seInput, waterModel);

            var dataSourceId = seInput.HistoricalSignal.ScadaDatasourceID;
            var scadaSource = waterModel.Components.SCADADataSource(dataSourceId, waterModel);
            var unitMngr = scadaSource.NewDatasourceUnitManager();
            var units = unitMngr.Elements().Cast<DatasourceUnit>().ToList();
            xlSensor.Units = GetSensorUnit(seInput, waterModel, units);

            xlSensor.ReadFrequency = readFrequency;
            xlSensor.TransmitFrequency = transmitFrequency;
            xlSensor.UtcOffSet = "00:00";
            xlSensor.TimeZoneId = timeZoneId;

            var location = seInput.GetPoint();
            xlSensor.Latitude = location.Y;
            xlSensor.Longitude = location.X;

            xlSensor.ReferenceElevation = GetReferenceElevation(seInput);
            xlSensor.ReferenceElevationUnits = GetElevationUnit(elevationUnit, seInput);

            xlSensor.Priority = 1;

            xlSensor.Tags = udxTagGroupsFieldValues.Any()
                ? udxTagGroupsFieldValues[scadaElement.Id]
                : GetTagsOrGroups(waterModel, seInput.TargetElement);

            Log.Debug($"Done with: {scadaElement}");

        }

        // order by DisplayName
        xlSensors.SensorItemsList = xlSensors.SensorItemsList.OrderBy(s => s.DisplayName).ToList();

        // Write to Excel
        //var success = await xlSensors.SaveAsync(xlSensors.SensorItemsList);
        var success = true;
        xlSensors.Save(xlSensors.SensorItemsList);

        Log.Debug($"Wrote Sensors to Excel. Path: {xlSensors.FilePath}");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }

    private string GetTagsOrGroups(IWaterModel waterModel, IWaterElement element)
    {
        if (element == null) return string.Empty;

        var tagsOrGroups = new List<string>();
        var zones = new List<IZone>();

        // Add upstream/downstream zones
        if (element is IBaseDirectedNodeInput)
        {
            var connectedElements = element.GetConnectedPipes(waterModel);
            foreach (var connectedElement in connectedElements)
            {
                if (connectedElement is IWaterZoneableNetworkElementInput)
                    zones.Add(((IWaterZoneableNetworkElementInput)connectedElement).Zone);
            }
        }

        // add zone of the element
        if (element is IWaterZoneableNetworkElementInput)
        {
            var zone = (element as IWaterZoneableNetworkElementInput)?.Zone;
            if (zone != null) zones.Add(zone);
        }

        // Get unique labels from zones 
        var zoneName = zones.Unique();// .GroupBy(z => z?.Label).Select(z => z?.First()?.Label).Where(n => n != null);
        if (zoneName != null)
            tagsOrGroups.AddRange(zoneName);


        // For pumps, get pump station names
        if (element is IPumpInput)
        {
            var pumpStationId = (element as IPumpInput)?.InputFields.FieldByLabel("Pump Station").GetValue<int?>(element.Id);
            if (pumpStationId.HasValue)
                tagsOrGroups.Add(waterModel.Element(pumpStationId.Value).Label);
        }


        // add element type
        tagsOrGroups.Add(element.WaterElementType.ToString());

        return String.Join(", ", tagsOrGroups.ToArray());
    }

    private string GetElevationUnit(string elevationUnit, ISCADAElementInput seInput)
    {
        var targetElement = seInput.TargetElement;
        if (targetElement != null)
        {
            if (targetElement is IPointNodeInput)
                return elevationUnit;
        }

        return String.Empty;
    }

    private string GetSensorType(ISCADAElementInput seInput, IWaterModel waterModel)
    {
        return seInput.TargetAttribute switch
        {
            SCADATargetAttribute.RelativeClosure
                => SensorTypeName.Other,

            SCADATargetAttribute.ConstituentConcentration
                => SensorTypeName.Chlorine,

            SCADATargetAttribute.PressureNodeDemand
                => SensorTypeName.Flow,

            SCADATargetAttribute.ValveStatus
                => SensorTypeName.ValveStatus,

            SCADATargetAttribute.PumpStatus
                => SensorTypeName.PumpStatus,

            SCADATargetAttribute.PipeStatus
                => SensorTypeName.Other,

            SCADATargetAttribute.TankLevel
                => SensorTypeName.Level,

            SCADATargetAttribute.Pressure
                => SensorTypeName.Pressure,

            SCADATargetAttribute.HydraulicGrade
                => SensorTypeName.HydraulicGrade,

            SCADATargetAttribute.PumpSetting
                => SensorTypeName.Other,

            SCADATargetAttribute.PressureValveSetting
                => SensorTypeName.Pressure,

            SCADATargetAttribute.TCValveSetting
                => SensorTypeName.Other,

            SCADATargetAttribute.FCValveSetting
                => SensorTypeName.Flow,


            SCADATargetAttribute.PressureOut
                => SensorTypeName.Pressure,

            SCADATargetAttribute.PressureIn
                => SensorTypeName.Pressure,

            SCADATargetAttribute.HydraulicGradeOut
                => SensorTypeName.HydraulicGrade,

            SCADATargetAttribute.HydraulicGradeIn
                => SensorTypeName.HydraulicGrade,

            SCADATargetAttribute.Discharge
                => SensorTypeName.Flow,

            SCADATargetAttribute.WirePower
                => SensorTypeName.Power,

            SCADATargetAttribute.UnAssigned
                => SensorTypeName.Other,

            _ => SensorTypeName.Other,
        };
    }



    private string GetSensorUnit(ISCADAElementInput seInput, IWaterModel waterModel, List<DatasourceUnit> units)
    {
        Unit? unit = null;

        var outputUnit = string.Empty;
        switch (seInput.TargetAttribute)
        {
            case SCADATargetAttribute.RelativeClosure:
            case SCADATargetAttribute.ValveStatus:
            case SCADATargetAttribute.PumpStatus:
            case SCADATargetAttribute.PipeStatus:
            case SCADATargetAttribute.PumpSetting:
            case SCADATargetAttribute.TCValveSetting:
                break;

            case SCADATargetAttribute.ConstituentConcentration:
                unit = GetUnitFromKeywork(units, "concentration");
                outputUnit = unit?.ShortLabel;
                break;

            case SCADATargetAttribute.PressureNodeDemand:
                unit = GetUnitFromKeywork(units, "demand");
                outputUnit = GetCorrectedFlowUnit(unit);
                break;
            case SCADATargetAttribute.TankLevel:
            case SCADATargetAttribute.HydraulicGrade:
            case SCADATargetAttribute.HydraulicGradeOut:
            case SCADATargetAttribute.HydraulicGradeIn:
                unit = GetUnitFromKeywork(units, "level");
                outputUnit = unit?.ShortLabel;
                break;
            case SCADATargetAttribute.Pressure:
            case SCADATargetAttribute.PressureValveSetting:
            case SCADATargetAttribute.PressureOut:
            case SCADATargetAttribute.PressureIn:
                unit = GetUnitFromKeywork(units, "pressure");
                outputUnit = GetCorrectedPressureUnit(unit);
                break;
            case SCADATargetAttribute.FCValveSetting:
            case SCADATargetAttribute.Discharge:
                unit = GetUnitFromKeywork(units, "flow");
                outputUnit = GetCorrectedFlowUnit(unit);
                break;
            case SCADATargetAttribute.WirePower:
                unit = GetUnitFromKeywork(units, "power");
                outputUnit = unit.ShortLabel;
                break;

            case SCADATargetAttribute.UnAssigned:
            default:
                break;


        }

        return outputUnit;

        //return seInput.TargetAttribute switch
        //{
        //    SCADATargetAttribute.RelativeClosure => "",

        //    SCADATargetAttribute.ConstituentConcentration
        //        => waterModel.Network.Junctions.InputFields.FieldByLabel("Concentration (Initial)").Unit.ShortLabel,

        //    SCADATargetAttribute.PressureNodeDemand
        //        => waterModel.Network.Junctions.ResultFields.FieldByLabel("Demand").Unit.ShortLabel,

        //    SCADATargetAttribute.ValveStatus => "",
        //    SCADATargetAttribute.PumpStatus => "",
        //    SCADATargetAttribute.PipeStatus => "",
        //    SCADATargetAttribute.TankLevel
        //        => GetCorrectedPressureUnit(waterModel.Network.Tanks.InputFields.FieldByLabel("Elevation").Unit.ShortLabel),

        //    SCADATargetAttribute.Pressure
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.ResultFields.FieldByLabel("Pressure").Unit.ShortLabel),

        //    SCADATargetAttribute.HydraulicGrade
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.InputFields.FieldByLabel("Elevation").Unit.ShortLabel),

        //    SCADATargetAttribute.PumpSetting => "",
        //    SCADATargetAttribute.TCValveSetting => "",

        //    SCADATargetAttribute.FCValveSetting
        //        => waterModel.Network.Junctions.ResultFields.FieldByLabel("Demand").Unit.ShortLabel,

        //    SCADATargetAttribute.PressureValveSetting
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.ResultFields.FieldByLabel("Pressure").Unit.ShortLabel),

        //    SCADATargetAttribute.PressureOut
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.ResultFields.FieldByLabel("Pressure").Unit.ShortLabel),

        //    SCADATargetAttribute.PressureIn
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.ResultFields.FieldByLabel("Pressure").Unit.ShortLabel),

        //    SCADATargetAttribute.HydraulicGradeOut
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.InputFields.FieldByLabel("Elevation").Unit.ShortLabel),

        //    SCADATargetAttribute.HydraulicGradeIn
        //        => GetCorrectedPressureUnit(waterModel.Network.Junctions.InputFields.FieldByLabel("Elevation").Unit.ShortLabel),

        //    SCADATargetAttribute.Discharge
        //        => waterModel.Network.Junctions.ResultFields.FieldByLabel("Demand").Unit.ShortLabel,

        //    SCADATargetAttribute.WirePower
        //        => waterModel.Network.Pumps.ResultFields.FieldByLabel("Wire Power").Unit.ShortLabel,

        //    SCADATargetAttribute.UnAssigned => "",
        //    _ => "",
        //};


    }



    private Unit? GetUnitFromKeywork(List<DatasourceUnit> units, string unitCheckKeywork)
    {
        Unit? unit = null;
        var unitCheck = units.Where(u => u.Name.ToLower().Contains(unitCheckKeywork));
        if (unitCheck.Any())
        {
            var unitIndex = (UnitConversionManager.UnitIndex)unitCheck.First().Unit;
            unit = Unit.FromIndex(unitIndex);
        }

        return unit;
    }

    private string GetCorrectedPressureUnit(string unit)
    {
        if (unit == "m H2O") return "m of head";

        return unit;
    }
    private string GetCorrectedPressureUnit(Unit? unit)
    {
        if (unit == null) return string.Empty;
        if (unit.ShortLabel == "m H2O")
            return "m of head";
        else
            return unit.ShortLabel;
    }
    private string GetCorrectedFlowUnit(Unit? unit)
    {
        if (unit == null) return string.Empty;
        if (unit.ShortLabel == "gpm") return "gal (U.S.)/min";
        else
            return unit.ShortLabel;
    }
    private double? GetReferenceElevation(ISCADAElementInput seInput)
    {
        var targetElement = seInput.TargetElement;

        if (targetElement != null && targetElement is IPointNodeInput)
            return (targetElement as IPhysicalNodeElementInput)?.Elevation;
        else
            return null;
    }

    #endregion

    #region Tanks Methods
    public TanksXlSheet WriteTanks(
        IWaterModel waterModel,
        string tanksExcelFilePath,
        double desiredTurnoverDays = 3.0,
        string? udxTagsGroupsFieldName = null,
        Func<string, string?>? getDisplayName = null
        )
    {
        Log.Debug($"About to work with all the tank...");

        var xlTanks = new TanksXlSheet(tanksExcelFilePath);
        var tanks = waterModel.Network.Tanks.Elements(ElementStateType.Active);

        var tankLevelUnit = waterModel.Units.NetworkUnits.Tank.LevelUnit;
        var tankVolumeUnit = waterModel.Units.NetworkUnits.Tank.VolumeUnit;

        var udxTagsGroupsFieldValues = new Dictionary<int, string>();
        if (udxTagsGroupsFieldName != null)
            udxTagsGroupsFieldValues = (Dictionary<int, string>)waterModel.Network.Tanks
                .InputFields.FieldByLabel(udxTagsGroupsFieldName).Field.GetValues();

        foreach (var tank in tanks)
        {
            Log.Debug($"About to work with the tank: {tank}");
            var xlTank = new TankItem();
            xlTanks.TankItemsList.Add(xlTank);

            xlTank.DisplayName = tank.Label;

            var scadaElementCheck = tank.GetConnectedSCADAElements(waterModel);
            if (scadaElementCheck.Any())
            {
                var tagId = scadaElementCheck.First().Input.HistoricalSignal?.SignalLabel;
                if (tagId != null)
                {
                    if (getDisplayName != null)
                        xlTank.DisplayName = getDisplayName(tagId) ?? tank.Label;
                }
                else
                {
                    xlTank.DisplayName = scadaElementCheck.First().Label;
                    //xlTank.DisplayName = scadaElementCheck.First().Input.HistoricalSignal?.Label ?? tank.Label;
                    if (xlTank.DisplayName.StartsWith("SE-"))
                        xlTank.DisplayName = xlTank.DisplayName.Substring(3);
                }
            }

            xlTank.BaseElevation = tank.Input.BaseElevation;
            xlTank.MinLevel = GetMinimumLevel(tank);
            xlTank.MaxLevel = GetMaximumLevel(tank);
            xlTank.LengthUnits = tankLevelUnit.ShortLabel;
            xlTank.MaxVolume = GetTankMaxVolume(waterModel, tank);
            xlTank.TankCurveName = GetTankCurveName(tank);
            xlTank.VolumeUnits = GetCorrectedVolumeUnit(tankVolumeUnit.ShortLabel);

            var levelTag = tank.GetConnectedSCADAElements(waterModel).Where(se => se.Input.TargetAttribute == SCADATargetAttribute.TankLevel);
            if (levelTag.Any())
            {
                xlTank.LevelTag = levelTag.First().Input.HistoricalSignal?.TagForWaterSight() ?? string.Empty;
            }

            xlTank.DesiredTurnoverDays = desiredTurnoverDays;

            xlTank.Tags = GetTagsOrGroups(waterModel, tank);
            if (udxTagsGroupsFieldValues.Count > 0)
                xlTank.Tags = udxTagsGroupsFieldValues[tank.Id];

            Log.Debug($"Worked with {tank}");
        }

        // order by DisplayName
        xlTanks.TankItemsList = xlTanks.TankItemsList.OrderBy(p => p.DisplayName).ToList();


        // drop the tanks that have no tank level
        var allCount = xlTanks.TankItemsList.Count;
        xlTanks.TankItemsList = xlTanks.TankItemsList.Where(t => !string.IsNullOrEmpty(t.LevelTag)).ToList();
        Log.Information($"Tank's Level tag check. Before: {allCount}, After: {xlTanks.TankItemsList.Count}");
        var noTanksCount = allCount - xlTanks.TankItemsList.Count;
        if (noTanksCount > 0)
            Log.Warning($"⚠️ Tanks with no level tag found! No Tags Count = {noTanksCount}");


        // Write to Excel
        //var success = await xlTanks.SaveAsync(xlTanks.TankItemsList);
        xlTanks.Save(xlTanks.TankItemsList);
        Log.Information($"Tanks sheet created.");

        Log.Debug($"Worked with {tanks.Count} tanks");
        Log.Debug(Util.LogSeparatorInfinity);

        return xlTanks;
    }

    private double GetTankMaxVolume(IWaterModel waterModel, ITank tank)
    {
        if (tank.Input.TankSection == TankSectionType.VariableArea)
            return tank.Input.ActiveVolumeFull;
        else
        {
            // VolumeFull is only available when the scenario has results
            if (!waterModel.ActiveScenario.HasResults)
            {
                Log.Debug($"About to run the '{waterModel.ActiveScenario.IdLabel}' scenario to get the tank volume.");
                waterModel.ActiveScenario.Run();
                Log.Information($"Ran the '{waterModel.ActiveScenario.IdLabel}' scenario to get the tank volume.");
            }

            return tank.Results.VolumeFull().Value;
        }

    }

    public async Task<bool> WriteTankCurvesAsync(IWaterModel waterModel, string tanksExcelFilePath, TanksXlSheet xlTanks)
    {
        var xlTankCurves = new TankCurvesXlSheet(tanksExcelFilePath);


        var tanks = waterModel.Network.Tanks.Elements(ElementStateType.Active);

        foreach (var tank in tanks)
        {
            if (tank.Input.TankSection == TankSectionType.VariableArea)
            {
                var curve = tank.Input.CrossSectionCurve.Get();
                var curveName = GetTankCurveName(tank);

                if (xlTanks.TankItemsList.Where(t => t.TankCurveName == curveName).Any())
                {
                    foreach (var row in curve)
                        xlTankCurves.TankCurveItemsList.Add(
                            new TankCurveItem(curveName, row.DepthRatio, row.VolumeRatio));
                }
            }
        }

        var success = true;

        // order by DisplayName
        xlTankCurves.TankCurveItemsList = xlTankCurves.TankCurveItemsList.OrderBy(p => p.CurveName).ToList();

        // Write to Excel
        //success = await xlTankCurves.SaveAsync(xlTankCurves.TankCurveItemsList);
        xlTankCurves.Save(xlTankCurves.TankCurveItemsList);

        Log.Information($"[Success = {success}] Worked with {tanks.Count} tank to create TankCurves");

        Log.Debug($"Worked with {tanks.Count} tank curves");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }


    private string GetTankCurveName(ITank tank)
    {
        // this will throw exception if there is no curve
        if (tank.Input.TankSection == TankSectionType.VariableArea)
        {
            var curve = tank.Input.CrossSectionCurve;
            return curve == null ? String.Empty : $"{tank.Label}_Curve";
        }

        return String.Empty;
    }

    private string GetCorrectedVolumeUnit(string tankVolumeUnit)
    {
        if (tankVolumeUnit == "ML") return "Ml";
        if (tankVolumeUnit == "MG") return "Mgal (U.S.)";
        if (tankVolumeUnit == "gal") return "gal (U.S.)";

        return tankVolumeUnit;
    }

    private double GetMinimumLevel(ITank tank)
    {
        if (tank.Input.OperatingRange == OperatingRangeTypeEnum.OperatingRange_LevelType)
            return tank.Input.MinimumLevel;

        return tank.Input.MinimumElevation - tank.Input.BaseElevation;
    }

    private double GetMaximumLevel(ITank tank)
    {
        if (tank.Input.OperatingRange == OperatingRangeTypeEnum.OperatingRange_LevelType)
            return tank.Input.MaximumLevel;

        return tank.Input.MaximumElevation - tank.Input.BaseElevation;
    }
    #endregion

    #region Pumps Methods
    private double? InterpolateHead(IPumpCurve pumpCurve, double flow)
    {
        // if the point falls on the curve, return the value directly
        var isOnCurve = pumpCurve.Where(p => p.Flow == flow);
        if (isOnCurve.Any())
            return isOnCurve.First().Head;

        // work on the interpolation logic
        var lowerFlowValues = pumpCurve.Where((c) => c.Flow < flow);
        var higherFlowValues = pumpCurve.Where((c) => c.Flow > flow);

        if (lowerFlowValues.Any() && higherFlowValues.Any())
        {
            var p1 = lowerFlowValues.Last();
            var p2 = higherFlowValues.First();
            return p1.Head + (flow - p1.Flow) * (p2.Head - p1.Head) / (p2.Flow - p1.Flow);
        }

        return null;
    }
    private double? Interpolate(List<GeometryPoint> points, double knownValue, bool interpolateForYGivenX = true)
    {
        // if interpolating for Y, change, Y to X.
        if (!interpolateForYGivenX)
            points = points.Select(p => new GeometryPoint(p.Y, p.X)).ToList();

        // if the point falls on the curve, return the value directly
        var isOnCurve = points.Where(p => p.X == knownValue);
        if (isOnCurve.Any())
            return isOnCurve.First().Y;

        // work on the interpolation logic
        var lowerValues = points.Where(p => p.X < knownValue);
        var higherValues = points.Where(p => p.X > knownValue);

        if (lowerValues.Any() && higherValues.Any())
        {
            var p1 = lowerValues.Last();
            var p2 = higherValues.First();

            return p1.Y + (knownValue - p1.X) * (p2.Y - p1.Y) / (p2.X - p1.X);
        }

        return null;
    }

    public bool WritePump(PumpsXlSheet xlPumps)
    {
        return xlPumps.Save(xlPumps.PumpItemsList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param curveName="waterModel"></param>
    /// <param curveName="pumpsExcelFilePath"></param>
    /// <param curveName="maxMotorEfficiency"></param>
    /// <param curveName="elementSearchThreshold">Search either upstream or downstream for this many elements to find the right ExternalDataSource tag</param>
    /// <param curveName="udxTagsGroupsFieldName"></param>
    /// <returns></returns>
    public PumpsXlSheet CreatePumpItems(
        IWaterModel waterModel,
        string pumpsExcelFilePath,
        int elementSearchThreshold = 10,
        string? udxTagsGroupsFieldName = null,
        bool generateCurveBasedOnCoefficientABC = true,
        Action<List<PumpItem>>? generatePumpStationName = null,
        Action<List<ISCADAElement>>? filterSuctionTag = null,
        Action<List<ISCADAElement>>? filterDischargeTag = null
        )
    {
        var xlPumps = new PumpsXlSheet(pumpsExcelFilePath);
        var pumps = waterModel.Network.Pumps.Elements(ElementStateType.Active);
        var pumpStationsField = waterModel.Network.Pumps.InputFields.FieldByLabel("Pump Station");
        var variableSpeedPumpFullSpeed = 100;

        // For debugging
        //var debugIDs = new List<int>() {
        //    2111
        //    , 2086
        //    //, 2002
        //    //, 1952
        //};
        //pumps = pumps.Where(p => debugIDs.Contains(p.Id)).ToList();
        //pumps = pumps.Take(50).ToList();

        // HmiNetwork
        var networkBuilder = new IdahoNetworkBuilder(waterModel.DomainDataSet);
        var network = networkBuilder.CreateNetwork();

        // UDX Tags/Groups
        var udxTagsGroupsFieldValues = new Dictionary<int, string>();
        if (udxTagsGroupsFieldName != null)
            udxTagsGroupsFieldValues = (Dictionary<int, string>)waterModel.Network.Pumps
                .InputFields.FieldByLabel(udxTagsGroupsFieldName).Field.GetValues();

        var counter = 1;
        var totalPumps = pumps.Count;
        foreach (var pump in pumps)
        {
            Log.Debug($"[{counter}]/[{totalPumps}] About to work with pump: {pump}");
            var pumpInput = (IPumpInput)pump;
            var pumpElement = (IWaterElement)pump;

            var xlPump = new PumpItem();
            xlPumps.PumpItemsList.Add(xlPump);

            xlPump.DisplayName = pump.Label;
            var pumpDef = pumpInput.PumpDefinition;
            if (pumpDef == null)
            {
                Log.Warning($"Given pump '{pump.IdLabel}' doesn't have any pump definition defined");
                continue;
            }



            // if Design Flow/Head is 0, but a pump curve exists
            // then take 60%th flow and corresponding head
            // this is a big assumption
            if (pumpDef.Head.PumpCurve.Count > 0
                && (xlPump.DesignHead == null || xlPump.DesignHead == 0)
                && (xlPump.DesignFlow == null || xlPump.DesignFlow == 0)
                && (xlPump.BestEfficiencyFlow == null || xlPump.BestEfficiencyFlow == 0)
                && (xlPump.BestEfficiencyHead == null || xlPump.BestEfficiencyHead == 0))
            {
                var pumpCurve = pumpDef.Head.PumpCurve.Get();
                var lastFlow = pumpCurve.Last().Flow;
                xlPump.DesignFlow = lastFlow * 0.6;
                xlPump.DesignHead = InterpolateHead(pumpCurve, xlPump.DesignFlow.Value);
                xlPump.Notes += $" DesignHead and DesignFlow are extracted (at 60% of max [{lastFlow:F2}] flow) from PumpCurve '{GetHeadCurveName(pump)}'.";
            }
            else
            {
                xlPump.DesignFlow = pumpDef?.Head.DesignFlow ?? null;
                xlPump.DesignHead = pumpDef?.Head.DesignHead ?? null;
                xlPump.BestEfficiencyFlow = GetPumpBestEfficiencyFlow(pumpInput);
                xlPump.BestEfficiencyHead = GetPumpBestEfficiencyHead(waterModel, xlPump.BestEfficiencyFlow, pumpInput, generateCurveBasedOnCoefficientABC);
                xlPump.Notes += generateCurveBasedOnCoefficientABC ? $" BEP Head is derived." : "";

            }

            // if BestEfficiency Flow/Head is 0, set this to Design Flow/Head
            if (xlPump.BestEfficiencyFlow == null || xlPump.BestEfficiencyFlow == 0)
                xlPump.BestEfficiencyFlow = xlPump.DesignFlow;
            if (xlPump.BestEfficiencyHead == null || xlPump.BestEfficiencyHead == 0)
                xlPump.BestEfficiencyHead = xlPump.DesignHead;

            // if Design Flow/Head is 0, set this to Best Efficiency Flow/Head
            if (xlPump.DesignFlow == 0) xlPump.DesignFlow = xlPump.BestEfficiencyFlow;
            if (xlPump.DesignHead == 0) xlPump.DesignHead = xlPump.BestEfficiencyHead;


            var bepPercentages = new List<double?>();
            bepPercentages.Add(Math.Min(pumpDef.Efficiency.BEPEfficiency, pumpDef.Efficiency.ConstantEfficiency));
            if (pumpDef.Efficiency.FlowEfficiencyCurve.Count > 0)
                bepPercentages.Add(pumpDef.Efficiency.FlowEfficiencyCurve.Get().Max(c => c.Efficiency));

            xlPump.BEPPercent = bepPercentages.Min();
            if (xlPump.BEPPercent > 1)
                xlPump.BEPPercent = xlPump.BEPPercent / 100;


            xlPump.FlowUnit = pumpDef.Units.FlowUnit.ShortLabel;
            xlPump.HeadUnit = pumpDef.Units.HeadUnit.ShortLabel;


            var pumpStationId = pumpStationsField?.GetValue<object>(pump.Id);
            xlPump.PumpStationDisplayName = string.Empty; //  pumpStationId == null ? string.Empty : waterModel.Element((int)pumpStationId)?.Label ?? string.Empty;
            xlPump.SuctionTag = GetSuctionPressureTag(pumpInput, waterModel, network, elementSearchThreshold, filterSuctionTag);
            xlPump.DischargeTag = GetDischargePressureTag(pumpInput, waterModel, network, elementSearchThreshold, filterDischargeTag);
            xlPump.FlowTag = GetDischargeFlowTag(pumpInput, waterModel, network, elementSearchThreshold);
            xlPump.PowerTag = GetPowerTag(pump, waterModel);
            xlPump.HeadCurveName = GetHeadCurveName(pump.Input.PumpDefinition.Label);
            xlPump.EfficiencyCurveName = GetEfficiencyCurveName(pump.Input.PumpDefinition.Label);
            xlPump.PowerCurveName = String.Empty; // WaterGEMS doesn't support Power Curves
            xlPump.PowerUnits = pumpDef.Units.PowerUnit.ShortLabel;
            xlPump.MotorEfficiency = pumpDef.Motor.MotorEfficiency == 0 ? null : pumpDef.Motor.MotorEfficiency;
            if (xlPump.MotorEfficiency > 1)
                xlPump.MotorEfficiency = xlPump.MotorEfficiency / 100;

            xlPump.IsVariableSpeed = pumpInput.InputFields
                 .FieldByName(StandardFieldName.Physical_IsVariableSpeedPump)
                 ?.GetValue<bool>(pump.Id)
                 ?? false;
            xlPump.FullSpeedValue = xlPump.IsVariableSpeed ? variableSpeedPumpFullSpeed : null;
            xlPump.VariableSpeedEffiCurveName = xlPump.IsVariableSpeed ? GetSpeedEfficiencyCurveName(pump) : null;
            xlPump.SpeedTagName = GetSpeedTagName(pumpElement, waterModel);
            xlPump.StatusTag = GetStatusTag(pumpElement, waterModel);

            xlPump.Groups = GetTagsOrGroups(waterModel, pump);
            if (udxTagsGroupsFieldValues.Count > 0)
                xlPump.Groups = udxTagsGroupsFieldValues[pump.Id];


            // Show tags in log
            var suctionTagHeader = "Suction Tag".PadRight(xlPump.SuctionTag.Length);
            var dischargeTagHeader = "Discharge Tag".PadRight(xlPump.DischargeTag.Length);
            var flowTagHeader = "Flow Tag".PadRight(xlPump.FlowTag.Length);
            var statusTagHeader = "Status Tag".PadRight(xlPump.StatusTag.Length);

            var suctionTag = string.IsNullOrEmpty(xlPump.SuctionTag) ? new string(' ', 11) : xlPump.SuctionTag;
            var dischargeTag = string.IsNullOrEmpty(xlPump.DischargeTag) ? new string(' ', 12) : xlPump.DischargeTag;
            var flowTag = string.IsNullOrEmpty(xlPump.FlowTag) ? new string(' ', 8) : xlPump.FlowTag;
            var statusTag = string.IsNullOrEmpty(xlPump.StatusTag) ? new string(' ', 9) : xlPump.StatusTag;

            var header = $"| {suctionTagHeader} | {dischargeTagHeader} | {flowTagHeader} | {statusTagHeader} |";
            var tags = $"| {suctionTag} | {dischargeTag} | {flowTag} | {statusTag} |";

            Log.Debug(new string('─', header.Length));
            Log.Debug(header);
            Log.Debug(tags);
            Log.Debug(new string('─', header.Length));
            Log.Information($"[{counter}]/[{totalPumps}] Worked with {pump}");
            counter++;
            Log.Debug(new string('x', 100));
        }

        // order by DisplayName
        xlPumps.PumpItemsList = xlPumps.PumpItemsList.OrderBy(p => p.DisplayName).ToList();

        // check if pump station name can be generated based on pump name
        if (generatePumpStationName != null)
            generatePumpStationName(xlPumps.PumpItemsList);


        Log.Debug($"Worked with {pumps.Count} pumps");
        Log.Debug(Util.LogSeparatorInfinity);

        return xlPumps;
    }



    private string GetSpeedEfficiencyCurveName(IPump pump)
    {
        return pump.Input.PumpDefinition.Motor.IsVariableSpeedDrive
            ? GetSpeedCurveName(pump.Input.PumpDefinition.Label)
            : string.Empty;
    }
    private string GetSpeedCurveName(string label)
    {
        return $"{label}_SpeedEffiCurve";
    }

    //private string GetEfficiencyCurveName(IPump pump)
    //{
    //    return pump.Input.PumpDefinition.Efficiency.PumpEfficiencyType == PumpEfficiencyTypeEnum.MultipleEfficiencyPointsType
    //                ? GetEfficiencyCurveName(pump.Input.PumpDefinition.Label)
    //                : string.Empty;
    //}
    private string GetEfficiencyCurveName(string label)
    {
        return $"{label}_EffiCurve";
    }

    private string GetHeadCurveName(IPump pump)
    {
        return GetHeadCurveName(pump.Input.PumpDefinition.Label);
    }
    private string GetHeadCurveName(string label)
    {
        return $"{label}_HeadCurve";
    }

    private string GetPumpSpeedEffiCurveName(IPump pump)
    {
        return GetPumpVFDEffiCurveName(pump.Input.PumpDefinition.Label);
    }
    private string GetPumpVFDEffiCurveName(string label)
    {
        return $"{label}_PowerEffiCurve";
    }

    private double? GetPumpBestEfficiencyFlow(IPumpInput pumpInput)
    {
        if (pumpInput.PumpDefinition == null)
            return null;

        var effi = pumpInput.PumpDefinition.Efficiency;

        // if we have a curve, use that first
        if (effi.FlowEfficiencyCurve.Count > 0)
            return effi.FlowEfficiencyCurve.Get().OrderBy(c => c.Efficiency).Last().Flow; // get max value
        else
            return effi.BEPFlow;

    }
    private double? GetPumpBestEfficiencyHead(IWaterModel waterModel, double? bestEffiflow, IPumpInput pumpInput, bool generateBasedOnCoefficientABC = true)
    {
        if (!bestEffiflow.HasValue)
            return null;

        var pumpCurve = pumpInput.PumpDefinition.Head.PumpCurve.Get();
        if (pumpCurve?.Count > 0)
        {
            // Check if flow value is in the head curve
            var flowCheck = pumpCurve.Where(c => c.Flow == bestEffiflow);
            if (flowCheck.Any())
                return flowCheck.First().Head;

            return InterpolateHead(pumpCurve, bestEffiflow.Value);
        }
        else // if curve is not there, generate a curve based on A,B, and C coefficient and then interpolate for the Head
        {
            var headCurvePoints = waterModel.Curves().PumpHeadCurve(pumpInput.PumpDefinition, generateBasedOnCoefficientABC);
            return Interpolate(headCurvePoints, bestEffiflow.Value);
        }
    }

    private string GetSuctionPressureTag(
        IBaseDirectedNodeInput directedNodeInput,
        IWaterModel waterModel,
        HmiNetwork network,
        int elementSearchThreshold = 10,
        Action<List<ISCADAElement>>? filterSuctionTag = null)
    {
        var timer = Stopwatch.StartNew();
        Log.Debug($"Finding suction pressure tag");

        var element = (IWaterElement)directedNodeInput;


        string unknownTag = string.Empty;
        string tag = unknownTag;

        // Check if there is a "Pressure In" ExternalDataSource Element within the pump
        var inPumpPressureSCADASignal = element
            .GetConnectedSCADAElements(waterModel)?.Where(se =>
                se.Input.TargetAttribute == SCADATargetAttribute.PressureIn
                && se.Input.HistoricalSignal != null);


        if (inPumpPressureSCADASignal.Any())
        {
            tag = inPumpPressureSCADASignal.First()?.Input.HistoricalSignal?.TagForWaterSight() ?? tag;
            Log.Debug($"Suction pressure (PressureIn) tag found within the pump, Tag: {tag}");
            return tag;
        }
        else
            Log.Debug($"No suction pressure (PressureIn) found within the pump");


        // Let's trace the network to find the ExternalDataSource Element
        var upStreamLinkIDs = new List<int>();
        var upStreamNodeIDs = new List<int>();
        //var downStreamLinkIDs = new List<int>();
        //var downStreamNodeIDs = new List<int>();

        var upStreamLinks = new List<IWaterElement>();
        var upStreamNodes = new List<IWaterElement>();
        //var downStreamLinks = new List<IWaterElement>();
        //var downStreamNodes = new List<IWaterElement>();


        directedNodeInput.GetConnectedUpAndDownElements(
            waterModel,
            out IWaterElement suctionLink,
            out IWaterElement suctionNode,
            out IWaterElement dischargeLink,
            out IWaterElement dischargeNode
            );


        var pumpFlow = ((IPipeResults)suctionLink).Flow();
        var useResults = waterModel.ActiveScenario.HasResults && pumpFlow > 0;

        Log.Debug($"Looking for the suction tags on the suction sides nodes by 'TracingUpStream'");
        suctionLink.TraceUpStream(
                    network,
                    out upStreamNodeIDs,
                    out upStreamLinkIDs,
                    useResults);


        // suction tag will only be possible on Node
        var scadaElements = new List<ISCADAElement>();
        var upStreamNodeSearchLimit = upStreamNodeIDs.Count >= elementSearchThreshold ? elementSearchThreshold : upStreamNodeIDs.Count;

        upStreamNodes = upStreamNodeIDs
            .Take(elementSearchThreshold)
            .Select(id => id.WaterElement(waterModel))
            .ToList();

        foreach (var upStreamNode in upStreamNodes)
        {
            var likelyScadaElements = upStreamNode
                .GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel);

            scadaElements.AddRange(likelyScadaElements);
        }

        // remove the duplicates
        scadaElements = scadaElements.Unique().ToList();

        // apply custom filter if any
        if (filterSuctionTag != null)
        {
            filterSuctionTag(scadaElements);
            Log.Debug($"    After custom filter, SE count = '{scadaElements.Count}' by TraceUpStream");
        }
        Log.Debug($"    Found '{scadaElements.Count}' SEs on the upstream nodes by TraceUpStream.");


        // look for nodes that are adjacent (by 1 level) to the upstream nodes
        if (scadaElements.Count == 0)
        {
            Log.Debug($"Looking for nodes that are adjacent (by +1 level) to the upstream nodes");
            var nearlyByNodes = new List<IWaterElement>();
            foreach (var upStreamNode in upStreamNodes)
            {
                var nearbyUpSteamNodes = upStreamNode.GetConnectedAdjacentNodes(waterModel, 2);
                nearlyByNodes.AddRange(nearbyUpSteamNodes);
            }

            foreach (var nearbyNode in nearlyByNodes.Unique())
            {
                var ses = nearbyNode.GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel);
                scadaElements.AddRange(ses);
            }

            // remove duplicates
            scadaElements = scadaElements.Unique().ToList();

            // apply custom filter if any
            if (filterSuctionTag != null)
            {
                filterSuctionTag(scadaElements);
                Log.Debug($"    After custom filter, SE count = '{scadaElements.Count}' by adjacent nodes to the TraceUpStream nodes");
            }

            Log.Debug($"    Found '{scadaElements.Count}' SCADA Elements that are adjacent (by 1 level) to the TraceUpStream nodes");
        }

        if (scadaElements.Count == 0)
        {
            // if still nothing then look around
            Log.Debug($"Looking for nodes that are connected to the directed node");

            var connectedNodes = suctionNode.GetConnectedAdjacentNodes(waterModel, elementSearchThreshold / 2);
            foreach (var connectedNode in connectedNodes)
            {
                var ses = connectedNode.GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel)
                    .ToList();

                scadaElements.AddRange(ses);
            }

            // remove duplicates
            scadaElements = scadaElements.Unique().ToList();

            // apply custom filter if any
            if (filterSuctionTag != null)
            {
                filterSuctionTag(scadaElements);
                Log.Debug($"    After custom filter, SE count = '{scadaElements.Count}' by connected nodes to the directed node");
            }

            Log.Debug($"    Found '{scadaElements.Count}' SCADA Elements that connected nodes to the directed node");

        }


        if (scadaElements.Count == 1)
            tag = scadaElements[0].Input.HistoricalSignal.TagForWaterSight();

        else if (scadaElements.Count > 1)
            tag = ">>Possible tags = " + string.Join(", ", scadaElements.Select(se => $"{se.Input.HistoricalSignal?.TagForWaterSight()}"));

        Log.Debug($"Suction Pressure Tag = {tag}. Time taken: {timer.Elapsed}");
        return tag;
    }

    private string GetDischargePressureTag(
        IBaseDirectedNodeInput directedNodeInput,
        IWaterModel waterModel,
        HmiNetwork network,
        int elementSearchThreshold = 10,
        Action<List<ISCADAElement>>? filterSuctionTag = null)
    {
        var timer = Stopwatch.StartNew();
        Log.Debug($"Finding discharge pressure tag");

        var element = directedNodeInput as IWaterElement;
        var unknownTag = string.Empty;
        var tag = unknownTag;


        // Check if there is a "Pressure Out" ExternalDataSource Element within the pump
        var outPumpPressureSCADASignal = element.GetConnectedSCADAElements(waterModel)
            ?.Where(se =>
                se.Input.TargetAttribute == SCADATargetAttribute.PressureOut
                && se.Input.HistoricalSignal != null);

        if (outPumpPressureSCADASignal.Any())
        {
            Log.Debug($"Discharge pressure (PressureOut) found within the pump");
            return outPumpPressureSCADASignal.First()?.Input.HistoricalSignal?.TagForWaterSight() ?? tag;
        }
        else
            Log.Debug($"No discharge pressure (PressureOut) found within the pump");


        Log.Debug($"Looking on downstream nodes for discharge pressure...");
        // Let's trace the network to find the ExternalDataSource Element
        var downStreamLinksIDs = new List<int>();
        var downStreamNodeIDs = new List<int>();
        var downStreamLinks = new List<IWaterElement>();
        var downStreamNodes = new List<IWaterElement>();

        directedNodeInput.GetConnectedUpAndDownElements(
            waterModel,
            out IWaterElement suctionLink,
            out IWaterElement suctionNode,
            out IWaterElement dischargeLink,
            out IWaterElement dischargeNode
            );


        dischargeLink.TraceDownStream(
            network,
            out downStreamNodeIDs,
            out downStreamLinksIDs,
            waterModel.ActiveScenario.HasResults);


        // suction tag will only be possible on Node
        var scadaElements = new List<ISCADAElement>();

        var downstreamNodes = downStreamNodeIDs
            .Take(elementSearchThreshold)
            .Select(id => id.WaterElement(waterModel))
            .ToList();

        foreach (var downStreamNode in downstreamNodes)
        {
            var likelyScadaElements = downStreamNode
                .GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel);
            scadaElements.AddRange(likelyScadaElements);
        }


        // remove the duplicates
        scadaElements = scadaElements.Unique().ToList();

        // Apply custom filter if any
        if (filterSuctionTag != null)
        {
            filterSuctionTag(scadaElements);
            Log.Debug($"    After custom filter, SE count = '{scadaElements.Count}'");
        }
        Log.Debug($"    Found '{scadaElements.Count}' SEs on the downstream nodes.");


        // look for nodes that are adjacent (by 1 level) to the downstream nodes
        if (scadaElements.Count == 0) // nothing found
        {
            Log.Debug($"Looking for `nodes that are adjacent (by 1 level) to the downstream nodes");
            var nearlyByNodes = new List<IWaterElement>();
            foreach (var downStreamNode in downstreamNodes)
            {
                var dsNodesLevel1 = downStreamNode.GetConnectedAdjacentNodes(waterModel, 2);
                nearlyByNodes.AddRange(dsNodesLevel1);
            }

            foreach (var nearbyNode in nearlyByNodes)
            {
                var ses = nearbyNode.GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel);
                scadaElements.AddRange(ses);
            }

            // remove duplicates
            scadaElements = scadaElements.Unique().ToList();

            // Apply custom filter if any
            if (filterSuctionTag != null)
            {
                filterSuctionTag(scadaElements);
                Log.Debug($"After custom filter, SE count = '{scadaElements.Count}'");
            }

            Log.Debug($"Found '{scadaElements.Count}' SCADA Elements that are adjacent (by 1 level) to the downstream nodes");
        }

        if (scadaElements.Count == 0)
        {
            // if still nothing then look around
            Log.Debug($"Looking for nodes that are connected to the directed node");

            var connectedNodes = dischargeNode.GetConnectedAdjacentNodes(waterModel, elementSearchThreshold / 2);
            foreach (var connectedNode in connectedNodes)
            {
                var ses = connectedNode.GetConnectedSCADAElementsPressureLevelOrGradeType(waterModel)
                    .ToList();

                scadaElements.AddRange(ses);
            }

            // remove duplicates
            scadaElements = scadaElements.Unique().ToList();

            // apply custom filter if any
            if (filterSuctionTag != null)
            {
                filterSuctionTag(scadaElements);
                Log.Debug($"    After custom filter, SE count = '{scadaElements.Count}' by connected nodes to the directed node");
            }

            Log.Debug($"    Found '{scadaElements.Count}' SCADA Elements that connected nodes to the directed node");

        }

        if (scadaElements.Count == 1)
            tag = scadaElements[0].Input.HistoricalSignal?.TagForWaterSight() ?? unknownTag;

        else if (scadaElements.Count > 1)
            tag = ">>Possible tags = " + string.Join(", ", scadaElements.Select(se => $"{se.Input.HistoricalSignal?.TagForWaterSight()}"));


        Log.Debug($"Discharge Pressure Tag = {tag}. Time taken: {timer.Elapsed}");
        return tag;
    }

    private string GetDischargeFlowTag(
        IBaseDirectedNodeInput directedNodeInput,
        IWaterModel waterModel,
        HmiNetwork network,
        int elementSearchThreshold = 10)
    {
        var timer = Stopwatch.StartNew();
        Log.Debug($"Finding flow tag");

        var element = (IWaterElement)directedNodeInput;

        var unknownTag = string.Empty;
        var tag = unknownTag;

        // Check if the pump has flow ExternalDataSource Tag
        var connectedSEs = element.GetConnectedSCADAElements(waterModel);
        var possibleFlowSCADAElement = connectedSEs.Where(se => se.Input.TargetAttribute == SCADATargetAttribute.Discharge);

        if (possibleFlowSCADAElement.Any())
        {
            tag = possibleFlowSCADAElement.First()?.Input.HistoricalSignal?.TagForWaterSight() ?? tag;
            Log.Debug($"Flow tag found within the pump. Tag: {tag}");
            return tag;
        }
        else
            Log.Debug($"No flow tag found within the pump");


        //
        // if pump doesn't have flow tag, hunt downstream links
        var downStreamLinkIDs = new List<int>();
        var downStreamNodeIDs = new List<int>();
        var upStreamLinkIDs = new List<int>();
        var upStreamNodeIDs = new List<int>();

        var downStreamLinks = new List<IWaterElement>();
        var downStreamNodes = new List<IWaterElement>();
        var upStreamLinks = new List<IWaterElement>();
        var upStreamNodes = new List<IWaterElement>();

        // Get the downStream link and node IDs
        var downStreamLink = (IWaterElement)directedNodeInput.DownstreamLink;
        element.TraceDownStream(
            network,
            out downStreamNodeIDs,
            out downStreamLinkIDs,
            waterModel.ActiveScenario.HasResults
            );


        // Get the upStream link and node IDs
        var upStreamLink = directedNodeInput.UpstreamLink(waterModel);
        element.TraceUpStream(
            network,
            out upStreamNodeIDs,
            out upStreamLinkIDs,
            waterModel.ActiveScenario.HasResults
            );

        // convert Ids to IWaterElement
        downStreamLinks = downStreamLinkIDs
            .Take(elementSearchThreshold)
            .Select(id => id.WaterElement(waterModel))
            .ToList();

        downStreamNodes = downStreamNodeIDs
            .Take(elementSearchThreshold)
            .Select(id => id.WaterElement(waterModel))
            .ToList();


        var scadaElements = new List<ISCADAElement>();

        // Find which (forward or reverse) loop to use
        var hasPumpDownStreamPipe = downStreamLinkIDs.Contains(downStreamLink.Id);
        if (!hasPumpDownStreamPipe)
            downStreamLinkIDs.Reverse();


        // Get all the connected SCADA Elements in the pipe(s)
        Log.Debug($"Looking for SCADA Elements on downStream links");
        foreach (var dsLink in downStreamLinks)
        {
            var ses = dsLink.GetConnectedSCADAElementsFlowType(waterModel);
            scadaElements.AddRange(ses);
        }

        scadaElements = scadaElements.Unique().ToList();
        Log.Debug($"    Found'{scadaElements.Count}' SEs on the downstream links");


        if (scadaElements.Count == 0)
        {
            Log.Debug($"Looking for SCADA Elements on adjacent links to the downStream links ");

            // Get all the immediate links from the downstream links
            var nearbyDownStreamLinks = new List<IWaterElement>();
            foreach (var dsLink in downStreamLinks)
                nearbyDownStreamLinks.AddRange(dsLink.GetConnectedAdjacentLinks(waterModel));

            foreach (var dsLink in nearbyDownStreamLinks)
            {
                var ses = dsLink.GetConnectedSCADAElementsFlowType(waterModel);
                scadaElements.AddRange(ses);
            }

            scadaElements = scadaElements.Unique().ToList();
            Log.Debug($"    Fount '{scadaElements.Count}' SEs on the adjacent links to the downstream links");
        }




        // Sometimes the flow meter could be on the suction side
        // look for the flow tag on the suction side 
        // if nothing found so far
        if (scadaElements.Count == 0)
        {
            Log.Debug($"Looking for SCADAElement on the up stream links");
            element.TraceUpStream(network, out upStreamNodeIDs, out upStreamLinkIDs, waterModel.ActiveScenario.HasResults);

            var hasPumpUpStreamPipe = upStreamLinkIDs.Contains(upStreamLink.Id);
            if (!hasPumpUpStreamPipe)
                upStreamLinkIDs.Reverse();

            upStreamLinks = upStreamLinkIDs
                .Take(elementSearchThreshold)
                .Select(id => id.WaterElement(waterModel))
                .ToList();

            foreach (var usLink in upStreamLinks)
            {
                var ses = usLink.GetConnectedSCADAElementsFlowType(waterModel).ToList();
                scadaElements.AddRange(ses);
            }

            scadaElements = scadaElements.Unique().ToList();
            Log.Debug($"    Found '{scadaElements.Count}' SCADA Elements on upStream links");
        }

        if (scadaElements.Count == 0)
        {
            Log.Debug($"Looking for SCADA Elements on adjacent links to the upStream links ");

            // Get all the immediate links from the downstream links
            var nearbyUpStreamLinks = new List<IWaterElement>();
            foreach (var usLink in upStreamLinks)
                nearbyUpStreamLinks.AddRange(usLink.GetConnectedAdjacentLinks(waterModel));

            foreach (var usLink in nearbyUpStreamLinks)
            {
                var ses = usLink.GetConnectedSCADAElementsFlowType(waterModel);
                scadaElements.AddRange(ses);
            }

            scadaElements = scadaElements.Unique().ToList();
            Log.Debug($"    Found '{scadaElements.Count}' SEs on the adjacent links to the downstream links");
        }




        //// look for pipes that are connected to the downstream/upstream links
        //// also expand the elementSearchThreshold by 2 factor
        ////var nearlyByLinks = new List<IWaterElement>();
        ////downStreamNodes = downStreamNodeIDs.Take(elementSearchThreshold * 2).Select(n=>(IWaterElement)waterModel.Element(n)).ToList();  
        ////upStreamNodes = upStreamNodeIDs.Take(elementSearchThreshold * 2).Select(n=>(IWaterElement)waterModel.Element(n)).ToList();  
        //if (scadaElements.Count == 0) // nothing found
        //{
        //    //var downAndUpStreamNodes = new List<IWaterElement>() ;
        //    //downAndUpStreamNodes.AddRange(downStreamNodes);
        //    //downAndUpStreamNodes.AddRange(upStreamNodes);

        //    //foreach (var node in downAndUpStreamNodes.Distinct(new ElementComparer<IWaterElement>()))
        //    //{
        //    //    if (node.Id == 1075) Debugger.Break();

        //    //    var connectedLinks= new List<IWaterElement>();
        //    //    node.TraceConnectedLinks(waterModel, connectedLinks, elementSearchThreshold * 2); // double the threshold as it has to search downstream as well as upstream
        //    //    nearlyByLinks.AddRange(connectedLinks);
        //    //}

        //    var timer = Stopwatch.StartNew();
        //    Log.Debug($"Trace Connected Elements started for {element.IdLabel}.");
        //    var nearlyByLinks = new List<IWaterElement>();
        //    element.TraceConnectedElements(waterModel, nearlyByLinks, elementSearchThreshold * 2);

        //    nearlyByLinks = nearlyByLinks.Distinct(OFWComparer.WaterElement).ToList();
        //    Log.Debug($"Trace Connected Elements finished. Found: {nearlyByLinks.Count}, Limit: {elementSearchThreshold * 2}. Time taken: {timer.Elapsed}");

        //    foreach (var nearbyLink in nearlyByLinks)
        //    {
        //        var ses = nearbyLink.ConnectedSCADAElementsFlowType(waterModel).ToList();
        //        scadaElements.AddRange(ses);
        //    }

        //    scadaElements = scadaElements.Distinct(OFWComparer.SCADAElement).ToList();
        //    Log.Debug($"Found '{scadaElements.Count}' SCADA Elements on downStream links's adjacent link");
        //}

        if (scadaElements.Count == 1)
            tag = scadaElements[0].Input.HistoricalSignal.TagForWaterSight();

        else if (scadaElements.Count > 1)
            tag = ">>Possible tags = " + string.Join(", ", scadaElements.Select(se => $"{se.Input.HistoricalSignal?.TagForWaterSight()}"));


        Log.Debug($"Discharge Flow Tag = {tag}. Time-taken: {timer.Elapsed}");

        return tag;
    }

    private string GetStatusTag(IWaterElement element, IWaterModel waterModel)
    {
        var tag = $"???_{element.Label}_Status";

        var scadaElements = element.GetConnectedSCADAElements(waterModel) ?? new List<ISCADAElement>();
        tag = scadaElements
            .Where(se => se.Input.TargetAttribute == SCADATargetAttribute.PumpStatus)?.FirstOrDefault()
            ?.Input.HistoricalSignal?.TagForWaterSight()
            ?? tag;

        return tag;
    }

    private string GetSpeedTagName(IWaterElement element, IWaterModel waterModel)
    {
        var scadaElements = element.GetConnectedSCADAElements(waterModel)
                     .Where(se => se.Input.TargetAttribute == SCADATargetAttribute.PumpSetting);

        return scadaElements.Any()
            ? scadaElements.First()?.Input.HistoricalSignal?.TagForWaterSight() ?? ""
            : string.Empty;
    }

    private string GetPowerTag(IWaterElement element, IWaterModel waterModel)
    {
        var powerTag = string.Empty;
        var pTagScadaElements = element.GetConnectedSCADAElements(waterModel)
                    ?.Where(se => se.Input.TargetAttribute == SCADATargetAttribute.WirePower);

        return pTagScadaElements.Any()
            ? pTagScadaElements.First()?.Input.HistoricalSignal?.TagForWaterSight() ?? powerTag
            : powerTag;
    }
    #endregion

    #region PumpCurves Methods

    public async Task<bool> WritePumpCurvesAsync(
        IWaterModel waterModel,
        string pumpsExcelFilePath,
        PumpsXlSheet xlPumps,
        bool maxEffiIs100Not1 = true,
        bool generateCurveBasedOnCoefficientABC = false,
        bool dropDriveEfficiency = false)
    {
        var pumpCurves = new PumpCurvesXlSheet(pumpsExcelFilePath);
        var modelCurves = waterModel.Curves();

        // Head Curve
        var headCurves = modelCurves.PumpHeadCurves(generateCurveBasedOnCoefficientABC: generateCurveBasedOnCoefficientABC);
        foreach (var item in headCurves)
        {
            var name = item.Key + "_HeadCurve";
            var curveData = item.Value;
            curveData?.ForEach(row =>
                pumpCurves.Curves.Add(new PumpCurveItem(name, CurveType.Head, row.X, row.Y)));
        }
        Log.Debug($"Head-Flow curves created for {headCurves.Count} curves.");

        // Efficiency Curve
        var effiCurves = modelCurves.PumpFlowEfficiencyCurves(generateCurveBasedOnCoefficientABC);
        foreach (var item in effiCurves)
        {
            var name = GetEfficiencyCurveName(item.Key);
            var curveData = item.Value;
            var curveItems = new List<PumpCurveItem>();

            curveData?.ForEach(row =>
            {
                if (row.X == double.NaN || row.Y == double.NaN)
                    Debugger.Break();

                var flow = row.X;
                var effi = row.Y;

                if (effi > 1 && !maxEffiIs100Not1)
                    effi = effi / 100;

                curveItems.Add(new PumpCurveItem(name, CurveType.Efficiency, flow, effi));

                // Remove duplicate rows
                var uniqueCurveItems = curveItems
                                        .GroupBy(r => new { r.Flow, r.Value })
                                        .Select(g => g.First())
                                        .ToList();

                pumpCurves.Curves.AddRange(uniqueCurveItems);

            });


        }
        Log.Debug($"Head-Efficiency curves created for {effiCurves.Count} curves.");

        // if an efficiency curve is missing (for whatever reason)
        // remove from xlPump items too
        foreach (var xlPump in xlPumps.PumpItemsList)
        {
            var effiCurvesCheck = pumpCurves.Curves
                                .Where(c => c.CurveType == CurveType.Efficiency
                                    && c.PumpCurveName == xlPump.EfficiencyCurveName);

            if (!effiCurvesCheck.Any())
            {
                Log.Debug($"Effi curve '{xlPump.EfficiencyCurveName}' has been removed from xlPumps. Name: '{xlPump.DisplayName}'.");
                xlPump.EfficiencyCurveName = string.Empty;
            }
        }



        // Power Curve
        // Not supported in WaterGEMS


        // Motor's speed vs efficiency curve (For VFDs)
        var motorEffiCurves = modelCurves.PumpMotorAndDriveEffiCurves(includeIdInLabel: false, dropDriveEfficiency: dropDriveEfficiency);
        foreach (var item in motorEffiCurves)
        {
            var name = GetPumpVFDEffiCurveName(item.Key);
            var curveData = item.Value;
            curveData?.ForEach(row =>
                pumpCurves.Curves.Add(new PumpCurveItem(name, CurveType.VFD_Efficiency, row.X, row.Y)));
        }


        // Write to Excel
        //var success = await pumpCurves.SaveAsync(pumpCurves.Curves);
        var success = true;
        success = pumpCurves.Save(pumpCurves.Curves);

        Log.Debug($"Motor Speed-Efficiency curves created for {motorEffiCurves.Count} curves.");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }
    #endregion

    #region Pump Stations Methods
    public bool WritePumpStations(
        IWaterModel waterModel,
        string pumpsExcelFilePath,
        PumpsXlSheet xlPumps)
    {
        var xlPumpStations = new PumpStationsXlSheet(pumpsExcelFilePath);
        var xlStations = xlPumpStations.PumpStationItemsList;



        //xlStations = waterModel.Network.PumpStations.Elements(ElementStateType.Active)
        //    .Select(ps => new PumpStationItem()
        //    {
        //        DisplayName = ps.Label
        //    }).ToList();

        //if (xlStations.Count == 0)
        //{

        //    var s = waterModel.Scenarios.Create();

        //    // group near by pumps
        //    var groupedPumps = Cluster.ByDistance(
        //        waterModel.Network.Pumps.Elements(ElementStateType.Active)
        //        .Select(p => p as IWaterElement).ToList(), 100);

        //    var labels = new List<string>();
        //    foreach (var item in groupedPumps)
        //    {
        //        labels = item.Value.Select(b => b.Label).ToList();
        //        var stationName = Cluster.ByName(labels);
        //        xlStations.Add(
        //            new PumpStationItem() { DisplayName = stationName });
        //    }
        //}

        // drop line items with non-empty display name
        // xlStations = xlStations.Where(s => s.DisplayName != string.Empty).ToList();

        if (xlPumps.PumpItemsList.Count == 0)
            xlPumps.Load();


        // group by pump station name
        var pumpGroups = xlPumps.PumpItemsList
                            .Where(p => !string.IsNullOrEmpty(p.PumpStationDisplayName))
                            .GroupBy(p => p.PumpStationDisplayName);

        foreach (var group in pumpGroups)
        {
            // Get the non-blank name/tags
            var psName = string.Empty;
            var suctionPressureTag = string.Empty;
            var dischargePressureTag = string.Empty;
            var flowTag = string.Empty;

            foreach (var pumpItem in group)
            {
                if (!(string.IsNullOrEmpty(pumpItem.PumpStationDisplayName))
                        && string.IsNullOrEmpty(psName))
                    psName = pumpItem.PumpStationDisplayName;

                if (!(string.IsNullOrEmpty(pumpItem.SuctionTag))
                        && string.IsNullOrEmpty(suctionPressureTag))
                    suctionPressureTag = pumpItem.SuctionTag;

                if (!(string.IsNullOrEmpty(pumpItem.SuctionTag))
                        && string.IsNullOrEmpty(dischargePressureTag))
                    dischargePressureTag = pumpItem.DischargeTag;

                if (!(string.IsNullOrEmpty(pumpItem.SuctionTag))
                        && string.IsNullOrEmpty(flowTag))
                    flowTag = pumpItem.FlowTag;

            }

            //// for matching psName and "UNK" tags, update with the above tag
            //var filteredPumpStations = xlStations.Where(ps => ps.DisplayName == psName);
            //foreach (var pump in filteredPumpStations)
            //{
            //    if (string.IsNullOrEmpty(pump.SuctionTag)) pump.SuctionTag = suctionPressureTag;
            //    if (string.IsNullOrEmpty(pump.DischargeTag)) pump.DischargeTag = dischargePressureTag;
            //    if (string.IsNullOrEmpty(pump.FlowTag)) pump.FlowTag = flowTag;
            //}

            xlStations.Add(new PumpStationItem()
            {
                DischargeTag = dischargePressureTag,
                FlowTag = flowTag,
                SuctionTag = suctionPressureTag,
                DisplayName = psName,
                Groups = string.Empty
            });
        }

        Log.Debug($"Pump Station items created. Count: {xlStations.Count}.");

        // Write to Excel
        //var success = await xlPumpStations.SaveAsync(xlStations);
        var success = true;
        xlPumpStations.Save(xlStations);

        Log.Debug(Util.LogSeparatorInfinity);
        return success;
    }
    #endregion

    #region Customer Meters Methods
    public async Task<bool> WriteCustomerMetersAsync(
        IWaterModel waterModel,
        string customerMeterExcelFilePath,
        string? udxAddressFieldName = null,
        string? udxIsCriticalFieldName = null,
        string? udxCustomerTypeFieldName = null,
        string? udxMeterTypeFieldName = null,
        string? udxInstallationDateFieldName = null,
        string? udxDeactivationDateFieldName = null,
        bool usePatternLabelForCustomerType = false
        )
    {
        Log.Debug($"About to work with the Customer Meters...");

        var xlCustomerMeters = new CustomerMetersXlSheet(customerMeterExcelFilePath);
        var customerMeters = waterModel.Network.CustomerMeters.Elements(ElementStateType.Active);

        //
        // Billing ID
        var billingIdField = waterModel.Network.CustomerMeters.InputFields.FieldByLabel("Billing ID").Field;
        var billingIdValues = billingIdField.GetValues();
        // if billing id is null through out
        // use label
        var allNull = billingIdValues.Values.Cast<string>().All(v => v == null);
        if (allNull)
            billingIdValues = (IDictionary)waterModel.Network.CustomerMeters.Labels();

        // UDX Values
        IDictionary udxAddressFieldValues = null;
        IDictionary udxIsCriticalFieldValues = null;
        IDictionary udxCustomerTypeFieldValues = null;
        IDictionary udxMeterTypeFieldValues = null;
        IDictionary udxInstallationDateFieldValues = null;
        IDictionary udxDeactivationFieldValues = null;

        var inputFields = waterModel.Network.CustomerMeters.InputFields;

        // Address
        if (udxAddressFieldName != null)
            udxAddressFieldValues = inputFields.FieldByLabel(udxAddressFieldName)?.Field.GetValues();


        // Is Critical
        if (udxIsCriticalFieldName != null)
            udxIsCriticalFieldValues = inputFields.FieldByLabel(udxIsCriticalFieldName).Field.GetValues();

        // Customer Type
        if (usePatternLabelForCustomerType && udxCustomerTypeFieldName == null)
        {
            udxCustomerTypeFieldValues = new Dictionary<int, string>();
            customerMeters.ForEach(cm
                => udxCustomerTypeFieldValues.Add(cm.Id, cm.Input.DemandPattern.Label));
        }
        if (udxCustomerTypeFieldName != null)
            udxCustomerTypeFieldValues = inputFields.FieldByLabel(udxCustomerTypeFieldName).Field.GetValues();

        // Meter Type
        if (udxMeterTypeFieldName != null)
            udxMeterTypeFieldValues = inputFields.FieldByLabel(udxMeterTypeFieldName).Field.GetValues();

        // Installation Date
        if (udxInstallationDateFieldName != null)
            udxInstallationDateFieldValues = inputFields.FieldByLabel(udxInstallationDateFieldName).Field.GetValues();

        // Deactivation Date
        if (udxDeactivationDateFieldName != null)
            udxDeactivationFieldValues = inputFields.FieldByLabel(udxDeactivationDateFieldName).Field.GetValues();

        foreach (var cm in customerMeters)
        {
            var xlCM = new CustomerMeterItem();
            xlCustomerMeters.CustomerMeterItemsList.Add(xlCM);

            var billingId = billingIdValues[cm.Id];
            xlCM.ID = billingId?.ToString() ?? cm.Id.ToString();

            xlCM.Name = cm.Label;
            var location = cm.Input.GetPoint();
            xlCM.LongitudeOrX = location.X;
            xlCM.LatitudeOrY = location.Y;

            if (cm.Input?.AssociatedElement != null)
            {
                xlCM.Zone = (cm.Input.AssociatedElement as IWaterZoneableNetworkElementInput).Zone?.Label ?? string.Empty;
            }

            // Address
            if (udxAddressFieldValues != null)
                xlCM.Address = udxAddressFieldValues[cm.Id].ToString();

            // Is Critical
            if (udxIsCriticalFieldValues != null)
                xlCM.IsCritcal = udxIsCriticalFieldValues[cm.Id].ToString();

            // Customer Type
            if (udxCustomerTypeFieldValues != null)
                xlCM.CustomerType = udxCustomerTypeFieldValues[cm.Id].ToString();

            // Meter Type
            if (udxMeterTypeFieldValues != null)
                xlCM.TypeOfMeter = udxMeterTypeFieldValues[cm.Id].ToString();

            // Installation Date
            if (udxInstallationDateFieldValues != null)
                xlCM.InstallationDate = udxInstallationDateFieldValues[cm.Id].ToString();

            // Deactivation Date
            if (udxDeactivationFieldValues != null)
                xlCM.DecativationDate = udxDeactivationFieldValues[cm.Id].ToString();
        }

        // order by Id
        xlCustomerMeters.CustomerMeterItemsList = xlCustomerMeters.CustomerMeterItemsList.OrderBy(p => p.ID).ToList();

        // Write to Excel
        var success = await xlCustomerMeters.SaveAsync(xlCustomerMeters.CustomerMeterItemsList);

        Log.Debug($"Worked with {customerMeters.Count} customer meters");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }
    #endregion

    #region Zones Methods
    public async Task<bool> WriteZonesAsync(string zoneExcelFilePath, List<ZoneItem> zoneItemMap)
    {
        Log.Debug($"About to work with the Zones...");

        var xlZones = new ZonesXlSheet(zoneExcelFilePath);


        foreach (var zoneItem in zoneItemMap)
        {
            xlZones.ZoneItemsList.Add(zoneItem);
        }

        // order by Id
        xlZones.ZoneItemsList = xlZones.ZoneItemsList.OrderBy(z => z.DisplayName).ToList();

        // Write to Excel
        var success = await xlZones.SaveAsync(xlZones.ZoneItemsList);


        Log.Debug($"Worked with {zoneItemMap.Count} zones");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }
    public async Task<bool> WriteZoneCharacetristicsAsync(string zoneCharacteristicsExcelFilePath, Dictionary<string, ZoneCharacteristicsItem> zoneCharItemMap)
    {
        Log.Debug($"About to work with the Zones...");

        var xlZoneChars = new ZoneCharacteristicsXlSheet(zoneCharacteristicsExcelFilePath);
        foreach (var zoneCharItem in zoneCharItemMap)
        {
            xlZoneChars.ZoneCharacteristicsItemsList.Add(zoneCharItem.Value);
        }

        // order by Id
        xlZoneChars.ZoneCharacteristicsItemsList = xlZoneChars.ZoneCharacteristicsItemsList.OrderBy(z => z.DisplayName).ToList();

        // Write to Excel
        var success = await xlZoneChars.SaveAsync(xlZoneChars.ZoneCharacteristicsItemsList);

        Log.Debug($"Worked with {zoneCharItemMap.Count} zones");
        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }

    public async Task<bool> WritePossibleSensorsAsync(List<Niraula.Extensions.Water.Sensors.Sensor> sensors, string xlFilePath)
    {
        var success = true;
        var sensorsCountThresholdToSplitIntoTabs = 500;
        if (sensors.Count > sensorsCountThresholdToSplitIntoTabs)
        {
            var fileInfo = new FileInfo(xlFilePath);
            xlFilePath = Path.Combine(fileInfo.DirectoryName, "PossibleSensors", fileInfo.Name);
            fileInfo = new FileInfo(xlFilePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            var sensorGroups = sensors.GroupBy(s => s.OriginElement.WaterElementType);
            foreach (var sensorGroup in sensorGroups)
            {

                var possibleSensors = new List<PossibleSensorItem>();
                foreach (var sensor in sensorGroup)
                {
                    possibleSensors.Add(new PossibleSensorItem(
                        sensorType: sensor.SensorType.ToString(),
                        tagName: sensor.TagName,
                        displayName: sensor.Label,
                        attribute: sensor.TargetAttribute.ToString(),
                        sourceElementIdLabel: sensor.OriginElement.IdLabel,
                        targetElmentIdLabel: sensor.NetworkElement.IdLabel
                        ));
                }

                var xlSheet = new PossibleSensorsXlSheet(
                    excelFilePath: xlFilePath.Replace(".xlsx", $"_{sensorGroup.Key}.xlsx"),
                    sheetName: sensorGroup.Key.ToString());

                success = await xlSheet.SaveAsync(possibleSensors);
                Log.Information($"Wrote to sheet: '{xlSheet.SheetName}' in Excle file: {xlSheet.FilePath}");
            }
        }
        else
        {
            // Write all into one sheet
            var xlSheet = new PossibleSensorsXlSheet(excelFilePath: xlFilePath);
            var possibleSensors = sensors.Select(s => new PossibleSensorItem(
                sensorType: s.SensorType.ToString(),
                tagName: s.TagName,
                displayName: s.Label,
                attribute: s.TargetAttribute.ToString(),
                sourceElementIdLabel: s.OriginElement.IdLabel,
                targetElmentIdLabel: s.NetworkElement.IdLabel
                )).ToList();

            success = await xlSheet.SaveAsync(possibleSensors);
        }

        Log.Debug(Util.LogSeparatorInfinity);

        return success;
    }

    public bool WritePossibleSensorsToCSV(List<Niraula.Extensions.Water.Sensors.Sensor> sensors, string csvFilePath)
    {
        var sb = new StringBuilder().AppendLine(PossibleSensorItem.HeaderCSV);

        var possibleSensors = sensors.Select(s => new PossibleSensorItem(
                sensorType: s.SensorType.ToString(),
                tagName: s.TagName,
                displayName: s.Label,
                attribute: s.TargetAttribute.ToString(),
                sourceElementIdLabel: s.OriginElement.IdLabel,
                targetElmentIdLabel: s.NetworkElement.IdLabel
                )).ToList();

        possibleSensors.ForEach(s => sb.AppendLine(s.ToCSV()));

        var success = true;
        try
        {
            var fileInfo = new FileInfo(csvFilePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            System.IO.File.WriteAllText(csvFilePath, sb.ToString());
        }
        catch (Exception ex)
        {
            success = false;
            Log.Error(ex, $"...while saving to csv File. Path: {csvFilePath}");
        }

        return success;

    }
    #endregion

    #region Private Properties
    #endregion
}
