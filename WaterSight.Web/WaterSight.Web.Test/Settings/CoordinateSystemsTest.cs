using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;


[TestFixture, Order(100400), Category("Settings"), Category("Coord Systems")]
public class CoordinateSystemsTest : TestBase
{
    #region Constructor
    public CoordinateSystemsTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Coordinate Systems Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public CoordinateSystems CoordinateSystems => WS.Settings.CoordinateSystems;
    #endregion

    #region Tests
    [Test]
    public async Task CoordinateSystems_Test()
    {
        var coordSystems = await CoordinateSystems.GetAll();
        Assert.IsNotNull(coordSystems);
        Assert.IsTrue(coordSystems.Any());
        Separator("All GET");

        // Set
        Assert.IsTrue(await CoordinateSystems.SetSensors(2248));
        Assert.IsTrue(await CoordinateSystems.SetCustomers(2248));
        Assert.IsTrue(await CoordinateSystems.SetWorkOrders(2248));
        Separator("Individual POSTs");

    }
    #endregion
}


