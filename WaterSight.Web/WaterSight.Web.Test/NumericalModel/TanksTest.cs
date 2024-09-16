using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.HydrulicStructures;

namespace WaterSight.Web.Test;


[TestFixture, Order(102000), Category("NumericalModel"), Category("Tanks")]
public class TanksTest : TestBase
{
    #region Constructor
    public TanksTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Tanks Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Tank Tank => WS.HydStructure.Tank;
    #endregion

    #region Tests
    [Test]
    public async Task Tanks_CRUD()
    {
        // Create
        var tankConfig = new TankConfig();
        tankConfig.Id = 0;
        tankConfig.Name = "Test Tank";
        tankConfig.ReferenceElevation = 99.99;
        tankConfig.MinLevel = 0;
        tankConfig.MaxLevel = 100;
        tankConfig.LowLevelAlarm = 5;
        tankConfig.HighLevelAlarm = 95;
        tankConfig.MaxVolume = 1;
        //tankConfig.TankCurveId =0;
        tankConfig.TankCurveName = null;
        tankConfig.VolumeUnits = "Mgal (U.S.)";
        tankConfig.LengthUnits = "ft";
        tankConfig.LevelSignalName = "Tank_Test_Tag";
        tankConfig.DesiredTurnoverTime = 3;
        tankConfig.Tags = "Tanks,Test";

        // Create
        var tankConfigCreated = await Tank.AddTankConfigAsync(tankConfig);
        Assert.IsNotNull(tankConfigCreated);
        tankConfig.Id = tankConfigCreated.Id;
        Assert.IsTrue(tankConfigCreated?.Id > 0);
        Separator("Tank create");

        // Read
        var tankFound = await Tank.GetTankConfigAsync(tankConfig.Id.Value);
        Assert.IsNotNull(tankFound);
        Assert.AreEqual(tankFound?.Id, tankConfig.Id);
        Assert.AreEqual(tankFound?.Name, tankConfig.Name);
        Separator("Tank read");

        // update
        var newName = "Tank config New Name";
        tankConfig.Name = newName;
        var success = await Tank.UpdateTankConfigAsync(tankConfig);
        Assert.That(success, Is.True);
        Separator("Tank updated");

        // Delete
        var deleted = await Tank.DeleteTankConfigAsync(tankConfigCreated.Id.Value);
        Assert.IsTrue(deleted);
        Separator("Tank deleted");

        // Delete All
        var allDeleted = await Tank.DeleteTanksConfigAsync();
        Assert.That(allDeleted, Is.True);
        Separator("All tanks deleted");
    }

    //
    // CONFIG
    //
    [Test]
    public async Task TankSensor_Config()
    {
        var tankConfigs = await Tank.GetTanksConfigAsync();
        Assert.IsNotNull(tankConfigs);
        Separator($"Getting all tanks config tested");
    }
    #endregion
}


