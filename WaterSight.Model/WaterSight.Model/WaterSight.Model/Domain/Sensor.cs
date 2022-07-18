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
    public Sensor(ISCADAElement scadaElement, SensorType type)
        : this(type, scadaElement, scadaElement.Input.TargetElement, scadaElement.Input.TargetAttribute)
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

        var point = (networkElement as IPointNodeInput).GetPoint();
        Location = new PointF((float)point.X, (float)point.Y);

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
        return $"{OriginElement.Label}_{GetTagLabelSuffix()}";
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
        switch (SensorType)
        {
            case SensorType.Pressure:
                var psr = "Pressure";
                if (IsDirection && IsDirectionOutwards)
                    psr = "Out_Pressure";
                if (IsDirection && !IsDirectionOutwards)
                    psr = "In_Pressure";
                return psr;

            case SensorType.Flow:
                return "Flow";

            case SensorType.Level:
                return "Level";

            case SensorType.Power:
                return "Power";

            case SensorType.Concentration:
                return "Conc.";

            case SensorType.Status:
                return "Status";

            case SensorType.HydraulicGrade:
                return "H. Grade";

            case SensorType.PumpSpeed:
                return "Speed";

            case SensorType.pH:
                return "PH";

            default:
                return "Unknown";
        }
    }

    private string GetTagNameSuffix()
    {
        switch (SensorType)
        {
            case SensorType.Pressure:
                var psr = "PSR";
                if (IsDirection && IsDirectionOutwards)
                    psr = "OUT_PSR";
                if (IsDirection && !IsDirectionOutwards)
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
    public bool IsDirection { get; set; } = false;
    public bool IsDirectionOutwards { get; set; } = false;
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
