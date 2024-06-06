using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;


[TestFixture, Order(100500), Category("Settings"), Category("Costs")]
public class CostsTest : TestBase
{
    #region Constructor
    public CostsTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Costs Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Costs Costs => WS.Settings.Costs;
    #endregion

    #region Tests
    [Test]
    public async Task ServiceExpectations_Test()
    {
        var costs = await Costs.GetAll();
        Assert.IsNotNull(costs);
        Assert.IsTrue(costs.Any());
        Separator("All GET");

        // Set
        Assert.IsTrue(await Costs.SetAvgVolumeticProductionCost(9.99, "$"));
        Assert.IsTrue(await Costs.SetAvgVolumetricTarrif(1.11, "$"));
        Assert.IsTrue(await Costs.SetAvgEnergyCost(99.99, "$"));
        Separator("Individual POSTs");


        // Get
        Assert.IsTrue((await Costs.GetAvgVolumeticProductionCost()) > 0);
        Assert.IsTrue((await Costs.GetAvgVolumetricTarrif()) > 0);
        Assert.IsTrue((await Costs.GetAvgEnergyCost()) > 0);
        Separator("Individual GETs");

    }
    #endregion
}


