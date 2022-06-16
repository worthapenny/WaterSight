using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.DT;

public enum DigitalTwinType
{
    Water = 0,
    Sewer = 1,
}

public class DigitalTwin : WSItem
{
    #region Constructor
    public DigitalTwin(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Digital Twin CRUD Operations

    //
    // ADD / CREATE
    // 
    public async Task<DigitalTwinConfig?> AddDigitalTwinAsync(DigitalTwinCreateOptions createOptions)
    {
        var url = EndPoints.DTConnectedQDtNameQDtType(createOptions.DigitalTwinName, (int)createOptions.DigitalTwinType);
        var dtConfig = await WS.AddAsync<DigitalTwinConfig>(url, "Digital Twin");
        if (dtConfig != null)
        {
            // update goals
            _ = await UpdateDigitalTwinGoalAsync(dtConfig.ID, createOptions);

            return dtConfig;
        }

        return dtConfig;
    }

    // 
    // READ
    //
    public async Task<List<DigitalTwinConfig>> GetDigitalTwinsAsync()
    {
        var url = EndPoints.DTConnectedUser;
        var digitalTwins = await WS.GetManyAsync<DigitalTwinConfig>(url, "Digital Twins");

        return digitalTwins;
    }

    public async Task<DigitalTwinConfig> GetDigitalTwinAsync(int dtId)
    {
        var url = EndPoints.DTDigitalTwinsQDT;
        return await WS.GetAsync<DigitalTwinConfig>(url, dtId, "Digital Twin");
    }
    

    //
    // UPDATE Digital Twin's Description
    //
    public async Task<bool> UpdateDigitalTwinDescriptionAsync(DigitalTwinConfig digitalTwinConfig)
    {
        var url = $"{EndPoints.DTDigitalTwinsQDT}&{EndPoints.Query.Name(digitalTwinConfig.Name)}";
        return await WS.UpdateAsync<dynamic>(
            null,
            new { Description = digitalTwinConfig.Description },
            url,
            "Digital Twin");
    }

    //
    // UPDATE Digital Twin's Avatar
    //
    public async Task<bool> UpdateDigitalTwinAvatarAsync(string imagePath, DigitalTwinConfig digitalTwinConfig)
    {
        if(!File.Exists(imagePath))
        {
            Logger.Error($"Given image path is not valid. Path: {imagePath}");
            return false;
        }

        var url = EndPoints.DTAvatarQDT;
        return await WS.PostFile(url, new FileInfo(imagePath), "Bitmap");
    }

    //
    // UPDATE Digital Twin GOALS
    //
    public async Task<bool> UpdateDigitalTwinGoalAsync(int dtId, DigitalTwinCreateOptions createOptions)
    {
        var goalsUrl = EndPoints.DTGoals(dtId: dtId);
        return await WS.UpdateAsync(null, createOptions, goalsUrl, "Goals");
    }


    //
    // DELETE
    //
    public async Task<bool> DeleteDigitalTwinAsync(int id)
    {
        // NOTE: do not use the digitalTwin query from EndPoint
        // as it will be different from  give id
        var url = $"{EndPoints.DTConnected}?digitalTwinId={id}";
        return await WS.DeleteAsync(id, url, "Digital Twin", false);
    }

    //
    // GET / READ
    //
    /*public async Task<DigitalTwinConfig?> GetAsync(this WS ws, int? dtId, string url, string typeName)
    {
        var res = await Request.Get(url);
        DigitalTwinConfig dt = null;

        if (res.IsSuccessStatusCode)
        {
            try
            {
                var jsonString = await res.Content.ReadAsStringAsync();
                dt = JsonConvert.DeserializeObject<DigitalTwinConfig>(jsonString);
                WS.Logger.Information($"{typeName} info found {(dtId == null ? "" : $"for id: {dtId}")}, {dt}.");
                return dt;
            }
            catch (Exception ex)
            {
                WS.Logger.Error(ex, $"...while getting {typeName} from id: {dtId} \nMessage:{ex.Message}");
                return dt;
            }
        }
        else
        {
            WS.Logger.Error($"Failed to get {typeName} data for id: {dtId}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return dt;
        }
    }*/
    #endregion

    #endregion
}

public static class DigitalTwinExt
{
    //public static async Task<List<DigitalTwinConfig>> GetDigitalTwinsAsync(this WS ws)
    //{
        
    //}

    

    //public static async Task<bool> IsWaterSightAccessibleAsync(this WS ws)
    //{
    //    var dts = await ws.GetDigitalTwinsAsync();
    //    return dts != null && dts.Count > 0;
    //}
}


[DebuggerDisplay("{ToString()}")]
public class DigitalTwinConfig
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string GUID { get; set; } = string.Empty;
    public bool Published { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool MarkedForDeletion { get; set; }
    public string ImodelName { get; set; } = string.Empty;
    public Entity Entity { get; set; } = new Entity();
    public int DigitalTwinType { get; set; }
    public List<TagGroup> TagGroups { get; set; } = new List<TagGroup>();
    public string RegionId { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{ID}: {Name}";
    }
}


[DebuggerDisplay("{ID}: {Name}")]
public class Entity
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BentleyId { get; set; } = string.Empty;
}

[DebuggerDisplay("{ToString()}")]
public class TagGroup
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public object? Description { get; set; }
    public int Occupants { get; set; }

    public override string ToString()
    {
        return $"{ID}: {Name}";
    }
}
