using NodaTime;
using NodaTime.TimeZones;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WaterSight.MonitoredFileDataPusher.IO;

namespace WaterSight.MonitoredFileDataPusher.Support;

public class Options
{
    #region Public Methods
    public string GetFilePath()
    {
        if (string.IsNullOrEmpty(DirectoryPathToMonitor)
            || string.IsNullOrEmpty(FileName))
        {
            var message = $"Invalid file path. Dir: '{DirectoryPathToMonitor}', File name: '{FileName}'";
            var ex = new FileNotFoundException(message);
            Log.Error(ex, message);
            throw ex;
        }

        return Path.Combine(DirectoryPathToMonitor, FileName);
    }
    #endregion

    #region Public Properties
    public string? DirectoryPathToMonitor { get; set; }
    public string? FileName { get; set; }
    public int? DigitalTwinId { get; set; }
    public string? PAT { get; set; }

    public string? TimeZoneString { get; set; }
    public string TableName { get; set; } = "*";

    [JsonIgnore]
    public DateTimeZone DateTimeZone => TzdbDateTimeZoneSource.Default.ForId(TimeZoneString ?? "");
    #endregion

    #region Public Static Methods
    public static Options? LoadFromFile(string filePath)
    {
        var options = JsonIO.LoadFromFile<Options>(filePath);
        return options;
    }
    #endregion
}
