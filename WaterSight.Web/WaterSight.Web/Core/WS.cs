using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.BlobStorages;
using WaterSight.Web.Custom;
using WaterSight.Web.DT;
using WaterSight.Web.ExternalService;
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
    public WS(string tokenRegistryPath, int digitalTwinId = -1, int epsgCode = -1, Env env = Env.Prod, ILogger? logger = null, string subDomainSuffix = "")
    {
        Options = new Options(digitalTwinId, tokenRegistryPath, env:env, subDomainSuffix: subDomainSuffix);
        Options.EPSGCode = epsgCode;
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
        Zone = new Zone(this);
        NumericModel = new NumericModel(this);
        Customers = new Customers.Customers(this);
        PowerBI = new PowerBI(this);
        Settings = new Settings.Settings(this);
        UserInfo = new UserInfo(this);
        Setup = new Setup.Setup(this);
        CustomWaterModel = new WaterModel(this);
        BlobStorage = new BlobStorage(this);

        if (logger == null)
        {
            var logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.MinimumLevel.Verbose()
                //.MinimumLevel.Debug()
                .MinimumLevel.ControlledBy(LoggingLevelSwitch) // Default's to Information 
                .WriteTo.Debug(outputTemplate: logTemplate)
                .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    logFileFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    flushToDiskInterval: TimeSpan.FromSeconds(5),
                    outputTemplate: logTemplate)
                .CreateLogger();

            // Set default to Debug
            SetLoggingLevelToDebug();
        }

        WS.Logger = Log.Logger;

        Log.Debug("");
        Log.Debug($"Logging is ready. Path: {logFileFile}");
    }
    #endregion



    #region Public Methods - CRUD

    public void SetLoggingLevelToInfo()
    {
        LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
    }

    public void SetLoggingLevelToDebug()
    {
        LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
    }
    public void SetLoggingLevelToVerbose()
    {
        LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
    }


    #region CRUD Operations


    //
    // ADD / CREATE
    //
    public async Task<T> AddAsync<T>(object data, string url, string typeName)
    {
        var jsonText = JsonConvert.SerializeObject(data);
        var res = await Request.PostJsonString(url, jsonText);
        //int? id = null;
        T t = default;
        if (res.IsSuccessStatusCode)
        {
            var content = await res.Content.ReadAsStringAsync();
            //id = Convert.ToInt32(content.Replace("\"", ""));
            t = JsonConvert.DeserializeObject<T>(content);

            Logger.Information($"{typeName} added, '{t}'.");
        }
        else
            Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return t;
    }
    public async Task<T> AddAsync<T>(string url, string typeName)
    {
        var res = await Request.Post(url, null);
        T retValue = default;
        if (res.IsSuccessStatusCode)
        {
            var responseString = await res.Content.ReadAsStringAsync();
            retValue = JsonConvert.DeserializeObject<T>(responseString);
            Logger.Information($"{typeName} added. {retValue}");
        }
        else
            Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return retValue;
    }

    //
    // GET / READ
    //
    /// <summary>
    /// If LRO is true, the return type must be bool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="id"></param>
    /// <param name="typeName"></param>
    /// <param name="isLRO">Supports Long Running Oprations. Return type must be bool</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string url, int? id, string typeName, bool isLRO = false)
    {
        T t = default;

        var res = await Request.Get(url);
        if (!res.IsSuccessStatusCode)
        {
            var resContentText = res.Content == null ? "" : await res.Content?.ReadAsStringAsync();
            Logger.Error($"Failed to get {typeName} data for id: {id}. Reason: {res.ReasonPhrase}. Text: {resContentText}. URL: {url}");
            return t;
        }

        if (!isLRO)
        {
            try
            {
                t = await Request.GetJsonAsync<T>(res);

                Logger.Information($"{typeName} info found {(id == null ? "" : $"for id: {id}")}, {t}.");
                return t;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"...while getting {typeName} from id: {id} \nMessage:{ex.Message}");
                return t;
            }

        }
        else // isLRO = true
        {
            var completed = await Request.WaitForLRO(res);

            t = (T)(object)completed;
            return t;
        }
    }

    //
    // GET Many / READ many
    //
    public async Task<List<T>> GetManyAsync<T>(string url, string typeName)
    {
        var res = await Request.Get(url);
        var t = new List<T>();

        if (res.IsSuccessStatusCode)
        {
            t = await Request.GetJsonAsync<List<T?>>(res) ?? t;
            Logger.Information($"Number of {typeName} received. Count = {t.Count}.");
        }
        else
            Logger.Error($"Failed to get {typeName} data. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return t;
    }

    //
    // UPDATE
    //
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">For better logging</param>
    /// <param name="t"></param>
    /// <param name="url"></param>
    /// <param name="typeName"></param>
    /// <param name="usePostMethod"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync<T>(int? id, T t, string url, string typeName, bool usePostMethod = false)
    {
        if (t == null)
        {
            Logger.Error($"{typeName} cannot be null");
            return false;
        }

        HttpResponseMessage? res = null;
        if (!usePostMethod)
            res = await Request.PutJsonString(url, JsonConvert.SerializeObject(t));
        else
            res = await Request.PostJsonString(url, JsonConvert.SerializeObject(t));

        if (res?.IsSuccessStatusCode ?? false)
            Logger.Information($"{typeName} updated successfully.");

        else
            Logger.Error($"Failed to update {typeName} with id: {id} ({t}). Reason: {res?.ReasonPhrase}. Text: {await res?.Content.ReadAsStringAsync()}. URL: {url}");

        return res.IsSuccessStatusCode;
    }

    // 
    // DELETE Single
    //
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">For better logging</param>
    /// <param name="url"></param>
    /// <param name="typeName"></param>
    /// <param name="supportsLRO"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(int? id, string url, string typeName, bool supportsLRO = false)
    {
        var res = await Request.Delete(url);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"{typeName}'s delete request was successful for id: {id}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"{typeName}'s delete request failed for id: {id}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    // 
    // DELETE Many
    //
    public async Task<bool> DeleteManyAsync(string url, string typeName, bool supportsLRO = false)
    {
        var res = await Request.Delete(url);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"{typeName}'s delete request for  was successful");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"{typeName}'s delete request failed. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    //
    // POST (Any)
    //
    public async Task<bool> PostAsync(string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
    {
        var res = await Request.Post(url, content);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"{typeName}'s post request for  was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"{typeName}'s post request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    //
    // POST (JSON)
    //
    public async Task<bool> PostJson(string url, object? payload, bool supportsLRO = false, string additionalInfo = "")
    {
        var jsonString = JsonConvert.SerializeObject(payload);
        var jsonStringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        return await PostAsync(
            url: url,
            content: jsonStringContent,
            typeName: "JSON",
            supportsLRO: supportsLRO,
            additionalInfo: additionalInfo
            );
    }

    //
    // POST FIle
    //
    public async Task<bool> PostFile(string url, FileInfo fileInfo, bool supportsLRO = false, string fileTypeName = "Excel")
    {
        if (!fileInfo.Exists)
        {
            Logger.Error($"Given {fileTypeName} file path is not valid. Path: {fileInfo.FullName}");
            return false;
        }

        if (Util.IsFileInUse(fileInfo.FullName))
            Debugger.Break();

        var res = await Request.PostFile(url, fileInfo);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"{fileTypeName} file uploaded successfully. Path: {fileInfo.FullName}");
            if (supportsLRO)
                return await Request.WaitForLRO(res);

            return res.IsSuccessStatusCode;
        }
        else
        {
            Logger.Error($"Failed to upload the {fileTypeName} file. Path: {fileInfo.FullName} Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return false;
        }
    }

    //
    // PUT (Any)
    //
    public async Task<bool> PutAsync(string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
    {
        var res = await Request.Put(url, content);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"{typeName}'s put request for  was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"{typeName}'s put request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }

    #endregion

    #endregion

    #region Public Properties
    public Options Options { get; }
    public EndPoints EndPoints { get; }
    public static ILogger Logger { get; private set; } // = new LoggerConfiguration().CreateLogger();
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();

    public DigitalTwin DigitalTwin { get; }
    public Sensors.Sensor Sensor { get; }
    public Alerts.Alert Alert { get; }
    public GIS.GIS GIS { get; }
    public HydStructure HydStructure { get; }
    public Zone Zone { get; }
    public NumericModel NumericModel { get; }
    public Customers.Customers Customers { get; }
    public PowerBI PowerBI { get; }
    public Settings.Settings Settings { get; }
    public UserInfo UserInfo { get; }
    public Setup.Setup Setup { get; }
    public WaterModel CustomWaterModel { get; }
    public BlobStorage BlobStorage { get; }
    #endregion
}
