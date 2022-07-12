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
    public const string SensorsFileName = "Sensors.xlsx";
    public const string PumpsFileName = "Pumps.xlsx";
    public const string TanksFileName = "Tanks.xlsx";
    public const string ConsumptionFileName = "ConsumptionMeter.xlsx";
    public const string ZonesFileName = "Zones.xlsx";
    public const string AlertsFileName = "Alerts.xlsx";
    public const string PowerBiFileName = "Power BI.xlsx";

    public ExcelFileNames(
        string setupDir,
        string sensorsFileName = SensorsFileName,
        string pumpsFileName = PumpsFileName,
        string tanksFileName = TanksFileName,
        string consumptionFileName = ConsumptionFileName,
        string zonesFileName = ZonesFileName,
        string alertsFileName = AlertsFileName,
        string powerBiFileName = PowerBiFileName)
    {
        SetupDir = setupDir;
        SensorsExcelPath = Path.Combine(setupDir, sensorsFileName);
        PumpsExcelPath = Path.Combine(setupDir, pumpsFileName);
        TanksExcelPath = Path.Combine(setupDir, tanksFileName);
        CustomerMeterExcelPath = Path.Combine(setupDir, consumptionFileName);
        ZonesExcelPath = Path.Combine(SetupDir, zonesFileName);
        AlertsExcelPath = Path.Combine(SetupDir, alertsFileName);
        PowerBIExcelPath = Path.Combine(SetupDir, powerBiFileName);
    }


    public string SensorsExcelPath { get; }
    public string PumpsExcelPath { get; }
    public string TanksExcelPath { get; }
    public string CustomerMeterExcelPath { get; }
    public string ZonesExcelPath { get; }
    public string AlertsExcelPath { get; }
    public string PowerBIExcelPath { get; }

    private string SetupDir { get; }

}


public struct ExcelSheetName
{
    public const string Sensors = "Sensors";
    public const string PossibleSensors = "Possible Sensors";
    public const string Tanks = "Tanks";
    public const string TankCurves = "Tank curves";
    public const string Pumps = "Pumps";
    public const string PumpCurves = "Pump Curves";
    public const string PumpStations = "Pumping Stations";
    public const string Zone = "Zones Balance";
    public const string ZoneCharacteristics = "Zones Characteristics";
    public const string CustomerMeters = "Customer meters";
    public const string Alerts = "Alerts";
    public const string PowerBI = "Power BI";
}
