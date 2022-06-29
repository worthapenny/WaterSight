using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.GIS;

public class GIS : WSItem
{
    #region Constants
    public const string PIPES_TYPE_NAME = "Pipes";
    public const string ZONES_TYPE_NAME = "Zones";
    #endregion

    #region Constructor
    public GIS(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    //
    // Shapefile Lists
    //
    public async Task<List<ShapefileProperty>> GetShapefileProperties()
    {
        var url = EndPoints.GeoFeaturesShpPropsQDT;
        var props = await WS.GetManyAsync<ShapefileProperty>(url, "Shapefile propperties");
        return props;
    }

    //
    // Zones
    //
    public async Task<bool?> UploadPressureZoneZippedShpFile(string shapefilePath)
    {

        WS.Logger.Debug($"About to upload a shapefile. Path: {shapefilePath}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Zones") + "#upload";
        var uploaded = await WS.PostFile(url, new FileInfo(shapefilePath), true, "Shapefile");
        return uploaded;
    }
    public async Task<bool?> DeletePressureZoneZippedShpFile()
    {
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Zones") + "#delete&removeVectorType=false";
        var deleted = await WS.DeleteAsync(null, url, "Pressure Zone Shapefile", true);
        return deleted;
    }

    //
    // PIPES
    //
    public async Task<bool?> UploadPipeZippedShpFile(string shapefilePath)
    {

        WS.Logger.Debug($"About to upload a shapefile. Path: {shapefilePath}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Pipes") + "#upload";

        var uploaded = await WS.PostFile(url, new FileInfo(shapefilePath), true, "Shapefile");
        return uploaded;
    }
    public async Task<bool?> DeletePipeZippedShpFile()
    {
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO("Pipes") + "#delete&removeVectorType=false";
        var deleted = await WS.DeleteAsync(null, url, "Pipe Shapefile", true);
        return deleted;
    }

    //
    // Any Shapefile
    //
    public async Task<bool?> UploadAnyZippedShpFile(string zippedShapefilePath, string dataTypeName)
    {
        // Check if path is valid
        if (!File.Exists(zippedShapefilePath))
        {
            Logger.Error($"Given path is not valid. Path:{zippedShapefilePath}");
            return false;
        }

        // Create the data type first
        // but before creating check if it exists
        var existingGisConfigs = await GetShapefileProperties();
        if (existingGisConfigs?.Count > 1)
        {
            var nameCheck = existingGisConfigs.Where(c => c.Type == dataTypeName);
            if (nameCheck.Any())
            {
                Logger.Debug($"Given type '{dataTypeName}' already exists, skip creating again.");
            }
            else
            {
                var urlDataTypes = EndPoints.GeoFeaturesVectorDataTypesQDTVectorType(dataTypeName);

                var dataTypeRes = await Request.PostJsonString(urlDataTypes, "{timeout: 30000}");
                if (!dataTypeRes.IsSuccessStatusCode)
                {
                    Logger.Error($"FAILED to create '{dataTypeName}' data type without which custom shapefile cannot be uploaded. Make sure '{dataTypeName}' does not exits already");
                    return false;
                }
                else
                {
                    WS.Logger.Information($"Given type '{dataTypeName}' create successfully.");
                }
            }
        }


        Support.Util.IsFileInUse(zippedShapefilePath);

        Logger.Debug($"About to upload a shapefile. Path: {zippedShapefilePath}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO(dataTypeName);
        var uploaded = await WS.PostFile(url, new FileInfo(zippedShapefilePath), true, "Shapefile");
        return uploaded;

    }
    public async Task<bool?> DeleteAnyZippedShpFile(string dataTypeName)
    {
        Logger.Debug($"About to delete a shapefile data type {dataTypeName}. Path: {dataTypeName}");
        var url = EndPoints.GeoFeaturesVectorDataQDTVectorTypeLRO(dataTypeName) + "#delete&removeVectorType=true";
        var deleted = await WS.DeleteAsync(null, url, "Any shapefile", true);
        return deleted;

    }
    #endregion
}

#region Model Classes

[DebuggerDisplay("{ToString()}")]
public class ShapefileProperty
{
    public string Name { get; set; }
    public string NameZip { get; set; }
    public string Type { get; set; }
    public object IdAsset { get; set; }
    public double SizeMb { get; set; }
    public double SizeMbZip { get; set; }
    public double SizeMbCollection { get; set; }
    public string Date { get; set; }
    public int BridgedVersion { get; set; }

    #region Overridden Methods
    public override string ToString()
    {
        return $"{Name} [{Type}]";
    }
    #endregion
}
#endregion