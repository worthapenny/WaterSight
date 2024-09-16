using Haestad.Calculations.Support;
using Haestad.Domain;
using Haestad.Framework.Application;
using Haestad.Support.Units;
using Haestad.Support.User;
using Niraula.Extensions.Water.Support;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WaterSight.Model.Generator.Data;

public class DataGenerator
{
    #region Constructor
    public DataGenerator(IWaterModel waterModel, SCADADataGeneratorOptions options)
    {
        if (options == null)
        {
            Log.Fatal($"Data generator option is null.");
            return;
        }

        WaterModel = waterModel;
        Options = options;
    }
    #endregion


    #region Public Methods
    public BasicResult Validate()
    {
        string message;

        //
        // Make sure the scenario label is valid
        //
        var scenarioCheck = WaterModel.Scenarios.Elements().Where(s => s.Label == Options.Model.ScenarioToUseLabel);
        if (!scenarioCheck.Any())
        {
            message = $"Given scenario name '{Options.Model.ScenarioToUseLabel}' is not valid.";
            Log.Fatal(message);
            return new BasicResult(false, message);
        }

        //
        // Make sure model does not have Actions on Valves
        //
        if (Options.Model.SimulationOptions.DaysPerSimulation > 0)
        {
            foreach (IControlAction action in WaterModel.Components.ControlActions.Elements())
            {
                if (action.Element != null && action.ActionType == ControlActionTypeEnum.SimpleActionType)
                {
                    var type = action.Element.WaterElementType;
                    if (type == WaterNetworkElementType.PRV ||
                        type == WaterNetworkElementType.PSV ||
                        type == WaterNetworkElementType.PBV ||
                        type == WaterNetworkElementType.FCV ||
                        type == WaterNetworkElementType.TCV ||
                        type == WaterNetworkElementType.GPV)
                    {

                        message = "This model contains control actions that change valve settings and/or status. " +
                            "This behavior is currently not supported when the days per simulation period is less than the total simulation period. " +
                            "This is because it is problematic to copy valve final results back to initial settings when stitching consecutive model runs together.";

                        Log.Fatal($"{message}");
                        return new BasicResult(false, message);
                    }
                }
            }
        }

        message = $"No validation error found on model: {Options.Model.ModelFilePath}";
        Log.Information(message);
        return new BasicResult(true, message);
    }

    public BasicResult Run(
        Randomizer randomizer,
        EnvironmentalManager environmentalManager,
        OutputManager outputManager,
        Action<OutputManager> onSimulationStepComplete,
        bool continueOnSimFailure)
    {
        string message;
        var timerFull = Stopwatch.StartNew();

        try
        {

            /*if (!outputManager.PrepareSCADADataOutputFile())
                return new BasicResult(false, "Preparing SCADA data output sqlite file didn't go well.");

            outputManager.CombineSCADADataRecordSensors();*/


            var initializer = new InitializingManager(WaterModel, Options);

            //if (!initializer.InitializeSensorElements())
            //    return new BasicResult(false, "Initialization of sensor elements didn't go well.");

            if (!initializer.SetScenario())
                return new BasicResult(false, "Failed around setting the scenario.");

            if (!initializer.SetCalculationOptions())
                return new BasicResult(false, "Failed around setting calculation options.");

            /*if (!initializer.InitializeSensorElements())
                return new BasicResult(false, "Failed around initializing sensor elements.");*/


            // These must be created after ther Initializer is complete, because their constructors read Calculation Options that the Initializer has set.
            var patternManager = new PatternManager(WaterModel, Options, randomizer, environmentalManager);
            //var scadaManager = new SCADAManager(WaterModel, Options, randomizer, outputManager, initializer);
            var billingManager = new BillingManager(WaterModel, Options, randomizer, outputManager, patternManager);
            var pumpManager = new PumpManager(WaterModel, Options);
            //var amiManager = new AMIManager(Logger, UserOptions, Model);

            if (!Options.Input.Ami.Enable)
            {
                if (!patternManager.InitializePatterns())
                    return new BasicResult(false, "Failed around initializing patterns");
            }
            else
                Log.Information($"AMI is not selected");

            // Supplemental patterns are used for both AMI and Non-AMI methodologies.
            if (!patternManager.InitializeSupplementalDemandPatterns())
                return new BasicResult(false, "Failed around initializing supplemental demand patterns");



            int totalDuration = (int)(Options.Model.SimulationOptions.End.Date.AddDays(1) - Options.Model.SimulationOptions.Start.Date).TotalDays;
            if (Options.Model.SimulationOptions.DaysPerSimulation <= 0
                || Options.Model.SimulationOptions.DaysPerSimulation > totalDuration)
            {
                Options.Model.SimulationOptions.DaysPerSimulation = totalDuration;
            }


            var simulationStartDate = Options.Model.SimulationOptions.Start;
            var simulationEndDate = Options.Model.SimulationOptions.Start.AddDays(Options.Model.SimulationOptions.DaysPerSimulation - 1);

            var timer = Stopwatch.StartNew();

            while (simulationStartDate <= Options.Model.SimulationOptions.End)
            {
                timer.Restart();

                Log.Information($"Preparing simulation: {simulationStartDate.ToShortDateString()} - {simulationEndDate.ToShortDateString()}");

                if (!initializer.SetModelDateRange(simulationStartDate, simulationEndDate))
                    return new BasicResult(false, "Failed around setting model date range");

                if (!initializer.UpdateElementInitialConditions())
                    return new BasicResult(false, "Failed around setting element initial conditions");


                if (!patternManager.SetPatterns(simulationStartDate))
                    return new BasicResult(false, "Failed around setting patterns");

                //if (Options.Input.Ami.Enable)
                //    AMIManager.UpdateAMIPatterns(simulationStartDate, simulationEndDate);

                if (!pumpManager.UpdatePumpCurves())
                    return new BasicResult(false, "Failed around updating pump curves");


                // Save the model (for troubleshooting)
                if (Options.Model.SaveEachSimulation)
                {
                    var startDate = WaterModel.ActiveScenario.Options.SimulationStartDate;
                    var durationInHrs = WaterModel.Units.ConvertValue(
                        value: WaterModel.ActiveScenario.Options.Duration,
                        fromUnit: WaterModel.ActiveScenario.Options.Units.DurationUnit.GetUnit(),
                        toUnit: Unit.Hours);

                    var start = $"{startDate:s}".Replace(":", "");
                    var end = $"{startDate.AddHours(durationInHrs):s}".Replace(":", "");

                    var orginalModelFileInfo = new FileInfo(Options.Model.ModelFilePath);
                    var fileNamePortion = $"{orginalModelFileInfo.Name.Replace(".wtg", "")}__{start}__{end}.wtg";
                    var fileName = System.IO.Path.Combine(Options.Output.TsdOutputOptions.OutputDir, "Models", fileNamePortion);

                    var fileInfo = new FileInfo(fileName);
                    if (!fileInfo.Directory.Exists)
                        fileInfo.Directory.Create();

                    // NOTE: <ANIR> Currently [Jan 2023] there is a bug in OFW where WaterModel.SaveAs
                    // doesn't work when a project is opened from application layers.
                    if (WaterApplicationManager.GetInstance()?.ParentFormUIModel != null)
                    {
                        WaterApplicationManager.GetInstance().ParentFormModel.SaveAsProject(
                            new ProjectProperties() { NominalProjectPath = fileName });
                    }
                    else
                    {
                        WaterModel.SaveAs(fileName);
                    }
                    Log.Debug($"WaterGEMS file created: {fileName}.");
                }


                if (!RunSimulation(continueOnSimFailure))
                    return new BasicResult(false, "Failed around running simulation");

                /*if (!scadaManager.RecordScada())
                    return new BasicResult(false, "Failed around recording SCADA");*/




                /*if (!Options.Input.Ami.Enable)
                    if (!billingManager.RecordBilling())
                        return new BasicResult(false, "Failed around recording billing");*/


                Log.Information($"This round of simulation completed. [{simulationStartDate} - {simulationEndDate}] Time taken: {timer.Elapsed}");
                Log.Debug(new string('-', 50));


                //if (!Options.Input.Ami.Enable)
                //    if (!billingManager.PopulateBillingTable())
                //        return new BasicResult(false, "Failed around populating billing table");

                /*if (!outputManager.ExportOutput())
                    return new BasicResult(false, "Failed around exporting output");*/


                // Notification simulation completed
                // so that data can be extracted
                Log.Debug($"About to call external method after simulation complete");
                onSimulationStepComplete(outputManager);
                Log.Debug($"Call to external method after simulation completed");
                Log.Debug(new string('•', 100));

                // update start and end date for next round
                simulationStartDate = simulationEndDate.AddDays(1);
                simulationEndDate = simulationStartDate.AddDays(Options.Model.SimulationOptions.DaysPerSimulation - 1);
                if (simulationEndDate > Options.Model.SimulationOptions.End)
                    simulationEndDate = Options.Model.SimulationOptions.End.Date;
            }

        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            message = $"...while running the SCADA data generator";
            Log.Error(ex, message);
            return new BasicResult(false, message);
        }

        message = $"SCADA data generator compute completed. Time taken: {timerFull.Elapsed}";
        timerFull.Stop();

        Log.Information(message);
        Log.Debug(new string('x', 100));

        return new BasicResult(true, message);
    }
    #endregion


    #region Private Methods
    private bool RunSimulation(bool continueOnSimFailure)
    {
        Log.Information($"Running simulation...");
        var success = false;

        var timer = Stopwatch.StartNew();

        Log.Debug($"Validating active scenario.");
        var userNotifications = WaterModel.ActiveScenario.Validate();
        if(userNotifications.Length > 0)
        {
            Log.Information($"Validation message found.");
            foreach (var un in userNotifications)
                Log.Error($"User Notification: {un.ToString(WaterModel)}");
        }
        else
            Log.Information($"No UserNotification messages found");


        try
        {
            WaterModel.RunActiveScenario();
            Log.Information($"Simulation completed. Time taken: {timer.Elapsed}");

            timer.Restart();
            Log.Debug($"About to run pump energy calculations...");
            RunPumpEnergyEngine();
            Log.Information($"Pump energy run completed. Time taken: {timer.Elapsed}");

            success = true;
        }
        catch (EngineFatalErrorException ex)
        {
#if DEBUG
            // Debugger.Break();
#endif
            Log.Error(ex, $"WaterGEMS Engine Error while calculating simulation.");
            for (int i = 0; i < ex.UserNotifications.Length; i++)
                Log.Error($"User Notification {i}: {ex.UserNotifications[i].ToString(WaterModel)}");

            success = continueOnSimFailure; 
        }

        if (!WaterModel.ActiveScenario.HasResults)
        {
            Log.Error($"Model has no results.");
            success = false;
        }


        // TODO <sjac> Add checks for model warnings/errors.
        return success;
    }

    public void RunPumpEnergyEngine()
    {
        try
        {
            var engine = WaterModel.DomainDataSet.NumericalEngine(StandardCalculationOptionFieldName.EnergyCostEngine);
            var scenarios = new ModelingElementCollection
            {
                WaterModel.DomainDataSet.ScenarioManager.Element(WaterModel.ActiveScenario.Id)
            };

            engine.Run(scenarios);
        }
        catch (EngineFatalErrorException ef)
        {
            Log.Warning($"Engine fatal error found");

            var uns = ef.UserNotifications;
            foreach (var un in uns)
            {
                Log.Error($"UserNotification: {un.ToString(WaterModel)}");
            }
        }
        catch (EngineWarningsException ew)
        {
            Log.Warning($"Engine warnings found");
            var uns = ew.UserNotifications;
            foreach (var un in uns)
            {
                Log.Error($"UserNotification: {un.ToString(WaterModel)}");
            }
        }


    }
 /*   private IEnergyCostPumpElementAdapter[] GetPumpAdaptors()
    {
        var pumpManager = WaterModel.DomainDataSet.DomainElementManager((int)DomainElementType.PumpElementManager);
        var activeScenarioId = WaterModel.ActiveScenario.Id;



        return WaterModel.Network.Pumps.Elements()
            .Select(p => new EnergyCostPumpElementAdapter(
            pumpElementManager: pumpManager,
            elementId: p.Id,
            selectedScenarioId: activeScenarioId)).ToArray();
    }
*/
    #endregion


    #region Private Properties
    private IWaterModel WaterModel { get; }
    private SCADADataGeneratorOptions Options { get; }
    #endregion

}

[DebuggerDisplay("{ToString()}")]
public class BasicResult
{
    #region Constructor
    public BasicResult()
        : this(true, "")
    {
    }
    public BasicResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
    #endregion

    #region Public Methods
    public override string ToString()
    {
        return $"[{Success}]: {Message} @ {At}";
    }
    #endregion

    #region Public Properties
    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime At => DateTime.Now;
    #endregion
}