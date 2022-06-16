using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Tank;


[DebuggerDisplay("Count: {TankItemsList.Count}")]
public class TankCurves : ExcelSheetBase
{
    #region Constructor
    public TankCurves(string excelFilePath)
        : base(ExcelSheetName.TankCurves, excelFilePath)
    {
        TankCurveItemsList = new List<TankCurveItem>();
    }
    #endregion

    #region Public Properties
    public List<TankCurveItem> TankCurveItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("ToString()")]
public class TankCurveItem
{
    #region Constructor
    public TankCurveItem()
    {
    }
    public TankCurveItem(string curveName, double depthRatio, double volumeRatio)
    {
        CurveName = curveName;
        DepthRatio = depthRatio;
        VolumeRatio = volumeRatio;
    }
    #endregion

    #region Public Properties
    [Column(1, "Curve Name*")]
    public string CurveName { get; set; }

    [Column(2, "Depth Ratio (%)*")]
    public double? DepthRatio { get; set; } = null;


    [Column(1, "Volume Ratio (%)*")]
    public double? VolumeRatio { get; set; } = null;
    #endregion


    #region Overridden Methods
    public override string ToString()
    {
        return $"{CurveName}, ({DepthRatio}, {VolumeRatio})";
    }
    #endregion

}