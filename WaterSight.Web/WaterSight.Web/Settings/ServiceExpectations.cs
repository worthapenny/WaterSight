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
    public async Task<List<ServiceExpectationsConfig?>> GetAll()
    {
        var url = EndPoints.DTServiceExpectations;
        return await WS.GetManyAsync<ServiceExpectationsConfig>(url, "Service Expectation");
    }
    #endregion

    #region Get 
    public async Task<ServiceExpectationsConfig?> GetMaxPressure()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationsConfig>(url, null, "SE Max Pressure");
    }
    public async Task<ServiceExpectationsConfig?> GetMinPressure()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationsConfig>(url, null, "SE Min Pressure");
    }
    public async Task<ServiceExpectationsConfig?> GetTargetPumpEffi()
    {
        var url = EndPoints.DTServiceExpectationsMaxPressure;
        return await WS.GetAsync<ServiceExpectationsConfig>(url, null, "SE Target Pump Effi");
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
    #endregion

    #endregion
}


#region Model
[DebuggerDisplay("{ToString()}")]
public class ServiceExpectationsConfig
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