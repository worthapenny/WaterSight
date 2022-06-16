using System;
using System.IO;

namespace WaterSight.Model.Domain;

public class DataSourceConnectionOptions
{
    #region Constructor
    public DataSourceConnectionOptions()
    {
        LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\Test_SCADALog.log");
    }
    #endregion

    #region Public Properties
    public string DataSourceFilePath { get; set; }
    public string DataSourceType { get; set; }

    public string DataSourceTableName { get; set; }
    public string DataSourceFormat { get; set; }
    //public string DataSourceTableSignalFieldName { get; set; }
    public string DataSourceTableTagFieldName { get; set; }
    public string DataSourceTableValueFieldName { get; set; }
    public string DataSourceTableDateTimeFieldName { get; set; }
    public bool IsHistorical { get; set; } = true;
    public double TimeToleranceBackwardMinutes { get; set; } = 15.0;
    public double TimeToleranceForwardMinutes { get; set; } = 15.0;

    public string LogFilePath { get; set; }
    #endregion
}
