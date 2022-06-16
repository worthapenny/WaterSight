using WaterSight.Web.Core;

namespace WaterSight.Web.Settings
{
    public class Settings : WSItem
    {
        #region Constructor
        public Settings(WS ws) : base(ws)
        {
            Units = new Units(ws);
            ServiceExpectations = new ServiceExpectations(ws);
            Costs = new Costs(ws);
            CoordinateSystems = new CoordinateSystems(ws);
            Location = new Location(ws);
            PatternWeeks = new PatternWeeks(ws);
            TimeZone = new TimeZone(ws);
            SpecialDay = new SpecialDay(ws);
        }
        #endregion

        #region Public Properties
        public Units Units { get; }
        public ServiceExpectations ServiceExpectations { get; }
        public Costs Costs { get; }
        public CoordinateSystems CoordinateSystems { get; }
        public Location Location { get; }
        public TimeZone TimeZone { get; }
        public PatternWeeks PatternWeeks { get; }
        public SpecialDay SpecialDay { get; }
        #endregion
    }
}
