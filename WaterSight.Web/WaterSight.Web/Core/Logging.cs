using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Linq;

namespace WaterSight.Web.Core;


public static class Logging
{
    #region Public Static Methods
    public static void SetupLogger(
        string appName = "UnknownProject",
        string logTemplate = "",
        LogEventLevel logEventLevel = LogEventLevel.Information)
    {
        
#if DEBUG
        logEventLevel = LogEventLevel.Debug;
#endif

        var genericLogFilePath = Path.Combine(GetLogFileDirectoryInfo(appName).FullName, $"{appName}..Log");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch) // Default Level = Information 
            .WriteTo.Debug(outputTemplate: logTemplate)
            .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                genericLogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                flushToDiskInterval: TimeSpan.FromSeconds(5),
                outputTemplate: logTemplate)
            .CreateLogger();

        LoggingLevelSwitch.MinimumLevel = logEventLevel;

        Log.Information(new string('█', 100));
        Log.Debug($"Logger is ready. Path: {genericLogFilePath}");
    }

    public static string GetLogFilePath(string appName)
    {
        var logDirectoryInfo = GetLogFileDirectoryInfo(appName);
        if (!logDirectoryInfo.Exists)
            return String.Empty;

        var logFilePath = logDirectoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
        Console.WriteLine($"Latest modified log file: {logFilePath.FullName}");
        Log.Information($"Latest modified log file: {logFilePath.FullName}");

        return logFilePath.FullName;
    }
    #endregion

    #region Private Methods
    public static DirectoryInfo GetLogFileDirectoryInfo(string appName)
    {
        var logDirectory = Path.Combine(Path.GetTempPath(), $"__{appName}");
        var logDirectoryInfo = new DirectoryInfo(logDirectory);

        if (!logDirectoryInfo.Exists) logDirectoryInfo.Create();
        return logDirectoryInfo;
    }
    #endregion

    #region Public Static Properties
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();
    #endregion

    #region Private Properties
    #endregion
}