using WaterSight.Web.Core;

namespace WaterSight.Web.HydrulicStructures;

public class HydStructure : WSItem
{
    #region Constructor
    public HydStructure(WS ws) : base(ws)
    {
        Tank = new Tank(ws);
        TankCurve = new TankCurve(ws);
        Pump = new Pump(ws);
        PumpStation = new PumpStation(ws);

    }
    #endregion

    #region Public Properties

    public Tank Tank { get; }
    public TankCurve TankCurve { get; }
    public Pump Pump { get; }
    public PumpStation PumpStation { get; }
    #endregion


}