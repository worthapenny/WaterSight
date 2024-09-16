using Haestad.Support.Units;
using Niraula.Extensions.Water.Support;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterSight.Model.Generator.Data;

public class PatternManager
{
    #region Constants
    public const string OmitFromBillsStr = "Omit From Bills";
    #endregion

    #region Constructor
    public PatternManager(IWaterModel waterModel, SCADADataGeneratorOptions options, Randomizer randomizer, EnvironmentalManager environmentalManager)
    {
        WaterModel = waterModel;
        Options = options;
        Randomizer = randomizer;
        EnvironmentalManager = environmentalManager;

        // Adding one day so the date range is inclusive.
        TotalDuration = (options.Model.SimulationOptions.End.AddDays(1) - options.Model.SimulationOptions.Start).TotalHours;
    }
    #endregion

    #region Public Methods
    public bool InitializePatterns()
    {
        Log.Debug("Initializing patterns.");

        foreach (IPattern pattern in WaterModel.Components.Patterns.Elements())
        {
            if (pattern.PatternCategory != Haestad.Calculations.Pressure.PatternCategory.PatternCategoryHydraulic)
                continue;

            RemoveMonthlyFactors(pattern);

            LoopPatternToDurationAndTimestep(pattern);

            ApplyPatternVariability(pattern);
        }

        Log.Information("Initialized patterns.");
        return true;
    }

    public bool InitializeSupplementalDemandPatterns()
    {
        Log.Debug("Initializing User-Defined Supplemental Demand Patterns.");

        int patternLength = (int)(TotalDuration / Options.Model.SimulationOptions.TimeStepsHours);



        if (Options.RandomDemandNodes is null)
        {
            Log.Information("No Random Demand Nodes defined.");
        }
        else
        {
            foreach (var randomDemandNode in Options.RandomDemandNodes)
            {
                IJunction element = WaterModel.Network.Junctions.Element(randomDemandNode.Id);
                if (element is null)
                {
                    Log.Warning($"Random Demand junction with id {randomDemandNode.Id} was not found and will be skipped. (Currently only Junction nodes are supported.)");
                    continue;
                }

                var pattern = WaterModel.Components.Patterns.Create();

                IDemands demands = element.Input.DemandCollection.Get();
                demands.Add(1.0, pattern);
                element.Input.DemandCollection.Set(demands);

                pattern.PatternFormat = Haestad.Calculations.Pressure.PatternFormat.Continuous;
                pattern.Notes = OmitFromBillsStr; // Used by the billing manager to omit this from bills.

                double avgFlowRateDisplayUnits = WaterModel.Units.ConvertValue(randomDemandNode.FlowRateGPM, Unit.GPM, WaterModel.Units.NetworkUnits.Pipe.FlowUnit.GetUnit());
                double avgLinearGrowthDurationHours = randomDemandNode.LinearGrowthDurationHours;
                double avgConstantDurationHours = randomDemandNode.ConstantDurationHours;
                double minimumGapBetweenEventsHours = randomDemandNode.MinimumGapBetweenEventsHours;

                double flowVariablityFactor = randomDemandNode.FlowVariabilityPercent / 100;
                double durationVariabilityFactor = randomDemandNode.DurationVariabilityPercent / 100;
                double timestepPercentChance = randomDemandNode.DailyPercentChance / (24 / Options.Model.SimulationOptions.TimeStepsHours); // Chance of an event starting at any particular timestep.

                Tuple<double[], double[]> curvePoints = new Tuple<double[], double[]>(new double[patternLength], new double[patternLength]);

                int i = 0;
                double hoursFromStart = 0;
                double lastEventEndHour = 0;
                while (i < patternLength)
                {
                    if ((hoursFromStart - lastEventEndHour > minimumGapBetweenEventsHours) && (Randomizer.RandomBetween(0, 100) < timestepPercentChance))
                    {
                        double eventStartHour = hoursFromStart;
                        // Start a new event
                        double peakFlowRatedDisplayUnits = avgFlowRateDisplayUnits * Randomizer.RandomBetween(1 - flowVariablityFactor, 1 + flowVariablityFactor);
                        double linearGrowthDurationHours = avgLinearGrowthDurationHours * Randomizer.RandomBetween(1 - durationVariabilityFactor, 1 + durationVariabilityFactor);
                        double constantDurationHours = avgConstantDurationHours * Randomizer.RandomBetween(1 - durationVariabilityFactor, 1 + durationVariabilityFactor);

                        while ((hoursFromStart - eventStartHour < linearGrowthDurationHours) && (i < patternLength))
                        {
                            curvePoints.Item1[i] = hoursFromStart;
                            curvePoints.Item2[i] = LinearInterpolate(0, peakFlowRatedDisplayUnits, (hoursFromStart - eventStartHour) / (linearGrowthDurationHours));

                            i += 1;
                            hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;
                        }
                        while ((hoursFromStart - eventStartHour < linearGrowthDurationHours + constantDurationHours) && (i < patternLength))
                        {
                            curvePoints.Item1[i] = hoursFromStart;
                            curvePoints.Item2[i] = peakFlowRatedDisplayUnits;

                            i += 1;
                            hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;
                        }
                        lastEventEndHour = hoursFromStart;
                    }
                    else
                    {
                        curvePoints.Item1[i] = hoursFromStart;
                        curvePoints.Item2[i] = 0;

                        i += 1;
                        hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;
                    }
                }

                curvePoints.Item2[patternLength - 1] = 0; // Force the last point to zero so it matches the starting multiplier. This may end an event one timestep sooner, which is not an issue.
                pattern.PatternStartingMultiplier = 0;

                PatternCurves[pattern.Id] = curvePoints;

            }
        }

        if (Options.ContinualDemandNodes is null)
        {
            Log.Information("No Continual Demand Nodes defined.");
        }
        else
        {
            foreach (var continualDemandNode in Options.ContinualDemandNodes)
            {
                IJunction element = WaterModel.Network.Junctions.Element(continualDemandNode.Id);
                if (element is null)
                {
                    Log.Warning($"Constant Demand junction with id {continualDemandNode.Id} was not found and will be skipped. (Currently only Junction nodes are supported.)");
                    continue;
                }

                double flowInDisplayUnits = WaterModel.Units.ConvertValue(continualDemandNode.FlowRateGPM, Unit.GPM, WaterModel.Units.NetworkUnits.Pipe.FlowUnit.GetUnit());
                var pattern = WaterModel.Components.Patterns.Create();

                IDemands demands = element.Input.DemandCollection.Get();
                demands.Add(flowInDisplayUnits, pattern);
                element.Input.DemandCollection.Set(demands);

                pattern.Notes = OmitFromBillsStr; // Used by the billing manager to omit this from bills.
                pattern.PatternFormat = Haestad.Calculations.Pressure.PatternFormat.Continuous;

                // Make a simple constant pattern. Not using the 'Fixed' pattern so we can use the Notes field to omit from bills.
                // TODO: This could be simplified - we could just make one constant pattern to be omitted, and re-use it. It probably doesn't need to have one step per timestep.
                pattern.PatternStartingMultiplier = 1;

                PatternCurves[pattern.Id] = new Tuple<double[], double[]>(new double[1], new double[1]);
                Tuple<double[], double[]> curvePoints = new Tuple<double[], double[]>(new double[patternLength], new double[patternLength]);

                int i = 0;
                double hoursFromStart = 0;
                while (i < patternLength)
                {
                    curvePoints.Item1[i] = hoursFromStart;
                    curvePoints.Item2[i] = 1;

                    i += 1;
                    hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;
                }

                PatternCurves[pattern.Id] = curvePoints;
            }
        }

        Log.Debug("Initialized User-Defined Supplemental Demand Patterns.");
        return true;
    }

    public bool SetPatterns(DateTime simulationStartDate)
    {
        Log.Debug($"Setting patterns for start date: {simulationStartDate.ToShortDateString()}");

        double modelDuration = WaterModel.ActiveScenario.Options.Duration;
        int numTimesteps = (int)(modelDuration / Options.Model.SimulationOptions.TimeStepsHours);

        int startTimestep = (int)((simulationStartDate - Options.Model.SimulationOptions.Start).TotalHours / Options.Model.SimulationOptions.TimeStepsHours);
        int endTimestep = startTimestep + numTimesteps;

        foreach (IPattern pattern in WaterModel.Components.Patterns.Elements())
        {
            if (pattern.PatternCategory != Haestad.Calculations.Pressure.PatternCategory.PatternCategoryHydraulic)
                continue;

            // When using AMI, some patterns will not need to be updated and will not have been added to PatternCurves.
            if (!PatternCurves.ContainsKey(pattern.Id))
                continue;

            Tuple<double[], double[]> fullPatternCurvePoints = PatternCurves[pattern.Id];

            IPattern tempPattern = WaterModel.Components.Patterns.Create();
            IPatternCurve newCurve = tempPattern.PatternCurve.Get();
            tempPattern.Delete();

            // Extract the portion of the entire pattern that is relevent for the current model simulation. 
            double hoursFromSimulationStart = 0;
            int hoursFromPatternStart = startTimestep;
            double startingMultiplier = fullPatternCurvePoints.Item2[startTimestep];
            for (int i = startTimestep; i < endTimestep; i++)
            {
                hoursFromSimulationStart += Options.Model.SimulationOptions.TimeStepsHours;
                double value = fullPatternCurvePoints.Item2[i];

                newCurve.Add(hoursFromSimulationStart, value);
            }

            // Add one timestep to the end (beyond the model duration) so that the starting multiplier can equal the ending timestep. This last timestep will never be used in computation since it is after the duration.
            //    This method allows us to have a smooth pattern that progresses from one model iteration to the next without a discontinuity, while adhering to the WaterGEMS requirement that patterns must loop.
            hoursFromSimulationStart += Options.Model.SimulationOptions.TimeStepsHours;
            newCurve.Add(hoursFromSimulationStart, startingMultiplier);

            pattern.PatternStartingMultiplier = startingMultiplier;
            pattern.PatternCurve.Set(newCurve); // This line is slow for long patterns, because collections are not optimized for large numbers of points (in Haestad.Domain, not the API).
        }

        Log.Information($"Done setting patterns for start date: {simulationStartDate.ToShortDateString()}");
        return true;
    }

    public double GetAverageMultiplierForDay(IPattern pattern, DateTime date)
    {
        // Handle the default 'Fixed' pattern. The tool will leave this unmodified.
        if (pattern is null)
            return 1;

        Tuple<int, DateTime> key = new Tuple<int, DateTime>(pattern.Id, date);
        if (PatternAverageMultiplierForDay.ContainsKey(key))
            return PatternAverageMultiplierForDay[key];

        double averageMultiplierForDay = GetPatternHourlyAverageMultiplier(pattern, date);

        double dayOfWeekMultiplier = GetDayOfWeekMultiplier(pattern, date);

        averageMultiplierForDay *= dayOfWeekMultiplier;

        PatternAverageMultiplierForDay[key] = averageMultiplierForDay;
        return averageMultiplierForDay;
    }
    #endregion

    #region Private Methods
    private void RemoveMonthlyFactors(IPattern pattern)
    {
        // Override all monthly demand variation. Seasonal demand variation will be handled on a day-to-day basis.
        pattern.MonthlyMultipliers.January = 1;
        pattern.MonthlyMultipliers.February = 1;
        pattern.MonthlyMultipliers.March = 1;
        pattern.MonthlyMultipliers.April = 1;
        pattern.MonthlyMultipliers.May = 1;
        pattern.MonthlyMultipliers.June = 1;
        pattern.MonthlyMultipliers.July = 1;
        pattern.MonthlyMultipliers.August = 1;
        pattern.MonthlyMultipliers.September = 1;
        pattern.MonthlyMultipliers.October = 1;
        pattern.MonthlyMultipliers.November = 1;
        pattern.MonthlyMultipliers.December = 1;
    }

    private void LoopPatternToDurationAndTimestep(IPattern pattern)
    {
        IPatternCurve originalCurve = pattern.PatternCurve.Get();
        double originalCurveStartingMultiplier = pattern.PatternStartingMultiplier;
        int originalLength = originalCurve.Count();

        // Shift into a more efficient structure.
        Tuple<double[], double[]> originalCurvePoints = new Tuple<double[], double[]>(new double[originalLength], new double[originalLength]);
        for (int i = 0; i < originalLength; i++)
        {
            var point = originalCurve[i];
            originalCurvePoints.Item1[i] = point.TimeFromStart;
            originalCurvePoints.Item2[i] = point.Multiplier;
        }

        int newLength = (int)(TotalDuration / Options.Model.SimulationOptions.TimeStepsHours);

        Tuple<double[], double[]> newCurvePoints = new Tuple<double[], double[]>(new double[newLength], new double[newLength]);

        bool isStepwise = false;
        if (pattern.PatternFormat == Haestad.Calculations.Pressure.PatternFormat.Stepwise)
            isStepwise = true;

        double hoursFromStart = 0;
        for (int i = 0; i < newLength; i++)
        {
            hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;
            double loopedValue = GetCurveValue(originalCurvePoints, hoursFromStart, isStepwise, originalCurveStartingMultiplier);
            newCurvePoints.Item1[i] = hoursFromStart;
            newCurvePoints.Item2[i] = loopedValue;
        }

        PatternCurves[pattern.Id] = newCurvePoints;
    }

    private void ApplyPatternVariability(IPattern pattern)
    {
        Tuple<double[], double[]> originalCurvePoints = PatternCurves[pattern.Id];
        double originalCurveStartingMultiplier = pattern.PatternStartingMultiplier;

        int length = originalCurvePoints.Item1.Count();

        Tuple<double[], double[]> newCurvePoints = new Tuple<double[], double[]>(new double[length], new double[length]);

        bool isStepwise = false;
        if (pattern.PatternFormat == Haestad.Calculations.Pressure.PatternFormat.Stepwise)
            isStepwise = true;

        double offsetHours = 0;
        double hourlyPercentVariation = 0;
        if (!(Options.DemandVariability is null))
        {
            // Note: these default to zero if the user options has a demandVariability element but omits the numeric multiplier.                
            offsetHours = Options.DemandVariability.MaxPatternOffsetHours;
            hourlyPercentVariation = Options.DemandVariability.HourlyPercentVariation / 100.0;
        }

        //double dailyLeadLag = Randomizer.RandomBetween(-offsetHours, offsetHours, false);
        double leadLag = Randomizer.RandomBetween(-offsetHours, offsetHours, false);
        double previousLeadLag = leadLag;
        //double endOfDay = 24;



        // TODO <sjac>: something about the following loop is running slowly for unit tests, but only when the unit tests are run in series - not when a single test is run.
        double hoursFromStart = 0;
        for (int i = 0; i < length; i++)
        {
            hoursFromStart += Options.Model.SimulationOptions.TimeStepsHours;

            GetNewLeadLag(ref leadLag, ref previousLeadLag, offsetHours);

            double lookupHour = hoursFromStart + leadLag;
            double timeShiftedValue = GetCurveValue(originalCurvePoints, lookupHour, isStepwise, originalCurveStartingMultiplier);

            double hourlyVariationMultiplier = (1 + Randomizer.RandomBetween(-hourlyPercentVariation, hourlyPercentVariation, true));

            double environmentalMultiplier = EnvironmentalManager.GetEnvironmentalMultiplier(hoursFromStart);

            double variedValue = timeShiftedValue * hourlyVariationMultiplier * environmentalMultiplier;

            newCurvePoints.Item1[i] = hoursFromStart;
            newCurvePoints.Item2[i] = variedValue;
        }

        PatternCurves[pattern.Id] = newCurvePoints;
    }

    private void GetNewLeadLag(ref double leadLag, ref double previousLeadLag, double offsetHours)
    {
        double newLeadLag;
        double driftSpeed = Options.Model.SimulationOptions.TimeStepsHours;
        // Have the leadlag time slowly drift but not suddenly change.
        double coinToss = Randomizer.RandomBetween(0, 1);

        if (coinToss < 0.8)
        {
            // Continue drifting in the same direction with some random accelleration.
            newLeadLag = leadLag + (leadLag - previousLeadLag) + Randomizer.RandomBetween(-driftSpeed, driftSpeed, true);
        }
        else
        {
            // Switch direction
            newLeadLag = leadLag + (previousLeadLag - Options.Model.SimulationOptions.TimeStepsHours) + Randomizer.RandomBetween(-driftSpeed, driftSpeed, true);
        }
        if (newLeadLag > offsetHours)
            newLeadLag = offsetHours;
        if (newLeadLag < -offsetHours)
            newLeadLag = -offsetHours;

        previousLeadLag = leadLag;
        leadLag = newLeadLag;
    }

    private double GetDayOfWeekMultiplier(IPattern pattern, DateTime date)
    {
        double multiplier = 1;
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                multiplier = pattern.DailyMultipliers.Monday;
                break;
            case DayOfWeek.Tuesday:
                multiplier = pattern.DailyMultipliers.Tuesday;
                break;
            case DayOfWeek.Wednesday:
                multiplier = pattern.DailyMultipliers.Wednesday;
                break;
            case DayOfWeek.Thursday:
                multiplier = pattern.DailyMultipliers.Thursday;
                break;
            case DayOfWeek.Friday:
                multiplier = pattern.DailyMultipliers.Friday;
                break;
            case DayOfWeek.Saturday:
                multiplier = pattern.DailyMultipliers.Saturday;
                break;
            case DayOfWeek.Sunday:
                multiplier = pattern.DailyMultipliers.Sunday;
                break;
        }
        return multiplier;
    }

    private double GetPatternHourlyAverageMultiplier(IPattern pattern, DateTime date)
    {
        double startHour = (date - Options.Model.SimulationOptions.Start.Date).TotalHours;
        double endHour = startHour + 24;


        Tuple<double[], double[]> curvePoints = PatternCurves[pattern.Id];

        int length = curvePoints.Item1.Count();

        bool isStepwise = false;
        if (pattern.PatternFormat == Haestad.Calculations.Pressure.PatternFormat.Stepwise)
            isStepwise = true;

        double currentHour = 0;
        double currentMultiplier = pattern.PatternStartingMultiplier;

        double runningTotal = 0;

        // Compute a weighted average multiplier.
        for (int i = 0; i < length; i++)
        {
            if (currentHour < startHour)
            {
                currentHour = curvePoints.Item1[i];
                currentMultiplier = curvePoints.Item2[i];
                continue;
            }

            // Note: Technically this approach isn't exact if the stopHour falls between two curve points. For this tool, the approach is acceptable.
            double nextHour = curvePoints.Item1[i];
            double nextMultiplier = curvePoints.Item2[i];

            if (isStepwise)
                runningTotal += (currentMultiplier * (nextHour - currentHour));
            else
            {
                runningTotal += (((nextMultiplier + currentMultiplier) / 2) * (nextHour - currentHour));
            }
            currentHour = nextHour;
            currentMultiplier = nextMultiplier;

            if (nextHour >= endHour)
                break;
        }

        return (runningTotal / (endHour - startHour));
    }

    private double GetCurveValue(Tuple<double[], double[]> curvePoints, double targetHoursFromStart, bool isStepwise, double curveStartingMultiplier)
    {
        // Interpolate/extrapolate value.
        // TODO <sjac>: Handle the case where targetHoursFromStart is negative (which it can be near the begining of the simulation due to the random lookup variation. For now we simply use the starting multipler.
        if (targetHoursFromStart <= 0.0)
            return curveStartingMultiplier;

        int length = curvePoints.Item1.Count();
        double curveDuration = curvePoints.Item1[length - 1];

        int index = length;
        while (index >= length)
        {
            index = Array.BinarySearch(curvePoints.Item1, targetHoursFromStart);
            if (index >= 0)
                // Exact match was found.
                return curvePoints.Item2[index];
            // Index is either negative index for the next largest item, or the negative of the length of the array.
            index = ~index;
            if (index >= length)
                // The curve is shorter than the target hour. Cycle back to the beginning and continue.
                targetHoursFromStart -= curveDuration;
        }

        double previousTimeFromStart = 0;
        double previousMultiplier = curveStartingMultiplier;
        if (index > 0)
        {
            previousMultiplier = curvePoints.Item2[index - 1];
            previousTimeFromStart = curvePoints.Item1[index - 1];
        }

        if (isStepwise)
            return previousMultiplier;

        double nextTimeFromStart = curvePoints.Item1[index];
        double nextMultiplier = curvePoints.Item2[index];

        if (nextTimeFromStart == targetHoursFromStart)
            return nextMultiplier;

        double deltaValue = nextMultiplier - previousMultiplier;
        double deltaTime = nextTimeFromStart - previousTimeFromStart;
        double interpolatedDeltaTime = targetHoursFromStart - previousTimeFromStart;
        double interpolatedValue = previousMultiplier + (deltaValue / deltaTime) * interpolatedDeltaTime;
        return interpolatedValue;
    }

    private double LinearInterpolate(double a, double b, double ratio)
    {
        return a + (b - a) * ratio;
    }
    #endregion


    #region Private Properties
    private IWaterModel WaterModel { get; }
    private SCADADataGeneratorOptions Options { get; }

    private Randomizer Randomizer { get; }
    private EnvironmentalManager EnvironmentalManager { get; }

    private double TotalDuration { get; }
    private IDictionary<Tuple<int, DateTime>, double> PatternAverageMultiplierForDay { get; } = new Dictionary<Tuple<int, DateTime>, double>();
    private Dictionary<int, Tuple<double[], double[]>> PatternCurves { get; } = new Dictionary<int, Tuple<double[], double[]>>();
    #endregion

}
