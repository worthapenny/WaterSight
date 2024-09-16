using Serilog;
using System.Diagnostics;

namespace WaterSight.Support;

public class WaterSightFolders
{
    #region Constants
    /*       
         *  PROJECT TEMPLATE
          
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
         03_Docs
         04_User_File_Archive
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
        if (string.IsNullOrEmpty(projectDir))
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
        WsConfigDir = Path.Combine(projectDir, WaterSightConfigDirName);
        WsConfigSettingsDir = Path.Combine(WsConfigDir, SettingsConfigDirName);
        WsModelsDir = Path.Combine(WsConfigDir, ModelsWSDirName);
        WsGisDir = Path.Combine(WsConfigDir, GisWSDirName);
        WsConsumptionDir = Path.Combine(WsConfigDir, ConsumptionWSDirName);
        WsScadaDir = Path.Combine(WsConfigDir, ScadaWSDirName);
        WsPowerBiDir = Path.Combine(WsConfigDir, PowerBiWSDirName);

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

    #region Public Static Methods
    public static WaterSightFolders CreateDirectories(string rootDir)
    {
        if(!Directory.Exists(rootDir)) 
            throw new DirectoryNotFoundException(rootDir);

        var wsFolder = new WaterSightFolders(rootDir);
        wsFolder.CreateFolders();

        return wsFolder;
    }
    #endregion

    #region Public Methods
    public void CreateFolders()
    {
        // Create Folders
        Directory.CreateDirectory(DocsDir);
        Directory.CreateDirectory(FileArchiveDir);

        // 01_WaterSight_Config and sub directories
        Directory.CreateDirectory(WsConfigDir);
        Directory.CreateDirectory(WsConfigSettingsDir);
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

    public string WsConfigDir { get; private set; }
    public string AnalysisDir { get; private set; }
    public string FileArchiveDir { get; private set; }
    public string DocsDir { get; private set; }

    public string WsConfigSettingsDir { get; private set; }
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
