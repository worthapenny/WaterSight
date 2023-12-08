using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace WaterSight.Model.Support.Data;

[DebuggerDisplay ("{ToString()}")]
public class CustomerMeterData
{
    #region Constructor
    public CustomerMeterData()
    {
    }

    public CustomerMeterData(
        string id,
        DateTime dateTime,
        double volume,
        string units,
        string zone) :this()
    {
        Id = id;
        DateTime = dateTime;
        Volume = volume;
        Units = units;
        Zone = zone;
    }

    #endregion

    #region Public Methods
    public string ToCsv()
    {
        return $"{Id},{DateTime.ToString(DateTimeFormat)},{Volume},{Units},{Zone}";
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ToCsv()}";
    }
    #endregion

    #region Public Static Properties
    public static string CsvHeader => "Id,Month,Volume,Units,Zone";
    #endregion


    #region Public Properties
    public string Id { get; set; }
    public DateTime DateTime { get; set; }
    public double Volume { get; set; }
    public string Units { get; set; }
    public string Zone { get; set; }

    [JsonIgnore]
    public string DateTimeFormat { get; set; } = "yyyy-MM-15";
    #endregion
}
