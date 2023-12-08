using Newtonsoft.Json;
using Serilog;
using WaterSight.Excel;
using WaterSight.UI.Support.Model;
using WaterSight.Web.Core;
using WaterSight.Web.DT;

namespace WaterSight.Domain;

public class Folders
{
    #region Constants
    /*       
         *  PROJECT TEMPLATE
         *  
         01_WaterSight_Config
            00_Settings_Config
            01_Models
            02_GIS
            03_Consumption
            04_SCADA
            05_Power_BI
         02_Analysis
            00_Settings_Config
            01_Models
            02_GIS
            03_Consumption
            04_SCADA
            05_Power_BI
            10_Scripts
            11_Pumps
            12_Tanks
         * 03_Docs
         * 04_User_File_Archive
    */

    public const string WaterSightConfigDirName = "01_WaterSight_Config";
    public const string AnalysisDirName = "02_Analysis";
    public const string DocsDirName = "03_Docs";
    public const string UserFileArchiveDirName = "04_User_File_Archive";

    public const string ConfigWSDirName = "00_Settings_Config";
    public const string ModelsWSDirName = "01_Models";
    public const string GisWSDirName = "02_GIS";
    public const string ConsumptionWSDirName = "03_Consumption";
    public const string ScadaWSDirName = "04_SCADA";
    public const string PowerBiWSDirName = "05_Power_BI";

    public const string ScriptsDirName = "10_Scripts";
    public const string PumpsDirName = "11_Pumps";
    public const string TanksDirName = "12_Tanks";
    #endregion

    #region Constructor
    public Folders(string projectDir)
    {
        ProjectDir = projectDir;

        DocsDir = Path.Combine(projectDir, DocsDirName);
        FileArchiveDir = Path.Combine(projectDir, UserFileArchiveDirName);

        // 01_WaterSight_Config and sub directories
        WaterSightDir = Path.Combine(projectDir, WaterSightConfigDirName);
        WsConfigDir = Path.Combine(WaterSightDir, ConfigWSDirName);
        WsModelsDir = Path.Combine(WaterSightDir, ModelsWSDirName);
        WsGisDir = Path.Combine(WaterSightDir, GisWSDirName);
        WsConsumptionDir = Path.Combine(WaterSightDir, ConsumptionWSDirName);
        WsScadaDir = Path.Combine(WaterSightDir, ScadaWSDirName);
        WsPowerBiDir = Path.Combine(WaterSightDir, PowerBiWSDirName);

        // 02_Analysis and sub directories
        AnalysisDir = Path.Combine(projectDir, AnalysisDirName);
        AnaConfigDir = Path.Combine(AnalysisDir, ConfigWSDirName);
        AnaModelsDir = Path.Combine(AnalysisDir, ModelsWSDirName);
        AnaGisDir = Path.Combine(AnalysisDir, GisWSDirName);
        AnaConsumptionDir = Path.Combine(AnalysisDir, ConsumptionWSDirName);
        AnaScadaDir = Path.Combine(AnalysisDir, ScadaWSDirName);
        AnaPowerBiDir = Path.Combine(AnalysisDir, PowerBiWSDirName);
        AnaScriptsDir = Path.Combine(AnalysisDir, ScriptsDirName);
        AnaPumpsDir = Path.Combine(AnalysisDir, PumpsDirName);
        AnaTanksDir = Path.Combine(AnalysisDir, TanksDirName);


        // Create Folders
        Directory.CreateDirectory(DocsDir);
        Directory.CreateDirectory(FileArchiveDir);

        // 01_WaterSight_Config and sub directories
        Directory.CreateDirectory(WaterSightDir);
        Directory.CreateDirectory(WsConfigDir);
        Directory.CreateDirectory(WsModelsDir);
        Directory.CreateDirectory(WsGisDir);
        Directory.CreateDirectory(WsConsumptionDir);
        Directory.CreateDirectory(WsScadaDir);
        Directory.CreateDirectory(WsPowerBiDir);

        // 02_Analysis and sub directories
        Directory.CreateDirectory(AnalysisDir);
        Directory.CreateDirectory(AnaConfigDir);
        Directory.CreateDirectory(AnaModelsDir);
        Directory.CreateDirectory(AnaGisDir);
        Directory.CreateDirectory(AnaConsumptionDir);
        Directory.CreateDirectory(AnaScadaDir);
        Directory.CreateDirectory(AnaPowerBiDir);
        Directory.CreateDirectory(AnaScriptsDir);
        Directory.CreateDirectory(AnaPumpsDir);
        Directory.CreateDirectory(AnaTanksDir);


        //WtrgFilePath = Path.Combine(projectDir, wtgFilePath);
        //PressureZoneZippedShpFilePath = Path.Combine(WsGisDir, pressureZoneZippedShpFileName);
        //PressureZoneShpFilePath = Path.Combine(WsGisDir, pressureZoneShpFileName);

    }
    #endregion

    #region Public Properties
    public string ProjectDir { get; set; }

    public string WaterSightDir { get; private set; }
    public string AnalysisDir { get; private set; }
    public string FileArchiveDir { get; private set; }
    public string DocsDir { get; private set; }

    public string WsConfigDir { get; private set; }
    public string WsModelsDir { get; private set; }
    public string WsGisDir { get; private set; }
    public string WsConsumptionDir { get; set; }
    public string WsScadaDir { get; private set; }
    public string WsPowerBiDir { get; private set; }


    public string AnaConfigDir { get; private set; }
    public string AnaModelsDir { get; private set; }
    public string AnaGisDir { get; private set; }
    public string AnaConsumptionDir { get; private set; }
    public string AnaScadaDir { get; private set; }
    public string AnaPowerBiDir { get; private set; }
    public string AnaScriptsDir { get; private set; }
    public string AnaPumpsDir { get; private set; }
    public string AnaTanksDir { get; private set; }

    #endregion

}

public class WaterSightProject
{
    #region Constants  
    readonly string _ProjectFileName = $"WaterSightProject.ws.json";
    #endregion

    public static async Task<WaterSightProject> LoadFromWebAsync(WS ws, DigitalTwinConfig dtConfig)
    {
        var instance = new WaterSightProject();
        instance.WSSetting = await instance.WSSetting.LoadFromWebAsync(ws, dtConfig);

        return instance;
    }

    #region Constructor
    public WaterSightProject()
    {
    }
    public WaterSightProject(
        int dtID,
        Env env = Env.Prod,
        string name = "",
        string shortName = "",
        int epsgCode = 4326,
        string timeZoneString = "", // https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/default-time-zones?view=windows-11
        double[]? latLng = null,
        string projectDir = ""
        ) : this()
    {


        // Folder Structure
        Folders = new Folders(projectDir);

        // Info
        Env = env;
        WSSetting.Info.DTID = dtID;
        WSSetting.Info.Name = name;
        WSSetting.Info.ShortName = shortName;

        // EPSG Codes
        WSSetting.Geo.SensorEPSG = epsgCode;
        WSSetting.Geo.CustomersEPSG = epsgCode;
        WSSetting.Geo.WorkOrdersEPSG = epsgCode;
        WSSetting.Geo.SmartMetersEPSG = epsgCode;


        if (latLng?.Length >= 2)
        {
            WSSetting.Geo.Latitude = latLng[0];
            WSSetting.Geo.Longitude = latLng[1];
        }

        // TimeZone
        if (!string.IsNullOrEmpty(timeZoneString))
        {
            WSSetting.Geo.TimeZoneString = timeZoneString;
            TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
        }


        XlFileNames = new FileNames(Folders.WaterSightDir);
        SCADA = new SCADA(Folders.WsScadaDir, null);

        Log.Debug($"Project initialized for  {WSSetting.Info.DTID}: {WSSetting.Info.Name} in {Env}");
    }
    #endregion

    #region Public Methods
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    public bool ToJsonFile()
    {
        if (!Directory.Exists(Folders.ProjectDir))
        {
            Log.Error($"Project directory '{Folders.ProjectDir}' is not valid");
            return false;
        }


        var success = true;
        try
        {
            var jsonFilePath = Path.Combine(Folders.ProjectDir, _ProjectFileName);
            File.WriteAllText(jsonFilePath, ToJson());
        }
        catch (Exception ex)
        {
            var message = $"While writing to {_ProjectFileName}";
            Log.Error(ex, message);
            success = false;
        }

        return success;
    }
    #endregion

    #region Public Properties
    public bool UseAnalysisDir { get; set; } = true;
    public Env Env { get; set; }

    public WsSetting WSSetting { get; set; } = new WsSetting();

    //public string? Name { get; set; }
    //public string? ShortName { get; set; }
    //public int DTID { get; set; }
    //public string TimeZoneString { get; set; }
    //public int EPSGCode { get; set; }

    [JsonIgnore]
    public TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.Local;

    [JsonIgnore]
    public Folders? Folders { get; set; }

    [JsonIgnore]
    public SCADA? SCADA { get; set; }
    //public double[]? LatLng { get; set;
    //

    [JsonIgnore]
    public FileNames? XlFileNames { get; set; }
    #endregion
}
