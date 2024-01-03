using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Watchdog;

public class WatchDog: WSItem
{
    #region Constructor
    public WatchDog(WS ws): base(ws)
    {            
    }
    #endregion

    #region Public Methods
    public async Task<Dictionary<string, DateTimeOffset>> OscOverviewAllDTs()
    {
        var url = EndPoints.WatchdogStatusOverviewOnSiteCoord;
        var idToDateTimeMap = await WS.GetAsync<Dictionary<string, DateTimeOffset>>(url, null, "OSC status overview");
        return idToDateTimeMap;
    }
    public async Task<DateTimeOffset> OscLastTalk()
    {
        var url = EndPoints.WatchdogStatusOnSiteCoordQDT;
        var at = await WS.GetAsync<DateTimeOffset>(url, null, "OSC status 1 DT");
        return at;
    }

    public async Task<List<DigitalTwinLastData>> PusherOverviewAllDTs()
    {
        var url = EndPoints.WatchdogStatusOverviewScadaPusher;
        var list = await WS.GetManyAsync<DigitalTwinLastData>(url, "Pusher Overview");
        return list;
    }
    public async Task<Dictionary<string, DateTimeOffset>> PusherSummary()
    {
        var url = EndPoints.WatchdogStatusScadaPusherSummaryQDT;
        var idToDateTimeMap = await WS.GetAsync<Dictionary<string, DateTimeOffset>>(url, null, "Pusher summary");
        return idToDateTimeMap;
    }
    public async Task<DateTimeOffset> PusherLastTalk()
    {
        var url = EndPoints.WatchdogStatusScadaPusherSummaryQDT;
        var at = await WS.GetAsync<DateTimeOffset>(url, null, "Pusher last talk");
        return at;
    }
    #endregion
}


#region Supporting Class

[DebuggerDisplay("ID: {DigitalTwinId}, # Signals: {SignalCount}, Latest: {Latest}")]
public class DigitalTwinLastData
{
    public int DigitalTwinId { get; set; }
    public DateTimeOffset Latest { get; set; }
    public int SignalCount { get; set; }
}
#endregion
