using Ganss.Excel;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace WaterSight.Excel.Alert;

[DebuggerDisplay("Count: {AlertItemsList.Count}")]
public class AlertXlSheet : ExcelSheetBase
{

    #region Constructor
    public AlertXlSheet(string excelFilePath)
        : base(ExcelSheetName.Alerts, excelFilePath)
    {
        AlertItemsList = new List<AlertItem>();
    }
    #endregion

    #region Public Methods
    public void LoadFromExcel()
    {
        var excelMapper = new ExcelMapper(base.FilePath);
        AlertItemsList = excelMapper.Fetch<AlertItem>(base.SheetName).ToList();
    }
    #endregion

    #region Public Properties
    public List<AlertItem> AlertItemsList { get; set; }
    #endregion
}


[DebuggerDisplay("{ToString()}")]
public class AlertItem
{
    #region Constants
    public const char DisplayNameSeparator = '|';
    const string RelationTypeHigh = "High";
    const string RelationTypeLow = "Low";
    const string TimeSeriesType15Min = "15 Min";
    const string TimeSeriesTypeDailyMinimum = "Daily Minimum";
    const string TimeSeriesTypeDailyAverage = "Daily Average";
    const string PatternHistoryLastMonth = "Last month";
    const string PatternHistoryLast2Months = "Last 2 months";
    const string PatternHistoryLast3Months = "Last 3 months";
    const string PatternHistoryLast6Months = "Last 6 months";
    const string PatternHistoryLast12Months = "Last 12 months";
    const string TruthKeywordYes = "Yes";
    const string TruthKeyword1 = "1";
    const string TruthKeywordTrue = "True";
    const string AlertTypePatternStr = "Pattern";
    const string AlertTypeAbsoluteStr = "Absolute";
    const string AlertTypeNoDataStr = "No Data";
    const string AlertTypeFlatReadingStr = "Flat Reading";

    const int AlertTypePattern = 1; // WaterSight.Web.Alerts.Alert.AlertType
    const int AlertTypeAbsolute = 2;
    const int AlertTypeNoData = 3;
    const int AlertTypeFlatReading = 4;

    #endregion

    #region Constructor
    public AlertItem()
    {
    }
    #endregion

    #region Public Methods
    public string DurationString(TimeSpan timeSpan)
    {
        return XmlConvert.ToString(timeSpan);
    }
    public List<string> GetSensorOrZoneNames()
    {
        if (string.IsNullOrWhiteSpace(SensorsOrZonesDisplayName))
            throw new InvalidOperationException();

        var displayNames = SensorsOrZonesDisplayName.Split(DisplayNameSeparator);
        var names = displayNames.Select(n => n.Trim()).ToList();

        return names;
    }
    #endregion

    #region Public Properties
    [Column(1, "Sensors or Zones Display Name")]
    public string SensorsOrZonesDisplayName { get; set; }


    [Column(2, "Alert Type")]
    public string AlertTypeStr { get; set; }
    [Ignore]
    public int AlertType // Type (in payload json)
    {
        get
        {
            if (AlertTypeStr == AlertTypePatternStr)
                return AlertTypePattern;
            if (AlertTypeStr == AlertTypeAbsoluteStr)
                return AlertTypeAbsolute;
            if (AlertTypeStr == AlertTypeFlatReadingStr)
                return AlertTypeFlatReading;
            if (AlertTypeStr == AlertTypeNoDataStr)
                return AlertTypeNoData;

            throw new InvalidOperationException();
        }
    }


    [Column(3, "High or Low")]
    public string? HighOrLowStr { get; set; }
    [Ignore]
    public string RelationType
    {
        get
        {
            if (HighOrLowStr == null)
                return null;

            if (HighOrLowStr == RelationTypeLow)
                return RelationTypeLow;

            if (HighOrLowStr == RelationTypeHigh)
                return RelationTypeHigh;

            return null;
        }

    }

    [Column(4, "Time Series Type")]
    public string TimeSeriesTypeStr { get; set; }

    [Ignore]
    public int? IntegrationPercentile
    {
        get
        {
            if (TimeSeriesTypeStr == TimeSeriesTypeDailyMinimum)
                return 5; // Daily minimum

            if (TimeSeriesTypeStr == TimeSeriesTypeDailyAverage)
                return 50; // Daily average

            return null; // 15 min series
        }
    }

    [Ignore]
    public string ResamplingInterval
    {
        get
        {
            // default to 15 minute
            var sampingSeries = new TimeSpan(0, 15, 0);

            // daily minimum or average
            if (TimeSeriesTypeStr == TimeSeriesTypeDailyAverage
                || TimeSeriesTypeStr == TimeSeriesTypeDailyMinimum)
                sampingSeries = new TimeSpan(1, 0, 0, 0);

            // 15 min
            if (TimeSeriesTypeStr == TimeSeriesType15Min)
                sampingSeries = new TimeSpan(0, 15, 0);

            return DurationString(sampingSeries);
        }
    }

    [Ignore]
    public int? PatternPercentileToFireEvent
    {
        get
        {
            // Only meaningfull when Alert is Absolute based
            var percentileHighValue = 95;
            var percentileLowValue = 5;

            if (RelationType == RelationTypeHigh)
                return percentileHighValue;

            if (RelationType == RelationTypeLow)
                return percentileLowValue;

            return null;
        }
    }

    [Ignore]
    public int? PatternPercentileToStopEvent
    {
        get
        {
            // Only meaningfull when Alert is Absolute based
            var percentileHighValue = 80;
            var percentileLowValue = 20;

            if (RelationType == RelationTypeHigh)
                return percentileHighValue;

            if (RelationType == RelationTypeLow)
                return percentileLowValue;

            return null;
        }
    }


    [Column(5, "Pattern History")]
    public string? PatternHistoryStr { get; set; } = null;

    [Ignore]
    public string PatternConfidenceHistoricalRange
    {
        get
        {
            if (PatternHistoryStr == PatternHistoryLastMonth)
                return DurationString(new TimeSpan(30, 0, 0, 0));

            if (PatternHistoryStr == PatternHistoryLast2Months)
                return DurationString(new TimeSpan(60, 0, 0, 0));

            if (PatternHistoryStr == PatternHistoryLast3Months)
                return DurationString(new TimeSpan(90, 0, 0, 0));

            if (PatternHistoryStr == PatternHistoryLast6Months)
                return DurationString(new TimeSpan(180, 0, 0, 0));

            if (PatternHistoryStr == PatternHistoryLast12Months)
                return DurationString(new TimeSpan(365, 0, 0, 0));

            // default to Last Month
            return DurationString(new TimeSpan(30, 0, 0, 0));
        }
    }

    [Column(6, "Threshold Value")]
    public double ThresholdValue { get; set; }


    [Column(7, "Minimum Duration")]
    public string MinimumDurationStr { get; set; }
    [Ignore]
    public string MinimumDuration // MinDuration
    {
        get
        {
            var duration = new TimeSpan(0, 30, 0); // default value
            try
            {
                var minDurationStrings = MinimumDurationStr.ToLower().Split(' ');
                if (minDurationStrings.Length > 0)
                {
                    if (minDurationStrings[1].Contains("hour"))
                    {
                        var minutes = Convert.ToInt16(minDurationStrings.First()) * 60;
                        duration = new TimeSpan(0, minutes, 0);
                    }

                    if (minDurationStrings[1].Contains("day"))
                    {
                        var days = Convert.ToInt16(minDurationStrings.First());
                        duration = new TimeSpan(days, 0, 0, 0);
                    }
                }
            }
            catch { }   // in case of error use default value

            return DurationString(duration);
        }
    }

    [Column(8, "Is Active")]
    public string IsActiveStr { get; set; }
    [Ignore]
    public bool IsActive
    {
        get
        {
            return IsActiveStr == TruthKeywordYes
                || IsActiveStr == TruthKeyword1
                || IsActiveStr == TruthKeywordTrue;
        }
    }

    [Column(9, "Display On Tank")]
    public string DisplayOnTankStr { get; set; }
    [Ignore]
    public bool DisplayOnTank
    {
        get
        {
            return DisplayOnTankStr == TruthKeywordYes
                || DisplayOnTankStr == TruthKeyword1
                || DisplayOnTankStr == TruthKeywordTrue;
        }
    }

    [Column(10, "Recipients Group Display Name")]
    public string? RecipientsGroup { get; set; }

    [Column(11, "Alert Display Name")]
    public string? AlertDisplayName { get; set; }


    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{AlertTypeStr}] {SensorsOrZonesDisplayName} on {TimeSeriesTypeStr} for {MinimumDurationStr}";
    }
    #endregion
}
