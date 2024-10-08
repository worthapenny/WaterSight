﻿using NUnit.Framework;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;


[TestFixture, Order(100600), Category("Settings"), Category("Location")]
public class LocationTest : TestBase
{
    #region Constructor
    public LocationTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Location Systems Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Location Location => WS.Settings.Location;
    #endregion

    #region Tests
    [Test, Category("Settings")]
    public async Task Location_Test()
    {
        var locationConfig = new LocationConfig();
        locationConfig.Latitude = 39.98741 ;
        locationConfig.Longitude = -104.95621;

        // Set
        Assert.IsTrue(await Location.SetLocation(locationConfig));
        Separator("Location Set");

        // Get
        var location = await Location.GetLocation();
        Assert.That(location, Is.Not.Null);
        Assert.That(location.Latitude, Is.EqualTo(39.98741).Within(0.001));
        Assert.That(location.Longitude, Is.EqualTo(-104.95621).Within(0.001));
        Separator("Location GET");

    }
    #endregion
}


