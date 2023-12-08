using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.DT;
using WaterSight.Web.Settings;

namespace WaterSight.UI.Support.Model;

public class WsSetting
{
    public async Task<WsSetting> LoadFromWebAsync(WS ws, DigitalTwinConfig dtConfig)
    {
        var instance = new WsSetting();
        instance.Info = Info.LoadFromDtConfig(dtConfig);
        instance.Geo = await Geo.LoadFromWebAsync(ws);
        instance.Units = await ws.Settings.Units.GetAllUnits();

        return instance;
    }

    public Info Info { get; set; } = new Info();

    public Geo Geo { get; set; } = new Geo();

    public UnitSystem UnitSystem { get; set; } = UnitSystem.US;
    public List<UnitsConfig?> Units { get; set; } = new List<UnitsConfig?>();
}

public class Info
{
    public Info LoadFromDtConfig(DigitalTwinConfig dtConfig)
    {
        if (dtConfig != null)
        {
            Name = dtConfig.Name;
            Description = dtConfig.Description;
            DTID = dtConfig.ID;
        }
        return this;
    }


    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DTID { get; set; }

    public string ShortName { get; set; } = string.Empty;


}

public class Geo
{
    public async Task<Geo> LoadFromWebAsync(WS ws)
    {
        var taskCoordinates = ws.Settings.CoordinateSystems.GetAll();
        var taskTimeZone = ws.Settings.TimeZone.GetTimeZoneName();
        var taskLocation = ws.Settings.Location.GetLocation();
        var tasks = new List<Task>() { taskCoordinates, taskTimeZone, taskLocation };

        await Task.WhenAll(tasks);

        // EPSG Codes
        foreach (var code in taskCoordinates.Result)
        {
            if (code?.Name == CoordinateSystems.NameSensors) SensorEPSG = code.Epsg ?? 4326;
            if (code?.Name == CoordinateSystems.NameCustomers) CustomersEPSG = code.Epsg ?? 4326;
            if (code?.Name == CoordinateSystems.NameSmartMeters) SmartMetersEPSG = code.Epsg ?? 4326;
            if (code?.Name == CoordinateSystems.NameWorkOrders) WorkOrdersEPSG = code.Epsg ?? 4326;
        }

        // Lat/Lng
        var location = taskLocation.Result;
        Latitude = location?.Latitude ?? 0;
        Longitude = location?.Longitude ?? 0;


        // TimeZone
        TimeZoneString = taskTimeZone.Result;

        return this;
    }

    public int SensorEPSG { get; set; } = 4326;
    public int CustomersEPSG { get; set; } = 4326;
    public int SmartMetersEPSG { get; set; } = 4326;
    public int WorkOrdersEPSG { get; set; } = 4326;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string TimeZoneString { get; set; } = string.Empty;


}

public enum UnitSystem
{
    US, SI
}