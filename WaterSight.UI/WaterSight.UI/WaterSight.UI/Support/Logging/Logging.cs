using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.SystemConsole.Themes;

namespace WaterSight.UI.Support.Logging;

public static class Logging
{
    public static void SetupLogger(
        LogEventLevel logEventLevel = LogEventLevel.Information,
        string logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}")
    {
        // Get the configurations
        var config = App.GetConfiguration();
        var loggingSection = config.GetSection("Logging");
        var logLevelStr = loggingSection.GetSection("LogEventLevel").Value;
        var logTemplateStr = loggingSection.GetSection("LogTemplate").Value;
        if (!string.IsNullOrEmpty(logLevelStr))
        {
            try
            {
                logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevelStr);
            }
            catch { }
        }
        else
        {
            logEventLevel = LogEventLevel.Information;

        }

#if DEBUG
            logEventLevel = LogEventLevel.Debug;
#endif

        // Log file path
        var logFileDir = loggingSection.GetSection("LogFileDir").Value;
        if(string.IsNullOrEmpty(logFileDir))
            logFileDir = Path.Join(Path.GetTempPath(), "__WaterSight.UI");

        if(!Directory.Exists(logFileDir))
            Directory.CreateDirectory(logFileDir);

        var genericLogFilePath = Path.Combine(logFileDir, "WaterSight.UI..log");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch) // Default's to Information 
            .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
            .WriteTo.Debug(outputTemplate: logTemplate)
            .WriteTo.File(
                genericLogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                flushToDiskInterval: TimeSpan.FromSeconds(5),
                outputTemplate: logTemplate)
            .WriteTo.Sink(
                InMemorySink,
                logEventLevel,
                LoggingLevelSwitch).CreateLogger();

        LoggingLevelSwitch.MinimumLevel = logEventLevel;

        Log.Information(new string('█', 100));
        Log.Debug($"Logger is ready. Path: {genericLogFilePath}");
    }


    #region Public Static Properties
    public static InMemorySink InMemorySink { get; private set; } = new InMemorySink();
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();
    #endregion

    #region Private Properties
    #endregion

}


