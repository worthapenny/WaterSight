using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;

namespace WaterSight.WaterSightDataCollector.Support.Logging;


public sealed class Logger

{
    #region Singleton Pattern
    private static readonly Logger _instance = new Logger();
    static Logger() { }
    private Logger() { }
    public static Logger Instance => _instance;
    #endregion

    #region Public Static Methods
    public void SetupLogger()
    {
        Log.Logger = LoggerConfig;
        LogLibrary.Separate_Block();
        Log.Information($"LoggerConfig is ready. Path: {GetLogFilePath()}");
        LogLibrary.Separate_XBig();
    }
    #endregion

    #region Private Methods
    private string GetLogFilePath()
    {
        var logDirectoryInfo = new DirectoryInfo(AppConstants.AppLogDir);
        if (!logDirectoryInfo.Exists)
            return String.Empty;

        var logFilePath = logDirectoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
        return logFilePath.FullName;
    }
    private Serilog.Core.Logger GetLoggerConfiguration()
    {        
        var logTemplateStr = "{Timestamp:dd HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
        var logEventLevel = LogEventLevel.Information;

#if DEBUG
        logEventLevel = LogEventLevel.Debug;
#endif


        var genericLogFilePath = Path.Combine(AppConstants.AppLogDir, $"WSC..Log");

        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch) // Default's to Information 
            .WriteTo.Console(outputTemplate: logTemplateStr, theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                genericLogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                flushToDiskInterval: TimeSpan.FromSeconds(5),
                outputTemplate: logTemplateStr,
                shared: true);

        // Add debug sink if unit test is running 
        if (IsRunningInNUnit())
            loggerConfig.WriteTo.Debug(outputTemplate: logTemplateStr);

        LoggingLevelSwitch.MinimumLevel = logEventLevel;

        var logger = loggerConfig.CreateLogger();
        return logger;
    }
    #endregion

    #region Private Static Methods
    private static bool IsRunningInNUnit()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Any(
            a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

    }
    #endregion

    #region Public Static Properties
    public LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();
    public Serilog.Core.Logger LoggerConfig => _loggerConfig ??= GetLoggerConfiguration();
    #endregion

    #region Private Static Properties
    #endregion

    #region Static Fields
    private Serilog.Core.Logger _loggerConfig;
    #endregion

}
