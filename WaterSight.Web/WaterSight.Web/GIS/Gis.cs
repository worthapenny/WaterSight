using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.GIS;

public class GIS : WSItem
{
    #region Constructor
    public GIS(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    //
    // Zones
    //
    public async Task<bool?> UploadPressureZoneZippedShpFile(string shapefilePath)
    {

        WS.Logger.Debug($"About to upload a shapefile. Path: {shapefilePath}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Zones") + "#upload";
        var res = await Request.PostFile(url, new FileInfo(shapefilePath));

        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Debug($"Pressure zone shapefile uploaded. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            WS.Logger.Error($"Failed to upload pressure zone shapefile. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }
    public async Task<bool?> DeletePressureZoneZippedShpFile()
    {
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Zones") + "#delete&removeVectorType=false";
        var res = await Request.Delete(url);

        if (res.IsSuccessStatusCode)
        {
            Logger.Debug($"Zones shapefile deleted. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            Logger.Error($"Failed to delete zones shapefile. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }

    //
    // PIPES
    //
    public async Task<bool?> UploadPipeZippedShpFile(string shapefilePath)
    {

        WS.Logger.Debug($"About to upload a shapefile. Path: {shapefilePath}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Pipes") + "#upload";
        var res = await Request.PostFile(url, new FileInfo(shapefilePath));

        if (res.IsSuccessStatusCode)
        {
            Logger.Debug($"Pipes shapefile uploaded. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            Logger.Error($"Failed to upload pipes shapefile. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }
    public async Task<bool?> DeletePipeZippedShpFile()
    {
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Pipes") + "#delete&removeVectorType=false";
        var res = await Request.Delete(url);

        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Debug($"Pipes shapefile deleted. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            WS.Logger.Error($"Failed to delete pipes shapefile. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }

    //
    // Any Shapefile
    //
    public async Task<bool?> UploadAnyZippedShpFile(string shapefilePath, string dataTypeName)
    {
        // Check if path is valid
        if(!File.Exists(shapefilePath))
        {
            Logger.Error($"Given path is not valid. Path:{shapefilePath}");
            return false;
        }

        // Create the data type first
        var url = EndPoints.GeoFeaturesVectorDataTypesQDTVectorType(dataTypeName);
        var dataTypeRes = await Request.PostJsonString(url, "{timeout: 30000}");
        if (!dataTypeRes.IsSuccessStatusCode)
        {
            Logger.Error($"Failed to create '{dataTypeName}' data type without which custom shapefile cannot be uploaded. Make sure '{dataTypeName}' does not exits already");
            return false;
        }
        else
        {
            WS.Logger.Information($"Given type '{dataTypeName}' create successfully.");
        }

        Support.Util.IsFileInUse(shapefilePath);

        Logger.Debug($"About to upload a shapefile. Path: {shapefilePath}");
        url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO(dataTypeName);
        var res = await Request.PostFile(url, new FileInfo(shapefilePath));

        if (res.IsSuccessStatusCode)
        {
            Logger.Debug($"Custom '{dataTypeName}' type shapefile uploaded. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            Logger.Error($"Failed to upload custom '{dataTypeName}' type shapefile. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }
    public async Task<bool?> DeleteAnyZippedShpFile(string dataTypeName)
    {
        Logger.Debug($"About to delete a shapefile data type {dataTypeName}. Path: {dataTypeName}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO(dataTypeName) + "#delete&removeVectorType=true";
        var res = await Request.Delete(url);

        if (res.IsSuccessStatusCode)
        {
            Logger.Debug($"Shapefile of type '{dataTypeName}' deleted. Checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
            Logger.Error($"Failed to delete shapefile of type '{dataTypeName}'. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }
    #endregion
}
