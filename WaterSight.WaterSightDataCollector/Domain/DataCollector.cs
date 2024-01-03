using Serilog;
using System.Diagnostics;
using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Support;
using WaterSight.WaterSightDataCollector.Support.Logging;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;

namespace WaterSight.WaterSightDataCollector.Domain;

public class DataCollector : WaterSightFacadeBase
{
    #region Constructor
    public DataCollector(int dtID, string dtName, Env env)
        : base(dtID, dtName, env)
    {
    }
    #endregion

    #region Public Methods

    public async Task CollectDataAndWriteToDatabaseAsync()
    {
        var sensorsConfig = await WS.Sensor.GetSensorsConfigAsync();

        if (sensorsConfig == null)
        {
            Log.Error($"Could not get sensors from WaterSight. No data will be collected!");
            return;
        }
        else
        {
            await CollectDataAndWriteToDatabaseAsync(
                sensorsConfig: sensorsConfig,
                type: IntegrationType.Raw);
        }
    }


    public async Task CollectDataAndWriteToDatabaseAsync(
        List<SensorConfig?> sensorsConfig,
        IntegrationType type = IntegrationType.Raw,
        int degreeOfParallelism = 5)
    {
        var startTime = Stopwatch.StartNew();
        LogLibrary.Separate_StartGroup();
        Log.Debug($"About to collect data and write that to a database.");


        // Create sqlite db if it doesn't exist
        //DatabaseManager.GetSqliteDb();
        //if (!File.Exists(DatabaseManager.DatabasePath))
        //    throw new ApplicationException($"SQLite database couldn't be created or located.");

        // work with each sensor     
        var options = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };

        var counter = 0;
        var maxSize = sensorsConfig.Count;
        await Parallel.ForEachAsync(sensorsConfig, options, async (sensorConfig, token) =>
        {
            try
            {
                counter++;

                if (sensorConfig != null)
                {
                    Log.Debug($"[{counter}/{maxSize}] About to work with '{sensorConfig}'");

                    var from = await DatabaseManager.GetLatestDateTimeAsync(sensorConfig.TagId)
                        ?? DateTimeOffset.Now.AddDays(-365);
                    from = from.AddMinutes(1); // to avoid any duplication


                    var gap = DateTimeOffset.Now - from;
                    if (gap > TimeSpan.FromMinutes(30))
                    {

                        var to = DateTimeOffset.Now;

                        // Collect the data from web
                        var data = await CollectSensorDataAsync(
                                     sensorConfig: sensorConfig,
                                     from: from,
                                     to: to,
                                     type);


                        // Write the data to database
                        if (data != null && data.Any())
                            await DatabaseManager.WriteToDatabaseAsync(data);
                        else
                            Log.Warning($"⚠️ No data found for {sensorConfig} to write");

                    }
                    else
                    {
                        Log.Information($"The gap between last time instance in the database < 30 minutes, skipped data collection)");
                    }
                }
                else
                {
                    Log.Warning($"⚠️ No sensor information found to work.");
                    Debugger.Break();
                }

                Log.Debug($"☑️☑️ [{counter}/{maxSize}] Done with {sensorConfig}");
                LogLibrary.Separate_XSmall();
            }
            catch (Exception ex)
            {
                var message = $"... while working with the sensor '{sensorConfig}'. Error: {ex.Message}";
                Log.Error(ex, message);
                Debugger.Break();
            }
        });

        LogLibrary.Separate_Equal();
        Log.Information($"[🕒 {startTime.Elapsed}] Total time taken to collect and store {sensorsConfig.Count} sensor's data");
        LogLibrary.Separate_EndGroup();
    }



    public async Task<List<TSD>> CollectSensorDataAsync(
        SensorConfig sensorConfig,
        DateTimeOffset from,
        DateTimeOffset to,
        IntegrationType type)
    {
        var starTime = Stopwatch.StartNew();
        Log.Debug($"About to collect data for '{sensorConfig.Name}' from {from} to {to}");

        var sensorTsdWeb = await WS.Sensor.GetSensorTSDAsync(
            id: sensorConfig.ID,
            startAt: from,
            endAt: to,
            integrationType: type);

        var tag = sensorConfig.TagId;
        var data = new List<TSD>();
        if (sensorTsdWeb != null)
        {
            foreach (var item in sensorTsdWeb.UnifiedTSDs)
                data.Add(new TSD(tag, item.Instant, item.UnifiedValue));

        }

        Log.Debug($"[🕒 {starTime.Elapsed}] Collected data for '{sensorConfig.Name}' from {from} to {to}");
        LogLibrary.Separate_Dot();

        return data;
    }


    #endregion


}
