using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterSight.Domain;
using WaterSight.UI.App;
using WaterSight.UI.Forms.Support;
using WaterSight.UI.Support;

namespace WaterSight.UI.Controls.Modules.WaterSightModules;

public partial class WaterModelControl : WaterSightControlBase, INotifyPropertyChanged
{
    #region Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Constructor
    public WaterModelControl()
    {
        InitializeComponent();
    }
    #endregion


    #region Overridden Methods
    protected override void InitializeEvents()
    {
        base.InitializeEvents();

        // Water Model File Path
        this.textBoxModelFilePath.DataBindings.Add(
            nameof(TextBox.Text),
            this,
            nameof(WaterModelFilePath));

        // Zipped FIle Path
        this.textBoxZipFilePath.DataBindings.Add(
            nameof(TextBox.Text),
            this,
            nameof(ZipFilePath));



        buttonZipModel.Click += async (s, e) => await ZipSelectedModelAsync();
        buttonModelOpen.Click += (s, e) => OpenWaterModel();
        buttonBrowseModel.Click += (s, e) => BrowseWaterModel();
    }



    protected override void InitializeVisually()
    {
        base.InitializeVisually();

        if (UIApp.Instance.ActiveProjectModel != null)
        {
            if (File.Exists(UIApp.Instance.ActiveProjectModel.WSProject.WaterModelPath))
            {
                WaterModelFilePath = UIApp.Instance.ActiveProjectModel.WSProject.WaterModelPath;
            }
            else
            {
                WaterModelFilePath = UseAnalysisDir
                    ? UIApp.Instance.ActiveProjectModel.Folders.AnaModelsDir
                    : UIApp.Instance.ActiveProjectModel.Folders.WsModelsDir;
            }
        }
        //textBoxModelFilePath.Text = filePath;


    }
    protected override void UseAnalysisDirChanged(bool useAnalysisDir)
    {
        base.UseAnalysisDirChanged(useAnalysisDir);

        if (string.IsNullOrEmpty(WaterModelFilePath)) return;

        if (useAnalysisDir)
        {
            var parts = WaterModelFilePath.Split(WaterSightFolders.WaterSightConfigDirName + "\\");
            if (parts.Length > 1)
                WaterModelFilePath = Path.Combine(parts[0], WaterSightFolders.AnalysisDirName, parts[1]);
        }
        else
        {
            var parts = WaterModelFilePath.Split(WaterSightFolders.AnalysisDirName + "\\");
            if (parts.Length > 1)
                WaterModelFilePath = Path.Combine(parts[0], WaterSightFolders.WaterSightConfigDirName, parts[1]);
        }
    }
    private string GetWaterModelDirPath()
    {
        return base.UseAnalysisDir
            ? Path.Combine(WaterSightDir, WaterSightFolders.AnalysisDirName, WaterSightFolders.ModelsWSDirName)
            : Path.Combine(WaterSightDir, WaterSightFolders.WaterSightConfigDirName, WaterSightFolders.ModelsWSDirName);
    }

    protected override void EnableControls(bool enable)
    {
        base.EnableControls(enable);

        labelModelFilePath.Enabled = enable;
        textBoxModelFilePath.Enabled = enable;

        labelZipFilePath.Enabled = enable;
        buttonZipModel.Enabled = enable;
        textBoxZipFilePath.Enabled = enable;
        checkBoxForceZip.Enabled = enable;

        buttonModelOpen.Enabled = enable;
        buttonBrowseModel.Enabled = enable;
        groupBoxModel.Enabled = enable;
    }
    protected override void ValidateInput()
    {
        //return base.ValidateInput();
    }
    protected override void Run()
    {
        base.Run();
    }
    protected override async Task<bool> SaveAsync()
    {
        if(UIApp.Instance.ActiveProjectModel != null)
            return await UIApp.Instance.ActiveProjectModel.SaveProjectFileAsync();

        return false;
    }
    #endregion

    #region Private Methods
    private async Task ZipSelectedModelAsync()
    {
        base.ProgressBarControl.Visible = true;
        base.ProgressBarControl.Style = ProgressBarStyle.Marquee;
        Application.DoEvents();

        try
        {
            if (File.Exists(ZipFilePath) && !checkBoxForceZip.Checked)
            {
                var message = $"SKIPPED created zip file, already exits. Path:{ZipFilePath}";
                Log.Debug(message);

                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "Zip Creation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (File.Exists(ZipFilePath) && checkBoxForceZip.Checked)
            {
                File.Delete(ZipFilePath);
                Log.Information($"Zip file deleted to create a new one. Path: {ZipFilePath}");
            }

            if (!File.Exists(WaterModelFilePath))
            {
                var message = $"Water model file path is not valid. Path:{WaterModelFilePath}";
                Log.Debug(message);

                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "Zip Creation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            var zipCreated = await ZipFileCreator.CreateZipFileAsync(
                fileName: ZipFilePath,
                files: new List<string>() { $"{WaterModelFilePath}.sqlite" });

            if (zipCreated)
            {
                var message = $"Zip file created. Path:{ZipFilePath}";
                Log.Debug(message);

                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "Zip Creation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var message = "Something went wrong, failed to create the zip file. Please check the log Tab";
                Log.Error(message);

                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "Zip Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        finally
        {
            base.ProgressBarControl.Visible = false;
        }
    }
    private void OpenWaterModel()
    {

    }
    private void BrowseWaterModel()
    {
        var initialDir = GetWaterModelDirPath();
        var fileDialog = NewOpenFileDialog(initialDir);

        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            WaterModelFilePath = fileDialog.FileName;
        }
    }
    private OpenFileDialog NewOpenFileDialog(string initialDir)
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.Filter = "WaterGEMS/CAD file (*.wtg)|*.wtg";
        fileDialog.InitialDirectory = initialDir;
        fileDialog.CheckFileExists = true;
        fileDialog.CheckPathExists = true;
        fileDialog.Title = "Select WaterGEMS/CAD file";
        fileDialog.Multiselect = false;

        return fileDialog;
    }
    private void NotifyPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion

    #region Public Properties
    public string WaterModelFilePath
    {
        get => _waterModelFilePath;
        private set
        {
            _waterModelFilePath = value;
            _zipFilePath = $"{value}.zip";
            if(UIApp.Instance.ActiveProjectModel != null)
                UIApp.Instance.ActiveProjectModel.WSProject.WaterModelPath = value;

            NotifyPropertyChanged(nameof(WaterModelFilePath));
            NotifyPropertyChanged(nameof(ZipFilePath));
        }
    }
    public string ZipFilePath => _zipFilePath;
    #endregion

    #region Fields
    private string _waterModelFilePath;
    private string _zipFilePath;


    #endregion
}
