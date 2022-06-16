using System;
using System.Diagnostics;

namespace WaterSight.Web.Core;

[DebuggerDisplay("{ToString()}")]
public class Options
{
    #region Constructor
    public Options(
        int digitalTwinId,
        //IConfiguration config, 
        string tokenRegistryPath,
        Env env = Env.Prod)
    {
        DigitalTwinId = digitalTwinId;
        //Configuration  = config;
        TokenRegistryPath = tokenRegistryPath;
        Env = env;


        //Logger.Information("Options initialized");
    }
    #endregion

    #region Public Properties
    public Env Env { get; private set; } = Env.Prod;
    public int DigitalTwinId { get; set; }
    public int? EPSGCode { get; set; }
    public DateTimeOffset StartAt { get; set; } = DateTimeOffset.UtcNow.AddDays(-30);
    public DateTimeOffset EndAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset StartAtModel { get; set; }
    public DateTimeOffset EndAtModel { get; set; }
    public string StartMonth { get; set; } = DateTimeOffset.UtcNow.AddDays(-30).ToString("yyyy-MM");
    public string EndMonth { get; set; } = DateTimeOffset.UtcNow.ToString("yyyy-MM");
    public TimeZoneInfo LocalTimeZone { get; set; } = TimeZoneInfo.Local;

    #endregion

    #region Public Static Properties
    //public static string TokenRegistryPath { get; private set; } = "";
    public string TokenRegistryPath { get; private set; } = "";
    //public static ILogger Logger { get; private set; }
    //public static IConfiguration Configuration { get; private set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DigitalTwinId} [{Env}] [{StartAt}, {EndAt}], Tz={LocalTimeZone}";
    }
    #endregion
}

