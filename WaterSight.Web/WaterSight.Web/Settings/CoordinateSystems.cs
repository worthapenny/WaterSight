using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;


namespace WaterSight.Web.Settings;
public class CoordinateSystems : WSItem
{
    #region Constructor
    public CoordinateSystems(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Get All Supported
    public async Task<List<CoordinateSystemsConfig?>> GetAll()
    {
        var url = EndPoints.DTCoordinateSystemQDT;
        return await WS.GetManyAsync<CoordinateSystemsConfig>(url, "CoordinateSystems");
    }
    #endregion

    #region Get 
    //public async Task<CoordinateSystemsConfig?> GetAvgVolumeticProductionCost()
    //{
    //    var url = EndPoints.?;
    //    return await WS.GetAsync<CoordinateSystemsConfig>(url, null, "?");
    //}
    //public async Task<CoordinateSystemsConfig?> GetAvgVolumetricTarrif()
    //{
    //    var url = EndPoints.?;
    //    return await WS.GetAsync<CoordinateSystemsConfig>(url, null, "?");
    //}
    //public async Task<CoordinateSystemsConfig?> GetAvgEnergyCost()
    //{
    //    var url = EndPoints.?;
    //    return await WS.GetAsync<CoordinateSystemsConfig>(url, null, "?");
    //}

    #endregion

    #region Set
    // NOTE: No EPSG code validation is performed
    // When invalid code is supplied, exception is thrown by the server
    public async Task<bool> SetSensors(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet("Sensors", epsg);
        return await WS.PutAsync(url, null, "Sensors", additionalInfo: $"Epsg: {epsg}");
    }
    public async Task<bool> SetCustomers(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet("Customers", epsg);
        return await WS.PutAsync(url, null, "Customers", additionalInfo: $"Epsg: {epsg}");
    }
    public async Task<bool> SetWorkOrders(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet("Work Orders", epsg);
        return await WS.PutAsync(url, null, "WorkOrders", additionalInfo: $"Epsg: {epsg}");
    }
    #endregion

    #endregion
}


#region Model
[DebuggerDisplay("{ToString()}")]
public class CoordinateSystemsConfig
{
    public int? ID { get; set; }
    public string? Name { get; set; }
    public int? Epsg { get; set; }

    public override string ToString()
    {
        return $"{ID}: {Name}, EPSG: {Epsg}";
    }
}
#endregion