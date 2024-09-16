using Newtonsoft.Json;
using Serilog;
using System.ComponentModel;
using WaterSight.Support.Web;
using WaterSight.Web.Core;
using WaterSight.Web.Support.IO;

namespace WaterSight.Support;

public class WaterSightProject : INotifyPropertyChanged
{
    #region Public Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Constructor
    public WaterSightProject()
    {
    }
    public WaterSightProject(string projectRootDir, string projectFileName)
    {
        if (!Directory.Exists(projectRootDir))
            throw new DirectoryNotFoundException(projectRootDir);

        ProjectDir = projectRootDir;
        ProjectFileName = projectFileName;
    }
    #endregion

    #region Static Constructor

    public static async Task<WaterSightProject> LoadFromWebAsync(WS ws)
    {
        var instance = new WaterSightProject();
        instance.WSSetting = await instance.WSSetting.LoadFromWeb(ws);

        Log.Information($"🌍 Project loaded from WaterSight web.");
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

    #region Public Methods
    public void Save()
    {
        JsonIO.WriteJsonToFile(
            JsonConvert.SerializeObject(this, Formatting.Indented),
            ProjectFullFilePath);

        Log.Information($"💾 Project info saved. Path: {ProjectFullFilePath}");
    }
    #endregion

    #region Protected Methods
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    #endregion

    #region Public Properties
    public string? ProjectDir
    {
        get => _projectDir;
        set
        {
            if (!Directory.Exists(value))
                throw new DirectoryNotFoundException(value);

            _projectDir = value;
            Folders = new WaterSightFolders(value);
            OnPropertyChanged(nameof(ProjectDir));
        }
    }
    public string? ProjectFileName
    {
        get => _projectFileName;
        set
        {
            _projectFileName = value;
            OnPropertyChanged(nameof(ProjectFileName));
        }
    }

    [JsonIgnore]
    public string ProjectFullFilePath => Path.Combine(Folders?.WsConfigDir, ProjectFileName);

    [JsonIgnore]
    public WaterSightFolders? Folders { get; set; }

    public string? WaterModelPath
    {
        get => _waterModelPath;
        set
        {
            _waterModelPath = value;
            OnPropertyChanged(nameof(WaterModelPath));
        }
    }

    public bool UseAnalysisDir { get; set; } = true;
    public Env Env
    {
        get => _env;
        set { _env = value; OnPropertyChanged(nameof(Env)); }
    }

    public WaterSightWebSetting WSSetting { get; set; } = new WaterSightWebSetting();


    //[JsonIgnore]
    //public TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.Local;
    #endregion

    #region Fields
    private string? _projectDir;
    private string? _projectFileName;
    private string? _waterModelPath;
    private bool _useAnalysisDir = true;
    private Env _env;

    #endregion
}
