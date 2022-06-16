using Haestad.Calculations.Pressure;
using Haestad.Domain;
using Haestad.Domain.ModelingObjects.Water.Support.PumpDefinitions;
using Haestad.Support.Support;
using Haestad.Water.Forms.Support.Components.Curves.PumpDefinitions;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using System.Collections.Generic;
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
    public Dictionary<string, List<GeometryPoint>> TankCurves(bool includeIdInLabel = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var tank in WaterModel.Network.Tanks.Elements())
        {
            var xCurve = tank.Input.CrossSectionCurve;
            if (xCurve == null)
                continue;

            var key = includeIdInLabel ? tank.IdLabel() : tank.Label;
            curves.Add(key, xCurve.Get().Select(c => new GeometryPoint(c.DepthRatio, c.VolumeRatio)).ToList());
        }

        return curves;
    }

    /// <summary>
    /// Flow and Head curve. Flow is in X value, Head is in Y value
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<GeometryPoint>> PumpHeadCurves(bool includeIdInLabel = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pumpDef in WaterModel.Components.PumpDefinitions.Elements())
        {
            var key = includeIdInLabel ? pumpDef.IdLabel() : pumpDef.Label;
            var head = pumpDef.Head;
            switch (pumpDef.Head.PumpDefinitionType)
            {
                case PumpDefinitionType.DepthFlowVariableSpeed:
                case PumpDefinitionType.DepthFlow:
                case PumpDefinitionType.VolumeFlow:
                case PumpDefinitionType.ConstantPower:
                    break;

                case PumpDefinitionType.MultiplePoint:
                    curves.Add(key,head.PumpCurve.Get().Select(p => new GeometryPoint(p.Flow, p.Head)).ToList());
                    break;

                case PumpDefinitionType.CustomExtended:
                    curves.Add(key, new List<GeometryPoint>()
                    {
                        new GeometryPoint(0, head.ShutoffHead),
                        new GeometryPoint(head.DesignFlow, head.DesignHead),
                        new GeometryPoint(head.MaxOperatingFlow, head.MaxOperatingHead),
                        new GeometryPoint(head.MaxExtendedFlow, 0),
                    });
                    break;

                case PumpDefinitionType.StandardExtended:
                case PumpDefinitionType.Standard:
                    curves.Add(key, new List<GeometryPoint>()
                    {
                        new GeometryPoint(0, head.ShutoffHead),
                        new GeometryPoint(head.DesignFlow, head.DesignHead),
                        new GeometryPoint(head.MaxOperatingFlow, head.MaxOperatingHead),
                    });
                    break;

                case PumpDefinitionType.DesignPoint:
                    var designPointMngr = new PumpDesignPointElementManager(
                        domainDataSet: WaterModel.DomainDataSet,
                        elementId: () => pumpDef.Id,
                        pumpDefinitionType: PumpDefinitionTypeEnum.DesignPointType);

                    var points = new List<GeometryPoint>();

                    var headField = WaterModel.Components.PumpDefinitions.InputFields.FieldByName(StandardFieldName.MaxOperatingHead);
                    var headUnit = headField.Unit.GetUnit();

                    var flowField = WaterModel.Components.PumpDefinitions.InputFields.FieldByName(StandardFieldName.MaxOperatingDischarge);
                    var flowUnit = flowField.Unit.GetUnit();

                    var dataRows = designPointMngr.Elements();
                    foreach (PumpDesignPointElement row in dataRows)
                        points.Add(new GeometryPoint(
                            flowUnit.ConvertFrom(row.Flow, flowField.StorageUnit),
                            headUnit.ConvertFrom(row.Head, headField.StorageUnit)));

                    curves.Add(key, points);
                    break;

                default:
                    break;
            }
        }

        return curves;
    }

    /// <summary>
    /// Flow Efficiency curve, Flow is in X value, Efficiency is in Y value
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<GeometryPoint>> PumpFlowEfficiencyCurves(bool includeIdInLabel = false)
    {
        var curves = new Dictionary<string, List<GeometryPoint>>();

        foreach (var pumpDef in WaterModel.Components.PumpDefinitions.Elements())
        {
            var pumpCurve = pumpDef.Efficiency.FlowEfficiencyCurve;
            if (pumpCurve == null)
                continue;

            var key = includeIdInLabel ? pumpDef.IdLabel() : pumpDef.Label;
            curves.Add(key, pumpCurve.Get().Select(p => new GeometryPoint(p.Flow, p.Efficiency)).ToList());
        }

        return curves;
    }
    #endregion


    #region Private Properties
    private IWaterModel WaterModel { get; set; }
    #endregion
}


