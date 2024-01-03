using Serilog;
using System.Diagnostics;
using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Support;
using WaterSight.WaterSightDataCollector.Support.Logging;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;

namespace WaterSight.WaterSightDataCollector.Domain;

public class DataLoader : WaterSightFacadeBase
{

    #region Constructor
    public DataLoader(int dtID, string dtName, Env env)
        : base(dtID, dtName, env)
    {
    }
    #endregion

    #region Public Methods
    public async Task LoadDataAsync(int degreeOfParallelism = 5)
    {
        var startTime = Stopwatch.StartNew();
        LogLibrary.Separate_StartGroup();
        Log.Debug($"About to collect data from database and load that to WaterSight.");


        var sensorsConfig = await WS.Sensor.GetSensorsConfigAsync();
        // work with each sensor     
        var options = new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };

        var counter = 0;
        var maxSize = sensorsConfig.Count;

        await Parallel.ForEachAsync(sensorsConfig, options, async (sensorConfig, token) =>
        {
            counter++;
            Log.Debug($"[{counter}/{maxSize}] About to work on {sensorConfig}");

            if (sensorConfig != null)
            {
                var from = sensorConfig.LastInstantInDatabase ?? DateTimeOffset.MinValue;
                var to = DateTimeOffset.Now;

                var gap = DateTimeOffset.Now - from;
                if (gap > TimeSpan.FromMinutes(30))
                {
                    //Collect the data from SQLite
                    var data = await CollectSensorDataAsync(
                        tag: sensorConfig.TagId,
                                 from: from,
                                 to: to
                        );

                    //Push to WaterSight
                    if (data != null && data.Any())
                        await LoadDataToWaterSightAsync(data, sensorConfig);
                    else
                        Log.Warning($"⚠️ No data found for {sensorConfig} to write");


                    Log.Debug($"☑️☑️ [{counter}/{maxSize}] Done collecting and loading data for {sensorConfig}");
                    LogLibrary.Separate_XSmall();
                }

                
            }


        });

        LogLibrary.Separate_Equal();
        Log.Information($"[🕒 {startTime.Elapsed}] Total time taken to collect and store {sensorsConfig.Count} sensor's data");
        LogLibrary.Separate_EndGroup();
    }



    private async Task<List<TSD>> CollectSensorDataAsync(string tag, DateTimeOffset? from, DateTimeOffset to)
    {
        var data = await DatabaseManager.ReadDatabaseForTSD(
            tag: tag,
            from: from ?? DateTimeOffset.MinValue,
            to: to);

        return data;
    }

    private async Task<bool> LoadDataToWaterSightAsync(List<TSD> data, SensorConfig sensorConfig)
    {
        var startTime = Stopwatch.StartNew();
        LogLibrary.Separate_StartGroup();
        Log.Debug($"About to load data to WaterSight for {sensorConfig}.");

        var success = await WS.Sensor.PostSensorTSDAsync(
            sensorId: sensorConfig.ID,
            data.Select(d => new TSDValue(0, d.Value, d.At)).ToList(),
            tagNameForLogging: sensorConfig.TagId
            );

        Log.Debug($"✅ [{startTime.Elapsed}] Done loading data to WaterSight for {sensorConfig}.");
        return success;
    }
    #endregion


    #region Private Methods

    #endregion
}
