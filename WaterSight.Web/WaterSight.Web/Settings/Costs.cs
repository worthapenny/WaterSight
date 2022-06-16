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
    public async Task<CostsConfig?> GetAvgVolumeticProductionCost()
    {
        var url = EndPoints.DTCostsAvgVolumetricProduction;
        return await WS.GetAsync<CostsConfig>(url, null, "AvgVolProdCost");
    }
    public async Task<CostsConfig?> GetAvgVolumetricTarrif()
    {
        var url = EndPoints.DTCostsAvgVolumetricTariff;
        return await WS.GetAsync<CostsConfig>(url, null, "AvgVolTarrif");
    }
    public async Task<CostsConfig?> GetAvgEnergyCost()
    {
        var url = EndPoints.DTCostsAvgEnergyCost;
        return await WS.GetAsync<CostsConfig>(url, null, "AvgEnergyCost");
    }

    #endregion

    #region Set
    public async Task<bool> SetAvgVolumeticProductionCost(double cost)
    {
        var url = EndPoints.DTCostsAvgVolumetricProductionSet(cost);
        return await WS.PostAsync(url, null, "AvgVolProdCost", additionalInfo: $"{cost}");

    }
    public async Task<bool> SetAvgVolumetricTarrif(double cost)
    {
        var url = EndPoints.DTCostsAvgVolumetricTariffSet(cost);
        return await WS.PostAsync(url, null, "AvgVolTarrif", additionalInfo: $"{cost}");
    }
    public async Task<bool> SetAvgEnergyCost(double cost)
    {
        var url = EndPoints.DTCostsAvgEnergyCostSet(cost);
        return await WS.PostAsync(url, null, "AvgEnergyCost", additionalInfo: $"{cost}");
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