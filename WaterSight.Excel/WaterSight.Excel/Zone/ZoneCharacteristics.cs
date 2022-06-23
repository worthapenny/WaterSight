using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Zone;

[DebuggerDisplay("Count: {CustomerMeterItemsList.Count}")]
public class ZoneCharacteristicsXlSheet : ExcelSheetBase
{
    #region Constructor
    public ZoneCharacteristicsXlSheet(string excelFilePath)
        : base(ExcelSheetName.ZoneCharacteristics, excelFilePath)
    {
        ZoneCharacteristicsItemsList = new List<ZoneCharacteristicsItem>();
    }
    #endregion

    #region Public Properties
    public List<ZoneCharacteristicsItem> ZoneCharacteristicsItemsList { get; set; }
    #endregion
}


[DebuggerDisplay("ToString()")]
public class ZoneCharacteristicsItem
{
    #region Constructor
    public ZoneCharacteristicsItem()
    {
    }
    #endregion

    #region Public Properties
    [Column(1, "Display Name*")]
    public string DisplayName { get; set; }

    [Column(2, "Population Served")]
    public int? PopulationServed { get; set; }

    [Column(3, "Number of Connections")]
    public int? NumberOfConnections { get; set; }

    [Column(4, "Number of Customers")]
    public int? NumberOfCustomers { get; set; }

    [Column(5, "% of MNF Consumed")]
    public double? PercentOfMnfConsumed { get; set; }

    [Column(6, "% Of Authorized Unbilled Consumption")]
    public double? PercentOfAuthUnbilledConsumption { get; set; }

    [Column(7, "Priority")]
    public int Priority { get; set; } = 1;

    [Column(8, "Parent Zone")]
    public string? ParentZone { get; set; } = string.Empty;

    [Ignore]
    public int? ParentZoneId { get; set; } = null;
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName}";
    }
    #endregion
}