using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.ExternalService;

public enum NavMenuItem
{
    PowerBI = 0,
    NetworkMonitoring = 1,
    Pumps = 2,
    Tanks = 3,
    WaterAudit = 4,
    EventManagement = 5,
    RealtimeSimulations = 6,
    CapitalPlanning = 7
}

public class PowerBI : WSItem
{
    #region Constructor
    public PowerBI(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD Operation
    //
    // CREATE
    public async Task<PowerBiConfig> AddPowerBiConfigAsync(PowerBiConfig powerBiConfig)
    {
        var url = EndPoints.DTPowerBIUrlQDT;
        powerBiConfig = await WS.AddAsync<PowerBiConfig>(powerBiConfig, url, "Power BI Item");
        
        return powerBiConfig;
    }

    //
    // READ / GET
    public async Task<List<PowerBiConfig?>> GetPowerBiConfigsAsync()
    {
        var url = EndPoints.DTPowerBIUrlQDT;
        return await WS.GetManyAsync<PowerBiConfig>(url, "Power BI Items");
    }

    //
    // UPDATE
    public async Task<bool> UpdatePowerBiConfig(PowerBiConfig powerBiConfig)
    {
        var url = EndPoints.DTPowerBIUrlQDT;
        return await WS.UpdateAsync(null, powerBiConfig, url, "Power BI");
    }

    //
    // DELTE
    public async Task<bool> DeletePowerBiConfig(PowerBiConfig powerBiConfig)
    {
        var url = EndPoints.DTPowerBIUrlQDT;
        url += $"&urlId={powerBiConfig.Id}";

        return await WS.DeleteAsync(null, url, "Power BI");
    }
    #endregion

    #endregion
}

#region Model Classes


[DebuggerDisplay("{ToString()}")]
public class PowerBiConfig
{
    #region Constructor
    public PowerBiConfig()
    {
    }
    public PowerBiConfig(string name, string url, int digitalTwinId, NavMenuItem navMenuItem = NavMenuItem.PowerBI)
    {
        Name = name;
        Url = url;
        DigitalTwinId = digitalTwinId;
        Id = -1;
        Menu = navMenuItem;
    }
    #endregion

    #region Public Properties
    public string? Name { get; set; }
    public string? Url { get; set; }
    public int? Id { get; set; }
    public int DigitalTwinId { get; set; }
    public NavMenuItem Menu { get; set; } = NavMenuItem.PowerBI;
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{Id}] {Name} in {Menu.ToString()}";
    }
    #endregion
}
#endregion