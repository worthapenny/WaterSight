using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.Json;
using System.Timers;
using System.Xml.Linq;
using WaterSight.DigitalTwinsHealth.Service.Support;
using WaterSight.Web.Core;
using WaterSight.Web.Landings;

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

        // make sure the digital twin config status json has valid path
        if (!File.Exists(Options.DTConfigStatsJsonFilePath))
        {
            message = $"Manually updated JSON file could not be located. Given path: {Options.DTConfigStatsJsonFilePath}";
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


        Timer.Interval = Options.RunApplicationEveryMinutes * 60 * 1000;
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

public class DigitalTwinManualStat
{
    #region Public Properties
    public int Id { get; set; }
    public string DTName { get; set; }
    public string Owner { get; set; }
    public string CoOwner { get; set; }
    public string Location { get; set; }
    public string StartDate { get; set; }
    public string IsActive { get; set; }
    public string IsPaid { get; set; }
    public int? NumberOfCustomers { get; set; }
    #endregion
}

public class DigitalTwinHealthModel
{
    #region Enum
    public enum ConfigurationStatus
    {
        [Description("Not Configured")]
        None = 0,

        [Description("Poorly Configured")]
        Poor = 1,

        [Description("Can be Improved")]
        Refine = 2,

        [Description("Well Configured")]
        Good = 3
    }
    #endregion

    #region Constructor
    public DigitalTwinHealthModel()
    {

    }
    #endregion

    #region Public Methods
    public void InitializeFields(Home home)
    {

    }
    public void CalculateStatus()
    {
        // Network Monitoring
        NetworkMonitoringStatus = ConfigurationStatus.None;
        if (SensorCountTotal > 0)
        {
            var okDataPercent = SensorCountOK / SensorCountTotal * 100;
            var noDataPercent = SensorCountNoData / SensorCountTotal * 100;
            var partialDataPercent = SensorCountPartialData / SensorCountTotal * 100;

            if (noDataPercent > 0.5) NetworkMonitoringStatus = ConfigurationStatus.Poor;
            else if (partialDataPercent > 0.5) NetworkMonitoringStatus = ConfigurationStatus.Refine;
            else if (okDataPercent > 0.5) NetworkMonitoringStatus = ConfigurationStatus.Good;
        }

        // Smart Meter
        SmartMeterMonitoringStatus = ConfigurationStatus.None;
        if (SmartMeterCountTotal > 0)
        {
            var okDataPercent = SmartMeterCountOK / SmartMeterCountTotal * 100;
            var noDataPercent = SmartMeterCountNoData / SmartMeterCountTotal * 100;
            var partialDataPercent = SmartMeterCountPartialData / SmartMeterCountTotal * 100;

            if (noDataPercent > 0.5) SmartMeterMonitoringStatus = ConfigurationStatus.Poor;
            else if (partialDataPercent > 0.5) SmartMeterMonitoringStatus = ConfigurationStatus.Refine;
            else if (okDataPercent > 0.5) SmartMeterMonitoringStatus = ConfigurationStatus.Good;
        }

        // Tanks
        TanksStatus = TankCount > 0 ? ConfigurationStatus.Good : ConfigurationStatus.Poor;

        // Pumps
        PumpsStatus = ConfigurationStatus.None;
        if(PumpCount > 0)
        {
            PumpsStatus = PumpsBelowEfficiencyCount > 0 ? ConfigurationStatus.Good : ConfigurationStatus.Poor;
        }

        // Water Audit
        WaterAuditStatus = ConfigurationStatus.None;
        if(PressureZoneCount > 0 && WaterLoss > 0 && WaterLossCost > 0)
        {
            WaterAuditStatus = ConfigurationStatus.Good;
        }

        // Alerts
        EventManagementStatus = ConfigurationStatus.None;
        if(EventsDefinedCount > 0 && EventsGeneratedCount > 0)
        {
            EventManagementStatus = ConfigurationStatus.Good;
        }
        else if (EventsDefinedCount > 0 && EventsGeneratedCount == 0)
        {
            EventManagementStatus = ConfigurationStatus.Refine;
        }

        // Modeling
        ModelingStatus = ConfigurationStatus.None;
        if (IsModelUploaded)
        {
            var simFailRate = SimulationRunFailedCountTotal / SimulationRunSuccessCountTotal;
            if (simFailRate > 0 && simFailRate <= 0.05) ModelingStatus = ConfigurationStatus.Good;
            else if (simFailRate > 0.05 && simFailRate <= 0.3) ModelingStatus = ConfigurationStatus.Refine;
            else if (simFailRate > 0.3) ModelingStatus = ConfigurationStatus.Poor;
        }


        // Power BI
        PowerBIStatus = PowerBiReportCount > 0 ? ConfigurationStatus.Good : ConfigurationStatus.None;


        // CAP
        CapitalPlanningStatus = ConfigurationStatus.None;


    }
    #endregion

    #region Public Properties
    public string Name { get; set; }
    public string Location { get; set; }
    public string Owner { get; set; }
    public string CoOwner { get; set; }
    public int DtID { get; set; }
    public string StartDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsPaid { get; set; }
    public int? NumberOfCustomers { get; set; }


    public ConfigurationStatus NetworkMonitoringStatus { get; private set; } = ConfigurationStatus.None;
    public int SensorCountTotal { get; set; }
    public int SensorCountOK { get; set; }
    public int SensorCountNoData { get; set; }
    public int SensorCountPartialData { get; set; }
    public int SensorCountOffline { get; set; }

    public ConfigurationStatus SmartMeterMonitoringStatus { get; private set; } = ConfigurationStatus.None;
    public int SmartMeterCountTotal { get; set; }
    public int SmartMeterCountOK { get; set; }
    public int SmartMeterCountNoData { get; set; }
    public int SmartMeterCountPartialData { get; set; }
    public int SmartMeterCountOffline { get; set; }


    public ConfigurationStatus TanksStatus { get; private set; } = ConfigurationStatus.None;
    public int TankCount { get; set; }


    public ConfigurationStatus PumpsStatus { get; private set; } = ConfigurationStatus.None;
    public int PumpCount { get; set; }
    public int PumpsBelowEfficiencyCount { get; set; }


    public ConfigurationStatus WaterAuditStatus { get; private set; } = ConfigurationStatus.None;
    public int PressureZoneCount { get; set; } = 0;
    public double WaterLoss { get; set; }
    public double WaterLossCost { get; set; }


    public ConfigurationStatus EventManagementStatus { get; private set; } = ConfigurationStatus.None;
    public int EventsDefinedCount { get; set; }
    public int EventsGeneratedCount { get; set; }


    public ConfigurationStatus ModelingStatus { get; private set; } = ConfigurationStatus.None;
    public bool IsModelUploaded { get; set; }
    public int SimulationRunSuccessCountTotal { get; set; }
    public int SimulationRunFailedCountTotal { get; set; }


    public ConfigurationStatus PowerBIStatus { get; private set; } = ConfigurationStatus.None;
    public int PowerBiReportCount { get; set; }


    public ConfigurationStatus CapitalPlanningStatus { get; private set; } = ConfigurationStatus.None;

    public DateTimeOffset ReportDateTime { get; } = DateTimeOffset.Now;
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
            RunApplicationEveryMinutes = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(RunApplicationEveryMinutes)]);
            LogEventLevel = (LogEventLevel)Convert.ToInt32(ConfigurationManager.AppSettings[nameof(LogEventLevel)]);
            LogTemplate = ConfigurationManager.AppSettings[nameof(LogTemplate)];
            Environment = (WaterSightEnvironment)Convert.ToInt16(ConfigurationManager.AppSettings[nameof(Environment)]);
            DTConfigStatsJsonFilePath = ConfigurationManager.AppSettings[nameof(DTConfigStatsJsonFilePath)];
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
    public int RunApplicationEveryMinutes { get; set; } = 24 * 60;
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Information;
    public string LogTemplate { get; set; } = "{Timestamp:HH:mm:ss.ff} | {Level:u3} | {Message}{NewLine}{Exception}";
    public string Name { get; } = "WaterSightDTsHealthMonitor";
    public Env Environment { get; set; } = Env.Prod;

    public string DTConfigStatsJsonFilePath { get; set; }
    #endregion


}

//public enum WaterSightEnvironment
//{
//    Prod = 0,
//    QA = 1,
//    Dev = 2
//}