using Ganss.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WaterSight.Excel.Zone;
using WaterSight.Web.Core;
using WaterSight.Web.Extensions;
using WaterSight.Web.Sensors;
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
    public void UploadZoneExcelFile()
    {
        //
        // There is no direct support to upload 
        // hence add zone configuration row by row
        //

        throw new NotSupportedException("WaterSight doesn't support uploading of Zone Excel file. Update ZoneConfig one by one.");
    }

    #region CRUD Operation
    // 
    // Create
    public async Task<ZoneConfig?> AddZoneConfigAsync(ZoneConfig zoneConfig)
    {
        var url = EndPoints.HydStructuresZonesQDT;
        int? id = await WS.AddAsync<int?>(zoneConfig, url, "Zone");
        if (id.HasValue)
        {
            zoneConfig.Id = id.Value;
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
        return await WS.GetAsync<ZoneConfig>(url, zoneId, "Zone");

    }

    //
    // GET USAGE [PUT]
    public async Task<ZoneUsage> GetZoneUsageWithoutPatternAsync(int zoneId, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        var url = EndPoints.HydStructuresZonesConfidenceRangeForQDT(zoneId);
        url += $"&{EndPoints.Query.GetStartDateTime(startAt)}";
        url += $"&{EndPoints.Query.GetEndDateTime(endAt)}";

        var payload = new
        {
            confidencePercentiles = new List<int>(),
            confidenceHistoricalRange = "P2M",
            forecastPeriod = "P0D",
            resamplingInterval = "PT15M",
            statisticPercentiles = new List<int>(),
            maximumGapSize = "PT15M",
            fillWithPattern = false,
            dirtySignal = true,
            isDryWeatherPattern = false,
        };


        var data = await WS.PutAsync<ZoneUsage>(url, payload, "ZoneUsage");
        return data;
    }

    public async Task<ZoneUsage> GetZoneUsageAsync(int zoneId, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        var url = EndPoints.HydStructuresZonesConfidenceRangeForQDT(zoneId);
        url += $"&{EndPoints.Query.GetStartDateTime(startAt)}";
        url += $"&{EndPoints.Query.GetEndDateTime(endAt)}";

        var payload = new
        {
            confidencePercentiles = new List<int>() { 5, 20, 50, 80, 95 },
            confidenceHistoricalRange = "P2M",
            forecastPeriod = "P0D",
            resamplingInterval = "PT15M",
            statisticPercentiles = new List<int>() { 5, 50, 95 },
            maximumGapSize = "PT15M",
            fillWithPattern = false,
            dirtySignal = true,
            isDryWeatherPattern = false,
        };


        var data = await WS.PutAsync<ZoneUsage>(url, payload, "ZoneUsage");
        return data;
    }

    //
    // UPDATE
    public async Task<bool> UpdateZoneConfigAsync(ZoneConfig zoneConfig)
    {
        var url = EndPoints.HydStructuresZonesForQDT(zoneConfig.Id);
        return await WS.UpdateAsync(zoneConfig.Id, zoneConfig, url, "Zone", false);
    }

    //
    // DELETE
    public async Task<bool> DeleteZonesConfigAsync(List<string> onlyDeleteZoneNames)
    {
        var zonesConfig = await GetZonesConfigAsync();
        Logger.Debug($"Total number of zones configured = {zonesConfig.Count}");
        if (onlyDeleteZoneNames.Any())
        {
            zonesConfig = zonesConfig.Where(p => onlyDeleteZoneNames.Contains(p.Name)).ToList();
            Logger.Debug($"Total number of zones configured after dropping the given zones = {zonesConfig.Count}");

        }

        var success = true;
        foreach (var zoneConfig in zonesConfig)
        {
            if (zoneConfig?.Name == "System")
                continue;

            var url = EndPoints.HydStructuresZonesForQDT(zoneConfig.Id);
            success = success && await WS.DeleteAsync(zoneConfig.Id, url, "Zone config");
        }


        //var url = EndPoints.HydStructuresZonesQDT;
        //var success= await WS.DeleteManyAsync(url, "Zones", true);

        Logger.Debug($"Deleted.");
        return success;
    }
    public async Task<bool> DeleteZoneConfigAsync(int zoneId)
    {
        var url = EndPoints.HydStructuresZonesForQDT(zoneId);
        return await WS.DeleteAsync(zoneId, url, "Zone", true);
    }

    public async Task<bool> ReComputeMassBalanceAsync()
    {
        var url = EndPoints.HydStructureMassBalancesComputeQDT;
        return await WS.GetAsync<bool>(url, null, "Mass Balance Recompute", true);
    }
    #endregion

    public async Task<string> DownloadToExcel(
        string rootPath,
        string fileNameWithExt
        )
    {
        // Create Zone Excel file based on WaterSight data
        var zonesConfig =await WS.Zone.GetZonesConfigAsync();
        var sensorsConfig = await WS.Sensor.GetSensorsConfigAsync();
        
        var xlFilePath = Path.Combine(rootPath, fileNameWithExt);
        var xlMapper = new ExcelMapper();

        //
        // Zone Balance
        var sheetName = "Zone Balance";
        var zoneItems = new List<ZoneItem>();
        foreach (var zc in zonesConfig)
        {
            foreach (var signalId in zc.InflowSignalIds)
            {
                var signalConfig = sensorsConfig.Where(sc => sc.ID == signalId).FirstOrDefault();
                zoneItems.Add(new ZoneItem()
                {
                    DisplayName = zc.Name,
                    TagId = signalConfig.TagId,
                    Type = ZoneFlowType.Inflow.ToString(),
                });
            }

            foreach (var signalId in zc.StorageSignalIds)
            {
                var signalConfig = sensorsConfig.Where(sc => sc.ID == signalId).FirstOrDefault();
                zoneItems.Add(new ZoneItem()
                {
                    DisplayName = zc.Name,
                    TagId = signalConfig.TagId,
                    Type = ZoneFlowType.Inflow.ToString(),
                });
            }

            foreach (var signalId in zc.OutflowSignalIds)
            {
                var signalConfig = sensorsConfig.Where(sc => sc.ID == signalId).FirstOrDefault();
                zoneItems.Add(new ZoneItem()
                {
                    DisplayName = zc.Name,
                    TagId = signalConfig.TagId,
                    Type = ZoneFlowType.Inflow.ToString(),
                });
            }
        }
        _ = await WriteToExcelAsync(zoneItems, xlFilePath, sheetName, xlMapper);

        //
        // Zone Characteristics
        sheetName = "Zones Characteristics";
        var zoneCharacteristics = new List<ZoneCharacteristicsItem>();
        foreach (var zc in zonesConfig)
        {
            var parentZoneCheck = zonesConfig.Where(d => d.Id == zc.ParentZoneId);
            var parentZone = string.Empty;
            if(parentZoneCheck.Any())
                parentZone = parentZoneCheck.First().Name;

            zoneCharacteristics.Add(new ZoneCharacteristicsItem()
            {
                DisplayName = zc.Name,
                ParentZone = parentZone,
                PopulationServed = zc.PopulationServed,
                NumberOfConnections = zc.NumberOfConnections,
                NumberOfCustomers = zc.NumberOfCustomers,
                WaterLossesMethod = zc.WaterLossesMethod.ToString(),
                PercentOfMnfConsumed = zc.WaterLossesPercentage,
                PercentOfAuthUnbilledConsumption = zc.AuthorizedUnbilledConsumption,
                Priority = zc.Priority,
                Tags = zc.Tags,
            });
        }
        _ = await WriteToExcelAsync(zoneCharacteristics, xlFilePath, sheetName, xlMapper);

        Logger.Information($"Wrote zone details to an Excel file. Path: {xlFilePath}");
        return xlFilePath;
    }

    #endregion
}

public enum WaterLossMethod
{
    BasedOnMNF = 0,
    AlPercentOfInput = 1,
    AlPercentOfRevenue = 2,
    AlPercentOfLosses = 3,
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
    public WaterLossMethod WaterLossesMethod { get; set; } = WaterLossMethod.BasedOnMNF;
    public double? WaterLossesPercentage { get; set; }
    public double? AuthorizedUnbilledConsumption { get; set; }
    public List<int?> InflowSignalIds { get; set; } = new List<int?>();
    public List<int?> OutflowSignalIds { get; set; } = new List<int?>();
    public List<int?> StorageSignalIds { get; set; } = new List<int?>();
    public int Priority { get; set; } = 1;
    public string Tags { get; set; } = String.Empty;
    public int? ParentZoneId { get; set; }
    public int? Level { get; set; }
    public int? PatternWeekId { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{Id}: {Name}, Count: In={InflowSignalIds.Count}, Out={OutflowSignalIds.Count}, Storage={StorageSignalIds.Count}";
    }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class ZoneUsage
{
    public string Message { get; set; }
    public string Units { get; set; }
    public List<double> SeriesPercentiles { get; set; } = new List<double>();
    public List<ZoneUsagePoint> Points { get; set; } = new List<ZoneUsagePoint>();

    public override string ToString()
    {
        return $"[{Units}] Number of Points: {Points.Count}";
    }
}


[DebuggerDisplay("{ToString()}")]
public class ZoneUsagePoint
{
    public DateTime Instant { get; set; }
    public double ValueSignal { get; set; }
    public List<double> ValuePercentiles { get; set; } = new List<double>();

    public override string ToString()
    {
        return $"{ValueSignal} @ {Instant}";
    }
}

#endregion