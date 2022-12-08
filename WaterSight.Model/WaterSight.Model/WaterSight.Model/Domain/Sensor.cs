using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Diagnostics;
using System.Drawing;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Domain;

[DebuggerDisplay("{ToString()}")]
public class Sensor
{
    #region Constructor
    public Sensor(SensorType sensorType)
    {
        SensorType = sensorType;
    }
    public Sensor(ISCADAElement scadaElement, SensorType sensorType)
        : this(sensorType, scadaElement, scadaElement.Input.TargetElement, scadaElement.Input.TargetAttribute)
    {
        TagName = scadaElement.Input.HistoricalSignal?.TagForWaterSight() ?? GetTagName();
        Label = scadaElement.Label ?? GetLabel();
    }



    public Sensor(
        SensorType sensorType,
        IWaterElement networkElement,
        IWaterElement originElement,
        SCADATargetAttribute targetAttribute)
        : this(sensorType)
    {
        NetworkElement = networkElement;
        OriginElement = originElement;
        TargetAttribute = targetAttribute;

        if (networkElement is IPointNodeInput)
        {
            var point = (networkElement as IPointNodeInput).GetPoint();
            Location = new PointF((float)point.X, (float)point.Y);
        }
        else if (networkElement is IBaseLinkInput)
        {
            var point = (networkElement as IBaseLinkInput).MidPoint();
            Location = new PointF((float)point.X, (float)point.Y);
        }

        TagName = GetTagName();
        Label = GetLabel();
    }


    #endregion

    #region Public Methods
    public string GetTagName()
    {
        return $"{OriginElement.Label}_{GetTagNameSuffix()}";
    }
    public string GetLabel()
    {
        return $"{OriginElement.Label}{GetTagLabelSuffix()}";
    }
    public void UpdateLabel()
    {
        Label = GetLabel();
    }
    #endregion

    #region Private Methods
    private string GetSensorUnit(SensorType sensorType)
    {
        throw new NotImplementedException();

        /*switch (sensorType)
        {
            case SensorType.Pressure:
                break;
            case SensorType.Flow:
                break;
            case SensorType.Level:
                break;
            case SensorType.Power:
                break;
            case SensorType.Concentration:
                break;
            case SensorType.Status:
                break;
            case SensorType.HydraulicGrade:
                break;
            case SensorType.PumpSpeed:
                break;
            case SensorType.pH:
                break;
            default:
                break;
        }*/
    }
    private string GetTagLabelSuffix()
    {
        var label = "";
        switch (SensorType)
        {
            case SensorType.Pressure:
                {
                    label = "Pressure";
                    if (IsDirectional && IsDirectionOutwards)
                        label = "Out_Pressure";
                    if (IsDirectional && !IsDirectionOutwards)
                        label = "In_Pressure";
                }
                break;

            case SensorType.Flow:
                label = "Flow";
                break;

            case SensorType.Level:
                label = "Level";
                break;

            case SensorType.Power:
                label = "Power";
                break;

            case SensorType.Concentration:
                label = "Conc.";
                break;

            case SensorType.Status:
                label = "Status";
                break;

            case SensorType.HydraulicGrade:
                label = "H. Grade";
                break;

            case SensorType.PumpSpeed:
                label = "Speed";
                break;

            case SensorType.pH:
                label = "PH";
                break;

            default:
                label = "Unknown";
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
                if (IsDirectional && IsDirectionOutwards)
                    psr = "OUT_PSR";
                if (IsDirectional && !IsDirectionOutwards)
                    psr = "IN_PSR";
                return psr;

            case SensorType.Flow:
                return "FLO";

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
                return "UNK";
        }
    }
    #endregion

    #region Overridden methods
    public override string ToString()
    {
        return $"{Label} [{SensorType}], Tgt = {TargetAttribute}";
    }
    #endregion

    #region Properties
    public SensorType SensorType { get; }
    public string TagName { get; set; }
    public string Label { get; set; }
    public string Unit => GetSensorUnit(SensorType);
    public bool IsDirectional { get; set; } = false;
    public bool IsDirectionOutwards
    {
        get { return _IsDirectionOutwards; }
        set { _IsDirectionOutwards = value; Label = GetLabel(); }
    }
    public SCADATargetAttribute TargetAttribute { get; set; }

    public PointF Location { get; } = new PointF();

    public double? ReferenceElevation => OriginElement is IPhysicalNodeElementInput
        ? (OriginElement as IPhysicalNodeElementInput)?.Elevation
        : null;



    //public IWaterNetworkElement NetworkElement { get; }
    //public IWaterNetworkElement OriginElement { get; }
    public IWaterElement NetworkElement { get; }
    public IWaterElement OriginElement { get; }
    #endregion

    #region Fields
    bool _IsDirectionOutwards = false;
    #endregion

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
}
