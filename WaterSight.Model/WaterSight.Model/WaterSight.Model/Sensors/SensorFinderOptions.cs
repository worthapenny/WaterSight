
namespace WaterSight.Model.Sensors;

public class SensorFinderOptions
{
    #region Constructor
    public SensorFinderOptions()
    {
    }
    #endregion

    #region Public Properties
    public bool ActiveElementsOnly { get; set; } = true;
    public bool TankLevel { get; set; } = true;
    public bool TankFlow { get; set; } = true;

    public bool PumpStatus { get; set; } = true;
    public bool PumpSpeedFactor { get; set; } = true;
    public bool PumpSuctionNodePressure { get; set; } = true;
    public bool PumpDischargeNodePressure { get; set; } = true;
    public bool PumpDischargePipeFlow { get; set; } = true;
    public bool PumpPower { get; set; } = true;
    public bool PumpCommonDischargePipeFlow { get; set; } = true;

    public bool ValveFlow { get; set; } = true;
    public bool ValveStatus { get; set; } = true;
    public bool ValveDownstreamPressure { get; set; } = true;
    public bool ValveUpstreamPressure { get; set; } = true;
    public bool TCVs { get; set; } = false;
    public bool GPVs { get; set; } = false;

    public bool ReservoirHead { get; set; } = true;
    public bool ReservoirFlow { get; set; } = true;
    #endregion
}
