using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System.Diagnostics;

namespace WaterSight.Model.Domain;

[DebuggerDisplay("{ToString()}")]
public class Sensor
{
    #region Constructor
    public Sensor(SensorType sensorType)
    {
        SensorType = sensorType;
    }
    public Sensor(
        SensorType sensorType,
       /* IWaterNetworkElement networkElement,
        IWaterNetworkElement originElement,*/
       IWaterElement networkElement,
       IWaterElement originElement,
        SCADATargetAttribute targetAttribute,
        string unit = "")
        : this(sensorType)
    {
        NetworkElement = networkElement;
        OriginElement = originElement;
        TargetAttribute = targetAttribute;
        Unit = unit;

        UpdateTagName();
        UpdateLabel();
    }
    #endregion

    #region Public Methods
    public void UpdateTagName()
    {
        TagName = $"{OriginElement.Label}_{GetTagNameSuffix()}";
    }
    public void UpdateLabel()
    {
        Label = $"{OriginElement.Label}_{GetTagLabelSuffix()}";
    }

    #endregion

    #region Private Methods

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
    public string Unit { get; set; }
    public bool IsDirection { get; set; } = false;
    public bool IsDirectionOutwards { get; set; } = false;
    public SCADATargetAttribute TargetAttribute { get; set; }

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
