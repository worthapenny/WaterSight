using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Support;
using WaterSight.Web.Test;
using WaterSight.Web.Zones;


namespace WaterSight.Base.Test.Zones;

public class ZoneTest:TestBase
{
    #region Constructor
    public ZoneTest()
        //: base(4549, Env.Qa)
    //: base(235, Env.Prod)
    {
        Logger.Debug($"----+----+---- Performing Zones Related Tests ----+----+----");
    }
    #endregion

    #region Public Properties
    public Zone Zone => WS.Zone;
    #endregion

    #region Tests
    [Test, Category("CRUD")]
    public async Task Zone_CRUD()
    {
        // Create
        var zoneConfig = NewZoneConfig();
        //var addZoneConfig = await Zone.AddZoneConfigAsync(zoneConfig);
        //Assert.IsNotNull(addZoneConfig);
        //Assert.AreEqual(zoneConfig.Id, addZoneConfig.Id);
        //Assert.IsTrue(zoneConfig.Id > 0);
        //Logger.Debug("Add new tested");

        // Read
        //var zoneConfigFound = await Zone.GetZoneConfigAsync(zoneConfig.Id);
        //Assert.IsNotNull(zoneConfigFound);
        //Assert.AreEqual(zoneConfig.Id, zoneConfigFound.Id);
        //Assert.AreEqual(zoneConfig.Name, zoneConfigFound.Name);
        //Logger.Debug("Read tested");

        var zones = await Zone.GetZonesConfigAsync();
        Assert.IsNotNull(zones);
        Assert.IsTrue(zones.Count > 0);
        Logger.Debug("Read many tested");

        // Update
        var newName = "New Zone Name";
        zoneConfig.Name = newName;
        var success = await Zone.UpdateZonesConfigAsync(zoneConfig);
        Assert.IsTrue(success);
        Logger.Debug("Update tested");

        // Delete
        var deleted = await Zone.DeleteZoneConfigAsync(zoneConfig.Id);
        Assert.IsTrue(deleted);
        Logger.Debug("Delete tested");

        // Delete All
        deleted = await Zone.DeleteZonesConfigAsync();
        Assert.IsTrue(deleted);
        var allZones = await Zone.GetZonesConfigAsync();
        Assert.AreEqual(0, allZones.Count);
        Logger.Debug("Delete all tested");

        Logger.Debug(Util.LogSeparatorDashes);
    }

    private ZoneConfig NewZoneConfig()
    {
        return new ZoneConfig()
        {
            Id=0,
            Name="NewZoneCS",
            IsSystemZone=false,
            IsDirty=false,
            FlowUnits = null,
            PopulationServed =0,
            NumberOfCustomers=0,
            NumberOfConnections=0,
            MinimumNightlyFlowConsumed=80,
            WaterLossesMethod = WaterLossMethod.BasedOnMNF,
            WaterLossesPercentage = 11,
            AuthorizedUnbilledConsumption=5,
            InflowSignalIds =  new List<int?>(),
            OutflowSignalIds = new List<int?>(),
            StorageSignalIds = new List<int?>(),
        };
    }


    //
    // CONFIG
    //
    [Test, Category("Config")]
    public async Task TestZone_Config()
    {
        var zoneConfigs = await Zone.GetZonesConfigAsync();
        Assert.IsNotNull(zoneConfigs);
        Assert.IsTrue(zoneConfigs.Count > 0);
        Logger.Debug($"Getting all zones config tested");
        Logger.Debug(Util.LogSeparatorDashes);
    }
    #endregion
}
