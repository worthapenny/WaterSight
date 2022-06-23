using NUnit.Framework;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;

[TestFixture, Order(100200), Category("Settings"), Category("Zones")]
public class TimezoneTest : TestBase
{
    #region Constructor
    public TimezoneTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Timezone Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public TimeZone TimeZone => WS.Settings.TimeZone;
    #endregion

    #region Tests
    [Test, Category("Settings")]
    public async Task Timezone_Test()
    {
        var timezoneConfig = new TimeZoneConfig();
        var timezoneName = "Nepal Standard Time";
        timezoneConfig.TimeZoneId = timezoneName;

        // Set
        var timezoneUpdated = await TimeZone.SetTimezone(timezoneConfig);
        Assert.AreEqual(true, timezoneUpdated);
        Separator("Timezone Set");

        // Get Current/Active
        var timezone = await TimeZone.GetTimeZoneName();
        Assert.IsNotNull(timezone);
        Assert.AreEqual(timezoneName, timezone);
        Separator("Timezone Get");

        // Get All supported timezones
        var timezones = await TimeZone.GetTimeZones();
        Assert.IsNotNull(timezones);
        Assert.IsNotEmpty(timezones);

    }
    #endregion
}
