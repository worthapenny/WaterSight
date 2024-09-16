//using CsvHelper.Configuration.Attributes;
//using System;
//using System.Diagnostics;

//namespace WaterSight.Model.Support.Data;

//public interface ITimeSeriesData
//{
//    string ToCSV(bool wrapTimeStampInQuotes = true);

//    DateTime Timestamp { get; set; }
//    double? Value { get; set; }
//    string? Tag { get; set; }
//    string CsvHeader { get; }
//}


//[DebuggerDisplay("{ToString()}")]
//public class TimeSeriesData : ITimeSeriesData
//{
//    #region Constructor
//    public TimeSeriesData()
//    {
//    }

//    public TimeSeriesData(DateTime dateTime, double? value, string tag)
//    {
//        Timestamp = dateTime;
//        Value = value;
//        Tag = tag;
//    }

//    #endregion

//    #region Public Methods
//    public string ToCSV(bool wrapTimeStampInQuotes = true)
//    {
//        var dt = $"'{Timestamp:u}'".Replace("Z", "");
//        if (!wrapTimeStampInQuotes) dt.Replace("'", "");
//        object value = Value.HasValue 
//            ? double.IsNaN(Value.Value) ? "\"NaN\"" : Value.Value 
//            : "null";

//        return $"{dt},\"{Tag}\",{value}";
//    }

//    public void AdjustForDaylightSavingTime(Tuple<DateTime, DateTime> startStopBoundary, TimeSpan adjustment)
//    {
//        if (Timestamp >= startStopBoundary.Item1
//            && Timestamp < startStopBoundary.Item2
//            )
//        {
//            Timestamp += adjustment;
//        }
//    }
//    #endregion

//    #region Overridden Methods
//    public override string ToString()
//    {
//        return $"[{Tag}] {Value} @ {Timestamp}";
//    }
//    #endregion

//    #region Public Properties
//    [Index(0)]
//    public DateTime Timestamp { get; set; }

//    [Index(1)]
//    public string? Tag { get; set; }

//    [Index(2)]
//    public double? Value { get; set; }

//    [CsvHelper.Configuration.Attributes.Ignore]
//    public string CsvHeader => $"{nameof(Timestamp)},{nameof(Tag)},{nameof(Value)}";
//    #endregion

//}