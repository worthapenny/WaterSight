using WaterSight.Web.Core;

namespace WaterSight.Web.Customers;

public class Customers : WSItem
{
    #region Constructor
    public Customers(WS ws) : base(ws)
    {
        Billings = new Billings(ws);
        Meters = new Meters(ws);
    }
    #endregion

    #region Public Properties
    public Billings Billings { get; }
    public Meters Meters { get; }
    #endregion
}

