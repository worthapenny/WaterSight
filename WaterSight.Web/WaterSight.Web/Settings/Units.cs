using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Settings;
public class Units : WSItem
{
    #region Constructor
    public Units(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    public async Task<bool> SetToUSUnitsAsync()
    {
        _ = await SetAreaUnit(Area.ft_squared);
        _ = await SetCO2EmissionFactorUnit(CO2EmissionFactor.ton_per_kWh, 2);
        _ = await SetCarbonFootprintUnit(CarbonFootprint.kg);
        _ = await SetCurrencyUnit(Currency.USD, 2);
        _ = await SetDiameterUnit(Length.inch);
        _ = await SetEnergyUnit(Energy.kWh, 1);
        _ = await SetFlowUnit(VolumeFlow.gal_US_per_min);
        _ = await SetLengthUnit(Length.ft);
        _ = await SetMassConcentrationUnit(MassConcentration.mg_per_L, 2);
        _ = await SetPowerUnit(Power.kW, 1);
        _ = await SetPressureUnit(Pressure.psi);
        _ = await SetRatioUnit(Ratio.percent, 1);
        _ = await SetTemperatureUnit(Temperature.degree_F);
        _ = await SetVelocityUnit(Speed.ft_per_s, 1);
        _ = await SetVolumeUnit(Volume.Mgal_US, 2);
        _ = await SetWatherUnit(Weather.Imperial);
        _ = await SetWaterAgeUnit(Duration.h, 1);

        return true;
    }




    #region Get / Update
    public async Task<List<UnitsConfig?>> GetAllUnits()
    {
        var url = EndPoints.DTIdDisplayUnitsOptionsIsTrue;
        var units = await WS.GetManyAsync<UnitsConfig>(url, "Units");
        return units;
    }
    public async Task<bool> SetUnit(string unitTypeName, string unitValue, int precision = 0)
    {
        var url = EndPoints.DTIdDisplayUnits;
        var payload = new { Name = unitTypeName, Units = unitValue, Precision = precision };
        var payloads = new List<object> { payload };
        var httpContent = new StringContent(JsonConvert.SerializeObject(payloads), Encoding.UTF8, "application/json");
        return await WS.PutAsync(url, httpContent, $"{unitTypeName}: {unitValue}", false, "SetUnit");
    }
    public async Task<bool> SetPressureUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Pressure", unitValue, precision);
    }
    public async Task<bool> SetLengthUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Length", unitValue, precision);
    }
    public async Task<bool> SetDiameterUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Diameter", unitValue, precision);
    }
    public async Task<bool> SetVolumeUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Volume", unitValue, precision);
    }
    public async Task<bool> SetEnergyUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Energy", unitValue, precision);
    }


    public async Task<bool> SetWaterAgeUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Duration", unitValue, precision);
    }
    public async Task<bool> SetTemperatureUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Temperature", unitValue, precision);
    }
    public async Task<bool> SetCurrencyUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Currency", unitValue, precision);
    }
    public async Task<bool> SetFlowUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("VolumeFlow", unitValue, precision);
    }
    public async Task<bool> SetAreaUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Area", unitValue, precision);
    }
    public async Task<bool> SetPowerUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Power", unitValue, precision);
    }
    public async Task<bool> SetVelocityUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Speed", unitValue, precision);
    }
    public async Task<bool> SetMassConcentrationUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("MassConcentration", unitValue, precision);
    }
    public async Task<bool> SetRatioUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Ratio", unitValue, precision);
    }
    public async Task<bool> SetWatherUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("Weather", unitValue, precision);
    }
    public async Task<bool> SetCO2EmissionFactorUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("CO2EmissionFactor", unitValue, precision);
    }
    public async Task<bool> SetCarbonFootprintUnit(string unitValue, int precision = 0)
    {
        return await SetUnit("CarbonFootprint", unitValue, precision);
    }
    #endregion

    #endregion
}

#region Model

[DebuggerDisplay("{ToString()}")]
public class UnitsConfig
{
    public string Description { get; set; }
    public string Format { get; set; }
    public string? Name { get; set; }
    public int Precision { get; set; } = 2;
    public string PrettyPrintName { get; set; }
    public string? Units { get; set; }

    [JsonIgnore]
    public List<string> UnitsList { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"{Name}: {Units}:{Precision}";
    }
}

#region Dynamically Generated Unit Type Classes

#region Pressure
public static class Pressure
{
    public static string atm => "atm";
    public static string bar => "bar";
    public static string cbar => "cbar";
    public static string daPa => "daPa";
    public static string dbar => "dbar";
    public static string dyn_per_cm_squared => "dyn/cm²";
    public static string ft_of_head => "ft of head";
    public static string GPa => "GPa";
    public static string hPa => "hPa";
    public static string inHg => "inHg";
    public static string wc => "wc";
    public static string kbar => "kbar";
    public static string kgf_per_cm_squared => "kgf/cm²";
    public static string kgf_per_m_squared => "kgf/m²";
    public static string kgf_per_mm_squared => "kgf/mm²";
    public static string kN_per_cm_squared => "kN/cm²";
    public static string kN_per_m_squared => "kN/m²";
    public static string kN_per_mm_squared => "kN/mm²";
    public static string kPa => "kPa";
    public static string kipf_per_ft_squared => "kipf/ft²";
    public static string kipf_per_in_squared => "kipf/in²";
    public static string Mbar => "Mbar";
    public static string MN_per_m_squared => "MN/m²";
    public static string MPa => "MPa";
    public static string m_of_head => "m of head";
    public static string micro_bar => "µbar";
    public static string micro_Pa => "µPa";
    public static string mbar => "mbar";
    public static string mmHg => "mmHg";
    public static string mPa => "mPa";
    public static string N_per_cm_squared => "N/cm²";
    public static string N_per_m_squared => "N/m²";
    public static string N_per_mm_squared => "N/mm²";
    public static string Pa => "Pa";
    public static string lb_per_ft_squared => "lb/ft²";
    public static string psi => "psi";
    public static string lbm_per_in_s_squared => "lbm/(in·s²)";
    public static string at => "at";
    public static string tf_per_cm_squared => "tf/cm²";
    public static string tf_per_m_squared => "tf/m²";
    public static string tf_per_mm_squared => "tf/mm²";
    public static string torr => "torr";

}

#endregion

#region VolumeFlow
public static class VolumeFlow
{
    public static string af_per_d => "af/d";
    public static string af_per_h => "af/h";
    public static string af_per_m => "af/m";
    public static string af_per_s => "af/s";
    public static string cl_per_day => "cl/day";
    public static string cL_per_min => "cL/min";
    public static string dm_cubed_per_min => "dm³/min";
    public static string ft_cubed_per_h => "ft³/h";
    public static string ft_cubed_per_min => "ft³/min";
    public static string ft_cubed_per_s => "ft³/s";
    public static string m_cubed_per_d => "m³/d";
    public static string m_cubed_per_h => "m³/h";
    public static string m_cubed_per_min => "m³/min";
    public static string m_cubed_per_s => "m³/s";
    public static string mm_cubed_per_s => "mm³/s";
    public static string cy_per_day => "cy/day";
    public static string yd_cubed_per_h => "yd³/h";
    public static string yd_cubed_per_min => "yd³/min";
    public static string yd_cubed_per_s => "yd³/s";
    public static string dl_per_day => "dl/day";
    public static string dL_per_min => "dL/min";
    public static string kl_per_day => "kl/day";
    public static string kL_per_min => "kL/min";
    public static string kgal_US_per_min => "kgal (U.S.)/min";
    public static string l_per_day => "l/day";
    public static string L_per_h => "L/h";
    public static string L_per_min => "L/min";
    public static string L_per_s => "L/s";
    public static string Ml_per_day => "Ml/day";
    public static string Mgal_imp_per_s => "Mgal (imp.)/s";
    public static string micro_l_per_day => "µl/day";
    public static string micro_L_per_min => "µL/min";
    public static string ml_per_day => "ml/day";
    public static string mL_per_min => "mL/min";
    public static string MGD => "MGD";
    public static string nl_per_day => "nl/day";
    public static string nL_per_min => "nL/min";
    public static string bbl_per_d => "bbl/d";
    public static string bbl_per_hr => "bbl/hr";
    public static string bbl_per_min => "bbl/min";
    public static string bbl_per_s => "bbl/s";
    public static string gal_U_K_per_d => "gal (U. K.)/d";
    public static string gal_imp_per_h => "gal (imp.)/h";
    public static string gal_imp_per_min => "gal (imp.)/min";
    public static string gal_imp_per_s => "gal (imp.)/s";
    public static string gpd => "gpd";
    public static string gal_US_per_h => "gal (U.S.)/h";
    public static string gal_US_per_min => "gal (U.S.)/min";
    public static string gal_US_per_s => "gal (U.S.)/s";

}

#endregion

#region Length
public static class Length
{
    public static string au => "au";
    public static string cm => "cm";
    public static string ch => "ch";
    public static string dm => "dm";
    public static string pica => "pica";
    public static string pt => "pt";
    public static string fathom => "fathom";
    public static string ft => "ft";
    public static string h => "h";
    public static string hm => "hm";
    public static string inch => "in";
    public static string kly => "kly";
    public static string km => "km";
    public static string kpc => "kpc";
    public static string ly => "ly";
    public static string Mly => "Mly";
    public static string Mpc => "Mpc";
    public static string m => "m";
    public static string micro_in => "µin";
    public static string micro_m => "µm";
    public static string mil => "mil";
    public static string mi => "mi";
    public static string mm => "mm";
    public static string nm => "nm";
    public static string NM => "NM";
    public static string pc => "pc";
    public static string shackle => "shackle";
    public static string R_dp => "R⊙";
    public static string twip => "twip";
    public static string ftUS => "ftUS";
    public static string yd => "yd";

}

#endregion

#region Area
public static class Area
{
    public static string ac => "ac";
    public static string ha => "ha";
    public static string cm_squared => "cm²";
    public static string dm_squared => "dm²";
    public static string ft_squared => "ft²";
    public static string in_squared => "in²";
    public static string km_squared => "km²";
    public static string m_squared => "m²";
    public static string micro_m_squared => "µm²";
    public static string mi_squared => "mi²";
    public static string mm_squared => "mm²";
    public static string nmi_squared => "nmi²";
    public static string yd_squared => "yd²";
    public static string ft_squared__US => "ft² (US)";

}

#endregion

#region Volume
public static class Volume
{
    public static string ac_ft => "ac-ft";
    public static string cl => "cl";
    public static string cm_cubed => "cm³";
    public static string dm_cubed => "dm³";
    public static string ft_cubed => "ft³";
    public static string hm_cubed => "hm³";
    public static string in_cubed => "in³";
    public static string km_cubed => "km³";
    public static string m_cubed => "m³";
    public static string micro_m_cubed => "µm³";
    public static string mi_cubed => "mi³";
    public static string mm_cubed => "mm³";
    public static string yd_cubed => "yd³";
    public static string dagal_US => "dagal (U.S.)";
    public static string dl => "dl";
    public static string dgal_US => "dgal (U.S.)";
    public static string hft_cubed => "hft³";
    public static string hl => "hl";
    public static string hgal_US => "hgal (U.S.)";
    public static string bl_imp => "bl (imp.)";
    public static string gal_imp => "gal (imp.)";
    public static string oz_imp => "oz (imp.)";
    public static string pt_imp => "pt (imp.)";
    public static string kft_cubed => "kft³";
    public static string kgal_imp => "kgal (imp.)";
    public static string kl => "kl";
    public static string kgal_US => "kgal (U.S.)";
    public static string l => "l";
    public static string Mft_cubed => "Mft³";
    public static string Mgal_imp => "Mgal (imp.)";
    public static string Ml => "Ml";
    public static string Mgal_US => "Mgal (U.S.)";
    public static string tsp => "tsp";
    public static string micro_l => "µl";
    public static string ml => "ml";
    public static string bbl => "bbl";
    public static string bl_US => "bl (U.S.)";
    public static string gal_US => "gal (U.S.)";
    public static string oz_US => "oz (U.S.)";
    public static string pt_US => "pt (U.S.)";
    public static string qt_US => "qt (U.S.)";

}

#endregion

#region Power
public static class Power
{
    public static string hp_S => "hp(S)";
    public static string Btu_per_h => "Btu/h";
    public static string daW => "daW";
    public static string dW => "dW";
    public static string hp_E => "hp(E)";
    public static string fW => "fW";
    public static string GJ_per_h => "GJ/h";
    public static string GW => "GW";
    public static string hp_H => "hp(H)";
    public static string J_per_h => "J/h";
    public static string kBtu_per_h => "kBtu/h";
    public static string kJ_per_h => "kJ/h";
    public static string kW => "kW";
    public static string hp_I => "hp(I)";
    public static string MJ_per_h => "MJ/h";
    public static string MW => "MW";
    public static string hp_M => "hp(M)";
    public static string micro_W => "µW";
    public static string mJ_per_h => "mJ/h";
    public static string mW => "mW";
    public static string nW => "nW";
    public static string PW => "PW";
    public static string pW => "pW";
    public static string TW => "TW";
    public static string W => "W";

}

#endregion

#region Energy
public static class Energy
{
    public static string BTU => "BTU";
    public static string cal => "cal";
    public static string Dth_EC => "Dth (E.C.)";
    public static string Dth_imp => "Dth (imp.)";
    public static string Dth_US => "Dth (U.S.)";
    public static string eV => "eV";
    public static string erg => "erg";
    public static string ft_lb => "ft·lb";
    public static string GBTU => "GBTU";
    public static string GeV => "GeV";
    public static string GJ => "GJ";
    public static string GWd => "GWd";
    public static string GWh => "GWh";
    public static string J => "J";
    public static string kBTU => "kBTU";
    public static string kcal => "kcal";
    public static string keV => "keV";
    public static string kJ => "kJ";
    public static string kWd => "kWd";
    public static string kWh => "kWh";
    public static string MBTU => "MBTU";
    public static string Mcal => "Mcal";
    public static string MeV => "MeV";
    public static string MJ => "MJ";
    public static string MWd => "MWd";
    public static string MWh => "MWh";
    public static string mJ => "mJ";
    public static string TeV => "TeV";
    public static string TWd => "TWd";
    public static string TWh => "TWh";
    public static string th_EC => "th (E.C.)";
    public static string th_imp => "th (imp.)";
    public static string th_US => "th (U.S.)";
    public static string Wd => "Wd";
    public static string Wh => "Wh";

}

#endregion

#region Speed
public static class Speed
{
    public static string cm_per_h => "cm/h";
    public static string cm_per_min => "cm/min";
    public static string cm_per_s => "cm/s";
    public static string dm_per_min => "dm/min";
    public static string dm_per_s => "dm/s";
    public static string ft_per_h => "ft/h";
    public static string ft_per_min => "ft/min";
    public static string ft_per_s => "ft/s";
    public static string in_per_h => "in/h";
    public static string in_per_min => "in/min";
    public static string in_per_s => "in/s";
    public static string km_per_h => "km/h";
    public static string km_per_min => "km/min";
    public static string km_per_s => "km/s";
    public static string kn => "kn";
    public static string m_per_h => "m/h";
    public static string m_per_min => "m/min";
    public static string m_per_s => "m/s";
    public static string micro_m_per_min => "µm/min";
    public static string micro_m_per_s => "µm/s";
    public static string mph => "mph";
    public static string mm_per_h => "mm/h";
    public static string mm_per_min => "mm/min";
    public static string mm_per_s => "mm/s";
    public static string nm_per_min => "nm/min";
    public static string nm_per_s => "nm/s";
    public static string ftUS_per_h => "ftUS/h";
    public static string ftUS_per_min => "ftUS/min";
    public static string ftUS_per_s => "ftUS/s";
    public static string yd_per_h => "yd/h";
    public static string yd_per_min => "yd/min";
    public static string yd_per_s => "yd/s";

}

#endregion

#region Duration
public static class Duration
{
    public static string d => "d";
    public static string h => "h";
    public static string micro_s => "µs";
    public static string ms => "ms";
    public static string m => "m";
    public static string mo => "mo";
    public static string ns => "ns";
    public static string s => "s";
    public static string wk => "wk";
    public static string yr => "yr";

}

#endregion

#region MassConcentration
public static class MassConcentration
{
    public static string cg_per_dL => "cg/dL";
    public static string cg_per_L => "cg/L";
    public static string cg_per_mL => "cg/mL";
    public static string dg_per_dL => "dg/dL";
    public static string dg_per_L => "dg/L";
    public static string dg_per_mL => "dg/mL";
    public static string g_per_cm_cubed => "g/cm³";
    public static string g_per_m_cubed => "g/m³";
    public static string g_per_mm_cubed => "g/mm³";
    public static string g_per_dL => "g/dL";
    public static string g_per_L => "g/L";
    public static string g_per_mL => "g/mL";
    public static string kg_per_cm_cubed => "kg/cm³";
    public static string kg_per_m_cubed => "kg/m³";
    public static string kg_per_mm_cubed => "kg/mm³";
    public static string kg_per_L => "kg/L";
    public static string kip_per_ft_cubed => "kip/ft³";
    public static string kip_per_in_cubed => "kip/in³";
    public static string micro_g_per_m_cubed => "µg/m³";
    public static string micro_g_per_dL => "µg/dL";
    public static string micro_g_per_L => "µg/L";
    public static string micro_g_per_mL => "µg/mL";
    public static string mg_per_m_cubed => "mg/m³";
    public static string mg_per_dL => "mg/dL";
    public static string mg_per_L => "mg/L";
    public static string mg_per_mL => "mg/mL";
    public static string ng_per_dL => "ng/dL";
    public static string ng_per_L => "ng/L";
    public static string ng_per_mL => "ng/mL";
    public static string pg_per_dL => "pg/dL";
    public static string pg_per_L => "pg/L";
    public static string pg_per_mL => "pg/mL";
    public static string lb_per_ft_cubed => "lb/ft³";
    public static string lb_per_in_cubed => "lb/in³";
    public static string ppg_imp => "ppg (imp.)";
    public static string ppg_US => "ppg (U.S.)";
    public static string slug_per_ft_cubed => "slug/ft³";
    public static string t_per_cm_cubed => "t/cm³";
    public static string t_per_m_cubed => "t/m³";
    public static string t_per_mm_cubed => "t/mm³";

}

#endregion

#region Temperature
public static class Temperature
{
    public static string degree_C => "°C";
    public static string degree_De => "°De";
    public static string degree_F => "°F";
    public static string degree_N => "°N";
    public static string degree_R => "°R";
    public static string degree_Ré => "°Ré";
    public static string degree_Rø => "°Rø";
    public static string K => "K";
    public static string m_degree_C => "m°C";
    public static string T_dp => "T⊙";

}

#endregion

#region Ratio
public static class Ratio
{
    public static string ppb => "ppb";
    public static string ppm => "ppm";
    public static string percent_dot => "‰";
    public static string ppt => "ppt";
    public static string percent => "%";

}

#endregion

#region Currency
public static class Currency
{
    //public static string USDoller => "$";
    //public static string Euro => "€";
    //public static string Pound => "£";

    public static string AED => "AED";
    public static string BRL => "BRL";
    public static string CHF => "CHF";
    public static string CNY => "CNY";
    public static string EUR => "EUR";
    public static string GBP => "GBP";
    public static string INR => "INR";
    public static string JPY => "JPY";
    public static string KRW => "KRW";
    public static string MXN => "MXN";
    public static string PHP => "PHP";
    public static string PLN => "PLN";
    public static string SEK => "SEK";
    public static string SGD => "SGD";
    public static string TRY => "TRY";
    public static string USD => "USD";
    public static string ZAR => "ZAR";
}

#endregion

#region Weather
public static class Weather
{
    public static string Imperial => "Imperial";
    public static string Metric => "Metric";
    //public static string Metric_SI => "Metric SI";
    //public static string Hybrid => "Hybrid";

}

#endregion

#region Carbon EmissionFactor / Footprint
public static class CO2EmissionFactor
{
    public static string kg_per_kWh => "kg/kWh";
    public static string lbs_per_kWh => "lbs/kWh";
    public static string ton_per_kWh => "ton/kWh";
    public static string tonne_per_kWh => "tonne/kWh";


}
public static class CarbonFootprint
{
    public static string kg => "kg";
}
#endregion

#endregion

#endregion