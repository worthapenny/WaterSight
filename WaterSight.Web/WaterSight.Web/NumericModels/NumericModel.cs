﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;
using WaterSight.Web.Support;

namespace WaterSight.Web.NumericModels;

public class NumericModel : WSItem
{


    #region Constants
    public const string ParameterLinkFlowAbs = "Reaches/flowabsolute";
    public const string ParameterLinkHeadLossGradient = "Reaches/headlossgradient";
    public const string ParameterLinkVelocity = "Reaches/velocity";
    public const string ParameterLinkHeadloss = "Reaches/headloss";
    public const string ParameterLinkFlow = "Reaches/flow";
    public const string ParameterLinkTrace = "Reaches/trace";
    public const string ParameterLinkAge = "Reaches/age";
    public const string ParameterLinkConcentration = "Reaches/concentration";

    public const string ParameterNodeDemand = "Nodes/demand";
    public const string ParameterNodeHGL = "Nodes/hgl";
    public const string ParameterNodePressure = "Nodes/pressure";
    public const string ParameterNodeTrace = "Nodes/trace";
    public const string ParameterNodeAge = "Nodes/age";
    public const string ParameterNodeConcentration = "Nodes/concentration";

    public const string ParameterPumpDischargePressure = "Pumps/dischargepressure";
    public const string ParameterPumpSuctionPressure = "Pumps/intakepressure";
    public const string ParameterPumpHead = "Pumps/pumphead";
    public const string ParameterPumpStatus = "Pumps/pumpstatus";
    public const string ParameterPumpRelativeSpeed = "Pumps/relativespeed";
    public const string ParameterPumpFlow = "Pumps/flow";
    public const string ParameterPumpTrace = "Pumps/trace";
    public const string ParameterPumpAge = "Pumps/age";
    public const string ParameterPumpConcentration = "Pumps/concentration";

    public const string ParameterTankHGL = "Tank/hgl";
    public const string ParameterTankLevel = "Tank/tanklevel";
    public const string ParameterTankPercentFull = "Tank/percentfull";
    public const string ParameterTankFlowOutNet = "Tank/flowoutnet";
    public const string ParameterTankTrace = "Tank/trace";
    public const string ParameterTankAge = "Tank/age";
    public const string ParameterTankConcentration = "Tank/concentration";

    public const string ParameterValveFromPressure = "Valves/valvefrompressure";
    public const string ParameterValveHeadLoss = "Valves/valveheadloss";
    public const string ParameterValveStatus = "Valves/valvestatus";
    public const string ParameterValveToPressure = "Valves/topressure";
    public const string ParameterValveFlow = "Valves/flow";
    public const string ParameterValveTrace = "Valves/trace";
    public const string ParameterValveAge = "Valves/age";
    public const string ParameterValveConcentration = "Valves/concentration";

    public const string ParameterReservoirFlowOut = "Reservoir/flowout";
    public const string ParameterReservoirHGL = "Reservoir/hgl";
    public const string ParameterReservoirTrace = "Reservoir/trace";
    public const string ParameterReservoirAge = "Reservoir/age";
    public const string ParameterReservoirConcentration = "Reservoir/concentration";
    #endregion

    #region Constructor
    public NumericModel(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Upload Files
    public async Task<bool> UpdloadZippedWaterModel(FileInfo fileInfo, TimeSpan? timeout = null, double? spinUpHours =0)
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

        // check for existing model domain, if not found, CREATE model domain
        if (modelDomains.Count > 0)
        {
            modelDomain = modelDomains.First();
            Logger.Information($"Found existing ModelDomain. {modelDomain}");
        }
        else
        {
            modelDomain = new ModelDomainConfig(
                digitalTwinId: WS.Options.DigitalTwinId,
                epsgCode: WS.Options.EPSGCode.ToString() ?? throw new InvalidDataException("ESPG code cannot be null"),
                name: GetNewWaterModelDomainName(WS.Options.DigitalTwinId)
                );
            modelDomain.SpinUpHours = spinUpHours;

            var id = await AddWaterModelDomain(modelDomain);
            if (id == null)
                modelDomain = null;
            else
                Logger.Information($"Created new ModelDomain. {modelDomain}. EPSG Code: {WS.Options.EPSGCode}");
        }


        if (modelDomain == null)
        {
            Logger.Error($"Model Domain cannot be null, see previous error. Returning");
            return false;
        }

        // upload the model
        var userName = "CSharpAPI"; // TODO: find a way to get the right username
        var query = EndPoints.Query;

        // // [OLD METHOD]
        //var url = EndPoints.NumModelingModelDomainUploadOpModelDomain +
        //    $"?{query.DTID}&{query.Username(userName)}&{query.ModelDomainName(modelDomain.Name)}" +
        //    $"&{query.EpsgCode(modelDomain.EpsgCode)}" +
        //    $"&{query.Frequency(modelDomain.Interval)}&{query.Duration(modelDomain.ForecastHours.StatQueryValue)}" +
        //    $"&{query.SpinupHours(modelDomain.SpinUpHours.StatQueryValue)}&{query.HindcastHours(modelDomain.HindcastHours.StatQueryValue)}" +
        //    $"&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fmodel%3Fmodelid%3D{modelDomain.Id}%23upload";


        // Get storage token and modify the URL
        var uploadTokenUrl = await WS.BlobStorage.GetStorageTokenUrlAsync();
        var uploadTokenUrlParts = uploadTokenUrl.Split('?');
        var fileName = fileInfo.Name;
        var uploadTokenUrlModified = $"{uploadTokenUrlParts[0]}/{Uri.EscapeUriString(fileName)}?{uploadTokenUrlParts[1]}";

        // NOT sure exactly what happens here but we need to do a PUT request
        var httpContent = new StringContent(string.Empty);
        httpContent.Headers.Add("X-Ms-Blob-Type", "BlockBlob");
        httpContent.Headers.Add("X-Ms-Client-Request-Id", "89c1bd86-9c47-48be-9a32-9f0086d5752d"); // Not sure where this ID is coming from
        httpContent.Headers.Add("X-Ms-Version", "2023-01-03");
        // what else?

        var isPutSuccessful = await WS.PutAsync(uploadTokenUrlModified, httpContent, "Upload Token URL");
        if (!isPutSuccessful)
        {
            Logger.Error($"PUT request on upload token failed.");
            return false;
        }

        var url = EndPoints.NumModelingModelDomainUploadOpModelDomainViaBlobStorage +
            $"?{query.ModelDomainName(modelDomain.Name)}" +
            $"&{query.DTID}" +
            $"&{query.EpsgCode(modelDomain.EpsgCode)}";


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
        var id = await WS.AddAsync<int?>(
            data: modelDomain,
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
            payLoad: modelDomain,
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
        if (!string.IsNullOrEmpty(waterModelDomainName))
            return await GetSimulationTimeSteps(waterModelDomainName);

        // Find out the water-model-domain-name
        var modelDomains = await GetModelDomainsWaterType();
        if (modelDomains == null || !modelDomains.Any())
        {
            Logger.Error($"Model domains cannot be blank.");
            return new List<DateTimeOffset>();
        }

        var waterModelDomain = modelDomains.First();
        return await GetSimulationTimeSteps(waterModelDomain.Name);
    }
    #endregion

    #region Model Elements
    public async Task<List<ModelScadaElementConfig>> GetMappedScadaElementsWaterModel(int modelElementId, string modelDomainName = "")
    {
        if (!string.IsNullOrEmpty(modelDomainName))
            return await GetMappedScadaElements(modelDomainName, modelElementId);

        // Find out the water-model-domain-name
        var modelDomains = await GetModelDomainsWaterType();
        if (modelDomains == null || !modelDomains.Any())
        {
            Logger.Error($"Model domains cannot be blank.");
            return new List<ModelScadaElementConfig>();
        }

        var waterModelDomain = modelDomains.First();
        return await GetMappedScadaElements(waterModelDomain.Name, modelElementId);
    }
    public async Task<List<ModelScadaElementConfig>> GetMappedScadaElements(string modelDomainName, int modelElementId)
    {
        var sb = new StringBuilder(EndPoints.NumModelingScadaElementScadaElementsQDT)
            .Append($"&{EndPoints.Query.ModelDomainName(modelDomainName)}")
            .Append($"&{EndPoints.Query.ModelElementId(modelElementId)}");

        var map = await WS.GetAsync<List<ModelScadaElementConfig>>(url: sb.ToString(), id: null, typeName: "Mapped SCADAElments");
        return map;
    }

    public async Task<Dictionary<string, List<ModelScadaElementConfig>>> GetModelTargetElementsWaterModel(string? modelDomainName = "")
    {
        if (!string.IsNullOrEmpty(modelDomainName))
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
    public async Task<List<ResultParameter>> GetParameters(
        string modelDomainName,
        int domainElementTypeId)
    {
        var sb = new StringBuilder(EndPoints.NumModelingParamGetByDomainAndElemType)
            .Append($"?{EndPoints.Query.DTID}")
            .Append($"&{EndPoints.Query.ModelDomainName(modelDomainName)}")
            .Append($"&{EndPoints.Query.DomainElementTypeId(domainElementTypeId)}");

        var url = sb.ToString();
        var parameters = await WS.GetManyAsync<ResultParameter>(url, "Result Parameters");
        return parameters;
    }
    #endregion

    #region Model Results
    public async Task<ElementResults> GetModelResultsAtTime(
        int elementId,
        int domainElementTypeId,
        string modelDomainName,
        DateTimeOffset at)
    {
        var sb = new StringBuilder(EndPoints.NumModelingElementsResults)
            .Append($"?{EndPoints.Query.ModelDomainName(modelDomainName)}")
            .Append($"&{EndPoints.Query.ElementId(elementId)}")
            .Append($"&{EndPoints.Query.DomainElementTypeId(domainElementTypeId)}")
            .Append($"&{EndPoints.Query.TimeStep(at)}")
            .Append($"&{EndPoints.Query.DTID}")
            .Append($"&{EndPoints.Query.EmergencyEventId(null)}");

        var url = sb.ToString();
        var results = await WS.GetAsync<ElementResults>(url, null, "Element Results");
        return results;
    }
    public async Task<ElementTsdResult> GetModelResults(
        int elementId,
        string parameterName,
        string modelDomainName,
        DateTimeOffset? startDate = null, // default = T-24hrs
        DateTimeOffset? endDate = null // default = Now
        )
    {
        var sb = new StringBuilder(EndPoints.NumModelingModelTSD)
            .Append($"/{elementId}")
            .Append($"?{EndPoints.Query.ModelDomainName(modelDomainName)}")
            .Append($"&{EndPoints.Query.ParameterName(parameterName)}")
            .Append($"&{EndPoints.Query.StartDateTime(startDate ?? DateTimeOffset.UtcNow.AddDays(-1))}")
            .Append($"&{EndPoints.Query.EndDateTime(endDate ?? DateTimeOffset.UtcNow)}")
            .Append($"&{EndPoints.Query.DTID}");

        var url = sb.ToString();
        var tsdResults = await WS.GetAsync<ElementTsdResult>(url, null, "Element TSD Result");
        return tsdResults;
    }
    public async Task<List<PreviousSimulation>> GetPreviousSimulations(
        DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        var url = $"{EndPoints.NumModelingModelRunsQDT}&startDate={startDate.UtcDateTime:O}&endDate={endDate.UtcDateTime:O}";
        var simulations = await WS.GetManyAsync<PreviousSimulation>(url, "PreviousSimulations");
        return simulations;
    }
    public async Task<List<PreviousSimulation>> GetPreviousSimulationsLast24Hrs()
    {
        return await GetPreviousSimulations(
            startDate: DateTimeOffset.UtcNow.AddDays(-1),
            endDate: DateTimeOffset.UtcNow);
    }

    public async Task<string?> DownloadPreviousSimulation(PreviousSimulation prevSim, bool onlyLogFiles, string outputDir)
    {
        var sw = Util.StartTimer();
        var onlyLogFilesInt = onlyLogFiles ? 1 : 0;
        var actionId = $"http://watersight.bentley.com/simulations/previous?modelrunid={prevSim.ID}#download";

        // Step 1 - Telll the server to prepare the file        
        var sb = new StringBuilder(EndPoints.NumModelingModelRunsDataFileQDT(prevSim.ID))
            .Append($"&onlyLogFiles={onlyLogFilesInt}")
            .Append($"&actionId={actionId}");
        var url = sb.ToString();

        Logger.Information($"About to prepare the file to download... URL: {url}");

        var posted = await WS.PostAsync(
            url: url,
            content: null,
            typeName: "PreviousSimDownload",
            supportsLRO: true);

        // Setp 2 - Download the file
        if (posted)
        {
            Logger.Information($"File is read to be downloaded to local...");
            sb = new StringBuilder(EndPoints.NumModelingModelRunsDataFileQDT(prevSim.ID))
                        .Append($"&onlyLogFiles={onlyLogFilesInt}");
            var urlDownload = sb.ToString() ;
                        
            var filePath = await WS.GetFile(
                url: urlDownload, 
                fileDestinationPath: outputDir,
                typeName: "DownloadPrevModelFile");

            if (File.Exists(filePath))
            {
                Logger.Information($"[🕛 {sw.Elapsed}] ✅ Downloaded successfully. Path: {filePath}");
                Logger.Debug(Util.LogSeparatorEquals);
            }
            else
                Logger.Error($"❌ Failed to download the file. Please review the logs.");

            return filePath;
        }
        else{
            Logger.Error($"❌ Preparing file failed. Please review the logs.");
        }

        return null;
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


#region Enums
public enum WaterDomainElementTypeId
{
    Pipe = 0,
    Node = 1,
    Pump = 2,
    Tank = 3,
    FCV = 4,
    GPV = 5,
    PBV = 6,
    TCV = 7,
    PRV = 8,
    PSV = 9,
    Reservoir = 10,
    SCADA = 11,
    Lateral = 12,
    Customer = 13,
    PumpStation = 14,
    IsolationValve = 15,
}

public enum SewerDomainElementTypeId
{
    PondOutletStructure = 30,
    CrossSectionNode = 31,
    CatchBasin = 32,
    Manhole = 33,
    JunctionChamber = 34,
    Pump = 35,
    Outfall = 36,
    WetWell = 37,
    PressureJunction = 38,
    AirValve = 39,
    Headwall = 40,
    PropertyConnection = 41,
    Gutter = 42,
    Conduit = 43,
    PressurePipe = 44,
    Channel = 45,
    Catchment = 46,
    Pond = 47,
    LID = 48,
}

#endregion

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
        return $"{ScadaElementId}: {SignalLabel} ({ResultAttribute})";
    }
    #endregion

}

[DebuggerDisplay("{ToString()}")]
public class ResultParameter
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Units { get; set; }
    public int Quantity { get; set; }
    public int? StorageUnit { get; set; }

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}: {DisplayName} Quants = {Quantity}";
    }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class ElementFieldResult
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public double Value { get; set; }


    #region Overridden Methods
    public override string ToString()
    {
        return $"{Value} [{DisplayName}]";
    }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class ElementResults
{
    public int ElementId { get; set; }
    public List<ElementFieldResult> ElementFieldResults { get; set; } = new List<ElementFieldResult>();

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ElementId}: Count = {ElementFieldResults.Count}";
    }
    #endregion
}


public class ElementTsdResult
{
    public string Name { get; set; }
    public string Parameter { get; set; }
    public string Units { get; set; }
    public List<SensorTSDWebPoint> Values { get; set; }
    public object Percentiles { get; set; }

    #region Overridden Methods
    public override string ToString()
    {
        return $"{Name} [{Parameter}, {Units}] Count = {Values.Count}";
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

    public bool UpdateValveStatus { get; set; }
    public int GpvType { get; set; }
    public int TcvType { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Name} [{Type}] Hours: [{SpinUpHours}, {HindcastHours}, {ForecastHours}]";
    }
}


[DebuggerDisplay("{ToString()}")]
public class PreviousSimulation
{
    public int ID { get; set; }
    public DateTimeOffset StartInstant { get; set; }
    public DateTimeOffset EndInstant { get; set; }
    public int ModelDomainID { get; set; }
    public string ModelDomainName { get; set; }
    public int ModelType { get; set; }
    public string DataPreparationError { get; set; }
    public string DataStorageError { get; set; }
    public string ModelExecutionError { get; set; }
    public string ModelFileName { get; set; }
    public bool RunSuccessful { get; set; }
    public double Runtime { get; set; }

    public override string ToString()
    {
        return $"{ID}: MDN = {ModelDomainName} [{ModelType}] [{StartInstant}, {EndInstant}] Run Success = {RunSuccessful}";
    }
}

#endregion