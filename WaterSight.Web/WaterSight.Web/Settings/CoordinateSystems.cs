using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;


namespace WaterSight.Web.Settings;
public class CoordinateSystems : WSItem
{
    #region Constants
    public const string NameSensors = "Sensors";
    public const string NameCustomers = "Customers";
    public const string NameSmartMeters = "Smart Meters";
    public const string NameWorkOrders= "Work Orders";
    #endregion

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
        var url = EndPoints.DTCoordinateSystemQDTSet(NameSensors, epsg);
        return await WS.PutAsync(url, null, NameSensors, additionalInfo: $"Epsg: {epsg}");
    }
    public async Task<bool> SetCustomers(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet(NameCustomers, epsg);
        return await WS.PutAsync(url, null, NameCustomers, additionalInfo: $"Epsg: {epsg}");
    }
    public async Task<bool> SetSmartMeters(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet(NameSmartMeters, epsg);
        return await WS.PutAsync(url, null, NameSmartMeters, additionalInfo: $"Epsg: {epsg}");
    }
    public async Task<bool> SetWorkOrders(int epsg)
    {
        var url = EndPoints.DTCoordinateSystemQDTSet(NameWorkOrders, epsg);
        return await WS.PutAsync(url, null, NameWorkOrders, additionalInfo: $"Epsg: {epsg}");
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