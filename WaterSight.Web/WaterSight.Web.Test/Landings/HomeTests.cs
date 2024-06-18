using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace WaterSight.Web.Test;

[TestFixture, Order(110100), Category("Landings"), Category("Home")]
public class HomeTests: TestBase
{
    #region Constructor
    public HomeTests()
    //: base(4549, Env.Qa)
    {
        Separator($"----+----+---- Performing Home Related Tests ----+----+----");
    }
    #endregion

    #region Test
    [Test, Category("CRUD")]
    public async Task Home_CRUD()
    {
        var queries = await WS.Home.GetStatQueries();
        Assert.That(queries.Any(), Is.True);

        var sensorCount = WS.Home.GetSensorCount(queries);
        Assert.That(sensorCount.Any(), Is.True);

    }
    #endregion
}
