using Haestad.Calculations.Pressure;
using Haestad.Domain;
using Haestad.Domain.ModelingObjects.Water.Support.PumpDefinitions;
using Haestad.Support.Library;
using Haestad.Support.Support;
using Haestad.Support.Units;
using Haestad.Water.Forms.Support.Components.Curves.PumpDefinitions;
using OpenFlows.Domain.DataObjects;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Domain;

public class Curves
{
    #region Constructor
    public Curves(IWaterModel waterModel)
    {
        WaterModel = waterModel;
    }
    #endregion

    #region Public methods
    public Dictionary<string, List<GeometryPoint>> Patterns(bool includeIdInLabel = false)
    {
        var patterns = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pattern in WaterModel.Components.Patterns.Elements())
        {
            var points = pattern.PatternCurve.Get().Select(p => new GeometryPoint(p.TimeFromStart, p.Multiplier));
            var key = includeIdInLabel ? pattern.IdLabel() : pattern.Label;

            patterns.Add(key, points.ToList());
        }

        return patterns;
    }
    //public Dictionary<string, List<GeometryPoint>> TankCurves(bool includeIdInLabel = false)
    //{
    //    var curves = new Dictionary<string, List<GeometryPoint>>();

    //    foreach (var tank in WaterModel.Network.Tanks.Elements())
    //    {
    //        var xCurve = tank.Input.CrossSectionCurve;
    //        if (xCurve == null)
    //            continue;

    //        var key = includeIdInLabel ? tank.IdLabel() : tank.Label;
    //        curves.Add(key, xCurve.Get().Select(c => new GeometryPoint(c.DepthRatio, c.VolumeRatio)).ToList());
    //    }

    //    return curves;
    //}


    /// <summary>
    /// Flow and Head curve. Flow is in X value, Head is in Y value
    /// </summary>
    /// <returns></returns>
    public List<GeometryPoint> PumpHeadCurve(IPumpDefinition pumpDef, bool generateBasedOnCoefficientABC = true)
    {
        var head = pumpDef.Head;
        switch (pumpDef.Head.PumpDefinitionType)
        {
            case PumpDefinitionType.DepthFlowVariableSpeed:
            case PumpDefinitionType.DepthFlow:
            case PumpDefinitionType.VolumeFlow:
            case PumpDefinitionType.ConstantPower:
                break;

            case PumpDefinitionType.MultiplePoint:
                return head.PumpCurve.Get().Select(p => new GeometryPoint(p.Flow, p.Head)).ToList();

            case PumpDefinitionType.CustomExtended:
                if (!generateBasedOnCoefficientABC)
                {
                    return new List<GeometryPoint>()
                        {
                            new GeometryPoint(0, head.ShutoffHead),
                            new GeometryPoint(head.DesignFlow, head.DesignHead),
                            new GeometryPoint(head.MaxOperatingFlow, head.MaxOperatingHead),
                            new GeometryPoint(head.MaxExtendedFlow, 0),
                        };
                }
                else
                    return GetPumpHeadCurveBasedOnCoefficients(pumpDef);

            case PumpDefinitionType.StandardExtended:
            case PumpDefinitionType.Standard:
                if (!generateBasedOnCoefficientABC)
                {
                    return new List<GeometryPoint>()
                        {
                            new GeometryPoint(0, head.ShutoffHead),
                            new GeometryPoint(head.DesignFlow, head.DesignHead),
                            new GeometryPoint(head.MaxOperatingFlow, head.MaxOperatingHead),
                        };
                }
                else
                    return GetPumpHeadCurveBasedOnCoefficients(pumpDef);

            case PumpDefinitionType.DesignPoint:
                if (!generateBasedOnCoefficientABC)
                {
                    var designPointMngr = new PumpDesignPointElementManager(
                    domainDataSet: this.WaterModel.DomainDataSet,
                    elementId: () => pumpDef.Id,
                    pumpDefinitionType: PumpDefinitionTypeEnum.DesignPointType);

                    var points = new List<GeometryPoint>();

                    var headField = this.WaterModel.Components.PumpDefinitions.InputFields.FieldByName(StandardFieldName.MaxOperatingHead);
                    var headUnit = headField.Unit.GetUnit();

                    var flowField = this.WaterModel.Components.PumpDefinitions.InputFields.FieldByName(StandardFieldName.MaxOperatingDischarge);
                    var flowUnit = flowField.Unit.GetUnit();

                    var dataRows = designPointMngr.Elements();
                    foreach (PumpDesignPointElement row in dataRows)
                        points.Add(new GeometryPoint(
                            flowUnit.ConvertFrom(row.Flow, flowField.StorageUnit),
                            headUnit.ConvertFrom(row.Head, headField.StorageUnit)));

                    return points;
                }
                else
                {
                    return GetPumpHeadCurveBasedOnCoefficients(pumpDef);
                }

            default:
                break;
        }

        return new List<GeometryPoint>();
    }

    /// <summary>
    /// Flow and Head curve. Flow is in X value, Head is in Y value
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<GeometryPoint>> PumpHeadCurves(/*bool includeIdInLabel = false,*/bool generateCurveBasedOnCoefficientABC = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pumpDef in WaterModel.Components.PumpDefinitions.Elements())
        {
            //var key = includeIdInLabel ? pumpDef.IdLabel() : pumpDef.Label;
            var key = pumpDef.Label;
            curves.Add(key, PumpHeadCurve(pumpDef, generateCurveBasedOnCoefficientABC));
        }

        return curves;
    }


    /// <summary>
    /// Flow Efficiency curve, Flow is in X value, Efficiency is in Y value
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<GeometryPoint>> PumpFlowEfficiencyCurves(/*bool includeIdInLabel = false,*/ bool generateBasedonCoefficientABC = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pumpDef in WaterModel.Components.PumpDefinitions.Elements())
        {
            //var key = includeIdInLabel ? pumpDef.IdLabel() : pumpDef.Label;
            var key = pumpDef.Label;

            switch (pumpDef.Efficiency.PumpEfficiencyType)
            {
                case PumpEfficiencyTypeEnum.ConstantEfficiencyType:
                    var efficiency = pumpDef.Efficiency.ConstantEfficiency;
                    var pumpHeadCurve = PumpHeadCurve(pumpDef, /*includeIdInLabel,*/ generateBasedonCoefficientABC);
                    curves.Add(key, pumpHeadCurve.Select(c => new GeometryPoint(c.X, efficiency)).ToList());
                    break;

                case PumpEfficiencyTypeEnum.BestEfficiencyPointType:
                    pumpHeadCurve = PumpHeadCurve(pumpDef, /*includeIdInLabel,*/ generateBasedonCoefficientABC);

                    var woPumpDefManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.IdahoPumpDefinitionElementManager);                    

                    IEfficiencyCurveCalculator effiCalculator = EfficiencyCurveCalculatorBase.NewEfficiencyCalculator(pumpDef.Efficiency.PumpEfficiencyType, woPumpDefManager);
                    var minFlow = pumpHeadCurve.First().X;
                    var maxFlow = pumpHeadCurve.Last().X;
                    var success = effiCalculator.Compute(pumpDef.Id, minFlow, maxFlow);
                    var effiValues = effiCalculator.GetEfficiencyField().GetEfficiency();
                    var flowValues = effiCalculator.GetEfficiencyFlowField().GetFlow();


                    var flowEfficiencyPoints = new List<GeometryPoint>();

                    // Get the first value
                    flowEfficiencyPoints.Add(new GeometryPoint(flowValues[0], Math.Round(effiValues[0]*100, 3)));

                    // Get the middle values
                    var interval = flowValues.Length / 9;
                    for (int i = 1; i < 9; i++)
                    {
                        int index = i * interval;
                        flowEfficiencyPoints.Add(new GeometryPoint(flowValues[index], Math.Round(effiValues[index] *100, 3)));
                    }

                    // Get the last value
                    flowEfficiencyPoints.Add(new GeometryPoint(flowValues[flowValues.Length-1], Math.Round(effiValues[flowValues.Length-1]*100, 3)));


                    curves.Add(key, flowEfficiencyPoints);
                    break;

                case PumpEfficiencyTypeEnum.MultipleEfficiencyPointsType:
                    var pumpCurve = pumpDef.Efficiency.FlowEfficiencyCurve;
                    if (pumpCurve == null)
                        continue;

                    curves.Add(key, pumpCurve.Get().Select(p => new GeometryPoint(p.Flow, p.Efficiency)).ToList());
                    break;

                default:
                    break;
            }
        }

        return curves;
    }

    

    public Dictionary<string, List<GeometryPoint>> PumpMotorAndDriveEffiCurves(bool includeIdInLabel = false, bool dropDriveEfficiency = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pumpDef in WaterModel.Components.PumpDefinitions.Elements())
        {
            var pumpCurve = pumpDef.Motor.SpeedEfficiencyCurve;
            if (pumpCurve == null)
                continue;

            var key = includeIdInLabel ? pumpDef.IdLabel() : pumpDef.Label;
            
            curves.Add(key, pumpCurve.Get().Select(p 
                => new GeometryPoint(
                    p.Speed, 
                    dropDriveEfficiency ? p.Efficiency/pumpDef.Motor.MotorEfficiency * 100 : p.Efficiency)
                ).ToList());
        }

        return curves;
    }
    #endregion


    #region Private Methods
    private Tuple<double, double, double> GetEfficiencyConstants(double bepEfficiency, double bepFlow, double maxFlow)
    {
        // Efficiency = A * Flow ^2 + B * Flow + C
        var A = -1 * bepEfficiency / (bepFlow * bepFlow + maxFlow * maxFlow - 2 * bepFlow * maxFlow);
        var B = 2 * bepEfficiency * bepFlow / Math.Pow(maxFlow - bepFlow, 2);
        var C = bepEfficiency - bepEfficiency * bepFlow * bepFlow / Math.Pow(maxFlow - bepFlow, 2);
                

        return new Tuple<double, double, double>(A, B, C);
    }
    private List<GeometryPoint> GetPumpHeadCurveBasedOnCoefficients(IPumpDefinition pumpDef)
    {

        // NOTE: unit must be either in ft/cfs or m or m3/s
        var headUnit = pumpDef.Units.HeadUnit;
        var flowUnit = pumpDef.Units.FlowUnit;
        var originalFlowUnit = flowUnit.GetUnit();
        var flowConversionFactor = 1.0;


        if (headUnit.GetUnit() == Unit.Feet)
        {
            flowConversionFactor = flowUnit.ConvertTo(flowConversionFactor, Unit.CFS);
            pumpDef.Units.FlowUnit.SetUnit(Unit.CFS);
        }
        else if (headUnit.GetUnit() == Unit.MeterMeters)
        {
            pumpDef.Units.FlowUnit.SetUnit(Unit.CubicMetersPerSecond);
            flowConversionFactor = flowUnit.ConvertTo(flowConversionFactor, Unit.CubicMetersPerSecond);
        }
        else
            throw new InvalidOperationException("The head unit must either be 'ft' or 'm'.");


        var A = pumpDef.Head.CoefficientA;
        var B = pumpDef.Head.CoefficientB;
        var C = pumpDef.Head.CoefficientC;

        var pumpCurvePoints = new List<GeometryPoint>();

        // Head = A - (B * Flow ^ C)

        // Far right end point (last flow point in the curve)
        var maxFlow = Math.Pow(A / B, 1 / C);


        var numberOfPointsOnTheCurve = 10; // one extra, last point, will be added in the loop. Total count will be 10 + 1
        var flowInterval = maxFlow / numberOfPointsOnTheCurve; // to create 10 points along the curve

        for (int i = 0; i <= numberOfPointsOnTheCurve; i++)
        {
            var flow = i * flowInterval;
            var head = A - (B * Math.Pow(flow, C));
            pumpCurvePoints.Add(new GeometryPoint(Math.Round(flow / flowConversionFactor, 3), Math.Round(head, 3)));
        }


        // convert the flow unit back to display unit
        pumpDef.Units.FlowUnit.SetUnit(originalFlowUnit);

        return pumpCurvePoints;
    }
    #endregion

    #region Private Methods
    private IPumpDefinitionCalculator NewPumpDefinitionCalculator(PumpDefinitionType pumpDefinitionType)
    {
        ISupportElementManager pumpDefinitionManager =WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.IdahoPumpDefinitionElementManager);
        switch (pumpDefinitionType)
        {
            case PumpDefinitionType.ConstantPower:
                return new PumpDefinitionConstantPowerCalculator(pumpDefinitionManager);
            case PumpDefinitionType.DesignPoint:
            case PumpDefinitionType.Standard:
                return new PumpDefinitionDesignPointCalculator(pumpDefinitionManager);
            case PumpDefinitionType.StandardExtended:
                return new PumpDefinitionDesignPointExtendedCalculator(pumpDefinitionManager,
                    ExtendedPumpDefinitionType.Standard);
            case PumpDefinitionType.CustomExtended:
                return new PumpDefinitionDesignPointExtendedCalculator(pumpDefinitionManager,
                    ExtendedPumpDefinitionType.Custom);
            case PumpDefinitionType.MultiplePoint:
                return new PumpDefinitionMultiplePointCalculator(pumpDefinitionManager);
            
            default:
                throw new ArgumentException();
        }

        return null;
    }
    #endregion

    #region Private Properties
    private IWaterModel WaterModel { get; set; }
    #endregion
}


