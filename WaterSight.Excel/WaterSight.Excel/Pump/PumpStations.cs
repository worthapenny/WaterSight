using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Pump;

[DebuggerDisplay("Count: {PumpStationItemsList.Count}")]
public class PumpStations:ExcelSheetBase
{
    #region Constructor
    public PumpStations(string excelFilePath)
        : base(ExcelSheetName.PumpStations, excelFilePath)
    {
        PumpStationItemsList = new List<PumpStationItem>();
    }
    #endregion

    #region Public Properties
    public List<PumpStationItem> PumpStationItemsList { get; set; }
    #endregion
}


[DebuggerDisplay("{ToString()}")]
public class PumpStationItem
{
    #region Constructor
    public PumpStationItem()
    {
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"PumpStation: {DisplayName}";
    }
    #endregion

    #region Public Properties
    [Column(1, "Display Name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column(2, "Suction Tag")]
    public string SuctionTag { get; set; } = string.Empty;

    [Column(3, "Discharge Tag")]
    public string DischargeTag { get; set; } = string.Empty;

    [Column(4, "Flow Tag")]
    public string FlowTag { get; set; } = string.Empty;

    [Column(5, "Groups")]
    public string Groups { get; set; } = string.Empty;
    #endregion
}