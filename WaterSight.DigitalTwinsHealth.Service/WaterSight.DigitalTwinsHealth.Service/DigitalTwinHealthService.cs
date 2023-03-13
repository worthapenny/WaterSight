using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.Json;
using System.Timers;
using System.Xml.Linq;
using WaterSight.DigitalTwinsHealth.Service.Support;

namespace WaterSight.DigitalTwinsHealth.Service;

#region Enum
public enum ServiceState
{
    SERVICE_STOPPED = 0x00000001,
    SERVICE_START_PENDING = 0x00000002,
    SERVICE_STOP_PENDING = 0x00000003,
    SERVICE_RUNNING = 0x00000004,
    SERVICE_CONTINUE_PENDING = 0x00000005,
    SERVICE_PAUSE_PENDING = 0x00000006,
    SERVICE_PAUSED = 0x00000007,
}
#endregion

#region Service Status Struct
[StructLayout(LayoutKind.Sequential)]
public struct ServiceStatus
{
    public int dwServiceType;
    public ServiceState dwCurrentState;
    public int dwControlsAccepted;
    public int dwWin32ExitCode;
    public int dwServiceSpecificExitCode;
    public int dwCheckPoint;
    public int dwWaitHint;
};
#endregion

public partial class DigitalTwinHealthService : ServiceBase
{
    #region Constructor
    public DigitalTwinHealthService()
    {
        InitializeComponent();
    }
    #endregion

    #region Public Methods
    public bool StartApplication()
    {
        var success = true;
        EventLog.WriteEntry(Options.Name, $"About to start the application '{Options.Name}'.", EventLogEntryType.Information, EventId++);





        EventLog.WriteEntry(Options.Name, $"Application '{Options.Name}' started.", EventLogEntryType.Information, EventId++);
        return success;
    }
    #endregion

    #region Platform Invoke
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    #endregion

    #region Protected Methods
    protected override void OnStart(string[] args)
    {
        var message = string.Empty;

        //
        // Update the service state to Start Pending.
        ServiceStatus serviceStatus = new ServiceStatus();
        serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
        serviceStatus.dwWaitHint = 100000;
        SetServiceStatus(this.ServiceHandle, ref serviceStatus);

        // Read options from AppSettings
        if (!Options.Load())
        {
            message = $"The options couldn't be loaded. See log file for more details.";
            Log.Information(message);
            EventLog.WriteEntry(Options.Name, $"Failed to start application '{Options.Name}'. Message: {message}.", EventLogEntryType.Error, EventId++);
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        // update Event Logs
        message = $"Done reading AppConfig.\nSettings:\n{Options.ToJsonString()}";
        EventLog.WriteEntry(Options.Name, message, EventLogEntryType.Information, EventId++);

        //
        // Prepare Logging
        //
        Logging.SetupLogger(Options);
        Log.Information(message);
        EventLog.WriteEntry(Options.Name, $"Logging service on '{Options.Name}' started. Log file path: {Logging.GetLogFilePath(Options.Name)}.", EventLogEntryType.Information, EventId++);


        Timer.Interval = Options.UpdateIntervalMinutes * 60 * 1000;
        Timer.Elapsed += (s, e) => StartApplication();
        Timer.Start();
        EventLog.WriteEntry(Options.Name, $"Intervallic timer started.", EventLogEntryType.Information, EventId++);

        //
        // Star the application right away too
        var started = StartApplication();
        if (!started)
        {
            Log.Information($"The core application failed to start. See log file for more details.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }


        //
        // Update the service state to Running.
        //
        serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
        SetServiceStatus(this.ServiceHandle, ref serviceStatus);
    }

    protected override void OnStop()
    {
        var message = $"About to stop the server on '{Options.Name}'";
        EventLog.WriteEntry(Options.Name, message, EventLogEntryType.Information, EventId++);
        Log.Information(message);

        // Update the service state to Stop Pending.
        ServiceStatus serviceStatus = new ServiceStatus();
        serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
        serviceStatus.dwWaitHint = 100000;
        SetServiceStatus(this.ServiceHandle, ref serviceStatus);

        Timer.Elapsed -= (s, e) => StartApplication();
        Timer.Stop();
        Timer.Dispose();

        // Update the service state to Stopped.
        message = $"Application '{Options.Name}' stopped.";
        EventLog.WriteEntry(Options.Name, message, EventLogEntryType.Information, EventId++);
        Log.Information(message);

        serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
        SetServiceStatus(this.ServiceHandle, ref serviceStatus);
    }
    #endregion

    #region Public Properties
    public DigitalTwinHealthServiceOptions Options { get; } = new DigitalTwinHealthServiceOptions();
    #endregion

    #region Private Properties
    private int EventId { get; set; } = 42800;
    private Timer Timer { get; } = new Timer(24 * 60 * 60 * 1000);
    #endregion
}

public class DigitalTwinHealthServiceOptions
{
    #region Constructor
    public DigitalTwinHealthServiceOptions()
    {
    }
    #endregion

    #region Public Methods
    public bool Load()
    {
        var success = true;

        try
        {
            UpdateIntervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(UpdateIntervalMinutes)]);
            LogEventLevel = (LogEventLevel)Convert.ToInt32(ConfigurationManager.AppSettings[nameof(LogEventLevel)]);
            LogTemplate = ConfigurationManager.AppSettings[nameof(LogTemplate)];
            Environment = (WaterSightEnvironment)Convert.ToInt16(ConfigurationManager.AppSettings[nameof(Environment)]);
            
        }
        catch (Exception ex)
        {
            var message = $"...while parsing data from appConfig file";
            Log.Error(ex, message);
            success = false;
        }

        Log.Information($"Information loaded from appConfig file");
        Log.Debug(new string('-', 100));
        return success;
    }
    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this);
    }
    #endregion

    #region Public Properties
    public int UpdateIntervalMinutes { get; set; } = 24 * 60;
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Information;
    public string LogTemplate { get; set; } = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
    public string Name { get; } = "WaterSightDTsHealthMonitor";
    public WaterSightEnvironment Environment { get; set; } = WaterSightEnvironment.Prod;


    #endregion


}

public enum WaterSightEnvironment
{
    Prod = 0,
    QA = 1,
    Dev = 2
}