//using Newtonsoft.Json;
//using WaterSight.Web.Core;
//using WaterSight.Web.DT;
//using WaterSight.Web.Settings;

//namespace WaterSight.UI.Support.Model;

//public class WsSetting
//{
//    public async Task<WsSetting> LoadSettings(WS ws, DigitalTwinConfig dtConfig)
//    {
//        var instance = new WsSetting();
//        instance.Info = Info.LoadFromDtConfig(dtConfig);
//        instance.Geo = await Geo.LoadFromWebAsync(ws);
//        instance.Units = await ws.Settings.Units.GetAllUnits();
//        instance.Operations = await Operations.LoadFromWebAsync(ws);
//        return instance;
//    }

//    public Info Info { get; set; } = new Info();

//    public Geo Geo { get; set; } = new Geo();

//    public Operations Operations { get; set; } = new Operations();

//    public UnitSystem UnitSystem { get; set; } = UnitSystem.US;

//    [JsonIgnore]
//    public List<UnitsConfig?> Units { get; set; } = new List<UnitsConfig?>();
//}

//public class Info
//{
//    public Info LoadFromDtConfig(DigitalTwinConfig dtConfig)
//    {
//        if (dtConfig != null)
//        {
//            Name = dtConfig.Name;
//            Description = dtConfig.Description;
//            DTID = dtConfig.ID;
//        }
//        return this;
//    }


//    public string Name { get; set; } = string.Empty;
//    public string Description { get; set; } = string.Empty;
//    public int DTID { get; set; }

//    public string ShortName { get; set; } = string.Empty;


//}

//public class Geo
//{
//    public async Task<Geo> LoadFromWebAsync(WS ws)
//    {
//        var taskCoordinates = ws.Settings.CoordinateSystems.GetAll();
//        var taskTimeZone = ws.Settings.TimeZone.GetTimeZoneName();
//        var taskLocation = ws.Settings.Location.GetLocation();
//        var tasks = new List<Task>() { taskCoordinates, taskTimeZone, taskLocation };

//        await Task.WhenAll(tasks);

//        // EPSG Codes
//        foreach (var code in taskCoordinates.Result)
//        {
//            if (code?.Name == CoordinateSystems.NameSensors) SensorEPSG = code.Epsg ?? 4326;
//            if (code?.Name == CoordinateSystems.NameCustomers) CustomersEPSG = code.Epsg ?? 4326;
//            if (code?.Name == CoordinateSystems.NameSmartMeters) SmartMetersEPSG = code.Epsg ?? 4326;
//            if (code?.Name == CoordinateSystems.NameWorkOrders) WorkOrdersEPSG = code.Epsg ?? 4326;
//        }

//        // Lat/Lng
//        var location = taskLocation.Result;
//        Latitude = location?.Latitude ?? 0;
//        Longitude = location?.Longitude ?? 0;


//        // TimeZone
//        TimeZoneString = taskTimeZone.Result;

//        return this;
//    }

//    public int SensorEPSG { get; set; } = 4326;
//    public int CustomersEPSG { get; set; } = 4326;
//    public int SmartMetersEPSG { get; set; } = 4326;
//    public int WorkOrdersEPSG { get; set; } = 4326;

//    public float Latitude { get; set; }
//    public float Longitude { get; set; }

//    public string TimeZoneString { get; set; } = string.Empty;
//}

//public class Operations
//{

//    #region Public Methods
//    public async Task<Operations> LoadFromWebAsync(WS ws)
//    {
//        //var opConfigs = await ws.Settings.ServiceExpectations.GetAll();
//        //MaxPressure =  ws.Settings.ServiceExpectations.GetMaxPressure(opConfigs) ?? 0;
//        //MinPressure = await ws.Settings.ServiceExpectations.GetMinPressure() ?? 0;
//        //TargetPumpEfficiency = await ws.Settings.ServiceExpectations.GetTargetPumpEffi() ?? 0;
//        //EnergyFromRenewableSource = await ws.Settings.ServiceExpectations.GetEnergyFromRenewableSources() ?? 0;
//        //CO2EmissionFactor = await ws.Settings.ServiceExpectations.GetCO2EmissionFactor() ?? 0;

//        //AverageVolumeProductionCost = await ws.Settings.Costs.GetAvgVolumeticProductionCost() ?? 0;
//        //AverageVolumeTarrif = await ws.Settings.Costs.GetAvgVolumetricTarrif() ?? 0;
//        //AverageEnergyCost = await ws.Settings.Costs.GetAvgEnergyCost() ?? 0;

//        return this;
//    }
//    #endregion

//    public double MaxPressure { get; set; } = 110;
//    public double MinPressure { get; set; } = 60;
//    public double TargetPumpEfficiency { get; set; } = 75;
//    public double EnergyFromRenewableSource { get; set; } = 0;
//    public double CO2EmissionFactor { get; set; } = 0.4;

//    public double AverageVolumeProductionCost { get; set; } = 0;
//    public double AverageVolumeTarrif{ get; set; } = 0;
//    public double AverageEnergyCost { get; set; } = 0;

    
//}

//public enum UnitSystem
//{
//    US, SI
//}