using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Formatting;
using Serilog.Sinks.SystemConsole.Themes;
using System.Collections.Concurrent;

namespace WaterSight.UI.Support.Logging;

public static class Logging
{
    public static void SetupLogger(
        LogEventLevel logEventLevel = LogEventLevel.Information,
        string logTemplate = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}")
    {
        var logFileDir = Path.Join(Path.GetTempPath(), "__WaterSight.UI");
        logEventLevel = LogEventLevel.Debug;

        if(!Directory.Exists(logFileDir))
            Directory.CreateDirectory(logFileDir);

        var genericLogFilePath = Path.Combine(logFileDir, "WaterSight.UI..log");

        Logger = new LoggerConfiguration()
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

        Log.Logger = Logger;
        LoggingLevelSwitch.MinimumLevel = logEventLevel;

        Log.Information(new string('█', 100));
        Log.Debug($"Logger is ready. Path: {genericLogFilePath}");
    }


    #region Public Static Properties
    public static ILogger Logger { get; private set; } 
    public static InMemorySink InMemorySink { get; private set; } = new InMemorySink();
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new LoggingLevelSwitch();
    #endregion

    #region Private Properties
    #endregion

}



public class InMemorySink : ILogEventSink
{
    readonly ITextFormatter _textFormatter = new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}");

    public ConcurrentQueue<string> Events { get; } = new ConcurrentQueue<string>();
    public event EventHandler<string>? Logged;

    public void Emit(LogEvent logEvent)
    {
        if (logEvent != null && Logged != null)
        {
            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            Events.Enqueue(renderSpace.ToString());

            Logged.Invoke(this, renderSpace.ToString());
        }
    }
}