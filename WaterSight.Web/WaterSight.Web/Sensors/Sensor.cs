using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Extensions;
using WaterSight.Web.Support;
using Ganss.Excel;
using Serilog;
using WaterSight.Web.Support.IO;

namespace WaterSight.Web.Sensors;

public class Sensor : WSItem
{
    #region Constructor
    public Sensor(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Sensor CRUD operation
    //
    // CREATE
    public async Task<SensorConfig?> AddSensorConfigAsync(SensorConfig sensor)
    {
        var url = EndPoints.RtdaSignalsQDT;

        if(sensor.PatternWeekId == null || sensor.PatternWeekId == 0)
        {
            var defaultPattern = WS.Settings.PatternWeeks.GetDefaultPatternWeekConfigAsync();
            sensor.PatternWeekId = defaultPattern.Id;
        }

        int? id = await WS.AddAsync<int?>(sensor, url, "Sensor");
        if (id.HasValue)
        {
            sensor.ID = id.Value;
            return sensor;
        }

        return null;
    }

    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload Excel file for Sensors.");
        var success = await WS.PostFile(EndPoints.RtdaSignalsFileQDT, fileInfo, false, "Excel");

        Logger.Debug(Util.LogSeparatorSquare);
        return success;
    }

    public async Task<List<SensorConfig>> AddMissingSensorConfigAsync(List<SensorConfig> sensorConfigs)
    {
        var addedSensorConfigs = new List<SensorConfig>();

        var existingSensors = await GetSensorsConfigAsync();
        var uniqueNames = existingSensors.Select(s => $"{s.TagId}__{s.Name}").ToList();
        foreach (var sensorConfig in sensorConfigs)
        {
            if (!uniqueNames.Contains($"{sensorConfig.TagId}__{sensorConfig.Name}"))
            {
                var addedSensorConfig = await AddSensorConfigAsync(sensorConfig);
                if (addedSensorConfig != null)
                {
                    Logger.Information($"Missing sensor added. Sensor: {sensorConfig}");
                    addedSensorConfigs.Add(sensorConfig);
                }
            }
        }

        Logger.Information($"Total number of added (missing) sensors: {addedSensorConfigs.Count}");
        return addedSensorConfigs;
    }

    //
    // READ
    public async Task<SensorConfig?> GetSensorConfigAsync(int id)
    {
        var url = EndPoints.RtdaSignalsForQDT(id);
        SensorConfig? sensorConfig = await WS.GetAsync<SensorConfig>(url, id, "Sensor");
        return sensorConfig;
    }
    public async Task<List<SensorConfig?>> GetSensorsConfigAsync()
    {
        var url = EndPoints.RtdaSignalsQDT;
        return await WS.GetManyAsync<SensorConfig>(url, "Sensors");
    }

    //
    // UPDATE
    public async Task<bool> UpdateSensorConfigAsync(SensorConfig sensor)
    {
        var url = EndPoints.RtdaSignalsForQDT(sensor.ID);
        return await WS.UpdateAsync(sensor.ID, sensor, url, "Sensor");
    }

    //
    // DELETE
    public async Task<bool> DeleteSensorConfigAsync(int sensorId)
    {
        // TODO: update endpoint with LRO query, (remove from here)
        var url = EndPoints.RtdaSignalsForQDT(sensorId, true) + $"&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fsignals%3Fsensorid%3D{sensorId}%23delete";
        return await WS.DeleteAsync(sensorId, url, "Sensor", true);
    }
    public async Task<bool> DeleteSensorsConfigAsync()
    {
        Logger.Verbose("About to delete all the sensors...");
        var url = EndPoints.RtdaSignalsLROQDT + $"&{EndPoints.Query.ActionIdSignalsDelete("delete")}"; ;
        return await WS.DeleteManyAsync(url, "Sensors", true);
    }
    #endregion

    #region Sensor TSD

    public async Task<SensorTsdWeb?> GetSensorTSDAsync(
        int id,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        IntegrationType integrationType = IntegrationType.Fifteen)
    {
        var url = EndPoints.RtdaTsValuesWithin(id, startAt, endAt, integrationType);
        var res = await Request.Get(url);
        var tsd15M = await Request.GetJsonAsync<SensorTsdWeb>(res) ?? new SensorTsdWeb();

        if (res.StatusCode == HttpStatusCode.OK)
            Logger.Debug($"✅ Sensor TSD received for {id} [{startAt:u},{endAt:u}]. Count = {tsd15M?.UnifiedTSDs?.Count}");
        else
            Logger.Error($"💀 Failed to get sensor TSD for {id} [{startAt:u},{endAt:u}]. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return tsd15M;
    }

    public async Task<bool> PostJsonFileAsync(
        string jsonFilePath,
        List<string>? tagIds, // to filter to tags of interest
        bool onlyNoDataSensors = false,
        bool useLastInstanceFromServer = true
        )
    {
        if(!File.Exists( jsonFilePath ))
            throw new FileNotFoundException( jsonFilePath );

        var tsdValues = JsonIO.LoadFromFile<List<TSDValue>>(jsonFilePath);
        if (tsdValues == null)
            throw new DataException($"Json data is not in expected format.");

        Log.Debug($"Data loaded from JSON. Count: {tsdValues.Count}");
        var success = await PostTsdDataAsync(
            tsdValues: tsdValues,
            tagIds: tagIds,
            onlyNoDataSensors: onlyNoDataSensors,
            useLastInstanceFromServer: useLastInstanceFromServer);

        Log.Debug($"Done posting TSD Data.");
        return success;
    }

    public async Task<bool> PostTsdDataAsync(
        List<TSDValue> tsdValues,
        List<string>? tagIds, // to filter to tags of interest
        bool onlyNoDataSensors = false,
        bool useLastInstanceFromServer = true)
    {
        // Pull all the sensor configs from WaterSight
        // and select to tags of interest
        var sensorsConfig = await GetSensorsConfigAsync();

        if (tagIds?.Any() ?? false)
        {
            sensorsConfig = sensorsConfig
                .Where(s => tagIds.Contains(s.TagId))
                .ToList();

            Log.Debug($"Due to given 'tasIds', number of sensors to work with: {sensorsConfig.Count}");
        }

        // filter to sensors that have no data at all on WaterSight
        if (onlyNoDataSensors)
        {
            sensorsConfig = sensorsConfig
                .Where(s => s.LastInstantInDatabase == null)
                .ToList();
            
            Log.Debug($"Number of sensors with no data: {sensorsConfig.Count}. Data for these sensors will be pushed.");
        }

        // Work on pushing the data
        var pushedAll = true;

        var counter = 0;
        var totalSensors = sensorsConfig.Count;
        foreach (var sensorConfig in sensorsConfig)
        {
            try
            {
                var wsTag = sensorConfig.TagId;

                var filteredData = tsdValues
                   .Where(d => d.ID?.ToString() == wsTag && d.Value.HasValue)
                   .ToList();

                Log.Debug($"Number of data row to push against the '{wsTag}', name '{sensorConfig.Name}' tag is: {filteredData.Count}");
                if(!filteredData.Any() )
                {
                    Log.Information($"No data found for tag '{wsTag}', name '{sensorConfig.Name}' to push.");
                    continue;
                }

                var lastInstance = sensorConfig.LastInstantInDatabase;
                if (useLastInstanceFromServer && lastInstance != null)
                {
                    filteredData = filteredData
                        .Where(d => d.Instant > lastInstance.Value)
                        .ToList();

                    Log.Debug($"Number of data row after '{lastInstance}' to push against the '{wsTag}', name '{sensorConfig.Name}' tag is: {filteredData.Count}");
                    if (!filteredData.Any())
                    {
                        Log.Information($"No data found after '{lastInstance}' for tag '{wsTag}', name '{sensorConfig.Name}' to push.");
                        continue;
                    }
                }

                // Replace the ID with 0 (per WaterSight requirements)
                filteredData.ForEach(d => d.ID = 0);

                Logger.Information($"About to push TSD from '{lastInstance:d}'. Tag = '{wsTag}', Count = '{filteredData.Count}'");
                if (filteredData.Any())
                {
                    var success = await PostSensorTSDAsync(
                        sensorId: sensorConfig.ID,
                        data: filteredData,
                        tagNameForLogging: sensorConfig.TagId);

                    pushedAll = pushedAll & success;

                    counter++;
                    Logger.Information($"✅ [{counter}/{totalSensors}] Pushed data for tag = {sensorConfig.TagId}, name = {sensorConfig.Name}. Count = {filteredData.Count}");
                }
                else
                {
                    Logger.Warning($"⚠️ No data to push. tag = {sensorConfig.TagId}, name = {sensorConfig.Name}. Count = {filteredData.Count}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"💀...while pushing data,  tag = {sensorConfig.TagId}, name = {sensorConfig.Name}.");
                Debugger.Break();
            }

            Logger.Information($"Done posting data for {sensorConfig.Name}");
            Logger.Debug(new string('•', 100));
        }

        return pushedAll;

    }

    public async Task<bool> PostSensorTSDAsync(int sensorId, List<TSDValue> data, string tagNameForLogging = "")
    {
        var url = EndPoints.RtdaTsValuesFor(sensorId);
        var success = true;

        // Make sure data are sorted chronologically
        data = data.OrderBy(d => d.Instant).ToList();

        var maxItemCount = 30000;
        var stopwatch = Util.StartTimer();
        try
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

            var counter = 1;
            var groups = data.GroupAt(maxItemCount);
            foreach (var subset in groups)
            {
                var jsonString = JsonConvert.SerializeObject(subset.ToArray(), jsonSettings);
                var res = await Request.PostJsonString(url, jsonString);

                if (res.IsSuccessStatusCode)
                {
                    Logger.Debug($"[{counter}/{groups.Count()}] Sensor TSD posted for {sensorId}: '{tagNameForLogging}', Object Length: {data.Count}. Date range: [{subset.First().Instant:d}, {subset.Last().Instant:d}]");
                    counter++;
                    continue;
                }
                else
                {
                    success = false;
                    Logger.Error($"[{counter}/{groups.Count()}] Failed to post sensor TSD for {sensorId}: '{tagNameForLogging}', Object Length: {data.Count}, Date range: [{subset.First().Instant:d}, {subset.Last().Instant:d}] Reason: {res.ReasonPhrase}. Text: {await res.Content?.ReadAsStringAsync()}. URL: {url}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"...while posting sensor TSD value for {sensorId}: '{tagNameForLogging}'. Message:\n{ex.Message}");
            success = false;
        }
        finally
        {
            var timeTaken = stopwatch.Elapsed;
            Logger.Information($"Total time-taken to post data of size {data.Count}: {timeTaken}");
            stopwatch.Stop();
        }

        Logger.Debug(Util.LogSeparatorDots);
        return success;

    }

    #endregion

    #region Sensor Purge
    public async Task<bool> PurgeSensorsAsync()
    {
        Logger.Verbose($"About to purge data on ALL sensors");
        var url = EndPoints.RtdaTsValuesPurge;
        var res = await Request.Delete(url);
        var success = await Request.WaitForLRO(res);

        if (success)
            Logger.Information($"Purge for all sensors was successful");
        else
            Logger.Error($"Purge for all sensors FAILED. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return success;
    }
    public async Task<bool> PurgeSensorAsync(int sensorId)
    {
        Logger.Verbose($"About to purge data on sensor id: {sensorId}");
        var url = EndPoints.RtdaTsValuesPurgeFor(sensorId);
        var res = await Request.Delete(url);
        var success = await Request.WaitForLRO(res);

        if (success)
            Logger.Information($"Purge for {sensorId} was successful");
        else
            Logger.Error($"Purge for {sensorId} FAILED. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

        return success;
    }

    #endregion

    #endregion

}

public enum SensorType
{
    Chlorine,
    Flow,
    HydraulicGrade,
    Level,
    Other,
    pH,
    Power,
    Pressure,
    PumpSpeed,
    PumpStatus,
    Temperature,
    Turbidity,
    ValveSetting,
    ValveStatus,
    Volume
}

public struct SensorTypeName
{
    public const string Flow = "Flow";
    public const string HydraulicGrade = "HGL";
    public const string Level = "Level";
    public const string Other = "Other";
    public const string pH = "pH";
    public const string Power = "Power";
    public const string Pressure = "Pressure";
    public const string PumpSpeed = "Pump Speed";
    public const string Chlorine = "Chlorine";
    public const string PumpStatus = "Pump Status";
    public const string Turbidity = "Turbidity";
    public const string Temperature = "Temperature";
    public const string ValveSetting = "Valve Setting";
    public const string ValveStatus = "Valve Status";
    public const string Volume = "Volume";
}

#region Model Classes

[DebuggerDisplay("{ToString()}")]
public class SensorConfig
{

    #region Public Methods
    public SensorConfig Copy()
    {
        string json = JsonConvert.SerializeObject(this);
        return JsonConvert.DeserializeObject<SensorConfig>(json);
    }
    public string CsvHeader()
    {
        var header =  $"{nameof(ID)},{nameof(TagId)},{nameof(Name)},{nameof(ParameterType)},{nameof(Units)},";
        header += $"{nameof(CommunicationFrequency)},{nameof(RegistrationFrequency)},{nameof(UtcOffSet)},";
        header += $"{nameof(Latitude)},{nameof(Longitude)},{nameof(ReferenceElevation)},{nameof(ReferenceElevationUnits)},";
        header += $"{nameof(LastInstantInDatabase)},{nameof(Priority)},{nameof(Tags)},{nameof(PatternWeekId)}";
        return header;
    }

    public string ToCsv()
    {
        var csv = $"{ID},{TagId},{Name},{ParameterType},{Units},";
        csv += $"{CommunicationFrequency},{RegistrationFrequency},{UtcOffSet},";
        csv += $"{Latitude},{Longitude},{ReferenceElevation},{ReferenceElevationUnits},";
        csv += $"{LastInstantInDatabase},{Priority},{PatternWeekId}";
        return csv;
    }
    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }
    #endregion

    #region Public Properties

    public int? CommunicationFrequency { get; set; } = 5;
    public int ID { get; set; }

    [Column("LastInstanceDB"), DataFormat("yyyy-MM-dd hh:mm:ss")]
    public DateTimeOffset? LastInstantInDatabase { get; set; }
    public double? Latitude { get; set; } = 0.0;
    public double? Longitude { get; set; } = 0.0;
    public string Name { get; set; } = String.Empty;
    public string Notes { get; set; } = String.Empty;
    public bool Offline { get; set; } = false;
    public string ParameterType { get; set; } = String.Empty;
    public object[] PatternSpecialPeriodIds { get; set; } = new object[0];
    public int? PatternWeekId { get; set; } = null;
    public int? Priority { get; set; } = 1;
    public double? ReferenceElevation { get; set; }
    public string? ReferenceElevationUnits { get; set; }
    public int? RegistrationFrequency { get; set; } = 5;

    public string TagId { get; set; } = String.Empty;
    public string? Tags { get; set; } = String.Empty;
    public string? TimeZoneId { get; set; } = string.Empty;
    public string? Units { get; set; } = String.Empty;
    public string? UtcOffSet { get; set; } = "00:00";
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{ParameterType}] {ID}: {Name}, Tag:{TagId}";
    }
    #endregion
}


[DebuggerDisplay("{ToString()}")]
public class SensorTsdWeb
{
    #region Public Methods
    public DataTable PointsToDataTable(string tagName, string valueFieldName = "Value")
    {
        if (UnifiedTSDs.Count == 0)
            return new DataTable();

        var dt = new DataTable();
        dt.Columns.Add("Tag", typeof(string));
        dt.Columns.Add("DateTime", typeof(DateTimeOffset));

        var valueCol = dt.Columns.Add(valueFieldName, typeof(double));
        valueCol.AllowDBNull = true;


        foreach (var tsd in UnifiedTSDs)
        {
            var row = dt.NewRow();
            row["Tag"] = tagName;
            row["DateTime"] = tsd.Instant;
            row["StatQueryValue"] = tsd.UnifiedValue;

            dt.Rows.Add(row);
        }

        return dt;
    }
    #endregion

    public string? Name { get; set; }

    // Parameter / Parameter Name
    public string? ParameterName { get; set; }
    public string? Parameter { get; set; }
    public string? UnitfiedParameterName => ParameterName ?? Parameter ?? string.Empty;


    // Percentiles / SeriesPercentiles
    public List<double>? SeriesPercentiles { get; set; }
    public List<double>? Percentiles { get; set; }
    public List<double>? UnifiedPercentiles => Percentiles ?? SeriesPercentiles ?? new List<double>();

    // Points / Values/ TSD
    public List<SensorTSDWebPoint>? Points { get; set; }
    public List<SensorTSDWebPoint>? Values { get; set; }
    public List<SensorTSDWebPoint> UnifiedTSDs => Points ?? Values ?? new List<SensorTSDWebPoint>();

    public int SensorID { get; set; }


    #region Overridden Methods
    public override string ToString()
    {
        return $"{Name}, Count = {UnifiedTSDs.Count}";
    }
    #endregion
}

[DebuggerDisplay("{Instant}: {ValueSignal}")]
public class SensorTSDWebPoint : TSDValue
{
    public bool Flag { get; set; }
    public double? ValueSignal { get; set; }
    public double? UnifiedValue => Value ?? ValueSignal;
    public double? ValuePercentiles { get; set; }
}

[DebuggerDisplay("{ID}: {Value} @ {Instant}")]
public class TSDValue
{
    #region Constructor
    public TSDValue()
    {
    }
    public TSDValue(object? id, double? value, DateTimeOffset dateTime)
    {
        ID = id;
        Value = value;
        Instant = dateTime;
    }
    #endregion

    public object? ID { get; set; }
    public double? Value { get; set; }

    public DateTimeOffset Instant { get; set; }

    [JsonIgnore]
    public TSDValue ZeroIdTsdValue => new TSDValue() { ID = 0, Instant = Instant, Value = Value };

    public static TSDValue FromCSV(string dataRow, TimeZoneInfo tzInfo, bool isTimeZoneAware)
    {
        var values = dataRow.Split(',');
        var dt = DateTime.Parse(values[0]);
        var dtz = new DateTimeOffset();
        if (isTimeZoneAware)
            dtz = DateTimeOffset.Parse(values[0]);

        return new TSDValue()
        {
            Instant = isTimeZoneAware ? dtz : new DateTimeOffset(dt, tzInfo.GetUtcOffset(dt)),
            ID = values[1],// Tag name
            Value = Convert.ToDouble(values[2]) // TagValue
        };
    }
}

#endregion