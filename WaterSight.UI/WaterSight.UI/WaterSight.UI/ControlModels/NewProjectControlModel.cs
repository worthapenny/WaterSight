using Newtonsoft.Json;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using WaterSight.Domain;
using WaterSight.UI.App;
using WaterSight.UI.Support.Model;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.UI.ControlModels;

public class NewProjectControlModel : INotifyPropertyChanged  //ReadWriteBase
{
    #region Constants
    public const double TypicalMaxPressureInPSI = 100;
    public const double TypicalMinPressureInPSI = 25;
    public const double TypicalTargetEfficiency = 75;
    public const double TypicalProductionCostDollarPerMgal = 1;
    public const double TypicalTariffDollarPerMgal = 1;
    public const double TypicalEnergyCostDollarPerMgal = 1;
    #endregion

    #region Public Events
    //public event EventHandler<int> DigitalTwinIdChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Constructor
    public NewProjectControlModel()
    //WS ws)//,
    //DigitalTwinConfig dtConfig)
    //: base(ws) //, dtConfig)
    {
        //WSProject = new WaterSightProject();
    }
    #endregion

    #region Public Overridden Methods
    //public async Task<bool> WaterSightLoadAsync(WS ws)
    //{
    //    var success = true;

    //    if (DTConfig != null)
    //    {
    //        var dtConfig = await ws.DigitalTwin.GetDigitalTwinAsync();
    //        if (dtConfig != null)
    //            DTConfig = dtConfig;
    //        else
    //            success = false;
    //    }

    //    return success;
    //}
    #endregion
    
    #region Public Methods
    public async Task<bool> SaveToWebAsync(WS ws, WaterSightProject wsProject)
    {
        if (ws == null)
        {
            Log.Information($"'{nameof(ws)}' cannot be null.");
            Debugger.Break();
            return false;
        }


        var success = true;


        // Update Name and Description
        await ws.DigitalTwin.UpdateDigitalTwinNameAndDescriptionAsync(
            name: wsProject.WSSetting.Info.Name,
            description: wsProject.WSSetting.Info.Description);

        // Update EPSG
        wsProject.WSSetting.Geo.CustomersEPSG = wsProject.WSSetting.Geo.SensorEPSG;
        wsProject.WSSetting.Geo.SmartMetersEPSG = wsProject.WSSetting.Geo.SensorEPSG;
        wsProject.WSSetting.Geo.WorkOrdersEPSG = wsProject.WSSetting.Geo.SensorEPSG;

        await ws.Settings.CoordinateSystems.SetSensors(epsg: wsProject.WSSetting.Geo.SensorEPSG);
        await ws.Settings.CoordinateSystems.SetCustomers(epsg: wsProject.WSSetting.Geo.CustomersEPSG);
        await ws.Settings.CoordinateSystems.SetSmartMeters(epsg: wsProject.WSSetting.Geo.SmartMetersEPSG);
        await ws.Settings.CoordinateSystems.SetWorkOrders(epsg: wsProject.WSSetting.Geo.WorkOrdersEPSG);

        // Lat / Long
        await ws.Settings.Location.SetLocation(
            latitude: WSProject.WSSetting.Geo.Latitude,
            longitude: wsProject.WSSetting.Geo.Longitude);

        // Timezone
        await ws.Settings.TimeZone.SetTimezone(
            timeZone: wsProject.WSSetting.Geo.TimeZoneString);

        // Units [SI is default so no need to change]
        if (wsProject.WSSetting.UnitSystem == UnitSystem.US)
            await ws.Settings.Units.SetToUSUnitsAsync();

        // Services
        var pressureUnit = "";
        await ws.Settings.ServiceExpectations.SetMaxPressure(
            pressure: wsProject.WSSetting.Operations.MaxPressure,
            unit: pressureUnit);
        await ws.Settings.ServiceExpectations.SetMinPressure(
            pressure: wsProject.WSSetting.Operations.MinPressure,
            unit: pressureUnit);
        await ws.Settings.ServiceExpectations.SetTargetPumpEfficiency(
            effi: wsProject.WSSetting.Operations.TargetPumpEfficiency);

        // Costs
        var costPerVolUnit = "";
        var costPerPowerUnit = "";
        await ws.Settings.Costs.SetAvgVolumeticProductionCost(
            cost: wsProject.WSSetting.Operations.AverageVolumeProductionCost,
            unit: costPerVolUnit);
        await ws.Settings.Costs.SetAvgVolumetricTarrif(
            cost: wsProject.WSSetting.Operations.AverageVolumeTariff,
            unit: costPerVolUnit);
        await ws.Settings.Costs.SetAvgEnergyCost(
            cost: wsProject.WSSetting.Operations.AverageEnergyCost,
            unit: costPerPowerUnit);


        return success;
    }
    

    

    public async Task LoadWaterSightProjectAsync(WS? ws, string? jsonFilePath)
    {
        if (ws == null && string.IsNullOrEmpty(jsonFilePath))
        {
            var message = $"Both arguments ({nameof(ws)} or {nameof(jsonFilePath)}) cannot be null";
            Debugger.Break();
            throw new ArgumentNullException(message);
        }

        if (WSProject == null)
        {
            WaterSightProject? wsProject = null;
            if (!string.IsNullOrEmpty(jsonFilePath))
                wsProject = WaterSightProject.LoadFromJson(jsonFilePath);

            else if (ws != null)
                wsProject = await WaterSightProject.LoadFromWebAsync(ws);

            if (wsProject == null)
            {
                var message = $"Fail to load WaterSightProject.";
                Debugger.Break();
                throw new ApplicationException(message);
            }
            else
                WSProject = wsProject;

        }
    }

    public void SetDigitalTwin(int id)
    {
        WSProject.WSSetting.Info.DTID = id;
        UIApp.Instance.ActiveProjectModel = this;
        UIApp.Instance.ActiveDiitialTwinId = id; 
    }

    public FolderBrowserDialog GetOpenFolderDialog(string initialDirectory)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();

        if (Directory.Exists(initialDirectory))
            folderBrowserDialog.InitialDirectory = initialDirectory;

        folderBrowserDialog.Description = "Select the project folder (where the template folders exits) ";
        folderBrowserDialog.ShowNewFolderButton = false;
        folderBrowserDialog.UseDescriptionForTitle = true;

        return folderBrowserDialog;
    }

    public SaveFileDialog GetSaveFileDialog()
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "WS Project files (*.ws.json)|*.ws.json";
        saveFileDialog.CheckFileExists = false;
        saveFileDialog.Title = "Create WaterSight Project File";

        return saveFileDialog;
    }
    public async Task<bool> FileSaveAsync(object content, string fullFilePath)
    {
        var success = true;

        try
        {
            var jsonContent = JsonConvert.SerializeObject(content, Formatting.Indented);
            await File.WriteAllTextAsync(fullFilePath, jsonContent);

            Log.Information($"Project info saved. Path: {fullFilePath}");
        }
        catch (Exception ex)
        {
            Debugger.Break();
            Log.Error(ex, "...while writing to the file");
            success = false;
        }

        return success;
    }
    public async Task<bool> SaveProjectFileAsync()
    {
        if(string.IsNullOrEmpty(UIApp.Instance.ActiveProjectFilePath))
            return false;

        if (WSProject == null)
            return false;

        var saved = await FileSaveAsync(
                     content: WSProject,
                     fullFilePath: UIApp.Instance.ActiveProjectFilePath);

        return saved;
    }
    #endregion

    private void NotifyPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    

    #region Public Properties
    public WaterSightProject WSProject { get; set; }


    public string DTInfo => $"{DigitalTwinId}: {Name}";

    
    public WaterSightFolders Folders => _wsFolders ??= new WaterSightFolders(ProjectPath);

    public string ModelFilePath
    {
        get => WSProject.ProjectDir;
        set { WSProject.ProjectDir = value; NotifyPropertyChanged(nameof(ProjectPath)); }
    }

    public string ProjectPath
    {
        get => WSProject.ProjectDir;
        set { WSProject.ProjectDir = value; NotifyPropertyChanged(nameof(ProjectPath)); }
    }
    public int DigitalTwinId
    {
        get => WSProject.WSSetting.Info.DTID;
        set
        {
            WSProject.WSSetting.Info.DTID = value;
            if (UIApp.Instance.WS != null) 
                UIApp.Instance.WS.Options.DigitalTwinId = value;
            NotifyPropertyChanged(nameof(DigitalTwinId));
        }
    }

    public string Name
    {
        get => WSProject.WSSetting.Info.Name;
        set { WSProject.WSSetting.Info.Name = value; NotifyPropertyChanged(nameof(Name)); }
    }
    public string Description
    {
        get => WSProject.WSSetting.Info.Description;
        set { WSProject.WSSetting.Info.Description = value; NotifyPropertyChanged(nameof(Description)); }
    }
    private string _projectShortName;
    public string ProjectShortName
    {
        get => _projectShortName;
        set { _projectShortName = value; NotifyPropertyChanged(nameof(ProjectShortName)); }
    }
    public int EpsgCOde
    {
        get => WSProject.WSSetting.Geo.SensorEPSG;
        set
        {
            WSProject.WSSetting.Geo.SensorEPSG = value;
            WSProject.WSSetting.Geo.SmartMetersEPSG = value;
            WSProject.WSSetting.Geo.WorkOrdersEPSG = value;
            WSProject.WSSetting.Geo.CustomersEPSG = value;
            NotifyPropertyChanged(nameof(ProjectPath));
        }
    }
    public string TimeZoneName
    {
        get => WSProject.WSSetting.Geo.TimeZoneString;
        set { WSProject.WSSetting.Geo.TimeZoneString = value; NotifyPropertyChanged(nameof(TimeZoneName)); }
    }
    public float Latitude
    {
        get => WSProject.WSSetting.Geo.Latitude;
        set { WSProject.WSSetting.Geo.Latitude = value; NotifyPropertyChanged(nameof(Latitude)); }
    }
    public float Longitude
    {
        get => WSProject.WSSetting.Geo.Longitude;
        set { WSProject.WSSetting.Geo.Longitude = value; NotifyPropertyChanged(nameof(Longitude)); }
    }
    public UnitSystem UnitSystem
    {
        get => WSProject.WSSetting.UnitSystem;
        set { WSProject.WSSetting.UnitSystem = value; NotifyPropertyChanged(nameof(UnitSystem)); }
    }
    public double MaxPressure
    {
        get => WSProject.WSSetting.Operations.MaxPressure;
        set { WSProject.WSSetting.Operations.MaxPressure = value; NotifyPropertyChanged(nameof(MaxPressure)); }
    }
    public double MinPressure
    {
        get => WSProject.WSSetting.Operations.MinPressure;
        set { WSProject.WSSetting.Operations.MinPressure = value; NotifyPropertyChanged(nameof(MinPressure)); }
    }
    public double TargetEfficiency
    {
        get => WSProject.WSSetting.Operations.TargetPumpEfficiency;
        set { WSProject.WSSetting.Operations.TargetPumpEfficiency = value; NotifyPropertyChanged(nameof(TargetEfficiency)); }
    }
    public double ProductionCost
    {
        get => WSProject.WSSetting.Operations.AverageVolumeProductionCost;
        set { WSProject.WSSetting.Operations.AverageVolumeProductionCost = value; NotifyPropertyChanged(nameof(ProductionCost)); }
    }
    public double Tariff
    {
        get => WSProject.WSSetting.Operations.AverageVolumeTariff;
        set { WSProject.WSSetting.Operations.AverageVolumeTariff = value; NotifyPropertyChanged(nameof(Tariff)); }
    }
    public double EnergyCost
    {
        get => WSProject.WSSetting.Operations.AverageEnergyCost;
        set { WSProject.WSSetting.Operations.AverageEnergyCost = value; NotifyPropertyChanged(nameof(EnergyCost)); }
    }



    #endregion

    #region Fields
    private WaterSightFolders _wsFolders;
    #endregion
}
