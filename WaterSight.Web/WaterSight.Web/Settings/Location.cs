using Newtonsoft.Json;
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
        var url = EndPoints.DTCoordinatesQDTSet(locationConfig.Latitude ?? 0, locationConfig.Longitude ?? 0);
        return await WS.PostAsync(url, null, "Location", additionalInfo: $"{locationConfig}");
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
    public LocationConfig(double[] latLng)
    {
        Latitude = latLng[0];
        Longitude = latLng[1];
    }
    public LocationConfig(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }
    #endregion

    [JsonProperty("Latitude")]
    public double? Latitude { get; set; }

    [JsonProperty("Longitude")]
    public double? Longitude { get; set; }

    public override string ToString()
    {
        return $"(Lng:{Longitude}, Lat:{Latitude})";
    }
}
#endregion