using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Settings;
public class Location : WSItem
{

    #region Constructor
    public Location(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Get
    public async Task<LocationConfig?> GetLocation()
    {
        var url = EndPoints.DTCoordinatesQDT;
        return await WS.GetAsync<LocationConfig>(url, null, "Location");
    }
    #endregion

    #region Set
    public async Task<bool> SetLocation(LocationConfig locationConfig)
    {
        var url = EndPoints.DTCoordinatesQDTSet(locationConfig.Latitude ?? 0, locationConfig.Longitude?? 0);
        return await WS.PostAsync(url, null, "Location", additionalInfo: $"{locationConfig}");
    }
    public async Task<bool> SetLocation(float? latitude, float? longitude)
    {
        var url = EndPoints.DTCoordinatesQDTSet(latitude ?? 0, longitude ?? 0);
        return await WS.PostAsync(url, null, "Location", additionalInfo: $"Lat/Long = [{latitude}, {longitude}]");
    }
    #endregion

    #endregion
}




#region Model
[DebuggerDisplay("{ToString()}")]
public class LocationConfig
{
    #region Constructor
    public LocationConfig()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="latLng">First parameter must be Lat</param>
    public LocationConfig(float[] latLng)
    {
        Latitude = latLng[0];
        Longitude = latLng[1];
    }
    public LocationConfig(float lat, float lng)
    {
        Latitude = lat;
        Longitude = lng;
    }
    #endregion

    [JsonProperty("Latitude")]
    public float? Latitude { get; set; }

    [JsonProperty("Longitude")]
    public float? Longitude { get; set; }

    public override string ToString()
    {
        return $"(Lng:{Longitude}, Lat:{Latitude})";
    }
}
#endregion