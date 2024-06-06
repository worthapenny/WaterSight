using Serilog;
using WaterSight.UI.App;
using WaterSight.UI.Auth;
using WaterSight.Web.Core;
using WaterSight.Web.DT;

namespace WaterSight.UI.ControlModels;

public class ProjectOpenSaveControlModel
{
    #region Constructor
    public ProjectOpenSaveControlModel()
    {
    }
    #endregion

    #region Public Methods
    public void Initialize(SignInControlModel signInControlModel)
    {
        SignInControlModel = signInControlModel;

        signInControlModel.AuthEvent += (s, e) => AuthEventChanged(e);


    }

    public OpenFileDialog GetOpenFileDialog(string initialDirectory)
    {
        var openFileDialog = new OpenFileDialog();

        if (Directory.Exists(initialDirectory))
            openFileDialog.InitialDirectory = initialDirectory;

        openFileDialog.Filter = "WS Project files (*.ws.json)|*.ws.json";
        openFileDialog.CheckFileExists = true;
        openFileDialog.CheckPathExists = true;
        openFileDialog.Title = "Select WaterSight Project File";
        openFileDialog.Multiselect = false;

        return openFileDialog;
    }
    //public SaveFileDialog GetSaveFileDialog()
    //{
    //    var saveFileDialog = new SaveFileDialog();
    //    saveFileDialog.Filter = "WS Project files (*.ws.json)|*.ws.json";
    //    saveFileDialog.CheckFileExists = false;
    //    saveFileDialog.Title = "Create WaterSight Project File";

    //    return saveFileDialog;
    //}

    public async Task<Dictionary<int, DigitalTwinConfig>> GetDigitalTwinConfigMapAsync()
    {
        if (_dtConfigMap != null)
            return _dtConfigMap;

        if (WS == null) CreateNewWS(accessToken: SignInControlModel?.AccessToken);
        var DTs = await WS.DigitalTwin.GetDigitalTwinsAsync();

        _dtConfigMap = new Dictionary<int, DigitalTwinConfig>();
        foreach (var dt in DTs)
            _dtConfigMap.Add(dt.ID, dt);

        return _dtConfigMap;
    }


    public async Task<bool> BindDigitalTwinComboBoxAsync(ComboBox cmb)
    {
        var success = true;
        var dts = (await GetDigitalTwinConfigMapAsync()).Values.ToList();

        var dtMap = new Dictionary<string, DigitalTwinConfig>();
        try
        {
            foreach (var dt in dts)
                dtMap.Add($"{dt.ID}: {dt.Name}", dt);

            if (dts.Count > 0)
            {
                cmb.DataSource = new BindingSource(dtMap, null);
                cmb.DisplayMember = "Key";
            }
            else
            {
                var message = $"No Digital Twins got loaded. Please make sure you have access to at least one. If you do, then for some reason we can't get the list of Digital Twins.";
                Log.Warning(message);
                success = false;
            }
        }
        catch (Exception ex)
        {
            success = false;
            Log.Error(ex, "...while loading the digital twin list");
        }

        return success;
    }
    #endregion


    #region Private Methods
    private void AuthEventChanged(AuthEvent e)
    {
        if (e == AuthEvent.LoggedIn
            || e == AuthEvent.RefreshCompleted)
        {
            if (WS == null) CreateNewWS(SignInControlModel?.AccessToken);
            WS.Options.RestToken = SignInControlModel?.AccessToken;
        }
    }

    private void CreateNewWS(string? accessToken)
    {
        WS = new WS(
            logger: Support.Logging.Logging.Logger,
            tokenRegistryPath: null,
            restToken: accessToken,
            subDomainSuffix: (SignInControlModel?.IsEURegion ?? false) ? "-weu" : string.Empty
            );
    }
    #endregion

    #region Public Properties
    public string? ProjectFilePath { get; set; }
    public DigitalTwinConfig? SelectedDTConfig { get; set; }
    public NewProjectControlModel? SelectedProject
    {
        get => _selecteNewProjectControlModel;
        set
        {
            _selecteNewProjectControlModel = value;
            UIApp.Instance.ActiveProjectModel = value;
        }
    }

    public WS? WS {
        get => UIApp.Instance.WS;
        set => UIApp.Instance.WS = value;
    }
    public SignInControlModel? SignInControlModel { get; private set; }
    public bool IsOffline { get; set; } =false;

    #endregion

    #region Fields
    private Dictionary<int, DigitalTwinConfig>? _dtConfigMap;
    private NewProjectControlModel? _selecteNewProjectControlModel;
    #endregion
}
