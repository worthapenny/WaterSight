using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;

namespace WaterSight.Web.Test;

[TestFixture, Order(101010), Category("Sensors")]
public class SensorsPatTest : TestBase
{
    #region Constructor
    public SensorsPatTest()
        : base(
        dtID: TEST_DT_Akshaya_2731,
        env: Env.Qa,
        pat: "XlRKb19UGDywaqfgq0rO94u7Qjph2OooeSKBaaLdcy5bbBaJeldY" // Generate a new one for testing
        )
    {
    }
    #endregion

    #region Tests
    [Test]
    public async Task GetSensorConfigTest()
    {
        var sensorConfigs = await WS.Sensor.GetSensorsConfigAsync();
        Assert.That(sensorConfigs, Is.Not.Empty);
    }

    [Test]
    public async Task PostTsdTest()
    {
        var sensorConfigs = await WS.Sensor.GetSensorsConfigAsync();
        Assert.That(sensorConfigs, Is.Not.Empty);

        var tsdData = new List<TSDValue>()
        {
            new TSDValue(0, 99.99, DateTimeOffset.UtcNow),
            new TSDValue(0, 99.99, DateTimeOffset.UtcNow.AddDays(-1))
        };

        var sensorConfig = sensorConfigs.FirstOrDefault();
        var success = await WS.Sensor.PostSensorTSDAsync(
            sensorId: sensorConfig.ID,
            data: tsdData,
            tagNameForLogging: sensorConfig.Name);

        Logger.Debug($"Test Sensor: {sensorConfig}");

        Assert.That(success, Is.True);
    }

   
    [Test]
    public async Task PostJsonTest()
    {
        var sensorConfigs = await WS.Sensor.GetSensorsConfigAsync();
        Assert.That(sensorConfigs, Is.Not.Empty);

        var jsonFilePath = @"C:\Users\Akshaya.Niraula\Downloads\JsonData\Data.json";
        Assert.That(File.Exists(jsonFilePath), Is.True);

        var success = await WS.Sensor.PostJsonFileAsync(
            jsonFilePath: jsonFilePath,
            tagIds: null,
            onlyNoDataSensors: false,
            useLastInstanceFromServer: true);
        
        Assert.That(success, Is.True);
    }


    #endregion
}
