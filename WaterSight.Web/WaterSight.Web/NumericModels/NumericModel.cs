﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.NumericModels;

public class NumericModel : WSItem
{
    #region Constructor
    public NumericModel(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Upload Files
    public async Task<bool> UpdloadZippedWaterModel(FileInfo fileInfo, TimeSpan? timeout = null)
    {
        if (!File.Exists(fileInfo.FullName))
        {
            Logger.Error($"Given path is not valid. Path: {fileInfo.FullName}");
            return false;
        }
        if (!fileInfo.Extension.ToLower().EndsWith("zip"))
        {
            Logger.Error($"Given file extension is not 'zip' type. Path: {fileInfo.FullName}");
            return false;
        }

        // See if an entry (model domain) is already there
        var modelDomains = await GetModelDomainsWaterType();
        ModelDomainConfig? modelDomain = null;

        // if model domain is not there, CREATE model domain
        if (modelDomains.Count > 0)
        {
            modelDomain = modelDomains.First();
            Logger.Information($"Found existing ModelDoamin. {modelDomain}");
        }
        else
        {
            modelDomain = new ModelDomainConfig(
                digitalTwinId: WS.Options.DigitalTwinId,
                epsgCode: WS.Options.EPSGCode.ToString() ?? throw new InvalidDataException("ESPG code cannot be null"),
                name: GetNewWaterModelDomainName(WS.Options.DigitalTwinId)
                );
            var id = await AddWaterModelDomain(modelDomain);
            if (id == null)
                modelDomain = null;
            else
                Logger.Information($"Created new ModelDoamin. {modelDomain}");
        }


        if (modelDomain == null)
        {
            Logger.Error($"Model Domain cannot be null, see previous error. Returning");
            return false;
        }

        // upload the model
        var userName = "CSharpAPI"; // TODO: find a way to get the right username
        var query = EndPoints.Query;
        var url = EndPoints.NumModelingModelDomainUploadOpModelDomain +
            $"?{query.DTID}&{query.Username(userName)}&{query.ModelDomainName(modelDomain.Name)}" +
            $"&{query.EpsgCode(modelDomain.EpsgCode)}" +
            $"&{query.Frequency(modelDomain.Interval)}&{query.Duration(modelDomain.ForecastHours.Value)}" +
            $"&{query.SpinupHours(modelDomain.SpinUpHours.Value)}&{query.HindcastHours(modelDomain.HindcastHours.Value)}" +
            $"&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fmodel%3Fmodelid%3D{modelDomain.Id}%23upload";

        var res = await Request.PostFile(url: url, fileInfo: fileInfo, timeout: timeout);
        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"Upload requested, checking LRO...");
            _ = await Request.WaitForLRO(res);
        }
        else
        {
            Logger.Error($"Failed to upload the Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return false;
        }

        Logger.Information($"Model uploaded. Path: {fileInfo.FullName}, ModelDomain = {modelDomain}");
        return true;
    }
    #endregion

    #region Water Model Domain CRUD
    public async Task<int?> AddWaterModelDomain(ModelDomainConfig modelDomain)
    {
        var id = await WS.AddAsync(
            t: modelDomain,
            url: EndPoints.NumModelingModelDomainDomainsQDT,
            typeName: "Model domain");

        if (id != null)
            modelDomain.Id = id.Value;

        return id;
    }

    public async Task<List<ModelDomainConfig>> GetModelDomainsWaterType()
    {        
        var url = EndPoints.NumModelingModelDomainDomainsQDT + $"&{EndPoints.Query.DTTypeWater}";
        var modelDomains = await WS.GetManyAsync<ModelDomainConfig>(url, "Water model domains");
        return modelDomains;
    }
    public async Task<List<ModelDomainConfig>> GetModelDomainsAllTypes()
    {
        var url = EndPoints.NumModelingModelDomainDomainsQDT;
        var modelDomains = await WS.GetManyAsync<ModelDomainConfig>(url, "Model Domain");
        return modelDomains;
    }

    public async Task<bool> DeleteWaterModelDomain(int? id)
    {
        return await WS.DeleteAsync(
            id: id,
            url: EndPoints.NumModelingModelDomainDomainsLRO(id.Value),
            typeName: "Model Domain",
            supportsLRO: true);
    }
    public async Task<bool> UpdateWaterModelDomain(ModelDomainConfig modelDomain)
    {
        return await WS.UpdateAsync(
            id: modelDomain.Id,
            t: modelDomain,
            url: EndPoints.NumModelingModelDomainDomainsQDTQDomainId(modelDomain.Id.Value),
            typeName: "Model Domain",
            usePostMethod: true);

    }
    #endregion

    #region Simulation Time Steps
    public async Task<List<DateTimeOffset>> GetSimulationTimeSteps(string modelDomainName)
    {
        var urlPart = EndPoints.NumModelingModelDomainTimeInstanceLastModelRun;
        urlPart += $@"?{EndPoints.Query.ModelDomainName(modelDomainName)}";
        urlPart += $"&{EndPoints.Query.DTID}&emergencyEventId=undefined";

        var dates = await WS.GetManyAsync<DateTimeOffset>(urlPart, "Simulation time-steps");
        return dates;
    }
    public async Task<List<DateTimeOffset>> GetSimulationTimeStepsWaterModel(string waterModelDomainName = "")
    {
        if(string.IsNullOrEmpty(waterModelDomainName))
            return await GetSimulationTimeSteps(waterModelDomainName);

        // Find out the water-model-domain-name
        var modelDomains = await GetModelDomainsWaterType();
        if(modelDomains == null || !modelDomains.Any())
        {
            Logger.Error($"Model domains cannot be blank.");
            return new List<DateTimeOffset>();
        }

        var waterModelDomain = modelDomains.First();
        return await GetSimulationTimeSteps(waterModelDomain.Name);
    }
    #endregion

    #region Model Elements
    public async Task<Dictionary<string, List<ModelScadaElementConfig>>> GetModelTargetElementsWaterModel(string modelDomainName= "")
    {
        if(!string.IsNullOrEmpty(modelDomainName))
            return await GetModelTargetElements(modelDomainName);

        // Find out the water-model-domain-name
        var modelDomains = await GetModelDomainsWaterType();
        if (modelDomains == null || !modelDomains.Any())
        {
            Logger.Error($"Model domains cannot be blank.");
            return new Dictionary<string, List<ModelScadaElementConfig>>();
        }

        var waterModelDomain = modelDomains.First();
        return await GetModelTargetElements(waterModelDomain.Name);
    }
    public async Task<Dictionary<string, List<ModelScadaElementConfig>>> GetModelTargetElements(string modelDomainName)
    {
        var url = EndPoints.NumModelingScadaElementModelElementsQDT;
        url += $"&{EndPoints.Query.ModelDomainName(modelDomainName)}";

        var map = await WS.GetAsync<Dictionary<string, List<ModelScadaElementConfig>>>(url, null, "SCADAElments");
        return map;
    }
    #endregion

    #endregion

    #region Private Methods
    private string GetNewWaterModelDomainName(int dtId)
    {
        return $"Water_{dtId}_{DateTime.Now:yyyyMMddHmmss}";
    }
    #endregion
}


#region Models

[DebuggerDisplay("{ToString()}")]
public class ModelScadaElementConfig
{
    #region Public Methods
    public int ScadaElementId { get; set; }
    public string SignalLabel { get; set; }
    public int ModelElementId { get; set; }
    public string ResultAttribute { get; set; }
    public string TargetField { get; set; }

    #endregion


    #region Overridden Methods
    public override string ToString()
    {
        return base.ToString();
    }
    #endregion

}



[DebuggerDisplay("{ToString()}")]
public class ModelDomainConfig
{
    #region Constructor
    public ModelDomainConfig()
    {
        // needed for JSON convert
    }
    public ModelDomainConfig(
        int digitalTwinId,
        string epsgCode,
        string name,
        int? id = 0,
        double spinupHours = 0.0,
        double hindcastHours = 18.0,
        double forecastHours = 6.0,
        int runEveryHour = 1,
        bool active = false,
        string modelFile = "not found",
        string modelType = "WaterGems",
        string demandAdjustment = "NoAdjustment"
        )
    {
        DigitalTwinId = digitalTwinId;
        EpsgCode = epsgCode;
        Id = id;
        Name = name;
        SpinUpHours = spinupHours;
        HindcastHours = hindcastHours;
        ForecastHours = forecastHours;
        Interval = runEveryHour;
        Active = active;
        ModelFile = modelFile;
        DemandAdjustment = demandAdjustment;
        Type = modelType;
    }
    #endregion

    public int? Id { get; set; }
    public int DigitalTwinId { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int Interval { get; set; }
    public double? SpinUpHours { get; set; }
    public double? HindcastHours { get; set; }
    public double? ForecastHours { get; set; }
    public string? EpsgCode { get; set; }
    public string? ModelFile { get; set; }
    public bool Active { get; set; }
    public string? DemandAdjustment { get; set; }

    [JsonIgnore]
    public int bridgedVersion { get; set; }
    public List<object> AdjustmentSignals { get; set; } = new List<object>();
    public List<object> AdjustmentZones { get; set; } = new List<object>();

    public override string ToString()
    {
        return $"{Id}: {Name} [{Type}] Hours: [{SpinUpHours}, {HindcastHours}, {ForecastHours}]";
    }
}


#endregion