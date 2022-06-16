using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Extensions;
using WaterSight.Web.Support;

namespace WaterSight.Web.Zones;

public class Zone : WSItem
{
    #region Constructor
    public Zone(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD Operation
    // 
    // Create
    public async Task<ZoneConfig?> AddZoneConfigAsync(ZoneConfig zoneConfig)
    {
        var url = EndPoints.HydStructuresZones;
        int? id = await WS.AddAsync(zoneConfig, url, "Zone");
        if (id.HasValue)
        {
            zoneConfig.Id= id.Value;
            return zoneConfig;
        }

        return null;
    }

    // 
    // GET / READ
    public async Task<List<ZoneConfig>> GetZonesConfigAsync()
    {
        var url = EndPoints.HydStructuresZonesQDT;
        return await WS.GetManyAsync<ZoneConfig>(url, "Zones");
    }
    public async Task<ZoneConfig> GetZoneConfigAsync(int zoneId)
    {
        var url = EndPoints.HydStructuresZonesForQDT(zoneId);
        return await WS.GetAsync<ZoneConfig>(url,zoneId, "Zone");
    }

    //
    // UPDATE
    public async Task<bool> UpdateZonesConfigAsync(ZoneConfig zoneConfig)
    {
        var url = EndPoints.HydStructuresZonesForQDT(zoneConfig.Id);
        return await WS.UpdateAsync(zoneConfig.Id, zoneConfig, url, "Zone", true);
    }

    //
    // DELETE
    public async Task<bool> DeleteZonesConfigAsync()
    {
        Logger.Debug($"About to delete all the zones...");
        var url = EndPoints.HydStructuresZonesQDT;
        var success= await WS.DeleteManyAsync(url, "Zones", true);

        Logger.Debug($"Deleted.");
        return success;
    }
    public async Task<bool> DeleteZoneConfigAsync(int zoneId)
    {
        var url = EndPoints.HydStructuresZonesForQDT(zoneId);
        return await WS.DeleteAsync(zoneId, url, "Zone", true);
    }
    #endregion

    #endregion
}

public enum WaterLossMethod
{
    BasedOnMNF =0,
    AlPercentOfInput=1,
    AlPercentOfRevenue=2,
    AlPercentOfLosses=3,        
}

#region Model Classes
[DebuggerDisplay("{ToString()}")]
public class ZoneConfig
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsSystemZone { get; set; } = false;
    public bool IsDirty { get; set; } = true;
    public string? FlowUnits { get; set; } = null;
    public int? PopulationServed { get; set; }
    public int? NumberOfCustomers { get; set; }
    public int? NumberOfConnections { get; set; }
    public double? MinimumNightlyFlowConsumed { get; set; }
    public WaterLossMethod WaterLossesMethod { get; set; }
    public double? WaterLossesPercentage { get; set; }
    public double? AuthorizedUnbilledConsumption { get; set; }
    public List<int?> InflowSignalIds { get; set; }
    public List<int?> OutflowSignalIds { get; set; }
    public List<int?> StorageSignalIds { get; set; }
    public int Priority { get; set; } = 1;
    public string Tags { get; set; } = String.Empty;
    public int? ParentZoneId { get; set; }
    public int Level { get; set; }
    public int? PatternWeekId { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{Id}: {Name}, Count: In={InflowSignalIds.Count}, Out={OutflowSignalIds.Count}, Storage={StorageSignalIds.Count}";
    }
    #endregion
}
#endregion