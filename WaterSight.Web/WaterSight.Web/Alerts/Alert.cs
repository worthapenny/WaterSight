using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Web.Alerts;

public class Alert : WSItem
{
    #region Constructor
    public Alert(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD Operations
    //
    // CREATE
    public async Task<AlertConfig?> AddAlertConfigAsync(AlertConfig alert)
    {
        var url = EndPoints.AlertingConfigsQDT;

        int? id = await WS.AddAsync(alert, url, "Alert");
        if (id.HasValue)
        {
            alert.Id = id.Value;
            return alert;
        }

        return null;
    }

    /*
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        return await WS.PostFile(EndPoints.HydStructuresPumpsQDT, fileInfo, "Excel");
    }
    */

    // READ / Get
    public async Task<AlertConfig?> GetAlertConfigAsync(int id)
    {
        var url = EndPoints.AlertingConfigsForId(id);
        AlertConfig? config = await WS.GetAsync<AlertConfig>(url, id, "Alert");
        return config;
    }
    public async Task<List<AlertConfig?>> GetAlertsConfigAsync()
    {
        var url = EndPoints.AlertingConfigsQDT;
        return await WS.GetManyAsync<AlertConfig>(url, "Alerts");
    }

    //
    // UPDATE
    public async Task<bool> UpdateAlertConfigAsync(AlertConfig alert)
    {
        var url = EndPoints.AlertingConfigsForId(alert.Id);
        return await WS.UpdateAsync(alert.Id, alert, url, "Alert");
    }

    //
    // DELETE
    public async Task<bool> DeleteAlertConfigAsync(int alertId)
    {
        var url = EndPoints.AlertingConfigsForId(alertId);
        return await WS.DeleteAsync(alertId, url, "Alert", true);
    }
    public async Task<bool> DeleteAlertsConfigAsync()
    {
        Logger.Verbose("About to delete all the alerts...");
        var url = EndPoints.AlertingConfigsQDT;
        return await WS.DeleteManyAsync(url, "Alerts", true);
    }
    #endregion

    #endregion
}


#region Model Classes
public struct TimeSeries
{
    public const string FifteenMin = "PT15M";
    public const string DailyMin = "D1min";
    public const string DailyAvg = "D1avg";
}

public struct Duration
{
    public const string NA = "";

    public const string MinuteFive = "PT5M";
    public const string MinuteTen = "PT10M";
    public const string MinuteFifteen = "PT15M";
    public const string MinuteThirth = "PT30M";

    public const string HourHalf = "PT30M";
    public const string HourOne = "PT1H";
    public const string HourTwo = "PT2H";
    public const string HourThree = "PT3H";
    public const string HourFour = "PT4H";
    public const string HourFive = "PT5H";
    public const string HourSix = "PT6H";
    public const string HourEight = "PT8H";
    public const string HourTen = "PT10H";
    public const string HourTwelve = "PT12H";
    public const string HourSixteen = "PT16H";
    public const string HourEighteen = "PT18H";
    public const string HourTwenty = "PT20H";
    public const string HourTwentyFour = "PT24H";

    public const string DayOne = "P1D";
    public const string DayTwo = "P2D";
    public const string DayThree = "P3D";
    public const string DayFour = "P4D";
    public const string DayFive = "P5D";
    public const string DaySix = "P6D";
    public const string DaySeven = "P7D";
    public const string DayEight = "P8D";

    public const string MonthOne = "P30D";
    public const string MonthTwo = "P60D";
    public const string MonthThree = "P90D";
    public const string MonthSix = "P180D";

    public const string YearOne = "P365D";

}
public enum AlertOriginOriginType
{
    Sensor,
    Zone,
}

[DebuggerDisplay("{ToString()}")]
public class AlertOrigin
{
    #region Constructor
    public AlertOrigin()
    {
    }
    public AlertOrigin(int originId, AlertOriginOriginType type)
    {
        OriginId = originId;
        OriginType = type;
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Origin: {OriginId} [{OriginType.ToString()}]";
    }
    #endregion

    #region Public Properties

    public int OriginId { get; set; }
    public AlertOriginOriginType OriginType { get; set; }
    #endregion
}
public enum AlertType
{
    Pattern =1,
    Absolute=2,
    NoData=3,
    FlatReading=4,
}
public struct AlertExtremes
{
    public const string High = "high";
    public const string Low = "low";
}

[DebuggerDisplay("{ToString()}")]
public class AlertConfig
{
    public int Id { get; set; }
    public object? Name { get; set; }
    public AlertType Type { get; set; }
    public bool FromManualAlert { get; set; }
    public bool DisplayOnTank { get; set; }
    public double ThresholdValue { get; set; }
    public string RelationType { get; set; } // high/low
    public string MinDuration { get; set; } // PT30M
    public bool Active { get; set; }
    public string ResamplingInterval { get; set; } // PT5M
    public int? IntegrationPercentile { get; set; }
    public string PatternConfidenceHistoricalRange { get; set; } // P60D
    public int? PatternPercentileToFireEvent { get; set; } // 95
    public int? PatternPercentileToStopEvent { get; set; } // 80
    public string ThresholdUnits { get; set; }
    public List<AlertOrigin> Origins { get; set; } = new List<AlertOrigin>();
    public int NumericalModelTestType { get; set; }

    public override string ToString()
    {
        return $"{Id}: {Name} [{Type}]";
    }
}
#endregion
