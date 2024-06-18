using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Settings;
public class Costs : WSItem
{

    #region Constructor
    public Costs(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    #region Get All Supported
    public async Task<List<CostsConfig?>> GetAll()
    {
        var url = EndPoints.DTCosts;
        return await WS.GetManyAsync<CostsConfig>(url, "Costs");
    }
    #endregion

    #region Get 
    public async Task<CostsConfig?> GetAvgVolumetricProductionCost()
    {
        var url = EndPoints.DTCostsAvgVolumetricProduction;
        return await WS.GetAsync<CostsConfig?>(url, null, "AvgVolProdCost");
    }
    public async Task<CostsConfig?> GetAvgVolumetricTariff()
    {
        var url = EndPoints.DTCostsAvgVolumetricTariff;
        return await WS.GetAsync<CostsConfig?>(url, null, "AvgVolTariff");
    }
    public async Task<CostsConfig?> GetAvgEnergyCost()
    {
        var url = EndPoints.DTCostsAvgEnergyCost;
        return await WS.GetAsync<CostsConfig?>(url, null, "AvgEnergyCost");
    }

    #endregion

    #region Set
    public async Task<bool> SetAvgVolumetricProductionCost(double cost, string unit)
    {
        var url = EndPoints.DTCostsAvgVolumetricProductionSet(cost, unit);
        return await WS.PostAsync(url, null, "AvgVolProdCost", additionalInfo: $"{cost} {unit}");

    }
    public async Task<bool> SetAvgVolumetricTariff(double cost, string unit)
    {
        var url = EndPoints.DTCostsAvgVolumetricTariffSet(cost, unit);
        return await WS.PostAsync(url, null, "AvgVolTariff", additionalInfo: $"{cost} {unit}");
    }
    public async Task<bool> SetAvgEnergyCost(double cost, string unit)
    {
        var url = EndPoints.DTCostsAvgEnergyCostSet(cost, unit);
        return await WS.PostAsync(url, null, "AvgEnergyCost", additionalInfo: $"{cost} {unit}");
    }
    #endregion
    #endregion
}


#region Model
[DebuggerDisplay("{ToString()}")]
public class CostsConfig
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Value { get; set; }
    public string? CurrentUnits { get; set; }

    public override string ToString()
    {
        return $"{Name}: {CurrentUnits} {Value}";
    }
}
#endregion