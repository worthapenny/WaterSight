using Haestad.Domain;
using Haestad.Support.Units;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Model.Generator.Data;

public class PumpManager
{
	#region Constructor
	public PumpManager(IWaterModel waterModel, SCADADataGeneratorOptions options/*, Randomizer randomizer*/)
	{
		WaterModel = waterModel;
		Options = options;
        /*Randomizer = randomizer;*/
    }
    #endregion

    #region Public Methods
    public bool UpdatePumpCurves()
    {
        Log.Debug("Updating pump curves...");

        DateTime startDate = Options.Model.SimulationOptions.Start;
        DateTime endDate = Options.Model.SimulationOptions.End;
        DateTime currentDate = WaterModel.ActiveScenario.Options.SimulationStartDate;

        if(ChangingPumps == null || ChangingPumps.Count == 0)
        {
            InitializeChangingPumpCurves();
        }

        foreach (ChangingPump changingPump in ChangingPumps)
        {
            PumpCoefficients coefficients = GetCoefficientsForDate(changingPump, startDate, endDate, currentDate);
            SetMultiplePumpPoints(changingPump.Pump.Input.PumpDefinition, coefficients, changingPump.DischargeUnit, changingPump.HeadUnit);
        }

        return true;
    }
    #endregion

    #region Private Methods
    private bool InitializeChangingPumpCurves()
    {
        Log.Debug("Initializing Changing Pumps.");
        ChangingPumps = new List<ChangingPump>();

        if (Options.ChangingPumps is null)
        {
            Log.Information("No Changing Pumps defined.");
            return true;
        }

        foreach (var changingPumpOptions in Options.ChangingPumps)
        {
            IPump pump = WaterModel.Network.Pumps.Element(changingPumpOptions.Id);
            if (pump is null)
            {
                Log.Debug($"Pump with ID {changingPumpOptions.Id} was not found, and will be skipped.");
                continue;
            }
            if (!pump.Input.IsActive)
            {
                Log.Debug($"Pump {pump.Label} with ID {pump.Id} is inactive, and will be skipped.");
                continue;
            }

            PumpCoefficients startCoefficients = new PumpCoefficients(changingPumpOptions.StartA, changingPumpOptions.StartB, changingPumpOptions.StartC);
            PumpCoefficients endCoefficients = new PumpCoefficients(changingPumpOptions.EndA, changingPumpOptions.EndB, changingPumpOptions.EndC);

            Unit dischargeUnit;
            Unit headUnit;

            try
            {
                dischargeUnit = Unit.FromLabel(Dimension.Flow, changingPumpOptions.DischargeUnit);
                if(dischargeUnit == null)
                    Log.Debug($"Discharge Unit {changingPumpOptions.DischargeUnit} for pump {changingPumpOptions.Id} was not recognized as a valid flow unit. This pump will be skipped");
            }
            catch
            {
#if DEBUG
                Debugger.Break();
#endif
                Log.Debug($"Discharge Unit {changingPumpOptions.DischargeUnit} for pump {changingPumpOptions.Id} was not recognized as a valid flow unit. This pump will be skipped");
                continue;
            }

            try
            {
                headUnit = Unit.FromLabel(Dimension.Length, changingPumpOptions.HeadUnit);
            }
            catch
            {
                Log.Debug($"Head Unit {changingPumpOptions.HeadUnit} for pump {changingPumpOptions.Id} was not recognized as a valid length unit.");
                continue;
            }

            ChangingPump changingPump = new ChangingPump(pump, startCoefficients, endCoefficients, dischargeUnit, headUnit);

            ChangingPumps.Add(changingPump);
        }

        foreach (ChangingPump changingPump in ChangingPumps)
        {
            IPump pump = changingPump.Pump;

            var existingPumpDefinition = pump.Input.PumpDefinition;

            // Copy the existing pump definition, so any efficiency values remain available
            ISupportElementManager pumpDefinitionManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.IdahoPumpDefinitionElementManager);
            int newID = pumpDefinitionManager.Copy(existingPumpDefinition.Id);
            var newPumpDefinition = WaterModel.Components.PumpDefinitions.Element(newID);
            newPumpDefinition.Label = $"{newPumpDefinition.Label} - Modified for {pump.Label}";

            // Switch to a multiple point head type, and initialize the curve values.
            newPumpDefinition.Head.PumpDefinitionType = Haestad.Calculations.Pressure.PumpDefinitionType.MultiplePoint;
            SetMultiplePumpPoints(newPumpDefinition, changingPump.StartCoefficients, changingPump.DischargeUnit, changingPump.HeadUnit);

            pump.Input.PumpDefinition = newPumpDefinition;
        }

        return true;
    }

    private void SetMultiplePumpPoints(IPumpDefinition pumpDefinition, PumpCoefficients pumpCoefficients, Unit dischargeCoefficientUnit, Unit headCoefficientUnit)
    {
        var pumpCurvePoints = pumpDefinition.Head.PumpCurve.Get();

        Unit dischargeDisplayUnit = pumpDefinition.Units.FlowUnit.GetUnit();
        Unit headDisplayUnit = pumpDefinition.Units.HeadUnit.GetUnit();

        pumpCurvePoints.Clear();

        double maxDischarge = Math.Pow((pumpCoefficients.A / pumpCoefficients.B), (1.0 / pumpCoefficients.C));

        int numSteps = 6;

        for (double discharge = 0; discharge < maxDischarge; discharge += maxDischarge / numSteps)
        {
            double head = pumpCoefficients.A - pumpCoefficients.B * Math.Pow(discharge, pumpCoefficients.C);
            if (head < 0) // Sometimes the last point will yield a slightly negative head, which breaks in WaterGEMS.
                continue;
            pumpCurvePoints.Add(WaterModel.Units.ConvertValue(discharge, dischargeCoefficientUnit, dischargeDisplayUnit), WaterModel.Units.ConvertValue(head, headCoefficientUnit, headDisplayUnit));
        }

        pumpDefinition.Head.PumpCurve.Set(pumpCurvePoints);
    }

    private PumpCoefficients GetCoefficientsForDate(ChangingPump pump, DateTime startDate, DateTime endDate, DateTime currentDate)
    {
        if (currentDate == startDate)
            return pump.StartCoefficients;
        double ratio = (currentDate - startDate).TotalDays / (endDate - startDate).TotalDays;

        double A = LinearInterpolate(pump.StartCoefficients.A, pump.EndCoefficients.A, ratio);
        double B = LinearInterpolate(pump.StartCoefficients.B, pump.EndCoefficients.B, ratio);
        double C = LinearInterpolate(pump.StartCoefficients.C, pump.EndCoefficients.C, ratio);

        PumpCoefficients coefficients = new PumpCoefficients(A, B, C);
        return coefficients;
    }

    private double LinearInterpolate(double a, double b, double ratio)
    {
        return a + (b - a) * ratio;
    }
    #endregion

    #region Private Properties
    private IWaterModel WaterModel { get; }
    private SCADADataGeneratorOptions Options { get; }
    //private Randomizer Randomizer { get; }

    private IList<ChangingPump> ChangingPumps { get; set; }
    #endregion

    #region Internal Struct
    internal struct PumpCoefficients
    {
        internal PumpCoefficients(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
        internal double A { get; }
        internal double B { get; }
        internal double C { get; }
    }

    internal struct ChangingPump
    {
        internal ChangingPump(IPump pump, PumpCoefficients startCoefficients, PumpCoefficients endCoefficients, Unit dischargeUnit, Unit headUnit)
        {
            Pump = pump;
            StartCoefficients = startCoefficients;
            EndCoefficients = endCoefficients;
            DischargeUnit = dischargeUnit;
            HeadUnit = headUnit;
        }

        internal IPump Pump { get; }
        internal PumpCoefficients StartCoefficients { get; }
        internal PumpCoefficients EndCoefficients { get; }
        internal Unit DischargeUnit { get; }
        internal Unit HeadUnit { get; }
    }
    #endregion
}