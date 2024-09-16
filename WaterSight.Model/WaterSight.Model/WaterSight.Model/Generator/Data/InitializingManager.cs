using Haestad.Calculations.Pressure;
using Haestad.Domain;
using Haestad.Support.Support;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Generator.Data;



public class InitializingManager
{
    #region Constructor
    public InitializingManager(IWaterModel waterModel, SCADADataGeneratorOptions options)
    {
        WaterModel = waterModel;
        Options = options;
    }
    #endregion

    #region Public Methods
    public bool SetScenario()
    {
        var success = false;
        var givenScenario = WaterModel.Scenarios.Elements().Where(s => s.Label == Options.Model.ScenarioToUseLabel).FirstOrDefault();
        Log.Debug($"Setting active scenario. ID: {givenScenario.Id}.");
        try
        {
            WaterModel.SetActiveScenario(givenScenario.Id);
            success = true;
        }
        catch (InvalidOperationException ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"Scenario ID {givenScenario.IdLabel} is not a valid scenario.";
            Log.Error(ex, message);
            return success;
        }

        Log.Information($"Active Scenario: {WaterModel.ActiveScenario.IdLabel}");
        return success;
    }

    public bool SetCalculationOptions()
    {
        var success = false;
        Log.Debug("Setting calculation options.");

        try
        {
            WaterModel.ActiveScenario.Options.TimeAnalysisType = EpaNetEngine_TimeAnalysisTypeEnum.EpsType;

            if (Options.Model.SimulationOptions.TimeStepsHours <= 0)
                Options.Model.SimulationOptions.TimeStepsHours = 0.25; // Set a default.

            WaterModel.ActiveScenario.Options.HydraulicTimeStep = Options.Model.SimulationOptions.TimeStepsHours;
            
            var field = (IEditField) WaterModel.ActiveScenario.Options.OptionsFields.FieldByName(StandardCalculationOptionFieldName.OverrideReportingTimeStep).Field;
            field.SetValue(WaterModel.ActiveScenario.Options.Id, (int)OverrideReportingTimeStepType.Constant);
            WaterModel.ActiveScenario.Options.ReportingTimeStep = Options.Model.SimulationOptions.TimeStepsHours;

            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...while updating Calc Options, {WaterModel.ActiveScenario.Options.IdLabel}";
            Log.Error(ex, message);
            return success;
        }

        Log.Information($"Calculation options updated.");
        return success;
    }

    public bool SetModelDateRange(DateTime startDate, DateTime endDate)
    {
        var success = false;

        try
        {
            // Use .Date to force the dates to midnight to avoid any time issues.
            startDate = startDate.Date;
            endDate = endDate.Date;

            Log.Information($"Setting Date Range: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}");

            if (endDate < startDate)
            {
                Log.Warning($"End date must be greater than or equal to start date. Start Date: {startDate.ToShortDateString()}. End Date: {endDate.ToShortDateString()}.");
                return false;
            }

            WaterModel.ActiveScenario.Options.SimulationStartDate = startDate;
            WaterModel.ActiveScenario.Options.StartTime = startDate;

            WaterModel.ActiveScenario.Options.Duration = (endDate.AddDays(1) - startDate).TotalHours; // Adding one day so the date range is inclusive.
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...while updating model date range, {WaterModel.ActiveScenario.Options.IdLabel}. Range: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}";
            Log.Error(ex, message);
            return success;
        }

        success = true;
        Log.Information($"Model simulation Date Range updated: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}");

        return success;
    }

    //public bool InitializeSensorElements()
    //{
    //    Log.Debug("Initializing sensor elements.");


    //    if (Options.SensorElements is null)
    //    {
    //        Log.Warning("No Sensor Elements defined.");
    //        return true;
    //    }

    //    foreach (var sensorNode in Options.SensorElements)
    //    {
    //        var element = WaterModel.Element(sensorNode.Id);

    //        if (element is null)
    //            Log.Information($"Sensor Element with id {sensorNode.Id} was not found and will be skipped.");
    //        else
    //        {
    //            SensorElement sensorElement = new SensorElement(element, sensorNode);
    //            Sensors.Add(sensorElement);
    //        }
    //    }

    //    Log.Information($"Sensor Elements initialized. Found: {Sensors.Count}");
    //    return true;
    //}

    public bool UpdateElementInitialConditions()
    {
        Log.Debug("Updating initial conditions.");

        if (!WaterModel.ActiveScenario.HasResults)
            return true;

        WaterModel.ActiveScenario.ActiveTimeStep = WaterModel.ActiveScenario.TimeStepsInSeconds.Length - 1;
        if (!UpdateTankInitialConditions())
            return false;

        if (!UpdatePumpInitialConditions())
            return false;

        if (!UpdateValveInitialConditions())
            return false;

        Log.Information($"Element initial conditions updated");
        return true;
    }
    #endregion

    #region Private Methods
    private bool UpdateTankInitialConditions()
    {
        foreach (var element in WaterModel.Network.Tanks.Elements(ElementStateType.Active))
            element.Input.InitialLevel = (double)element.Results.Level();

        Log.Debug($"Tank initial conditions updated.");
        return true;
    }

    private bool UpdatePumpInitialConditions()
    {
        foreach (var element in WaterModel.Network.Pumps.Elements(ElementStateType.Active))
        {
            element.Input.InitialRelativeSpeedFactor = (double)element.Results.CalculatedRelativeSpeedFactor();
            Log.Verbose($"InitialRelativeSpeedFactor: {element.Input.InitialRelativeSpeedFactor}");

            element.Input.InitialStatus = (int)element.Results.CalculatedPumpStatus();
            Log.Verbose($"InitialStatus: {element.Input.InitialStatus}");

            // When the last results value is not either on or off, set it to off
            // setting OFF is better option. Controls could turn it on if needed
            if(!(element.Input.InitialStatus == (int)PumpStatusEnum.PumpOnType
                || element.Input.InitialStatus == (int)PumpStatusEnum.PumpOffType))
            {
                element.Input.InitialStatus = (int)PumpStatusEnum.PumpOffType;
            }
        }

        Log.Debug($"Pump initial conditions updated");
        return true;
    }

    private bool UpdateValveInitialConditions()
    {
        // TODO <sjac>: This function currently does not work as expected, and has been deactivated.
        //      Problem 1: Copying the 'InitialSetting' from the last timestep to the first doesn't work because there is some floating point or conversion issue happening.
        //          If a valve is holding a hydraulic grade of 800, the element.Results.CalculatedSetting() for the last timestep is something like 800.02.
        //          This results in the setting being stepped up slightly each iteration. The effect is pretty unnoticable for zones fed by a single valve, but causes divergence for zones fed by multiple PRVs.
        //      Problem 2: Copying the 'InitialStatus' from the last timestep to the first doesn't work properly. This is because there is no distinction in the results enums between 'closed (but active)' and 'closed (but inactive)'. So if a valve is temporarily closed (because the pressure is outside the setpoint), this gets copied back to the initial settings as a valve that is closed and not responding to pressure. I don't think there's enough information in the results file to differentiate.
        //      These problems cause the Watertown model to diverge drastically over multiple iterations, because they change the ratio of flow going into Town Hall Zone from PRV 1 and PRV 4. 
        //      (Note: for testing this issue, it can be helpful to set PRV 4 to a hydraulic grade of 801 feet, because this makes PRV 1 only turn on during the day, and be off at midnight. This helps by simplifying what is happening at midnight when two iterations are stitched together.)
        //      Until these issues are fixed, the tool will not support models that have controls that change valve setting or status.

        foreach (var element in WaterModel.Network.PRVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialSetting = (double)element.Results.CalculatedSetting();  // TODO <sjac> Problematic because Calculated setting has something like a floating point drift compared to Initial Setting. On subsequent iterations, this progresses like 800-->800.02595574435634-->800.05192133280957...
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus(); // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        foreach (var element in WaterModel.Network.PSVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialSetting = (double)element.Results.CalculatedSetting();  // TODO <sjac> Problematic because Calculated setting has something like a floating point drift compared to Initial Setting. On subsequent iterations, this progresses like 800-->800.02595574435634-->800.05192133280957...
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus();  // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        foreach (var element in WaterModel.Network.PBVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialSetting = (double)element.Results.CalculatedSetting();  // TODO <sjac> Problematic because Calculated setting has something like a floating point drift compared to Initial Setting. On subsequent iterations, this progresses like 800-->800.02595574435634-->800.05192133280957...
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus();  // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        foreach (var element in WaterModel.Network.FCVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialFlowSetting = (double)element.Results.CalculatedFlowSetting();  // TODO <sjac> Problematic because Calculated setting has something like a floating point drift compared to Initial Setting. On subsequent iterations, this progresses like 800-->800.02595574435634-->800.05192133280957...
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus();  // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        foreach (var element in WaterModel.Network.TCVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialCoefficient = (double)element.Results.CalculatedSetting();  // TODO <sjac> Problematic because Calculated setting has something like a floating point drift compared to Initial Setting. On subsequent iterations, this progresses like 800-->800.02595574435634-->800.05192133280957...
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus();  // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        foreach (var element in WaterModel.Network.GPVs.Elements(ElementStateType.Active))
        {
            //element.Input.InitialStatus = (ValveSettingType)element.Results.CalculatedStatus();  // TODO: <sjac> Problematic because 'closed' in results can mean 'temporarily closed', but in initial settings it means 'permanently closed'.
        }

        return true;
    }
    #endregion

    #region Public Properties
    //public List<SensorElement> Sensors { get; set; } = new List<SensorElement>();
    #endregion

    #region Private Properties
    private IWaterModel WaterModel { get; }
    private SCADADataGeneratorOptions Options { get; }
    #endregion

    public class SensorElement : SensorNode
    {
        #region Constructor
        public SensorElement(IElement element, SensorNode sensorNode)
            : base()
        {
            Element = element;
            OutagePercentChance = sensorNode.OutagePercentChance;
            OutageDurationHours = sensorNode.OutageDurationHours;
            OutageDurationPercentVariability = sensorNode.OutageDurationPercentVariability;
        }

        #endregion

        #region Public Properties
        public IElement Element { get; }
        #endregion
    }
}