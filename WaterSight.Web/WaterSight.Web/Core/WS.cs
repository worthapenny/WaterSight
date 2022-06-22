using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using WaterSight.Web.DT;
using WaterSight.Web.HydrulicStructures;
using WaterSight.Web.NumericModels;
using WaterSight.Web.User;
using WaterSight.Web.Zones;

namespace WaterSight.Web.Core;

/// <summary>
/// The main access point for the WaterSight Web API
/// </summary>
public class WS
{

    #region Constructor
    public WS(string tokenRegistryPath, int digitalTwinId = -1, Env env = Env.Prod, ILogger? logger = null)
    {
        Options = new Options(digitalTwinId, tokenRegistryPath, env);
        Request.options = Options;
        EndPoints = new EndPoints(Options);

        var logFileFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\WS.Base_.log");

        var fileInfo = new FileInfo(logFileFile);
        if (!(fileInfo.Directory?.Exists ?? false))
            fileInfo.Directory?.Create();

        DigitalTwin = new DigitalTwin(this);
        Sensor = new Sensors.Sensor(this);
        Alert = new Alerts.Alert(this);
        GIS = new GIS.GIS(this);
        HydStructure = new HydStructure(this);
        Zone = new Zones.Zone(this);
        NumericModel = new NumericModel(this);
        Customers = new Customers.Customers(this);
        Settings = new Settings.Settings(this);
        UserInfo = new UserInfo(this);
        Setup = new Setup.Setup(this);

        if (logger == null)
        {
            var logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.MinimumLevel.Verbose()
                .MinimumLevel.Debug()
                .WriteTo.Debug(outputTemplate: logTemplate)
                .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    logFileFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    flushToDiskInterval: TimeSpan.FromSeconds(5),
                    outputTemplate: logTemplate)
                .CreateLogger();
        }

        WS.Logger = Log.Logger;

        Log.Debug("");
        Log.Debug($"Logging is ready. Path: {logFileFile}");
    }
    #endregion

    #region Public Static Methods
    #endregion

    #region Public Properties
    public Options Options { get; }
    public EndPoints EndPoints { get; }
    public static ILogger Logger { get; private set; } // = new LoggerConfiguration().CreateLogger();

    public DigitalTwin DigitalTwin { get; }
    public Sensors.Sensor Sensor { get; }
    public Alerts.Alert Alert { get; }
    public GIS.GIS GIS { get; }
    public HydStructure HydStructure { get; }
    public Zone Zone { get; }
    public NumericModel NumericModel { get; }
    public Customers.Customers Customers { get; }
    public Settings.Settings Settings { get; }
    public UserInfo UserInfo { get; }
    public Setup.Setup Setup { get; }
    #endregion
}
