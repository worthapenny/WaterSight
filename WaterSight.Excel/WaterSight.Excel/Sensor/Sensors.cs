using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WaterSight.Excel.Sensor;

[DebuggerDisplay("Count: {SensorItemsList.Count}")]
public class SensorsXlSheet : ExcelSheetBase
{
    #region Constructor
    public SensorsXlSheet(string excelFilePath)
        : base(ExcelSheetName.Sensors, excelFilePath)
    {
        SensorItemsList = new List<SensorItem>();
    }
    #endregion

    #region Public Methods
    public void LoadFromExcel()
    {
        var excelMapper = new ExcelMapper(base.FilePath);
        SensorItemsList = excelMapper.Fetch<SensorItem>(base.SheetName).ToList();
    }
    #endregion

    #region Public Properties
    public List<SensorItem> SensorItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class SensorItem
{
    #region Constructor
    public SensorItem()
    {
    }
    #endregion

    #region Public Properties
    [Column(1, "Sensor Tag*")]
    public string? SensorTag { get; set; }


    [Column(2, "Display Name*")]
    public string DisplayName { get; set; }

    [Column(3, "Type*")]
    public string Type { get; set; }

    [Column(4, "Units*")]
    public string? Units { get; set; }

    [Column(5, "Read Frequency*")]
    public int ReadFrequency { get; set; }

    [Column(6, "Transmit Frequency*")]
    public int TransmitFrequency { get; set; }

    [Column(7, "UTC Offset*")]
    public string UtcOffSet { get; set; }

    [Column(8, "Latitude or Y*")]
    public double Latitude { get; set; }

    [Column(9, "Longitude Or X*")]
    public double Longitude { get; set; }

    [Column(10, "Ref. Elevation")]
    public double? ReferenceElevation { get; set; }

    [Column(11, "Ref. Elevation Units")]
    public string? ReferenceElevationUnits { get; set; }

    [Column(12, "Priority")]
    public int Priority { get; set; } = 1;

    [Column(13, "Tags/Groups")]
    public string? Tags { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName}, {SensorTag}, ({Type})";
    }
    #endregion
}
