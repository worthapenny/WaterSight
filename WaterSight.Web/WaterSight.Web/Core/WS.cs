﻿using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Alerts;
using WaterSight.Web.BlobStorages;
using WaterSight.Web.Custom;
using WaterSight.Web.Customers;
using WaterSight.Web.DT;
using WaterSight.Web.ExternalService;
using WaterSight.Web.HydrulicStructures;
using WaterSight.Web.Landings;
using WaterSight.Web.NumericModels;
using WaterSight.Web.Support;
using WaterSight.Web.Support.IO;
using WaterSight.Web.User;
using WaterSight.Web.Zones;

namespace WaterSight.Web.Core;

/// <summary>
/// The main access point for the WaterSight Web API
/// </summary>
public class WS
{

    #region Constructor
    public WS(
        string tokenRegistryPath,
        int digitalTwinId = -1,
        int epsgCode = -1,
        Env env = Env.Prod,
        ILogger? logger = null,
        string subDomainSuffix = "",
        string? restToken = null,
        string? pat = null,
        string? logFilesDir = null)
    {
        Options = new Options(
            digitalTwinId,
            tokenRegistryPath,
            env: env,
            subDomainSuffix: subDomainSuffix,
            restToken: restToken);

        Options.PAT = pat;
        Options.EPSGCode = epsgCode;
        Request._options = Options;
        EndPoints = new EndPoints(Options);


        DigitalTwin = new DigitalTwin(this);
        Sensor = new Sensors.Sensor(this);
        SmartMeter = new SmartMeters.SmartMeter(this);
        Alert = new Alerts.Alert(this);
        GIS = new GIS.GIS(this);
        HydStructure = new HydStructure(this);
        Zone = new Zone(this);
        NumericModel = new NumericModel(this);
        Customers = new Users(this);
        PowerBI = new PowerBI(this);
        Settings = new Settings.Settings(this);
        UserInfo = new UserInfo(this);
        Setup = new Setup.Setup(this);
        CustomWaterModel = new WaterModel(this);
        BlobStorage = new BlobStorage(this);
        Home = new Home(this);
        WatchDog = new Watchdog.WatchDog(this);
        EmailGroup = new EmailGroup(this);

        if (logger == null)
        {
            var logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
            Logging.SetupLogger(
                appName: "WaterSightAPI",
                logTemplate: logTemplate,
                logEventLevel: Serilog.Events.LogEventLevel.Debug,
                logFilesDir: logFilesDir
                );
        }

        Logger = Serilog.Log.Logger;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        Logger.Information("☑️ WaterSight instance created.");
    }
    #endregion



    #region Public Methods 

    public void UpdateAttributes(int id, Env env)
    {
        Options.DigitalTwinId = id;
        Options.Env = env;
        EndPoints.Update(apiVersion: "v1");
    }


    #region CRUD Operations


    //
    // ADD / CREATE
    //
    public async Task<T> AddAsync<T>(object data, string url, string typeName)
    {
        var jsonText =JsonConvert.SerializeObject(data);
        var res = await Request.PostJsonString(url, jsonText);
        //int? id = null;
        T t = default;
        if (res.IsSuccessStatusCode)
        {
            var content = await res.Content.ReadAsStringAsync();
            //id = Convert.ToInt32(content.Replace("\"", ""));
            t = JsonConvert.DeserializeObject<T>(content);

            Logger.Information($"✅ {typeName} added, '{t}'.");
        }
        else
            Logger.Error($"💀 Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return t;
    }
    public async Task<T> AddAsync<T>(string url, string typeName)
    {
        var res = await Request.Post(url, new StringContent(string.Empty));
        T retValue = default;
        if (res.IsSuccessStatusCode)
        {
            var responseString = await res.Content.ReadAsStringAsync();
            retValue = JsonConvert.DeserializeObject<T>(responseString);
            Logger.Information($"✅ {typeName} added. {retValue}");
        }
        else
            Logger.Error($"💀 Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

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
            Logger.Error($"💀 Failed to get {typeName} data for id: {id}. Reason: {res.ReasonPhrase}. Text: {resContentText}. URL: {url}");
            return t;
        }

        if (!isLRO)
        {
            try
            {
                t = await Request.GetJsonAsync<T>(res);

                Logger.Information($"✅ {typeName} info found {(id == null ? "" : $"for id: {id}")}, {t}.");
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
    // Get File 
    //
    public async Task<string?> GetFile(string url, string fileDestinationPath, string typeName)
    {
        var filePath = await Request.GetFile(url, fileDestinationPath);
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            Logger.Information($"✅ Download complete for {typeName}. Path = {filePath}.");
        else
            Logger.Error($"💀 Failed to download {typeName} content. URL: {url}");

        return filePath;
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
            Logger.Information($"✅ Number of {typeName} received. Count = {t.Count}.");
        }
        else
            Logger.Error($"💀 Failed to get {typeName} data. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

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
    /// <param name="payLoad"></param>
    /// <param name="url"></param>
    /// <param name="typeName"></param>
    /// <param name="usePostMethod"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync<T>(int? id, T payLoad, string url, string typeName, bool usePostMethod = false)
    {
        if (payLoad == null)
        {
            Logger.Error($"{typeName} cannot be null");
            return false;
        }

        HttpResponseMessage? res = null;
        if (!usePostMethod)
        {
            res = await Request.PutJsonString(url, JsonConvert.SerializeObject(payLoad));
        }
        else
            res = await Request.PostJsonString(url, JsonConvert.SerializeObject(payLoad));

        if (res?.IsSuccessStatusCode ?? false)
            Logger.Information($"✅ {typeName} updated successfully.");

        else
            Logger.Error($"💀 Failed to update {typeName} with id: {id} ({payLoad}). Reason: {res?.ReasonPhrase}. Text: {await res?.Content.ReadAsStringAsync()}. URL: {url}");

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
            Logger.Information($"✅ {typeName}'s delete request was successful for id: {id}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"⚠️ {typeName}'s delete request failed for id: {id}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
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
            Logger.Information($"✅ {typeName}'s delete request for  was successful");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"⚠️ {typeName}'s delete request failed. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
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
            Logger.Information($"✅ {typeName}'s post request for  was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"⚠️ {typeName}'s post request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
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
            Logger.Error($"💀 Given {fileTypeName} file path is not valid. Path: {fileInfo.FullName}");
            return false;
        }

        if (Util.IsFileInUse(fileInfo.FullName))
            Debugger.Break();

        var res = await Request.PostFile(url, fileInfo);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"✅ {fileTypeName} file uploaded successfully. Path: {fileInfo.FullName}");
            if (supportsLRO)
                return await Request.WaitForLRO(res);

            return res.IsSuccessStatusCode;
        }
        else
        {
            Logger.Error($"💀 Failed to upload the {fileTypeName} file. Path: {fileInfo.FullName} Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
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
            Logger.Information($"✅ {typeName}'s put request was successful. {additionalInfo}");
            if (supportsLRO)
                _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Warning($"⚠️ {typeName}'s put request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        }

        return res.IsSuccessStatusCode;
    }
    public async Task<T> PutAsync<T>(string url, object payload, string typeName, bool supportsLRO = false, string additionalInfo = "")
    {
        T t = default;

        var res = await Request.PutJsonString(url, JsonConvert.SerializeObject(payload));
        if (!res.IsSuccessStatusCode)
        {
            var resContentText = res.Content == null ? "" : await res.Content?.ReadAsStringAsync();
            Logger.Error($"💀 Failed to get {typeName} data. Reason: {res.ReasonPhrase}. Text: {resContentText}. URL: {url}");
            return t;
        }


        if (!supportsLRO)
        {
            try
            {
                t = await Request.GetJsonAsync<T>(res);

                Logger.Information($"✅ {typeName} info found, {t}.");
                return t;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"...💀...while getting {typeName} \nMessage:{ex.Message}");
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

    #endregion

    #endregion

    #region Public Properties
    public Options Options { get; }
    public EndPoints EndPoints { get; }
    public static ILogger Logger { get; private set; } // = new LoggerConfiguration().CreateLogger();
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();

    public DigitalTwin DigitalTwin { get; }
    public Sensors.Sensor Sensor { get; }
    public SmartMeters.SmartMeter SmartMeter { get; }
    public Alerts.Alert Alert { get; }
    public GIS.GIS GIS { get; }
    public HydStructure HydStructure { get; }
    public Zone Zone { get; }
    public NumericModel NumericModel { get; }
    public Users Customers { get; }
    public PowerBI PowerBI { get; }
    public Settings.Settings Settings { get; }
    public UserInfo UserInfo { get; }
    public Setup.Setup Setup { get; }
    public WaterModel CustomWaterModel { get; }
    public BlobStorage BlobStorage { get; }
    public Home Home { get; }
    public Watchdog.WatchDog WatchDog { get; }
    public EmailGroup EmailGroup { get; }
    #endregion
}
