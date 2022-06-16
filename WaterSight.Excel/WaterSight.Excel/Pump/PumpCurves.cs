using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Pump;

[DebuggerDisplay("Count: {Curves.Count}")]
public class PumpCurves : ExcelSheetBase
{
    #region Constructor
    public PumpCurves(string excelFilePath)
        : base(ExcelSheetName.PumpCurves, excelFilePath)
    {
        Curves = new List<PumpCurveItem>();
    }
    #endregion


    #region Public Properties
    public List<PumpCurveItem> Curves { get; set; }
    #endregion
}

public enum CurveType
{
    Head,
    Efficiency,
}

[DebuggerDisplay("{ToString()}")]
public class PumpCurveItem
{
    #region Constructor
    public PumpCurveItem()
    {
    }
    public PumpCurveItem(
        string curveName,
        CurveType curveType,
        double flow,
        double value)
    {
        PumpCurveName = curveName;
        CurveType = curveType;
        Flow = flow;
        Value = value;
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{CurveType}] {PumpCurveName} ({Flow},{Value})";
    }
    #endregion

    #region Public Properties
    [Column(1, "Pump Curve Name")]
    public string PumpCurveName { get; set; }

    [Column(2, "Curve Type")]
    public CurveType CurveType { get; set; }

    [Column(3, "Flow")]
    public double Flow { get; set; }

    [Column(4, "Value")]
    public double Value { get; set; }
    #endregion


}
