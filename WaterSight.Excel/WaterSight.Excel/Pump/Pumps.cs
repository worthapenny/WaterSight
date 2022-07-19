using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Pump;

[DebuggerDisplay("Count: {PumpItemsList.Count}")]
public class PumpsXlSheet : ExcelSheetBase
{
    #region Constructor
    public PumpsXlSheet(string excelFilePath)
        : base(ExcelSheetName.Pumps, excelFilePath)
    {
        PumpItemsList = new List<PumpItem>();
    }
    #endregion

    #region Public Properties
    public List<PumpItem> PumpItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class PumpItem
{
    #region Constructor
    public PumpItem()
    {
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Pump: {DisplayName}";
    }
    #endregion

    #region Public Properties
    [Column(1, "Display Name")]
    public string DisplayName { get; set; }

    [Column(2, "Design Flow")]
    public double? DesignFlow { get; set; }

    [Column(3, "Design Head")]
    public double? DesignHead { get; set; }

    [Column(4, "Best Efficiency Flow")]
    public double? BestEfficiencyFlow { get; set; }

    [Column(5, "Best Efficiency Head")]
    public double? BestEfficiencyHead { get; set; }

    [Column(6, "BEP Percent")]
    public double? BEPPercent { get; set; }

    [Column(7, "Flow Unit")]
    public string FlowUnit { get; set; }

    [Column(8, "Head Unit")]
    public string HeadUnit { get; set; }

    [Column(9, "Pump Station Display Name")]
    public string PumpStationDisplayName { get; set; }

    [Column(10, "Suction Tag")]
    public string? SuctionTag { get; set; }

    [Column(11, "Discharge Tag")]
    public string? DischargeTag { get; set; }

    [Column(12, "Flow Tag")]
    public string? FlowTag { get; set; }

    [Column(13, "Power Tag")]
    public string? PowerTag { get; set; }

    [Column(14, "Head Curve Name")]
    public string HeadCurveName { get; set; }

    [Column(15, "Efficiency Curve Name")]
    public string? EfficiencyCurveName { get; set; }

    [Column(16, "Power Curve Name")]
    public string? PowerCurveName { get; set; }

    [Column(17, "Power Units")]
    public string PowerUnits { get; set; }

    [Column(18, "Motor Efficiency")]
    public double? MotorEfficiency { get; set; }

    [Column(19, "Is Variable Speed")]
    public bool IsVariableSpeed { get; set; } = false;

    [Column(20, "Full Speed Value")]
    public double? FullSpeedValue { get; set; }

    [Column(21, "Variable Speed Effi Curve Name")]
    public string? VariableSpeedEffiCurveName { get; set; }

    [Column(22, "Speed Tag Name")]
    public string? SpeedTagName { get; set; }

    [Column(23, "Status Tag")]
    public string? StatusTag { get; set; }

    [Column(24, "Groups")]
    public string? Groups { get; set; }

    [Column(25, "Notes")]
    public string? Notes { get; set; }

    #endregion
}