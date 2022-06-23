using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WaterSight.Web.DT;
using WaterSight.Web.HydrulicStructures;
using WaterSight.Web.NumericModels;
using WaterSight.Web.Support;
using WaterSight.Web.User;
using WaterSight.Web.Zones;

namespace WaterSight.Web.Core;

/// <summary>
/// The main access point for the WaterSight Web API
/// </summary>
public class WS
{

    #region Constructor
    public WS(string tokenRegistryPath, int digitalTwinId = -1, Env env = Env.Prod, ILogger? logger = null)
    {
        Options = new Options(digitalTwinId, tokenRegistryPath, env);
        Request.options = Options;
        EndPoints = new EndPoints(Options);

        var logFileFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\WS.Base_.log");

        var fileInfo = new FileInfo(logFileFile);
        if (!(fileInfo.Directory?.Exists ?? false))
            fileInfo.Directory?.Create();

        DigitalTwin = new DigitalTwin(this);
        Sensor = new Sensors.Sensor(this);
        Alert = new Alerts.Alert(this);
        GIS = new GIS.GIS(this);
        HydStructure = new HydStructure(this);
        Zone = new Zones.Zone(this);
        NumericModel = new NumericModel(this);
        Customers = new Customers.Customers(this);
        Settings = new Settings.Settings(this);
        UserInfo = new UserInfo(this);
        Setup = new Setup.Setup(this);

        if (logger == null)
        {
            var logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.MinimumLevel.Verbose()
                .MinimumLevel.Debug()
                .WriteTo.Debug(outputTemplate: logTemplate)
                .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    logFileFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    flushToDiskInterval: TimeSpan.FromSeconds(5),
                    outputTemplate: logTemplate)
                .CreateLogger();
        }

        WS.Logger = Log.Logger;

        Log.Debug("");
        Log.Debug($"Logging is ready. Path: {logFileFile}");
    }
    #endregion

    #region Public Methods - CRUD

    //
    // ADD / CREATE
    //
    public async Task<int?> AddAsync<T>(T t, string url, string typeName)
    {
        var res = await Request.PostJsonString(url, JsonConvert.SerializeObject(t));
        int? id = null;
        if (res.IsSuccessStatusCode)
        {
            var idString = await res.Content.ReadAsStringAsync();
            id = Convert.ToInt32(idString.Replace("\"", ""));
            WS.Logger.Information($"{typeName} added, id: {id}.");
        }
        else
            WS.Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return id;
    }
    public  async Task<T> AddAsync<T>( string url, string typeName)
    {
        var res = await Request.Post(url, null);
        T retValue = default;
        if (res.IsSuccessStatusCode)
        {
            var responseString = await res.Content.ReadAsStringAsync();
            retValue = JsonConvert.DeserializeObject<T>(responseString);
            WS.Logger.Information($"{typeName} added. {retValue}");
        }
        else
            WS.Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return retValue;
    }

    //
    // GET / READ
    //
    public  async Task<T> GetAsync<T>(string url, int? id, string typeName)
    {
        var res = await Request.Get(url);
        T t = default;

        if (res.IsSuccessStatusCode)
        {
            try
            {
                var jsonString = await res.Content.ReadAsStringAsync();
                t = JsonConvert.DeserializeObject<T>(jsonString);
                WS.Logger.Information($"{typeName} info found {(id == null ? "" : $"for id: {id}")}, {t}.");
                return t;
            }
            catch (Exception ex)
            {
                WS.Logger.Error(ex, $"...while getting {typeName} from id: {id} \nMessage:{ex.Message}");
                return t;
            }
        }
        else
        {
            WS.Logger.Error($"Failed to get {typeName} data for id: {id}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return t;
        }
    }

    //
    // GET Many / READ many
    //
    public  async Task<List<T>> GetManyAsync<T>(string url, string typeName)
    {
        var res = await Request.Get(url);
        var t = new List<T>();

        if (res.IsSuccessStatusCode)
        {
            t = await Request.GetJsonAsync<List<T?>>(res) ?? t;
            WS.Logger.Information($"Number of {typeName} received. Count = {t.Count}.");
        }
        else
            WS.Logger.Error($"Failed to get {typeName} data. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return t;
    }

    //
    // UPDATE
    //
    public  async Task<bool> UpdateAsync<T>(int? id, T t, string url, string typeName, bool usePostMethod = false)
    {
        if (t == null)
        {
            WS.Logger.Error($"{typeName} cannot be null");
            return false;
        }

        HttpResponseMessage? res = null;
        if (!usePostMethod)
            res = await Request.PutJsonString(url, JsonConvert.SerializeObject(t));
        else
            res = await Request.PostJsonString(url, JsonConvert.SerializeObject(t));

        if (res?.IsSuccessStatusCode ?? false)
            WS.Logger.Information($"{typeName} updated successfully.");

        else
            WS.Logger.Error($"Failed to update {typeName} with id: {id} ({t}). Reason: {res?.ReasonPhrase}. Text: {await res?.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }

    // 
    // DELETE Single
    //
    public  async Task<bool> DeleteAsync(int? id, string url, string typeName, bool supportsLRO = false)
    {
        var res = await Request.Delete(url);
        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Information($"{typeName}'s delete request was successful for id: {id}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            WS.Logger.Warning($"{typeName}'s delete request failed for id: {id}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    // 
    // DELETE Many
    //
    public  async Task<bool> DeleteManyAsync(string url, string typeName, bool supportsLRO = false)
    {
        var res = await Request.Delete(url);
        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Information($"{typeName}'s delete request for  was successful");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            WS.Logger.Warning($"{typeName}'s delete request failed. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    //
    // POST (Any)
    //
    public  async Task<bool> PostAsync(string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
    {
        var res = await Request.Post(url, content);
        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Information($"{typeName}'s post request for  was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            WS.Logger.Warning($"{typeName}'s post request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    //
    // POST FIle
    //
    public async Task<bool> PostFile(string url, FileInfo fileInfo, bool supportsLRO = false, string fileTypeName = "Excel")
    {
        if (!fileInfo.Exists)
        {
            WS.Logger.Error($"Given {fileTypeName} file path is not valid. Path: {fileInfo.FullName}");
            return false;
        }

        if (Util.IsFileInUse(fileInfo.FullName))
            Debugger.Break();

        var res = await Request.PostFile(url, fileInfo);
        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Information($"{fileTypeName} file uploaded successfully. Path: {fileInfo.FullName}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
            return true;
        }
        else
        {
            WS.Logger.Error($"Failed to upload the {fileTypeName} file. Path: {fileInfo.FullName} Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return false;
        }
    }

    //
    // PUT (Any)
    //
    public  async Task<bool> PutAsync(string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
    {
        var res = await Request.Put(url, content);
        if (res.IsSuccessStatusCode)
        {
            WS.Logger.Information($"{typeName}'s put request for  was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            WS.Logger.Warning($"{typeName}'s put request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }
    #endregion

    #region Public Properties
    public Options Options { get; }
    public EndPoints EndPoints { get; }
    public static ILogger Logger { get; private set; } // = new LoggerConfiguration().CreateLogger();

    public DigitalTwin DigitalTwin { get; }
    public Sensors.Sensor Sensor { get; }
    public Alerts.Alert Alert { get; }
    public GIS.GIS GIS { get; }
    public HydStructure HydStructure { get; }
    public Zone Zone { get; }
    public NumericModel NumericModel { get; }
    public Customers.Customers Customers { get; }
    public Settings.Settings Settings { get; }
    public UserInfo UserInfo { get; }
    public Setup.Setup Setup { get; }
    #endregion
}
