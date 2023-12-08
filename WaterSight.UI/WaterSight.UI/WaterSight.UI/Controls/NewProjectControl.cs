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
using WaterSight.UI.ControlModels;

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

        //this.textBoxDtInfo.Text = model.DTInfo;
        //this.textBoxName.Text = model.WSProject.WSSetting.Info.Name;
        //this.textBoxDescription.Text = model.WSProject.WSSetting.Info.Description;
        //this.textBoxShortName.Text = model.WSProject.WSSetting.Info.ShortName;
        //this.textBoxEpsgCode.Text = model.WSProject.WSSetting.Geo.SensorEPSG.ToString();
        //this.textBoxTimeZone.Text = model.WSProject.WSSetting.Geo.TimeZoneString;
        this.textBoxLat.Text = model.WSProject.WSSetting.Geo.Latitude.ToString();
        this.textBoxLng.Text = model.WSProject.WSSetting.Geo.Longitude.ToString();
        this.radioButtonUnitUS.Checked = model.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.US;
        this.radioButtonUnitSI.Checked = model.WSProject.WSSetting.UnitSystem == Support.Model.UnitSystem.SI;

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

        this.linkLabelGoogleMap.Click += (s, e) => OpenGoogleMap(
            lat: this.textBoxLat.Text,
            lng: this.textBoxLng.Text);
        this.linkLabelTimeZone.Click += (s, e) => OpenTimeZonePage();

        this.buttonWsLoad.Click += async (s, e) => await LoadFromWSAsync();
        this.buttonWsSave.Click += async (s, e) => await SaveToWSAsync();
        this.buttonFileSave.Click += async (s, e) => await SaveToFileAsync();

        this.radioButtonUnitSI.Click += (s, e) => model.WSProject.WSSetting.UnitSystem = Support.Model.UnitSystem.SI;
        this.radioButtonUnitUS.Click += (s, e) => model.WSProject.WSSetting.UnitSystem = Support.Model.UnitSystem.US;
    }


    #endregion

    #region Private Methods
    private async Task LoadFromWSAsync()
    {
        if (NewProjectControlModel != null)
            await NewProjectControlModel.WaterSightLoadAsync();
    }

    private async Task<bool> SaveToWSAsync()
    {
        if (NewProjectControlModel != null)
            return await NewProjectControlModel.WaterSightSaveAsync();

        return false;
    }

    

    private async Task<bool> SaveToFileAsync()
    {
        var saved = false;
        if (NewProjectControlModel != null)
        {
            var saveFileDialog = NewProjectControlModel.GetSaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saved = await NewProjectControlModel.FileSaveAsync(
                    content: NewProjectControlModel.WSProject,
                    fullFilePath: saveFileDialog.FileName);
            }
        }
        else
        {
            Log.Error($"{nameof(NewProjectControlModel)} cannot be null");
        }

        return saved;
    }
    private void OpenGoogleMap(string lat, string lng)
    {
        var googleMapLink = @"https://www.google.com/maps/";

        if (lat?.Length > 3 && lng?.Length > 3)
            googleMapLink += $"search/{lat},{lng}/@{lat},{lng},10z"; // z=10 => Zoom to city level

        var process = new Process();
        process.StartInfo = new ProcessStartInfo(googleMapLink);
        process.StartInfo.UseShellExecute = true;
        process.Start();

        Log.Debug($"Process started for: {googleMapLink}");
    }
    private void OpenTimeZonePage()
    {
        var link = @"https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/default-time-zones";
        var process = new Process();
        process.StartInfo = new ProcessStartInfo(link);
        process.StartInfo.UseShellExecute = true;
        process.Start();

        Log.Debug($"Process started for: {link}");
    }
    #endregion

    #region Public Properties
    public NewProjectControlModel? NewProjectControlModel { get; private set; }
    #endregion
}
