using NUnit.Framework;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;

[TestFixture, Order(100200), Category("Settings"), Category("Zones")]
public class TimeZoneTest : TestBase
{
    #region Constructor
    public TimeZoneTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing TimeZone Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public TimeZone TimeZone => WS.Settings.TimeZone;
    #endregion

    #region Tests
    [Test, Category("Settings")]
    public async Task TimeZone_Test()
    {
        var timeZoneConfig = new TimeZoneConfig();
        var timeZoneName = "America/New_York";
        timeZoneConfig.TimeZoneId = timeZoneName;

        // Set
        var timeZoneUpdated = await TimeZone.SetTimezone(timeZoneConfig);
        Assert.AreEqual(true, timeZoneUpdated);
        Separator("TimeZone Set");

        // Get Current/Active
        var timeZone = await TimeZone.GetTimeZoneName();
        Assert.IsNotNull(timeZone);
        Assert.AreEqual(timeZoneName, timeZone);
        Separator("TimeZone Get");

        // Get All supported timeZones
        var timeZones = await TimeZone.GetTimeZones();
        Assert.IsNotNull(timeZones);
        Assert.IsNotEmpty(timeZones);

    }
    #endregion
}
