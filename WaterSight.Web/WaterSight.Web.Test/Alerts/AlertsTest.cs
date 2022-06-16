using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Alerts;
using WaterSight.Web.Core;
using WaterSight.Web.Test;

namespace WaterSight.Base.Test.Alerts;

public class AlertsTest: TestBase
{
    #region Constructor
    public AlertsTest()
    //: base(4549, Env.Qa)
    {
        Separator($"----+----+---- Performing Alerts Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Alert Alert => WS.Alert;
    #endregion

    #region Test
    [Test]
    public async Task Alerts_CRUD()
    {
        var sensors = await WS.Sensor.GetSensorsConfigAsync();
        Assert.IsNotEmpty(sensors);

        // Create Object
        var alert = NewThresholdAlertConfig(
            origin: new AlertOrigin(sensors.First().ID, AlertOriginOriginType.Sensor),
            unit: "ft"
            );
        Assert.IsNotNull(alert);

        // Create
        var alertCreated = await Alert.AddAlertConfigAsync(alert);
        Assert.IsNotNull(alertCreated);
        Assert.IsTrue(alertCreated?.Id > 0);
        Separator("Done create testing");


        // Read
        var alertFound = await Alert.GetAlertConfigAsync(alert.Id);
        Assert.IsNotNull(alertFound);
        Assert.AreEqual(alertFound?.Id, alert.Id);
        Assert.AreEqual(alertFound?.Name, alert.Name);
        Logger.Debug("Done read, single item, testing");

        var alertsFound = await Alert.GetAlertsConfigAsync();
        Assert.IsNotNull(alertsFound);
        Assert.IsNotEmpty(alertsFound);
        Logger.Debug("Done read, many items, testing");


        // Update
        var newName = "Alert Test Updated";
        alert.Name = newName;
        var success = await Alert.UpdateAlertConfigAsync(alert);
        Assert.IsTrue(success);
        Separator("Done update testing");


        // Delete
        if (alertCreated != null)
        {
            var deleted = await Alert.DeleteAlertConfigAsync(alertCreated.Id);
            Assert.IsTrue(deleted);
            Separator("Done delete, single item, testing");

            var deletedMany = await Alert.DeleteAlertsConfigAsync();
            Assert.IsTrue(deletedMany);
            Separator("Done delete, many items, testing");
        }
    }

    public AlertConfig NewThresholdAlertConfig(AlertOrigin origin, string unit)
    {
        var alertConfig =  new AlertConfig()
        {
            Name = "NewHighAlertTest",
            Type = AlertType.Absolute,
            FromManualAlert = false,
            DisplayOnTank = false,
            ThresholdValue = 999.99,
            ThresholdUnits = unit,
            RelationType = AlertExtremes.High,
            MinDuration = Duration.HourHalf,
            Active = true,
            ResamplingInterval = Duration.MinuteFive,
            IntegrationPercentile = null,
            PatternConfidenceHistoricalRange = Duration.MonthTwo,
            PatternPercentileToFireEvent = 95,
            PatternPercentileToStopEvent = 80,
            NumericalModelTestType = 0 // TODO
        };

        alertConfig.Origins.Add(origin);
        return alertConfig;
    }
    #endregion
}
