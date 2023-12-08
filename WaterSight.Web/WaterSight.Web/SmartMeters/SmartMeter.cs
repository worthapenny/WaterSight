using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Support;

namespace WaterSight.Web.SmartMeters;

public class SmartMeter : WSItem
{

    #region Constructor
    public SmartMeter(WS ws) : base(ws, "Smart meter")
    {
    }
    #endregion

    #region Public Methods

    #region Smart Meter CRUD Operation

    //
    // CREATE
    public async Task<SmartMeterConfig?> AddSmartMeterConfigAsync(SmartMeterConfig config)
    {
        // Post: https://connect-watersight.bentley.com/api/v1/SmartMeter/SmartMeter?digitalTwinId=308
        var url = EndPoints.SmartMetersSmartMetersQDT;
        int? id = await WS.AddAsync<int?>(config, url, Name);
        if (id.HasValue)
        {
            config.ConsumptionPointId = id.Value;
            return config;
        }

        return null;
    }



    // 
    // READ
    public async Task<SmartMeterConfig> GetSmartMeterConfigAsync(int consumptionPointId)
    {
        // GET https://connect-watersight.bentley.com/api/v1/SmartMeter/SmartMeter?digitalTwinId=308&externalIdFilter=&limit=10&offset=0&epsg=2039&nameFilter=&isCritical=&isLarge=&sortByLastInstantinDb=false
        var url = EndPoints.SmartMetersSmartMetersForId(consumptionPointId);
        SmartMeterConfig? config = await WS.GetAsync<SmartMeterConfig>(url, consumptionPointId, Name);
        return config;
    }



    //
    // Update
    public async Task<bool> UpdateSmartMeterConfigAsync(SmartMeterConfig config)
    {
        // PUT https://connect-watersight.bentley.com/api/v1/SmartMeter/SmartMeter/152109056?digitalTwinId=308
        var url = EndPoints.SmartMetersSmartMetersForId(config.ConsumptionPointId);
        return await WS.UpdateAsync(config.ConsumptionPointId, config, url, Name);
    }



    //
    // DELETE
    public async Task<bool> DeleteSmartMeterConfigAsync(int consumptionPointId)
    {
        // Delete: https://connect-watersight.bentley.com/api/v1/SmartMeter/SmartMeter/134283264?digitalTwinId=238
        var url = EndPoints.SmartMetersSmartMetersForId(consumptionPointId);
        return await WS.DeleteAsync(consumptionPointId, url, Name);
    }

    #endregion

    #region Import Excel 
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload Excel file for {Name}.");

        // POST:  https://connect-watersight.bentley.com/api/v1/SmartMeter/SmartMetersFromFile?digitalTwinId=308&actionId=http://watersight.bentley.com/administration/smart-meters#smartMetersImport
        var url = EndPoints.SmartMetersSmartMetersFromFileQDT;
        var success = await WS.PostFile(url, fileInfo, false, $"{Name} - Excel");

        Logger.Debug(Util.LogSeparatorSquare);
        return success;
    }
    #endregion

    #endregion
}

#region Enums
public enum ConsumptionTypeEnum
{
    Domestic = 0,
    Industrial = 1,
    NotDefined = 2,
    Other = 3,
    Services = 4
}

public enum MeterTypeEnum
{
    Velocity = 0,
    NotDefined = 1,
    Volumetric = 2
}

public enum ParameterTypeEnum
{
    [Description("Flow")]
    Flow = 0,

    [Description("Volume")]
    Volume = 1,
}
#endregion

#region Model Classes

[DebuggerDisplay("{ToString()}")]
public class SmartMeterConfig
{
    #region Public Properties
    public string? Address { get; set; } = string.Empty;
    public int CommunicationFrequency { get; set; } = 15;
    public int ConsumptionPointId { get; set; } = 0;
    public int ConsumptionType { get; set; } = (int)ConsumptionTypeEnum.NotDefined;
    public string? ConsumptionTypeAsString { get; set; } = string.Empty;
    public DateTime? DeactivationDate { get; set; } = null;
    public object Diameter { get; set; } = null;
    public string DiameterUnits { get; set; } = string.Empty;
    public bool DiscardFromMassBalance { get; set; } = true;
    public string ExternalId { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; } = new DateTime(1999, 9, 9);
    public bool IsCritical { get; set; } = false;
    public string IsCriticalAsString { get; set; } = "No";
    public bool IsLarge { get; set; } = false;
    public string IsLargeAsString { get; set; } = "No";
    public DateTimeOffset? LastInstantInDatabase { get; set; } = null;
    public double? Latitude { get; set; } = 0.0;
    public double? Longitude { get; set; } = 0.0;
    public int MeterType { get; set; } = (int)MeterTypeEnum.Volumetric;
    public string? MeterTypeAsString { get; set; } = MeterTypeEnum.Volumetric.ToString();
    public string Name { get; set; } = string.Empty;
    public string ParameterType { get; set; } = ParameterTypeEnum.Flow.ToString();
    public int? PatternWeekId { get; set; } = null;
    public int Priority { get; set; } = 1;
    public int RegistrationFrequency { get; set; } = 15;
    public int SignalId { get; set; } = 0;
    public string Tags { get; set; } = string.Empty;
    public string Units { get; set; } = "gpd";
    public string UtcOffSet { get; set; } = "00:00:00";
    public string ZoneName { get; set; } = string.Empty;

    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{ParameterType}] {ConsumptionPointId}: {Name}, Tag:{SignalId}";
    }
    #endregion
}
#endregion