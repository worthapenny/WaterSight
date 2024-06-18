using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.HydrulicStructures;


public class Tank : WSItem
{
    #region Constructor
    public Tank(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD Operations
    public async Task<TankConfig?> AddTankConfigAsync(TankConfig tankConfig)
    {
        var url = EndPoints.HydStructuresTankQDT;
        int? id = await WS.AddAsync<int?>(tankConfig, url, "Tank");
        if (id.HasValue)
        {
            tankConfig.Id = id.Value;
            return tankConfig;
        }

        return null;
    }
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload Excel file for Tanks.");
        return await WS.PostFile(EndPoints.HydStructuresTanksQDT, fileInfo, false, "Excel");
    }
    public async Task<bool> UpdateTankConfigAsync(TankConfig tankConfig)
    {
        var url = EndPoints.HydStructuresTankForQDT(tankConfig.Id.Value);
        return await WS.UpdateAsync(tankConfig.Id, tankConfig, url, "Tank", true);
    }
    public async Task<TankConfig?> GetTankConfigAsync(int id)
    {
        var url = EndPoints.HydStructuresTankForQDT(id);
        var tankConfig = await WS.GetAsync<TankConfig>(url, id, "Tank");
        return tankConfig;
    }
    public async Task<List<TankConfig?>> GetTanksConfigAsync()
    {
        var url = EndPoints.HydStructuresTankQDT;
        return await WS.GetManyAsync<TankConfig>(url, "Tanks");
    }
    public async Task<bool> DeleteTankConfigAsync(int id)
    {
        var url = EndPoints.HydStructuresTankForQDT(id);
        return await WS.DeleteAsync(id, url, "Tank", false);
    }
    public async Task<bool> DeleteTanksConfigAsync()
    {
        Logger.Warning($"WaterSigh API doesn't provide an end-point for deleting all at once... So, deleting individually.");

        var success = true;
        var tanks = await GetTanksConfigAsync();
        foreach (var tank in tanks)
        {
            var deleted = await DeleteTankConfigAsync(tank.Id.Value);
            Logger.Information($"[{deleted}] Deleted. {tank}");
            success &= deleted;
        }

        return success;

    }
    #endregion

    #endregion

}



#region Model Classes
[DebuggerDisplay("{ToString()}")]
public class TankConfig
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public double? ReferenceElevation { get; set; }
    public double? MinLevel { get; set; }
    public double? MaxLevel { get; set; }
    public double? LowLevelAlarm { get; set; }
    public double? HighLevelAlarm { get; set; }
    public string? LengthUnits { get; set; }
    public double? MaxVolume { get; set; }
    public string? TankCurveName { get; set; }
    public int? TankCurveId { get; set; }
    public string? VolumeUnits { get; set; }
    public string? LevelSignalName { get; set; }
    public double? DesiredTurnoverTime { get; set; }
    public string? Tags { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Name}";
    }
}
#endregion

