using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.HydrulicStructures;

public class TankCurve : WSItem
{
    #region Constructor
    public TankCurve(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region CRUD Operations

    public async Task<TankCurveConfig?> AddTankCurveConfigAsync(TankCurveConfig tankCurveConfig)
    {
        var url = EndPoints.HydStructuresTankCurveQDTCurveType(0);
        int? id = await WS.AddAsync(tankCurveConfig, url, "Tank curve");
        if (id.HasValue)
        {
            tankCurveConfig.Id = id.Value;
            return tankCurveConfig;
        }

        return null;
    }
    public async Task<bool> UpdateTankCurveConfigAsync(TankCurveConfig tankCurveConfig)
    {
        var url = EndPoints.HydStructuresTankCurveForQDT(tankCurveConfig.Id);
        return await WS.UpdateAsync(tankCurveConfig.Id, tankCurveConfig, url, "Tank curve", false);
    }
    public async Task<TankCurveConfig?> GetTankCurveConfigAsync(int id)
    {
        var url = EndPoints.HydStructuresTankCurveForQDT(id);
        var tankConfig = await WS.GetAsync<TankCurveConfig>(url, id, "Tank curve");
        return tankConfig;
    }
    public async Task<List<TankCurveConfig?>> GetTankCurvesConfigAsync()
    {
        var url = EndPoints.HydStructuresTankCurveQDTCurveType(0);
        return await WS.GetManyAsync<TankCurveConfig>(url, "Tank curves");
    }
    public async Task<bool> DeleteTankCurveConfigAsync(int id)
    {
        var url = EndPoints.HydStructuresTankCurveForQDT(id);
        return await WS.DeleteAsync(id, url, "Tank curve", false);
    }
    public async Task<bool> DeleteTankCurvesConfigAsync()
    {
        throw new NotSupportedException("WaterSight API doesn't provide an end-point for this. Delete one by one calling");
    }
    #endregion


    #endregion
}


#region Model Classes

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

[DebuggerDisplay("{ToString()}")]
public class TankCurveConfig
{
    #region Constructor
    public TankCurveConfig()
    {
    }
    public TankCurveConfig(string name, int curveType = 0, string xUnits = "%", string yUnits = "%")
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

#endregion