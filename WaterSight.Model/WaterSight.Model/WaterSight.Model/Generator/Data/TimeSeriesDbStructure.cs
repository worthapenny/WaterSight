using System.IO;

namespace WaterSight.Model.Generator.Data;

public class TimeSeriesDbStructure
{
    #region Constants
    public const string TimeseriesDataTableName = "TimeseriesData";
    public const string TagFieldName = "Tag";
    public const string ValueFieldName = "Value";
    public const string TimestampFieldName = "Timestamp";
    #endregion

    #region Constructor    
    public TimeSeriesDbStructure(
        string filePath, 
        string dbTableName = TimeseriesDataTableName,
        string tagColName = TagFieldName,
        string valueColName = ValueFieldName,
        string timestampColName = TimestampFieldName
        )
    {
        FilePath = filePath;
        FileInfo = new FileInfo(filePath);

        TsdTableName = dbTableName;
        TagColName = tagColName;
        ValueColName = valueColName;
        TimestampColName = timestampColName;
    }

    #endregion

    #region Public Properties
    public string FilePath { get; }
    public FileInfo FileInfo { get; }

    public string TsdTableName { get; set; } = TimeseriesDataTableName;
    public string TagColName { get; set; } = TagFieldName;
    public string ValueColName { get; set; } = ValueFieldName;
    public string TimestampColName { get; set; } = TimestampFieldName;
    #endregion

}


