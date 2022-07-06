using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Customer;

[DebuggerDisplay("Count: {CustomerMeterItemsList.Count}")]
public class CustomerMetersXlSheet : ExcelSheetBase
{
    #region Constructor
    public CustomerMetersXlSheet(string excelFilePath)
        : base(ExcelSheetName.CustomerMeters, excelFilePath)
    {
        CustomerMeterItemsList = new List<CustomerMeterItem>();
    }
    #endregion

    #region Public Properties
    public List<CustomerMeterItem> CustomerMeterItemsList { get; set; }
    #endregion
}


//// Label to ID Map for Customer Elements
//var labelToIdMap = new Dictionary<string, int>();
//foreach (var cm in WaterModel.Network.Elements())
//{
//    if (!labelToIdMap.ContainsKey(cm.Label))
//        labelToIdMap.Add(cm.Label, cm.Id);
//}

//// Assign zones to each record
//foreach (var billingRecord in billingData)
//{
//    if(labelToIdMap.TryGetValue(billingRecord.Premise, out int id))
//    {
//        var zone = WaterModel.Element(id) as IWaterZoneableNetworkElementInput;
//        billingRecord.Zone = zone;
//    }
//}



[DebuggerDisplay("ToString()")]
public class CustomerMeterItem
{
    #region Constructor
    public CustomerMeterItem()
    {
    }
    #endregion

    #region Public Properties
    [Column(1, "ID*")]
    public string ID { get; set; }

    [Column(2, "Name")]
    public string? Name { get; set; }

    [Column(3, "Address")]
    public string? Address { get; set; }

    [Column(4, "Email")]
    public string? Email { get; set; }

    [Column(5, "Diameter")]
    public double? Diameter { get; set; }

    [Column(6, "Units")]
    public string? Units { get; set; }

    [Column(7, "Brand")]
    public string? Brand { get; set; }

    [Column(8, "Type of Meter")]
    public string? TypeOfMeter { get; set; }

    [Column(9, "Installation Date")]
    public string InstallationDate { get; set; } = "1999/01/01";

    [Column(10, "Deactivation Date")]
    public string? DecativationDate { get; set; }

    [Column(11, "Customer Type")]
    public string? CustomerType { get; set; }

    [Column(12, "Is Critical")]
    public string IsCritcal { get; set; } = "No";

    [Column(13, "Latitude or Y*")]
    public double LatitudeOrY { get; set; }

    [Column(14, "Longitude or X*")]
    public double LongitudeOrX { get; set; }

    [Column(15, "Zone")]
    public string Zone { get; set; }

    #endregion
    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}, {Zone})";
    }
    #endregion
}