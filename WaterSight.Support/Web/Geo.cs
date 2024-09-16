using NodaTime;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Support.Web;


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
        Latitude = (float)(location?.Latitude ?? 0);
        Longitude = (float)(location?.Longitude ?? 0);


        // TimeZone
        TimeZoneString = taskTimeZone.Result;

        return this;
    }

    public int SensorEPSG { get; set; } = 4326;
    public int CustomersEPSG { get; set; } = 4326;
    public int SmartMetersEPSG { get; set; } = 4326;
    public int WorkOrdersEPSG { get; set; } = 4326;

    public float Latitude { get; set; }
    public float Longitude { get; set; }

    public string TimeZoneString { get; set; } = string.Empty;
    public DateTimeZone DataTimeZone => DateTimeZoneProviders.Tzdb[TimeZoneString];
}
