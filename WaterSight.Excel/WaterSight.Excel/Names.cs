using System.IO;

namespace WaterSight.Excel;

public class FileNames
{
    #region Constructor
    public FileNames(string setupDir)
    {
        ExcelFileNames = new ExcelFileNames(setupDir: setupDir);
        CsvFileNames = new CsvFileNames(setupDir: setupDir);
    }
    public FileNames(ExcelFileNames excelFileName, CsvFileNames csvFileName)
    {
        ExcelFileNames = excelFileName;
        CsvFileNames = csvFileName;
    }
    #endregion

    public ExcelFileNames ExcelFileNames { get; set; }
    public CsvFileNames CsvFileNames { get; set; }
}

public class CsvFileNames
{
    public CsvFileNames(
        string setupDir,
        string consumption = "Consumptions.csv")
    {
        SetupDir = setupDir;
        Consumptions = Path.Combine(SetupDir, consumption);
    }

    public string SetupDir { get; set; }
    public string Consumptions { get; set; }
}

public class ExcelFileNames
{
    public ExcelFileNames(
        string setupDir,
        string sensorsName = "Sensors.xlsx",
        string pumpsName = "Pumps.xlsx",
        string tanksName = "Tanks.xlsx",
        string consumption = "ConsumptionMeter.xlsx",
        string zonesName = "Zones.xlsx")
    {
        SetupDir = setupDir;
        SensorsExcelPath = Path.Combine(setupDir, sensorsName);
        PumpsExcelPath = Path.Combine(setupDir, pumpsName);
        TanksExcelPath = Path.Combine(setupDir, tanksName);
        CustomerMeterExcelPath = Path.Combine(setupDir, consumption);
        ZonesExcelPath = Path.Combine(SetupDir, zonesName);
    }


    public string SensorsExcelPath { get; }
    public string PumpsExcelPath { get; }
    public string TanksExcelPath { get; }
    public string CustomerMeterExcelPath { get; }
    public string ZonesExcelPath { get; }

    private string SetupDir { get; }
}


public struct ExcelSheetName
{
    public const string Sensors = "Sensors";
    public const string Tanks = "Tanks";
    public const string TankCurves = "Tank curves";
    public const string Pumps = "Pumps";
    public const string PumpCurves = "Pump Curves";
    public const string PumpStations = "Pumping Stations";
    public const string Zone = "Zone Balance";
    public const string ZoneCharacteristics = "Zones Characteristics";
    public const string CustomerMeters = "Customer meters";
}
