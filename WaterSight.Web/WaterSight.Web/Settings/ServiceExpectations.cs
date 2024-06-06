using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;


namespace WaterSight.Web.Settings;

public class ServiceExpectations : WSItem
{
    #region Constants
    public const string NameMaxPressure = "MaxPressure";
    public const string NameMinPressure = "MinPressure";
    public const string NameMinPumpEfficiency = "MinPumpEfficiency";
    public const string NameRenewableEnergy = "RenewableEnergy";
    public const string NameCO2EmissionFactor = "CO2EmissionFactor";
    #endregion

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
    public ServiceExpectationItemConfig? GetMaxPressure(List<ServiceExpectationItemConfig?> serviceConfigs)
    {
        var itemCheck = serviceConfigs.Where(s => s.Name == NameMaxPressure);
        return itemCheck.Any() ? itemCheck.FirstOrDefault() : null;
    }
    public ServiceExpectationItemConfig? GetMinPressure(List<ServiceExpectationItemConfig?> serviceConfigs)
    {
        var itemCheck = serviceConfigs.Where(s => s.Name == NameMinPressure);
        return itemCheck.Any() ? itemCheck.FirstOrDefault() : null;
    }
    public ServiceExpectationItemConfig? GetTargetPumpEffi(List<ServiceExpectationItemConfig?> serviceConfigs)
    {
        var itemCheck = serviceConfigs.Where(s => s.Name == NameMinPumpEfficiency);
        return itemCheck.Any() ? itemCheck.FirstOrDefault() : null;
    }
    public ServiceExpectationItemConfig? GetEnergyFromRenewableSources(List<ServiceExpectationItemConfig?> serviceConfigs)
    {
        var itemCheck = serviceConfigs.Where(s => s.Name == NameRenewableEnergy);
        return itemCheck.Any() ? itemCheck.FirstOrDefault() : null;
    }
    public ServiceExpectationItemConfig? GetCO2EmissionFactor(List<ServiceExpectationItemConfig?> serviceConfigs)
    {
        var itemCheck = serviceConfigs.Where(s => s.Name == NameCO2EmissionFactor);
        return itemCheck.Any() ? itemCheck.FirstOrDefault() : null;
    }
    #endregion

    #region Set
    public async Task<bool> SetMaxPressure(double pressure, string unit)
    {
        var url = EndPoints.DTServiceExpectationsMaxPressureSet(pressure, unit);
        return await WS.PostAsync(url, null, "SE Max Pressure", additionalInfo: $"{pressure} {unit}");

    }
    public async Task<bool> SetMinPressure(double pressure, string unit)
    {
        var url = EndPoints.DTServiceExpectationsMinPressureSet(pressure, unit);
        return await WS.PostAsync(url, null, "SE Min Pressure", additionalInfo: $"{pressure} {unit}");
    }
    public async Task<bool> SetTargetPumpEfficiency(double effi)
    {
        var unit = "%";
        var url = EndPoints.DTServiceExpectationsTargetPumpEfficiencySet(effi, unit);
        return await WS.PostAsync(url, null, "SE Target Pump Effi.", additionalInfo: $"{effi} {unit}");
    }
    public async Task<bool> SetEnergyFromRenewableSources(double energy, string unit)
    {
        var url = EndPoints.DTServiceExpectationsEnergyFromRenewableSourcesSet(energy, unit);
        return await WS.PostAsync(url, null, "SE Energy from Renewable Sources.", additionalInfo: $"{energy} {unit}");
    }
    public async Task<bool> SetCO2EmissionFactor(double carbonFootprint, string unit)
    {
        var url = EndPoints.DTServiceExpectationsCO2EmissionFactorSet(carbonFootprint, unit);
        return await WS.PostAsync(url, null, "SE Carbon Footprint.", additionalInfo: $"{carbonFootprint} {unit}");
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