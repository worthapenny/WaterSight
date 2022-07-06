using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Custom;
using WaterSight.Web.DT;
using WaterSight.Web.Test;

namespace WaterSight.Web.Test;


public class CustomWaterModelTest : TestBase
{
    #region Constructor
    public CustomWaterModelTest()
    //: base(4549, Env.Qa)
    //:base(179, Core.Env.Prod) // watertown
    {
        Separator($"----+----+---- Performing Custom > Water Model Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public WaterModel WaterModel => WS.CustomWaterModel;
    #endregion

    #region Tests
    [Test, Category("TSD Collection")]
    public async Task GetAllScadaElementsOutputDataTest()
    {
        // if there is no model uploaded,
        //      upload one, as model results are required for this test
        var modelDoamin = await WS.NumericModel.GetModelDomainsAllTypes();
        if(!(modelDoamin?.Where(d=>d.Type?.ToLower().Contains("gems") ?? false).Any() ?? false))
        {
            var numericalModelTest = new NumericalModelTest();
            var uploaded = await numericalModelTest.UploadWaterModel();
            Assert.That(uploaded, Is.True);
        }

        var modelMeasuredDataList = await WaterModel.GetAllScadaElementsOutputData();
    }

    #endregion
}
