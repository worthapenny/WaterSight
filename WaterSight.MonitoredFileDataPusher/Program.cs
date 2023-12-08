using Serilog;
using WaterSight.MonitoredFileDataPusher.Service;
using WaterSight.MonitoredFileDataPusher.Support;

namespace WaterSight.MonitoredFileDataPusher;

public class Program
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        // redirect console output to parent process;
        // must be before any calls to Console.WriteLine()
        AttachConsole(ATTACH_PARENT_PROCESS);
        Console.WriteLine("About to setup logger");

        //
        // Logging
        //
        string logTemplate = "{Timestamp:dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: logTemplate)
            .CreateLogger();

        Log.Information("Logging is read!");

        //
        // Load Options
        //
        var exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string jsonFilePath = Path.Combine(exeDirectory, "Options.json");

        var options = Options.LoadFromFile(jsonFilePath);
        if (options == null)
        {
            Log.Fatal($"Options can't be loaded. Check previous logs to troubleshoot");
            return -1;
        }
        Options = options;

        //
        // Start Monitoring
        //
        var monitoringService = new FileMonitoringService(options.GetFilePath());
        monitoringService.ChangeDetected += (s, e) => ChangeDetected(e);
        monitoringService.StartMonitoring();

        // keep the main thread alive
        Console.ReadLine();

        return 0;
    }

    private static void ChangeDetected(FileSystemEventArgs e)
    {
        Log.Information($"Change detected, type = {e.ChangeType} for file: {e.Name}");

        var dataFilePath = e.FullPath;
        if (dataFilePath.EndsWith(".xlsx"))
        {
            Log.Debug($"XLSX file detected.");
            //ExcelFile.ExtractScadaData(
            //    dataFilePath: dataFilePath,
            //    options: Options);
        }
        else
        {
            Log.Warning($"No supported file detected to extract the data!");
        }
    }

    #region Private static Properties
    private static Options Options { get; set; }
    #endregion
}