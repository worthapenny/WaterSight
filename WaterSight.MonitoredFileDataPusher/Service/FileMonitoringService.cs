using Serilog;

namespace WaterSight.MonitoredFileDataPusher.Service;


public class FileMonitoringService
{
    #region Public Events
    public event EventHandler<FileSystemEventArgs>? ChangeDetected;
    #endregion

    #region Constructor
    public FileMonitoringService(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        Log.Debug($"Given file to monitor: {filePath}");
        var folderPath = new FileInfo(filePath).Directory?.FullName;

        if (folderPath == null)
            throw new DirectoryNotFoundException(folderPath);

        MonitoredFolderPath = folderPath;
        Log.Information($"Monitoring service to folder path is ready but not started. Path: {folderPath}");

        FileSystemWatcher = new FileSystemWatcher(folderPath);
        FileSystemWatcher.IncludeSubdirectories = false;
        FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;

        FileSystemWatcher.Changed += (s, e) => ChangeDetected?.Invoke(s, e);
    }
    #endregion

    #region Public Methods
    public void StartMonitoring()
    {
        if (MonitoringTask == null || MonitoringTask.IsCompleted)
        {
            FileSystemWatcher.EnableRaisingEvents = true;
            IsMonitoring = true;
            Log.Information($"Monitoring started for path: {MonitoredFolderPath}");
        }
        else if (MonitoringTask?.Status == TaskStatus.Running)
        {
            FileSystemWatcher.EnableRaisingEvents = true;

            IsMonitoring = true;
            Log.Information($"Monitoring started for path: {MonitoredFolderPath}");
        }
        else
        {
            IsMonitoring = false;
            Log.Information($"Monitoring process is either already running or couldn't be started for path: {MonitoredFolderPath}");
        }
    }

    public void StopMonitoring()
    {
        FileSystemWatcher.EnableRaisingEvents = false;
        IsMonitoring = false;
        Log.Information($"Monitoring stopped for path: {MonitoredFolderPath}");
    }
    #endregion



    #region Public Properties
    public string? MonitoredFolderPath { get; }
    public bool IsMonitoring { get; private set; } = false;
    public FileSystemWatcher FileSystemWatcher { get; }
    public Task? MonitoringTask { get; private set; }
    #endregion
}


