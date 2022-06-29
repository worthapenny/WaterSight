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

    #region Public Methods
    public void ReadExcel()
    {
        ZoneCharacteristicsItemsList = Read<ZoneCharacteristicsItem>();
        //try
        //{
        //}
        //catch (System.Exception ex)
        //{

        //}
    }
    #endregion

    #region Public Properties
    public List<ZoneCharacteristicsItem> ZoneCharacteristicsItemsList { get; set; }
    #endregion
}


[DebuggerDisplay("{ToString()}")]
public class ZoneCharacteristicsItem
{
    #region Constructor
    public ZoneCharacteristicsItem()
    {
    }
    #endregion

    #region Public Properties
    [Column("Display Name*")]
    public string DisplayName { get; set; }

    [Column("Parent Zone")]
    public string? ParentZone { get; set; } = string.Empty;

    [Column("Population Served")]
    public int? PopulationServed { get; set; }

    [Column("Number of Connections")]
    public int? NumberOfConnections { get; set; }

    [Column("Number of Customers")]
    public int? NumberOfCustomers { get; set; }

    [Column("Percentage of MNF Consumed")]
    public double? PercentOfMnfConsumed { get; set; }

    [Column("Percentage Of Authorized Unbilled Consumption")]
    public double? PercentOfAuthUnbilledConsumption { get; set; }

    [Column("Priority")]
    public int Priority { get; set; } = 1;


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