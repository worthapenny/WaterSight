using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.HydrulicStructures;

namespace WaterSight.Web.Test.NumericalModel;


public class TankCurvesTest : TestBase
{
    #region Constructor
    public TankCurvesTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Tank curves Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public TankCurve TankCurve => WS.HydStructure.TankCurve;
    #endregion

    #region Tests
    [Test]
    public async Task TankCurves_CRUD()
    {
        // Create Object
        TankCurveConfig tankCurveConfig = NewTankCurveConfig();

        // Create
        var tankCurveConfigCreated = await TankCurve.AddTankCurveConfigAsync(tankCurveConfig);
        Assert.IsNotNull(tankCurveConfigCreated);
        tankCurveConfig.Id = tankCurveConfigCreated.Id;
        Assert.IsTrue(tankCurveConfigCreated?.Id > 0);
        Separator("Added tank curve");

        // Read
        var tankCurveFound = await TankCurve.GetTankCurveConfigAsync(tankCurveConfig.Id);
        Assert.IsNotNull(tankCurveFound);
        Assert.AreEqual(tankCurveFound?.Id, tankCurveConfig.Id);
        Assert.AreEqual(tankCurveFound?.Name, tankCurveConfig.Name);
        Separator("Read tank curve");

        // Read Many
        var tankCurveConfigs = await TankCurve.GetTankCurvesConfigAsync();
        Assert.IsNotNull(tankCurveConfigs);
        Assert.IsTrue(tankCurveConfigs.Count > 0);
        Separator("Got all tank curves");

        // update
        var newName = "Tank curve config New Name";
        tankCurveConfig.Name = newName;
        tankCurveConfig.CurveData.Add(new CurveData(15, 15));
        var success = await TankCurve.UpdateTankCurveConfigAsync(tankCurveConfig);
        Assert.IsTrue(success);
        Separator("Updated tank curve");

        // Delete
        var deleted = await TankCurve.DeleteTankCurveConfigAsync(tankCurveConfigCreated.Id);
        Assert.IsTrue(deleted);
        Separator($"Deleted single tank curve");

        // Delete All
        Assert.CatchAsync<NotSupportedException>(async () => await TankCurve.DeleteTankCurvesConfigAsync());
        Separator($"Deleted all tank cruves");
    }

    private static TankCurveConfig NewTankCurveConfig()
    {
        var tankCurveConfig = new TankCurveConfig();
        tankCurveConfig.Name = "Test Tank Curve";
        tankCurveConfig.CurveData.Add(new CurveData(0, 0));
        tankCurveConfig.CurveData.Add(new CurveData(5, 5));
        tankCurveConfig.CurveData.Add(new CurveData(10, 10));
        return tankCurveConfig;
    }

    #endregion
}


