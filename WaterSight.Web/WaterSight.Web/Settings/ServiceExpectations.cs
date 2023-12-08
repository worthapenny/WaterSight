using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;


namespace WaterSight.Web.Settings;

public class ServiceExpectations : WSItem
{
    #region Constructor
    public ServiceExpectations(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Get All Supported
    public async Task<List<ServiceExpectationItemConfig?>> GetAll()
    {
        var url = EndPoints.DTServiceExpectations;
        return await WS.GetManyAsync<ServiceExpectationItemConfig>(url, "Service Expectation");
    }
    #endregion

    #region Get 
    public async Task<ServiceExpectationItemConfig?> GetMaxPressure()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationItemConfig>(url, null, "SE Max Pressure");
    }
    public async Task<ServiceExpectationItemConfig?> GetMinPressure()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationItemConfig>(url, null, "SE Min Pressure");
    }
    public async Task<ServiceExpectationItemConfig?> GetTargetPumpEffi()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationItemConfig>(url, null, "SE Target Pump Effi");
    }

    #endregion

    #region Set
    public async Task<bool> SetMaxPressure(double pressure)
    {
        var url = EndPoints.DTServiceExpectationsMaxPressureSet(pressure);
        return await WS.PostAsync(url, null, "SE Max Pressure", additionalInfo: $"{pressure}");

    }
    public async Task<bool> SetMinPressure(double pressure)
    {
        var url = EndPoints.DTServiceExpectationsMinPressureSet(pressure);
        return await WS.PostAsync(url, null, "SE Min Pressure", additionalInfo: $"{pressure}");
    }
    public async Task<bool> SetTargetPumpEfficiency(double effi)
    {
        var url = EndPoints.DTServiceExpectationsTargetPumpEfficiencySet(effi);
        return await WS.PostAsync(url, null, "SE Target Pump Effi.", additionalInfo: $"{effi}");
    }
    public async Task<bool> SetEnergyFromRenewableSources(double energy)
    {
        var url = EndPoints.DTServiceExpectationsEnergyFromRenewableSourcesSet(energy);
        return await WS.PostAsync(url, null, "SE Energy from Renewable Sources.", additionalInfo: $"{energy}");
    }
    public async Task<bool> SetCO2EmissionFactor(double carbonFootprint, string unit)
    {
        var url = EndPoints.DTServiceExpectationsCO2EmissionFactorSet(carbonFootprint, unit);
        return await WS.PostAsync(url, null, "SE Carbon Footprint.", additionalInfo: $"{carbonFootprint}");
    }
    #endregion

    #endregion
}


#region Model
[DebuggerDisplay("{ToString()}")]
public class ServiceExpectationItemConfig
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Value { get; set; }
    public string? CurrentUnits { get; set; }

    public override string ToString()
    {
        return $"{Name} = {Value} {CurrentUnits}";
    }
}
#endregion