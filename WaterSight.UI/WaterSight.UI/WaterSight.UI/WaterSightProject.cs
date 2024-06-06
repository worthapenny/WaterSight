using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using WaterSight.Excel;
using WaterSight.UI.Support.Model;
using WaterSight.Web.Core;
using WaterSight.Web.DT;

namespace WaterSight.Domain;

public class WaterSightFolders
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

    public const string SettingsConfigDirName = "00_Settings_Config";
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
    public WaterSightFolders(string projectDir)
    {
        if (projectDir == null)
        {
            var message = $"Project Dir cannot be null.";
            Log.Error(message);
            Debugger.Break();
            throw new ArgumentNullException(message);
        }

        ProjectDir = projectDir;
        DocsDir = Path.Combine(projectDir, DocsDirName);
        FileArchiveDir = Path.Combine(projectDir, UserFileArchiveDirName);

        // 01_WaterSight_Config and sub directories
        WaterSightDir = Path.Combine(projectDir, WaterSightConfigDirName);
        WsConfigDir = Path.Combine(WaterSightDir, SettingsConfigDirName);
        WsModelsDir = Path.Combine(WaterSightDir, ModelsWSDirName);
        WsGisDir = Path.Combine(WaterSightDir, GisWSDirName);
        WsConsumptionDir = Path.Combine(WaterSightDir, ConsumptionWSDirName);
        WsScadaDir = Path.Combine(WaterSightDir, ScadaWSDirName);
        WsPowerBiDir = Path.Combine(WaterSightDir, PowerBiWSDirName);

        // 02_Analysis and sub directories
        AnalysisDir = Path.Combine(projectDir, AnalysisDirName);
        AnaConfigDir = Path.Combine(AnalysisDir, SettingsConfigDirName);
        AnaModelsDir = Path.Combine(AnalysisDir, ModelsWSDirName);
        AnaGisDir = Path.Combine(AnalysisDir, GisWSDirName);
        AnaConsumptionDir = Path.Combine(AnalysisDir, ConsumptionWSDirName);
        AnaScadaDir = Path.Combine(AnalysisDir, ScadaWSDirName);
        AnaPowerBiDir = Path.Combine(AnalysisDir, PowerBiWSDirName);
        AnaScriptsDir = Path.Combine(AnalysisDir, ScriptsDirName);
        AnaPumpsDir = Path.Combine(AnalysisDir, PumpsDirName);
        AnaTanksDir = Path.Combine(AnalysisDir, TanksDirName);

        //CreateFolders();

    }
    #endregion

    #region Public Methods
    public void CreateFolders()
    {
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
    //readonly string _ProjectFileName = $"WaterSightProject.ws.json";
    #endregion

    #region Static Constructor

    public static async Task<WaterSightProject> LoadFromWebAsync(WS ws)
    {
        var instance = new WaterSightProject();
        instance.WSSetting = await instance.WSSetting.LoadFromWeb(ws);
        return instance;
    }
    public static WaterSightProject? LoadFromJson(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException($"Give json file path is invalid. Path:{jsonFilePath}");

        var project = JsonIO.LoadFromFile<WaterSightProject>(jsonFilePath);
        return project;
    }
    #endregion


    #region Constructor
    public WaterSightProject()
    {
    }    
    #endregion

    #region Public Methods
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    //public bool ToJsonFile()
    //{
    //    if (!Directory.Exists(Folders.ProjectDir))
    //    {
    //        Log.Error($"Project directory '{Folders.ProjectDir}' is not valid");
    //        return false;
    //    }

    //    var _ProjectFileName = $"{WSSetting.Info.Name}"; 
    //    var success = true;
    //    try
    //    {
    //        var jsonFilePath = Path.Combine(Folders.ProjectDir, _ProjectFileName);
    //        File.WriteAllText(jsonFilePath, ToJson());
    //    }
    //    catch (Exception ex)
    //    {
    //        var message = $"While writing to {_ProjectFileName}";
    //        Log.Error(ex, message);
    //        success = false;
    //    }

    //    return success;
    //}
    #endregion

    #region Public Properties
    public string? ProjectDir
    {
        get => _projectDir;
        set { _projectDir = value; Folders = new WaterSightFolders(value); }
    } 

    public string? WaterModelPath { get; set; }
    

    public bool UseAnalysisDir { get; set; } = true;
    public Env Env { get; set; }

    public WaterSightWebSetting WSSetting { get; set; } = new WaterSightWebSetting();


    [JsonIgnore]
    public TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.Local;

    [JsonIgnore]
    public WaterSightFolders? Folders { get; set; }

    [JsonIgnore]
    public SCADA? SCADA { get; set; }

    [JsonIgnore]
    public FileNames? XlFileNames { get; set; }
    #endregion

    #region Fields
    private string? _projectDir;
    #endregion
}
