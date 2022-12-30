using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Settings;

public class TimeZone:WSItem
{
    #region Constructor
    public TimeZone(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD
    public async Task<List<TimeZoneConfig>> GetTimeZones()
    {
        var url = EndPoints.DTTimezones;
        return await WS.GetManyAsync<TimeZoneConfig>(url, "Timezones");
    }
    public async Task<string> GetTimeZoneName()
    {
        var url = EndPoints.DTTimezone;
        return await WS.GetAsync<string>(url, null, "Timezone");
    }
    #endregion

    #region Set
    public async Task<bool> SetTimezone(TimeZoneConfig timezoneConfig)
    {
        var url = EndPoints.DTTimezoneSet(timezoneConfig.TimeZoneId);
        return await WS.PutAsync(url, null, "Timezone", additionalInfo: $"{timezoneConfig}");
    }
    #endregion

    #endregion
}

#region Model
[DebuggerDisplay("{ToString()}")]
public class TimeZoneConfig
{
    #region Constructor
    public TimeZoneConfig()
    {
    }    
    public TimeZoneConfig(string timeZoneId)
    {
       TimeZoneId = timeZoneId;
    }
    #endregion
        
    public string TimeZoneId { get; set; }

    public double Offset { get; set; }

    public override string ToString()
    {
        return $"{TimeZoneId} ({Offset})";
    }
}
#endregion