namespace WaterSight.WaterSightDataCollector;

public class AppConstants
{
    public static string DigitalTwinName { get; set; } = string.Empty;

    public static string AppDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _appName);
    public static string DigitalTwinDir => Path.Combine(AppDir, DigitalTwinName);
    public static string AppLogDir => Path.Combine(DigitalTwinDir, "Logs");


    private static string _appName = "WaterSightDataCollector";
}
