using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Tank;

[DebuggerDisplay("Count: {TankItemsList.Count}")]
public class TanksXlSheet : ExcelSheetBase
{
    #region Constructor
    public TanksXlSheet(string excelFilePath)
        : base(ExcelSheetName.Tanks, excelFilePath)
    {
        TankItemsList = new List<TankItem>();
    }
    #endregion

    #region Public Properties
    public List<TankItem> TankItemsList { get; set; }
    #endregion
}


[DebuggerDisplay("ToString()")]
public class TankItem
{
    #region Constructor
    public TankItem()
    {
    }
    #endregion

    #region Public Properties
    [Column(1, "Display Name*")]
    public string DisplayName { get; set; }


    [Column(2, "Base Elevation*")]
    public double BaseElevation { get; set; }

    [Column(3, "Min Level*")]
    public double MinLevel { get; set; }

    [Column(4, "Max Level*")]
    public double MaxLevel { get; set; }

    [Column(5, "Length Units*")]
    public string LengthUnits { get; set; }

    [Column(6, "Max Volume*")]
    public double MaxVolume { get; set; }

    [Column(7, "Tank Curve Name")]
    public string TankCurveName { get; set; }

    [Column(8, "Volume Units*")]
    public string VolumeUnits { get; set; }

    [Column(9, "Level Tag*")]
    public string LevelTag { get; set; }

    [Column(10, "Desired Turnover Days")]
    public double DesiredTurnoverDays { get; set; }


    [Column(11, "Tags/Groups")]
    public string? Tags { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName}, {LevelTag}";
    }
    #endregion
}