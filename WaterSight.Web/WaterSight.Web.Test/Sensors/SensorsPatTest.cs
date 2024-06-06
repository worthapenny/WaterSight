using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;

namespace WaterSight.Web.Test.Sensors;

public class SensorsPatTest : TestBase
{
    #region Constructor
    public SensorsPatTest()
        : base(
        dtID: 139, // TEST_DT_Akshaya_4736,
        env: Env.Prod, // TEST_ENV,
        pat: "ozPhDjQwKSeajHtPwF7BMCoZaujuHK4kEmVsfgrfjV7wz0sauTZc" // Generate a new one for testing
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
        
        var success = await WS.Sensor.PostJsonFileAsync(
            jsonFilePath: jsonFilePath,
            tagIds: null,
            onlyNoDataSensors: false,
            useLastInstanceFromServer: true);
        
        Assert.That(success, Is.True);
    }


    #endregion
}
