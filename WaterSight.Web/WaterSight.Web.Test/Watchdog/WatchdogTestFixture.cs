using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.SmartMeters;
using WaterSight.Web.Watchdog;

namespace WaterSight.Web.Test;

public class WatchdogTestFixture: TestBase
{
    #region Conscturctor
    public WatchdogTestFixture()
    //: base(3377, Env.Qa)
    {
        Separator($"----+----+---- Performing Watchdog Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public WatchDog WatchDog => WS.WatchDog;
    #endregion

    #region Tests
    [Test]
    public async Task WatchdogTests()
    {
        var oscMap = await WS.WatchDog.OscOverviewAllDTs();
        Assert.That(oscMap, Is.Not.Empty);

        var oscLastTalk = await WS.WatchDog.OscLastTalk();
        Assert.That(oscLastTalk, Is.Not.Null);

        var pusherOverview = await WS.WatchDog.PusherOverviewAllDTs();
        Assert.That(pusherOverview, Is.Not.Empty);

        var pusherSummary = await WS.WatchDog.PusherSummary();
        Assert.That(pusherSummary, Is.Not.Empty);

        var pusherLastTalk = await WS.WatchDog.PusherLastTalk();
        Assert.That(pusherLastTalk, Is.Not.Null);
    }
    #endregion
}
