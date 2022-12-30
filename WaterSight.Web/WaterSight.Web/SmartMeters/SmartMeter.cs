using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Extensions;
using WaterSight.Web.Support;
using Ganss.Excel;

namespace WaterSight.Web.SmartMeters;

public class SmartMeter: WSItem
{
    #region Constructor
    public SmartMeter(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Smart Meter CRUD Operation
    //https://connect-watersight.bentley.com/api/v1/SmartMeters/SmartMeters?digitalTwinId=238
    // Delete: https://connect-watersight.bentley.com/api/v1/SmartMeters/SmartMeters/134283264?digitalTwinId=238

    #endregion

    #endregion
}

#region Model Classes

[DebuggerDisplay("{ToString()}")]
public class SmartMeterConfig
{
    #region Public Properties
    public int ConsumptionPointId { get; set; }
    public int SignalId { get; set; }
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string ParameterType { get; set; }
    public string Units { get; set; }
    public int RegistrationFrequency { get; set; }
    public int CommunicationFrequency { get; set; }
    public object Address { get; set; }
    public object Diameter { get; set; }
    public string DiameterUnits { get; set; }
    public string InstallationDate { get; set; }
    public DateTime? DeactivationDate { get; set; } = null;
    public string? MeterTypeAsString { get; set; } = null;
    public int MeterType { get; set; } = 1;
    public string? ConsumptionTypeAsString { get; set; } = null;
    public int ConsumptionType { get; set; } = 1;
    public string IsCriticalAsString { get; set; } = "No";
    public bool IsCritical { get; set; } = false;
    public string IsLargeAsString { get; set; } = "No";
    public bool IsLarge { get; set; } = false;
    public double? Latitude { get; set; } = 0.0;
    public double? Longitude { get; set; } = 0.0;
    public object ZoneName { get; set; }
    public string Tags { get; set; } = "";
    public DateTimeOffset? LastInstantInDatabase { get; set; } = null;
    public int? PatternWeekId { get; set; } = null;
    public string UtcOffSet { get; set; } = "00:00:00";
    public bool DiscardFromMassBalance { get; set; } = true;
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"[{ParameterType}] {ConsumptionPointId}: {Name}, Tag:{SignalId}";
    }
    #endregion
}
#endregion