using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Domain;
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
    public async Task<List<DigitalTwinConfig>> GetDigitalTwinIdsAsync()
    {
        if (WS == null) CreateNewWS(accessToken: SignInControlModel?.AccessToken);
        var dts = await WS.DigitalTwin.GetDigitalTwinsAsync();
        return dts;
    }

    public async Task<bool> BindDigitalTwinComboBoxAsync(ComboBox cmb)
    {
        var success = true;
        var dts = await GetDigitalTwinIdsAsync();

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
            tokenRegistryPath: null,
            restToken: accessToken,
            subDomainSuffix: (SignInControlModel?.IsEURegion ?? false) ? "-weu" : string.Empty);
    }
    #endregion

    #region Public Properties
    public string? ProjectFilePath { get; set; }
    public DigitalTwinConfig? SelectedDTConfig { get; set; }

    public WS? WS { get; private set; }
    public SignInControlModel? SignInControlModel { get; private set; }

    #endregion
}
