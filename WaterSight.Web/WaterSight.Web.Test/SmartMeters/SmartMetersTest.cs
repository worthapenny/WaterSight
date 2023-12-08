using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WaterSight.Web.Alerts;
using WaterSight.Web.Core;
using WaterSight.Web.Sensors;
using WaterSight.Web.SmartMeters;

namespace WaterSight.Web.Test.SmartMeters;

[TestFixture, Order(101500), Category("SmartMeter")]
public class SmartMetersTest: TestBase
{
    #region Constructor
    public SmartMetersTest()
    //: base(4549, Env.Qa)
    {
        Separator($"----+----+---- Performing SmartMeter Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public SmartMeter SmartMeter => WS.SmartMeter;
    #endregion

    #region Tests
    [Test, Category("CRUD"), Category("SmartMeter")]
    public async Task SmartMeter_CRUD()
    {
        // Create Object
        var sm = await NewSmartMeterConfigAsync();
        Assert.IsNotNull(sm);

        // Create
        var smCreated = await SmartMeter.AddSmartMeterConfigAsync(sm);
        Assert.That(sm, Is.Not.Null);
        Assert.That(sm.ConsumptionPointId > 0);
        Separator("Done create testing");

        // Read
        var smFound = await SmartMeter.GetSmartMeterConfigAsync(sm.ConsumptionPointId);
        Assert.That(smFound, Is.Not.Null);
        Assert.That(smFound.ConsumptionPointId, Is.EqualTo(sm.ConsumptionPointId));
        Separator("Done reading 'single item' testing");

        // Update
        var newName = "Smart Meter Name Updated";
        sm.Name = newName;
        var success = await SmartMeter.UpdateSmartMeterConfigAsync(sm);
        Assert.That(success, Is.True);
        Separator("Done update testing");

        // Delete
        if(smCreated != null)
        {
            var deleted = await SmartMeter.DeleteSmartMeterConfigAsync(smCreated.ConsumptionPointId);
            Assert.That(deleted, Is.True);
            Separator("Done delete testing");
        }

    }
    #endregion

    #region Helper Methods
    public async Task<SmartMeterConfig> NewSmartMeterConfigAsync()
    {
        var sm = new SmartMeterConfig();
        sm.Address = $"420 Lost Ln, Ghosttown"; 
        sm.CommunicationFrequency = 15;
        sm.ConsumptionPointId = 0;
        sm.ConsumptionType = (int)ConsumptionTypeEnum.NotDefined;
        sm.ConsumptionTypeAsString = "Not Defined";
        sm.DiameterUnits = "";
        sm.ExternalId = $"SmartMeter_Test_{DateTime.Now:u}";
        sm.InstallationDate = new DateTime(1999, 9, 9);
        sm.IsCritical = true;
        sm.IsLarge = true;
        sm.Latitude = -104.98332839223629;
        sm.Longitude = 39.98767804586302;
        sm.MeterType = (int)MeterTypeEnum.NotDefined;
        sm.Name = $"SmartMeter_TestName_{DateTime.Now:u}";
        sm.ParameterType = $"{ParameterTypeEnum.Volume.ToString()}";
        sm.RegistrationFrequency = 15;
        sm.SignalId = 0;
        sm.Tags = "";
        sm.UtcOffSet = "00:00";
        sm.ZoneName = "";

        return sm;
    }
    #endregion
}
