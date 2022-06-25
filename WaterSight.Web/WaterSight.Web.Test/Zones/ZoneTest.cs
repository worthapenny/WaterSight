using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Support;
using WaterSight.Web.Zones;


namespace WaterSight.Web.Test;

[TestFixture, Order(104000), Category("Zone")]
public class ZoneTest : TestBase
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
        // Read Many
        var zones = await Zone.GetZonesConfigAsync();
        Assert.IsNotNull(zones);
        Assert.IsTrue(zones.Count > 0); // System zone will always be there so it should be true 
        Logger.Debug("Read many tested");

        // New zone object/payload
        var zoneConfig = NewZoneConfig();

        // Delete (if old one is still out there)
        var zoneCheck = zones.Where(z => z.Name == zoneConfig.Name);
        if(zoneCheck.Any())
        {
            var deletedOldZone = await Zone.DeleteZoneConfigAsync(zoneCheck.First().Id);
            Assert.AreEqual(true, deletedOldZone);
        }

        // Find out the parent zone first
        var systemZoneCheck = zones.Where(z => z.IsSystemZone);
        Assert.AreEqual(true, systemZoneCheck.Any());
        var systemZone = systemZoneCheck.First();


        // Create
        var addZoneConfig = await Zone.AddZoneConfigAsync(zoneConfig);
        addZoneConfig.ParentZoneId = systemZone.Id;
        zoneConfig.ParentZoneId = systemZone.Id;

        Assert.IsNotNull(addZoneConfig);
        Assert.AreEqual(zoneConfig.Id, addZoneConfig.Id);
        Assert.IsTrue(zoneConfig.Id > 0);
        Separator("Add new tested");

        // Read single
        var zoneConfigFound = await Zone.GetZoneConfigAsync(zoneConfig.Id);
        Assert.IsNotNull(zoneConfigFound);
        Assert.AreEqual(zoneConfig.Id, zoneConfigFound.Id);
        Assert.AreEqual(zoneConfig.Name, zoneConfigFound.Name);
        Separator("Read tested");

        //// It appears that the Update by changing the name is not allowed
        //// Update
        
        //var newName = "New Zone Name";
        //zoneConfig.Name = newName;
        //var success = await Zone.UpdateZonesConfigAsync(zoneConfig);
        //Assert.IsTrue(success);
        //Separator("Update tested");

        // Delete
        var deleted = await Zone.DeleteZoneConfigAsync(zoneConfig.Id);
        Assert.IsTrue(deleted);
        Separator("Delete tested");

        // Delete All
        deleted = await Zone.DeleteZonesConfigAsync();
        Assert.IsTrue(deleted);
        var allZones = await Zone.GetZonesConfigAsync();
        Assert.AreEqual(1, allZones.Count); // System zone cannot be deleted hence count = 1
        Separator("Delete all tested");

        Logger.Debug(Util.LogSeparatorDashes);
    }

    private ZoneConfig NewZoneConfig()
    {
        return new ZoneConfig()
        {
            Id = 0,
            Name = $"NewZoneCS_{DateTime.Now:yyyyMMdd}",
            IsSystemZone = false,
            IsDirty = false,
            FlowUnits = null,
            PopulationServed = 0,
            NumberOfCustomers = 0,
            NumberOfConnections = 0,
            MinimumNightlyFlowConsumed = 80,
            WaterLossesMethod = WaterLossMethod.BasedOnMNF,
            WaterLossesPercentage = 11,
            AuthorizedUnbilledConsumption = 5,
            InflowSignalIds = new List<int?>(),
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
