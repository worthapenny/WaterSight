using System.Data;
using System.Diagnostics;

namespace WaterSight.WaterSightDataCollector.Support;

[DebuggerDisplay("[{Tag}] {Value} @ {At}")]
public class TSD
{
    #region Constants
    const string col_name_at = "Timestamp";
    const string col_name_tag = "Tag";
    const string col_name_value = "Value";
    #endregion

    #region Constructor
    public TSD()
        :this(string.Empty, DateTimeOffset.MinValue, double.NaN)
    {            
    }
    public TSD(string tag, DateTimeOffset at, double? value)        
    {            
        Tag = tag;
        At = at;
        Value = value;
    }

    #endregion

    #region Public Static Methods
    public static DataTable NewDataTable()
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add(col_name_at, typeof(DateTimeOffset));
        dataTable.Columns.Add(col_name_tag, typeof(string));
        dataTable.Columns.Add(col_name_value, typeof(double));

        return dataTable;
    }
    #endregion

    #region Public Methods
    public void AddDataRow(
        DataTable table        )
    {
        var row = table.NewRow();
        row[col_name_at] = At;
        row[col_name_tag] = Tag;
        row[col_name_value] = Value;
        table.Rows.Add(row);
    }
    #endregion

    #region Properties
    public string Tag { get; set; }
    public DateTimeOffset At { get; set; }
    public double? Value { get; set; }
    #endregion
}
