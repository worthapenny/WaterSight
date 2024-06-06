using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterSight.Domain;
using WaterSight.Model.Generator;
using WaterSight.UI.App;
using WaterSight.UI.ControlModels;
using WaterSight.UI.Forms.Support;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.UI.Controls;

public partial class NewProjectControl : UserControl
{
    #region Constructor
    public NewProjectControl()
    {
        InitializeComponent();
    }
    #endregion

    #region Public Methods
    public void Initialize(NewProjectControlModel model)
    {
        NewProjectControlModel = model;

        // Make sure we are working against the right DT
        //NewProjectControlModel.WS.Options.DigitalTwinId = model.WSProject.WSSetting.Info.DTID;

        //this.textBoxProjectPath.Text = model.WSProject.ProjectDir;
        //this.textBoxLat.Text = model.WSProject.WSSetting.Geo.Latitude.ToString();
        //this.textBoxLng.Text = model.WSProject.WSSetting.Geo.Longitude.ToString();
        this.radioButtonUnitUS.Checked = model.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.US;
        this.radioButtonUnitSI.Checked = model.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.SI;

        // Project Path
        this.textBoxProjectPath.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject,
            nameof(model.WSProject.ProjectDir));

        // DT Info
        this.textBoxDtInfo.DataBindings.Add(
            nameof(TextBox.Text),
            model,
            nameof(model.DTInfo));

        // Name
        this.textBoxName.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Info,
            nameof(model.WSProject.WSSetting.Info.Name));

        // Description
        this.textBoxDescription.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Info,
            nameof(model.WSProject.WSSetting.Info.Description));

        // Short Name
        this.textBoxShortName.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Info,
            nameof(model.WSProject.WSSetting.Info.ShortName));

        // EPSG
        this.textBoxEpsgCode.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Geo,
            nameof(model.WSProject.WSSetting.Geo.SensorEPSG));

        // TimeZone
        this.textBoxTimeZone.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Geo,
            nameof(model.WSProject.WSSetting.Geo.TimeZoneString));


        // Latitude
        this.textBoxLat.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Geo,
            nameof(model.WSProject.WSSetting.Geo.Latitude));

        // Longitude
        this.textBoxLng.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Geo,
            nameof(model.WSProject.WSSetting.Geo.Longitude));

        // Set units for Groupbox
        UnitSystemChanged(Support.Model.UnitSystem.US);

        // Service
        this.textBoxMaxPressure.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.MaxPressure));
        this.textBoxMinPressure.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.MinPressure));
        this.textBoxTargetEfficiency.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.TargetPumpEfficiency));

        // Costs
        this.textBoxProductionCost.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.AverageVolumeProductionCost));
        this.textBoxTariff.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.AverageVolumeTariff));
        this.textBoxEnergyCost.DataBindings.Add(
            nameof(TextBox.Text),
            model.WSProject.WSSetting.Operations,
            nameof(model.WSProject.WSSetting.Operations.AverageEnergyCost));


        this.linkLabelGoogleMap.Click += (s, e) => OpenGoogleMap(
            lat: this.textBoxLat.Text,
            lng: this.textBoxLng.Text,
            dtName: this.textBoxName.Text);
        this.linkLabelTimeZone.Click += (s, e) => OpenTimeZonePage();

        this.buttonWsLoad.Click += async (s, e) => await LoadFromWebAsync();
        this.buttonWsSave.Click += async (s, e) => await SaveToWebAsync();
        this.buttonFileSave.Click += async (s, e) => await SaveToFileAsync();
        this.buttonMakeCurrent.Click += (s, e) => MakeDigitalTwinCurrent();

        this.radioButtonUnitSI.Click += (s, e) => UnitSystemChanged(Support.Model.UnitSystem.SI);
        this.radioButtonUnitUS.Click += (s, e) => UnitSystemChanged(Support.Model.UnitSystem.US);
    }
    #endregion

    #region Private Methods
    private void UnitSystemChanged(Support.Model.UnitSystem unit)
    {
        if (NewProjectControlModel != null)
        {
            NewProjectControlModel.WSProject.WSSetting.UnitSystem = unit;

            var pressureUnit = "psi";
            var currencyUnit = "$";
            var flowUnit = "MGal";

            if (unit == Support.Model.UnitSystem.SI)
            {
                pressureUnit = "m";
                currencyUnit = "€";
                flowUnit = "m³/h";
            }

            groupBoxCosts.Text = $"Costs ({currencyUnit}/{flowUnit})";
            groupBoxServiceExpectations.Text = $"Service ({pressureUnit})";
        }
    }
    private async Task LoadFromWebAsync()
    {
        if(UIApp.Instance.WS == null)
        {
            var message = $"Could not communicate with WaterSight";
            Debugger.Break();
            MessageBox.Show(this, message, "No WaterSight?", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        var busyWindow = new BusyWindow();
        busyWindow.Show(ParentForm);
        Application.DoEvents();

        try
        {
            if (NewProjectControlModel != null && UIApp.Instance.WS != null)
            {
                UIApp.Instance.WS.Options.DigitalTwinId = this.NewProjectControlModel.DigitalTwinId;
                await NewProjectControlModel.LoadWaterSightProjectAsync(ws: UIApp.Instance.WS, jsonFilePath: null);
            }
            else
            {
                Debugger.Break();
            }
        }
        finally
        {
            busyWindow.Done();
        }

    }

    private async Task<bool> SaveToWebAsync()
    {
        if (UIApp.Instance.WS == null)
        {
            var message = $"Could not communicate with WaterSight";
            Debugger.Break();
            MessageBox.Show(this, message, "No WaterSight?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if(NewProjectControlModel?.WSProject == null)
        {
            var message = $"WaterSight project is not loaded.";
            Debugger.Break();
            MessageBox.Show(this, message, "No WaterSight?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        var busyWindow = new BusyWindow();
        busyWindow.Show(ParentForm);
        Application.DoEvents();

        try
        {
            var success = await NewProjectControlModel.SaveToWebAsync(
                ws: UIApp.Instance.WS,
                wsProject: NewProjectControlModel.WSProject);
            if (success)
            {
                var message = $"Updated WaterSight successfully.";
                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        finally
        {
            busyWindow.Done();
        }

        return false;
    }

    private void MakeDigitalTwinCurrent()
    {
        if (NewProjectControlModel == null)
        {
            var message = $"{nameof(NewProjectControlModel)} cannot be null";
            Log.Error(message);
            using (new CenterWinDialog(ParentForm))
                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        NewProjectControlModel.SetDigitalTwin(NewProjectControlModel.DigitalTwinId);

        // Close the form
        ParentForm.Close();

    }

    private async Task<bool> SaveToFileAsync()
    {
        if (NewProjectControlModel == null)
        {
            Log.Error($"{nameof(NewProjectControlModel)} cannot be null");
            return false;
        }

        if(string.IsNullOrEmpty(NewProjectControlModel.ProjectPath)
            && !Directory.Exists(NewProjectControlModel.ProjectPath))
        {
            var message = $"Please make sure 'Project Path' is given and is valid.";
            Log.Error(message);

            using (new CenterWinDialog(ParentForm))
                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        var saved = false;
        var projectFilePath = UIApp.Instance.ActiveProjectFilePath;

        if (!File.Exists(projectFilePath))
        {
            var saveFileDialog = NewProjectControlModel.GetSaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                projectFilePath = saveFileDialog.FileName;
            }
        }

        if (projectFilePath != null)
        {
            saved = await NewProjectControlModel.SaveProjectFileAsync();
        }
            //saved = await NewProjectControlModel.FileSaveAsync(
            //         content: NewProjectControlModel.WSProject,
            //         fullFilePath: projectFilePath);

        return saved;
    }
    private void OpenGoogleMap(string lat, string lng, string? dtName = null)
    {
        var googleMapLink = @"https://www.google.com/maps";

        if (lat?.Length > 3 && lng?.Length > 3)
            googleMapLink += $"/search/{lat},{lng}/@{lat},{lng},10z"; // z=10 => Zoom to city level

        if (!string.IsNullOrEmpty(dtName))
            dtName = Uri.EscapeDataString(dtName);
        googleMapLink += $"/place/{dtName}";

        var process = new Process();
        process.StartInfo = new ProcessStartInfo(googleMapLink);
        process.StartInfo.UseShellExecute = true;
        process.Start();

        Log.Debug($"Process started for: {googleMapLink}");
    }
    private void OpenTimeZonePage()
    {
        var link = @"https://en.wikipedia.org/wiki/List_of_tz_database_time_zones";
        var process = new Process();
        process.StartInfo = new ProcessStartInfo(link);
        process.StartInfo.UseShellExecute = true;
        process.Start();

        Log.Debug($"Process started for: {link}");
    }


    private void LoadTypicalOperationValues()
    {
        if (NewProjectControlModel == null) return;

        NewProjectControlModel.WSProject.WSSetting.Operations.MaxPressure = NewProjectControlModel.TypicalMaxPressureInPSI;
        NewProjectControlModel.WSProject.WSSetting.Operations.MinPressure = NewProjectControlModel.TypicalMinPressureInPSI;
        NewProjectControlModel.WSProject.WSSetting.Operations.TargetPumpEfficiency = NewProjectControlModel.TypicalTargetEfficiency;

        if (NewProjectControlModel.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.SI)
        {
            NewProjectControlModel.WSProject.WSSetting.Operations.MaxPressure = Math.Round(NewProjectControlModel.TypicalMaxPressureInPSI * 0.70325, 0);
            NewProjectControlModel.WSProject.WSSetting.Operations.MinPressure = Math.Round(NewProjectControlModel.TypicalMinPressureInPSI * 0.70325, 0);
        }

        textBoxMaxPressure.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.MaxPressure}";
        textBoxMinPressure.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.MinPressure}";
        textBoxTargetEfficiency.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.TargetPumpEfficiency}";
    }


    private void LoadTypicalCosts()
    {
        if (NewProjectControlModel == null) return;


        if (NewProjectControlModel.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.US)
        {
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeProductionCost = NewProjectControlModel.TypicalProductionCostDollarPerMgal;
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeTariff = NewProjectControlModel.TypicalTariffDollarPerMgal;
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageEnergyCost = NewProjectControlModel.TypicalEnergyCostDollarPerMgal;
        }
        else
        {
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeProductionCost = Math.Round(NewProjectControlModel.TypicalProductionCostDollarPerMgal * 1, 0);
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeTariff = Math.Round(NewProjectControlModel.TypicalTariffDollarPerMgal * 3785.4118, 0);
            NewProjectControlModel.WSProject.WSSetting.Operations.AverageEnergyCost = Math.Round(NewProjectControlModel.TypicalEnergyCostDollarPerMgal * 3785.4118, 0);
        }

        textBoxProductionCost.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeProductionCost}";
        textBoxTariff.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.AverageVolumeTariff}";
        textBoxEnergyCost.Text = $"{NewProjectControlModel.WSProject.WSSetting.Operations.AverageEnergyCost}";
    }

    private void buttonBrowseProject_Click(object sender, EventArgs e)
    {
        if (NewProjectControlModel == null) return;

        var folderDialog = NewProjectControlModel.GetOpenFolderDialog(initialDirectory: string.Empty);

        DialogResult result = folderDialog.ShowDialog();
        if (result == DialogResult.OK)
            NewProjectControlModel.WSProject.ProjectDir = folderDialog.SelectedPath;
    }

    private void buttonLoadDefaultService_Click(object sender, EventArgs e)
    {
        LoadTypicalOperationValues();
    }
    private void buttonLoadDefaultCosts_Click(object sender, EventArgs e)
    {
        LoadTypicalCosts();
    }
    #endregion

    #region Public Properties
    public NewProjectControlModel? NewProjectControlModel { get; private set; }
    #endregion
}
