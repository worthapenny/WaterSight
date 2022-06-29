using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Web.HydrulicStructures;

[DebuggerDisplay("{ToString()}")]
public class CurveConfigBase
{
    #region Constructor
    public CurveConfigBase()
    {
    }
    public CurveConfigBase(string name, int curveType = 0, string xUnits = "%", string yUnits = "%")
    {
        Name = name;
        CurveType = curveType;
        XUnits = xUnits;
        YUnits = yUnits;
    }
    #endregion

    #region Public Properties
    public int Id { get; set; }
    public string? Name { get; set; }
    public int CurveType { get; set; }
    public List<CurveData> CurveData { get; set; } = new List<CurveData>();
    public string? XUnits { get; set; }
    public string? YUnits { get; set; }
    #endregion

    #region Overrideen Methods
    public override string ToString()
    {
        return $"{Id}: {Name} (Type: {CurveType}, Data count: {CurveData.Count})";
    }
    #endregion
}


[DebuggerDisplay("{ToString()}")]
public class CurveData
{
    #region Constructor
    public CurveData()
    {
    }
    public CurveData(double x, double y, double z = double.NaN)
    {
        X = x;
        Y = y;
        Z = z;
    }
    #endregion

    #region Public Properties
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public DateTimeOffset? Instant { get; set; }
    public bool Flag { get; set; } = false;
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{X}, {Y}, {Z}] @ {Instant}";
    }
    #endregion
}