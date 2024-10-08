﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;

namespace WaterSight.Web.Test;


[TestFixture, Order(101000), Category("Sensors")]
public class SensorsTest : TestBase
{
    #region Constructor
    public SensorsTest()
    //: base(4549, Env.Qa)
        //:base(139, Env.Prod)
    {
        Separator($"----+----+---- Performing Sensors Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Sensor Sensor => WS.Sensor;
    #endregion

    #region Tests

    [Test]
    public async Task Sensors_CRUD()
    {
        // Create Object
        var sensor = await NewSensorConfigAsync();
        Assert.IsNotNull(sensor);

        // Create
        var sensorCreated = await Sensor.AddSensorConfigAsync(sensor);
        Assert.IsNotNull(sensorCreated);
        Assert.IsTrue(sensorCreated?.ID > 0);
        Separator("Done create testing");


        // Read
        var sensorFound = await Sensor.GetSensorConfigAsync(sensor.ID);
        Assert.IsNotNull(sensorFound);
        Assert.AreEqual(sensorFound?.ID, sensor.ID);
        Assert.AreEqual(sensorFound?.Name, sensor.Name);
        Logger.Debug("Done read, single item, testing");


        // Update
        var newName = "Sensor Test Updated";
        sensor.Name = newName;
        var success = await Sensor.UpdateSensorConfigAsync(sensor);
        Assert.That(success, Is.True);
        Separator("Done update testing");


        // Delete
        if (sensorCreated != null)
        {
            var deleted = await Sensor.DeleteSensorConfigAsync(sensorCreated.ID);
            Assert.IsTrue(deleted);
            Separator("Done delete, single item, testing");
        }

    }

    [Test]
    public async Task Sensors_Delete_all()
    {
        var success = await Sensor.DeleteSensorsConfigAsync();
        Assert.That(success, Is.True);
        Separator("Done delete, all items, testing");
    }


    //
    // CONFIG
    //
    [Test]
    public async Task Sensors_Config()
    {
        // Sensors Config (all)
        var sensorsConfig = await Sensor.GetSensorsConfigAsync();
        Assert.IsTrue(sensorsConfig?.Count > 0);

        // Sensor Config (one)
        var sensorConfig = await Sensor.GetSensorConfigAsync(sensorsConfig[0].ID);
        Assert.AreEqual(sensorsConfig[0].ID, sensorConfig.ID);
        Assert.AreEqual(sensorsConfig[0].Name, sensorConfig.Name);

        Logger.Debug("Done read, single/multi items, testing");
    }

    //
    // TSD
    //
    [Test]
    public async Task Sensor_TSD_NoPattern()
    {
        // Delete all existing
        var deleted = await Sensor.DeleteSensorsConfigAsync();
        Assert.IsTrue(deleted);

        // Create
        var sensor = await NewSensorConfigAsync();
        var sensorCreated = await Sensor.AddSensorConfigAsync(sensor);
        Assert.IsNotNull(sensorCreated);
        Assert.IsTrue(sensorCreated?.ID > 0);

        // Push new TSD data
        List<TSDValue> tsdList = GetLocalTSDList();
        var success = await Sensor.PostSensorTSDAsync(
            sensorId: sensor.ID,
            data: tsdList);
        Assert.That(success, Is.True);

        // Pull sensor list
        var sensorsConfig = await Sensor.GetSensorsConfigAsync();
        Assert.IsTrue(sensorsConfig?.Count > 0);

        // Pull Raw SCADA data
        var fromAt = new DateTimeOffset(DateTime.Now, WS.Options.LocalTimeZone.GetUtcOffset(DateTime.Now));
        fromAt = fromAt.AddDays(-5);
        var endAt = fromAt.AddDays(3);
        var rawSensorTSD = await Sensor.GetSensorTSDAsync(
            sensorsConfig[0].ID,
            fromAt,
            endAt,
            IntegrationType.Raw);

        Assert.IsNotNull(rawSensorTSD);
        if (rawSensorTSD != null)
        {
            Assert.AreEqual(sensorsConfig[0]?.Name, rawSensorTSD?.Name);
            Assert.IsTrue(rawSensorTSD?.UnifiedTSDs.Count > 0);
        }
    }

    private List<TSDValue> GetLocalTSDList()
    {
        var tsdValue9999 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(72, 0, 0),
            Value = 9999
        };
        var tsdValue8888 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(72, 15, 0),
            Value = 8888
        };
        var tsdValue7777 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(72, 30, 0),
            Value = 7777
        };
        var tsdValue6666 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(72, 45, 0),
            Value = 6666
        };
        var tsdValue5555 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(73, 0, 0),
            Value = 5555
        };
        var tsdValue4444 = new TSDValue()
        {
            ID = 0,
            Instant = DateTimeOffset.Now - new TimeSpan(73, 15, 0),
            Value = 4444
        };

        return new List<TSDValue>()
    {
        tsdValue4444,
        tsdValue5555,
        tsdValue6666,
        tsdValue7777,
        tsdValue8888,
        tsdValue9999
    };
    }

    public async Task<SensorConfig> NewSensorConfigAsync()
    {
        var sensor = new SensorConfig
        {
            TagId = $"Sensor_Test_{DateTime.Now:u}",
            Name = $"Sensor Test_{DateTime.Now:u}",
            ParameterType = "Level",
            Units = "ft",
            Priority = 1,
            Latitude = -104.98332839223629,
            Longitude = 39.98767804586302,
            ReferenceElevation = 100,
            ReferenceElevationUnits = "ft",
            RegistrationFrequency = 15,
            CommunicationFrequency = 15,
            Tags = "",
            UtcOffSet = "00:00",
            TimeZoneId = "America/New_York"
        };

        var allPatternWeeks = await WS.Settings.PatternWeeks.GetPatternWeeksConfigAsync();
        var patternWeek = allPatternWeeks.Where(p => p.IsDefault).First();
        sensor.PatternWeekId = patternWeek.ID;


        return sensor;
    }
    #endregion

    
}

