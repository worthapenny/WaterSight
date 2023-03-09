using System.IO;

namespace WaterSight.Excel;

public class FileNames
{
    #region Constructor
    public FileNames(string waterSightDir)
    {
        ExcelFileNames = new ExcelFileNames(waterSightDir);
        CsvFileNames = new CsvFileNames(waterSightDir);
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
    public const string ConsumptionDirName = "03_Consumption";

    public CsvFileNames(
        string configDirWS,
        string consumption = "Consumptions.csv")
    {
        ConfigDirWS = configDirWS;
        Consumptions = Path.Combine(ConfigDirWS, ConsumptionDirName, consumption);
    }

    public string ConfigDirWS { get; set; }
    public string Consumptions { get; set; }
}

public class ExcelFileNames
{
    public const string ConfigWSDirName = "00_Settings_Config";

    public const string SensorsFileName = "Sensors.xlsx";
    public const string PumpsFileName = "Pumps.xlsx";
    public const string TanksFileName = "Tanks.xlsx";
    public const string ConsumptionFileName = "ConsumptionMeter.xlsx";
    public const string ZonesFileName = "Zones.xlsx";
    public const string AlertsFileName = "Alerts.xlsx";
    public const string PowerBiFileName = "Power BI.xlsx";

    public ExcelFileNames(
        string waterSightDir,
        string sensorsFileName = SensorsFileName,
        string pumpsFileName = PumpsFileName,
        string tanksFileName = TanksFileName,
        string consumptionFileName = ConsumptionFileName,
        string zonesFileName = ZonesFileName,
        string alertsFileName = AlertsFileName,
        string powerBiFileName = PowerBiFileName)
    {
        var configDirWS = Path.Combine(waterSightDir, ConfigWSDirName);

        SensorsExcelPath = Path.Combine(configDirWS, sensorsFileName);
        PumpsExcelPath = Path.Combine(configDirWS, pumpsFileName);
        TanksExcelPath = Path.Combine(configDirWS, tanksFileName);
        CustomerMeterExcelPath = Path.Combine(configDirWS, consumptionFileName);
        ZonesExcelPath = Path.Combine(configDirWS, zonesFileName);
        AlertsExcelPath = Path.Combine(configDirWS, alertsFileName);
        PowerBIExcelPath = Path.Combine(configDirWS, powerBiFileName);
    }


    public string SensorsExcelPath { get; }
    public string PumpsExcelPath { get; }
    public string TanksExcelPath { get; }
    public string CustomerMeterExcelPath { get; }
    public string ZonesExcelPath { get; }
    public string AlertsExcelPath { get; }
    public string PowerBIExcelPath { get; }


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
