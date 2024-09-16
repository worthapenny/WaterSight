using WaterSight.Web.Core;

namespace WaterSight.Support.Web;

public class Operations
{

    #region Public Methods
    public async Task<Operations> LoadFromWebAsync(WS ws)
    {
        //var opConfigs = await ws.Settings.ServiceExpectations.GetAll();
        //MaxPressure =  ws.Settings.ServiceExpectations.GetMaxPressure(opConfigs) ?? 0;
        //MinPressure = await ws.Settings.ServiceExpectations.GetMinPressure() ?? 0;
        //TargetPumpEfficiency = await ws.Settings.ServiceExpectations.GetTargetPumpEffi() ?? 0;
        //EnergyFromRenewableSource = await ws.Settings.ServiceExpectations.GetEnergyFromRenewableSources() ?? 0;
        //CO2EmissionFactor = await ws.Settings.ServiceExpectations.GetCO2EmissionFactor() ?? 0;

        //AverageVolumeProductionCost = await ws.Settings.Costs.GetAvgVolumeticProductionCost() ?? 0;
        //AverageVolumeTarrif = await ws.Settings.Costs.GetAvgVolumetricTarrif() ?? 0;
        //AverageEnergyCost = await ws.Settings.Costs.GetAvgEnergyCost() ?? 0;

        return this;
    }
    #endregion

    public double MaxPressure { get; set; } = 110;
    public double MinPressure { get; set; } = 60;
    public double TargetPumpEfficiency { get; set; } = 75;
    public double EnergyFromRenewableSource { get; set; } = 0;
    public double CO2EmissionFactor { get; set; } = 0.4;

    public double AverageVolumeProductionCost { get; set; } = 0;
    public double AverageVolumeTariff { get; set; } = 0;
    public double AverageEnergyCost { get; set; } = 0;


}