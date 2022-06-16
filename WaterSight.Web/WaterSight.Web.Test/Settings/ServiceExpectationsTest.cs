using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test.Settings;


public class ServiceExpectationsTest : TestBase
{
    #region Constructor
    public ServiceExpectationsTest()
        //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Service Expectations Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public ServiceExpectations ServiceExpectations => WS.Settings.ServiceExpectations;
    #endregion

    #region Tests
    [Test]
    public async Task ServiceExpectations_Test()
    {
        var expectations = await ServiceExpectations.GetAll();
        Assert.IsNotNull(expectations);
        Assert.IsTrue(expectations.Any());
        Separator("All GET");

        // Set
        Assert.IsTrue(await ServiceExpectations.SetMaxPressure(9999));
        Assert.IsTrue(await ServiceExpectations.SetMinPressure(1111));
        Assert.IsTrue(await ServiceExpectations.SetTargetPumpEfficiency(99));
        Separator("Individual POSTs");


        // Get
        Assert.IsTrue((await ServiceExpectations.GetMaxPressure())?.Value > 0);
        Assert.IsTrue((await ServiceExpectations.GetMinPressure())?.Value > 0);
        Assert.IsTrue((await ServiceExpectations.GetTargetPumpEffi())?.Value > 0);
        Separator("Individual GETs");

    }
    #endregion
}


