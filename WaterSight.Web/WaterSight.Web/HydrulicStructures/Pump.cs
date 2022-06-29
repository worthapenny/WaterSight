using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Zones;

namespace WaterSight.Web.HydrulicStructures;

public class Pump : WSItem
{
    #region Constructor
    public Pump(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload Excel file for Pumps.");
        return await WS.PostFile(EndPoints.HydStructuresPumpsQDT, fileInfo, true, "Excel");
    }

    #region CRUD Operation Pump Curve

    #endregion

    #region CRUD Operation Pumps
    //
    // Create
    public async Task<PumpConfig?> AddPumpConfigAsync(PumpConfig pumpConfig)
    {
        var url = EndPoints.HydStructuresPumpQDT;
        int? id = await WS.AddAsync<int?>(pumpConfig, url, "Pump");
        if (id.HasValue)
{
            pumpConfig.Id = id.Value;
            return pumpConfig;
        }

        return null;
    }

    //
    // GET/Read
    public async Task<List<PumpConfig>> GetPumpsConfigAsync()
    {
        var url = EndPoints.HydStructuresPumpQDT;
        return await WS.GetManyAsync<PumpConfig>(url, "Pumps");
    }
    public async Task<PumpConfig> GetPumpConfigAsync(int pumpId)
    {
        var url = EndPoints.HydStructuresPumpForQDT(pumpId);
        return await WS.GetAsync<PumpConfig>(url, pumpId, "Pump");
    }

    //
    // UPDATE
    public async Task<bool> UpdatePumpConfigAsync(PumpConfig pumpConfig)
    {
        var url = EndPoints.HydStructuresPumpForQDT(pumpConfig.Id);
        return await WS.UpdateAsync(pumpConfig.Id, pumpConfig, url, "Pump", true);
    }

    //
    // DELETE
    public async Task<bool> DeletePumpsConfigAsync()
    {
        Logger.Debug($"About to delete all the pumps...");
        var url = EndPoints.HydStructuresPumpQDT;
        var success = await WS.DeleteManyAsync(url, "Pumps", true);

        Logger.Debug($"Deleted.");
        return success;
    }
    public async Task<bool> DeleteZoneConfigAsync(int pumpId)
    {
        var url = EndPoints.HydStructuresPumpForQDT(pumpId);
        return await WS.DeleteAsync(pumpId, url, "Pump", true);
    }
    #endregion

    #endregion

}

#region Model

[DebuggerDisplay("{ToString()}")]
public class PumpConfig
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public double DesignFlow { get; set; }
    public double DesignHead { get; set; }
    public double EfficiencyFlow { get; set; }
    public double EfficiencyHead { get; set; }
    public double Efficiency { get; set; }
    public string FlowUnits { get; set; }
    public string HeadUnits { get; set; }
    public string PumpingStationName { get; set; }
    public int PumpingStationId { get; set; }
    public string SuctionSignalName { get; set; }
    public string DischargeSignalName { get; set; }
    public string FlowSignalName { get; set; }
    public string WirePowerSignalName { get; set; }
    public string HeadCurveName { get; set; }
    public int HeadCurveId { get; set; }
    public string EfficiencyCurveName { get; set; }
    public int EfficiencyCurveId { get; set; }
    public string PowerCurveName { get; set; }
    public int PowerCurveId { get; set; }
    public string PowerUnits { get; set; }
    public int MotorEfficiency { get; set; }
    public string IsVariableSpeedAsString { get; set; }
    public bool IsVariableSpeed { get; set; }
    public int? FullSpeedValue { get; set; }
    public string SpeedEfficiencyCurveName { get; set; }
    public int? SpeedEfficiencyCurveId { get; set; }
    public string SpeedSignalName { get; set; }
    public string StatusSignalName { get; set; }
    public string Tags { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{Id}: {Name}, ";
    }
    #endregion
}
#endregion

#region Model Classes
public class PumpCurveConfig : CurveConfigBase
{
}
#endregion