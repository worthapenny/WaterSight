using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Zone;

[DebuggerDisplay("Count: {ZoneItemsList.Count}")]
public class Zones : ExcelSheetBase
{
    #region Constructor
    public Zones(string excelFilePath)
        : base(ExcelSheetName.Zone, excelFilePath)
    {
        ZoneItemsList = new List<ZoneItem>();
    }
    #endregion

    #region Public Properties
    public List<ZoneItem> ZoneItemsList { get; set; }
    #endregion
}

public enum ZoneFlowType
{
    Inflow,
    Outflow,
    Storage,
    Unknown
}


[DebuggerDisplay("ToString()")]
public class ZoneItem
{
    #region Constructor
    public ZoneItem()
    {
    }
    #endregion


    #region Public Properties
    [Column(1, "Display Name*")]
    public string DisplayName { get; set; }

    [Column(2, "Sensor Tag*")]
    public string TagId { get; set; }

    [Column(3, "Type*")]
    public string Type { get; set; } // Inflow, Outflow, or Storage

    [Ignore]
    public ZoneFlowType ZoneFlowType => Enum.TryParse(Type, out ZoneFlowType flowType) ? flowType : ZoneFlowType.Unknown;

    [Ignore]
    public int? SensorId { get; set; } = null;

    #endregion
    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName}, ({Type})";
    }
    #endregion
}