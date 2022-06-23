using NUnit.Framework;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;


[TestFixture, Order(100500), Category("Settings"), Category("Location")]
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
        locationConfig.Latitude = 1234.1234;
        locationConfig.Longitude = -4321.4321;

        // Set
        Assert.IsTrue(await Location.SetLocation(locationConfig));
        Separator("Location Set");

        // Get
        var location = await Location.GetLocation();
        Assert.IsNotNull(location);
        Assert.AreEqual(1234.1234, location?.Latitude, 0.001);
        Assert.AreEqual(-4321.4321, location?.Longitude, 0.001);
        Separator("Location GET");

    }
    #endregion
}


