using Serilog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WaterSight.Domain;
using WaterSight.UI.App;
using WaterSight.UI.ControlModels;
using WaterSight.UI.Forms.Support;
using WaterSight.Web.Core;
using WaterSight.Web.DT;

namespace WaterSight.UI.Controls
{
    public partial class ProjectOpenSaveControl : UserControl
    {

        #region Constructor
        public ProjectOpenSaveControl()
        {
            InitializeComponent();

            buttonLoadDigitalTwins.Click += async (s, e) => await LoadDigitalTwinsAsync();
            buttonNewProject.Click += async (s, e) => await LoadWaterSightProjectControlFromWebAsync();
            buttonCreateFolders.Click += (s, e) => CreateWaterSightProjectDirectories();
        }
        #endregion

        #region Public Methods

        public void Initialize(SignInControlModel signInControlModel)
        {
            ProjectOpenSaveControlModel = new ProjectOpenSaveControlModel();
            ProjectOpenSaveControlModel.Initialize(signInControlModel);
        }
        public async Task LoadDigitalTwinsAsync()
        {
            if (ProjectOpenSaveControlModel == null)
            {
                Log.Error($"'{nameof(Initialize)}' must be called first.");
                return;
            }

            var loaded = await ProjectOpenSaveControlModel.BindDigitalTwinComboBoxAsync(this.comboBoxDigitalTwins);
            if (!loaded)
            {
                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, "Failed to load Digital Twin list. See log for more info.", "DT load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Private Methods
        private void CreateWaterSightProjectDirectories()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the root folder to create WaterSight folders template";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.UseDescriptionForTitle = true;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var wsFolders = new WaterSightFolders(folderBrowserDialog.SelectedPath);
                wsFolders.CreateFolders();
                Log.Debug($"Folders created. Path: {folderBrowserDialog.SelectedPath}");

                var message = $"Directories created. Path: {wsFolders.ProjectDir}";
                using (new CenterWinDialog(ParentForm))
                    MessageBox.Show(this, message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task LoadWaterSightProjectControlFromWebAsync()
        {
            if (ProjectOpenSaveControlModel == null)
            {
                Log.Error($"'{nameof(Initialize)}' must be called first.");
                return;
            }


        }

        private void buttonBrowseProjectPath_Click(object sender, EventArgs e)
        {
            if (ProjectOpenSaveControlModel == null)
            {
                Log.Error($"'{nameof(Initialize)}' must be called first.");
                return;
            }

            var openFileDialog = ProjectOpenSaveControlModel.GetOpenFileDialog(this.textBoxProjectPath.Text);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Invoke(async () =>
                {
                    UIApp.Instance.ActiveProjectFilePath = openFileDialog.FileName;
                    var newProjectControl = await LoadWaterSightProjectControlFromFileAsync();
                    if (newProjectControl != null)
                    {
                        ShowUserControlOnForm(newProjectControl, "New Digital Twin");
                        this.textBoxProjectPath.Text = ProjectOpenSaveControlModel.ProjectFilePath;
                    }
                });
            }
            else
            {
                Log.Debug($"User canceled the OpenFileDialog box");
            }
        }

        private void ShowUserControlOnForm(Control control, string title)
        {
            var size = control.Size;
            size.Width += 16;
            size.Height += 50;
            var centeredForm = new CenteredToolForm(
                title: title,
                parentForm: ParentForm,
                control: control,
                size: size);

            centeredForm.ShowDialog();
        }

        private async Task<NewProjectControl?> LoadWaterSightProjectControlFromFileAsync()
        {
            // Make sure the Control model is initialized
            if (ProjectOpenSaveControlModel == null)
            {
                Debugger.Break();
                throw new Exception("ProjectOpenSaveControlModel can't be null");
            }

            // Make sure project file path is valid and provided
            var projectFilePath = UIApp.Instance.ActiveProjectFilePath;
            if (projectFilePath == null || !File.Exists(projectFilePath))
            {
                var message = $"Given project file path is not valid. Path:{projectFilePath}";
                Debugger.Break();
                throw new FileNotFoundException(message);
            }


            var wsProject = WaterSightProject.LoadFromJson(projectFilePath);
            if (wsProject == null)
            {
                var message = $"Failed to load WaterSight project from given path. Path: {projectFilePath}";
                MessageBox.Show(this, message, "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var newProjectControl = await GetNewProjectControlAsync(ws: null, jsonFilePath: projectFilePath);
            return newProjectControl;

        }

        private async Task<NewProjectControl> GetNewProjectControlAsync(WS? ws, string? jsonFilePath)
        {

            var newProjectControl = new NewProjectControl();
            var newProjectControlModel = new NewProjectControlModel();
            await newProjectControlModel.LoadWaterSightProjectAsync(ws, jsonFilePath);
            newProjectControl.Initialize(newProjectControlModel);

            return newProjectControl;
        }



        //private async Task LoadProjectControlAsync()
        //{
        //    // Make sure the Control model is initialized
        //    if (ProjectOpenSaveControlModel == null)
        //    {
        //        Debugger.Break();
        //        throw new Exception("ProjectOpenSaveControlModel can't be null");
        //    }

        //    // Make sure project file path is valid and provided
        //    var projectFilePath = UIApp.Instance.ActiveProjectFilePath;
        //    if (projectFilePath == null || File.Exists(projectFilePath))
        //    {
        //        var message = $"Given project file path is not valid. Path:{projectFilePath}";
        //        Debugger.Break();
        //        throw new FileNotFoundException(message);
        //    }


        //    var wsProject = WaterSightProject.LoadFromJson(projectFilePath);
        //    if (wsProject == null)
        //    {
        //        var message = $"Failed to load WaterSight project from given path. Path: {projectFilePath}";
        //        MessageBox.Show(this, message, "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }



        //    Log.Information($"Selected project file: {projectFilePath}");
        //    ProjectOpenSaveControlModel.ProjectFilePath = projectFilePath;

        //    // Check if Offline is true
        //    // when so, no need to talk to WaterSight
        //    if (ProjectOpenSaveControlModel.IsOffline)
        //    {
        //        ProjectOpenSaveControlModel.SelectedProject.WSProject = WaterSightProject.LoadFromJson(projectFilePath);
        //        return;
        //    }

        //    // Get all the Digital Twins
        //    var dtMap = await ProjectOpenSaveControlModel.GetDigitalTwinConfigMapAsync();

        //    // Check if given id is valid
        //    if (!dtMap.TryGetValue(wsProject.WSSetting.Info.DTID, out DigitalTwinConfig? projectDT))
        //    {
        //        var message = $"Somehome no Digital Twin found for the ID = '{wsProject.WSSetting.Info.DTID}'";
        //        Debugger.Break();
        //        throw new ArgumentException(message);
        //    }

        //    ProjectOpenSaveControlModel.SelectedDTConfig = projectDT;
        //    Invoke(() => { comboBoxDigitalTwins.SelectedItem = projectDT; Application.DoEvents(); });
        //    await LoadProjectControlAsync(loadFromProjectFile: true);
        //}

        //private async Task LoadProjectControlAsync(bool loadFromProjectFile = false)
        //{
        //    if (ProjectOpenSaveControlModel?.WS == null)
        //    {
        //        Log.Error($"'{nameof(Initialize)}' must be called first.");
        //        return;
        //    }

        //    if (ProjectOpenSaveControlModel?.WS != null && ProjectOpenSaveControlModel?.SelectedDTConfig == null)
        //    {
        //        var message = $"Digital Twin has not been selected, you must select a Digital Twin";
        //        var dialogResult = MessageBox.Show(this, message, "No Digital Twin selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    var digitalTwinId = ProjectOpenSaveControlModel.SelectedDTConfig.ID;

        //    // Update WS per selected DT in the combobox
        //    ProjectOpenSaveControlModel.WS.Options.DigitalTwinId = digitalTwinId;

        //    var newProjectControl = new NewProjectControl();
        //    var newProjectControlModel = new NewProjectControlModel(ws: ProjectOpenSaveControlModel.WS);


        //    newProjectControlModel.DigitalTwinIdChanged += (s, e) => SelectedDigitalTwinChangedAsync(e);

        //    // Get all the Digital Twins
        //    var dtMap = await ProjectOpenSaveControlModel.GetDigitalTwinConfigMapAsync();

        //    // Check if given id is valid
        //    if (!dtMap.TryGetValue(digitalTwinId, out DigitalTwinConfig? projectDT))
        //    {
        //        var message = $"Somehome no Digital Twin found for the ID = '{digitalTwinId}'";
        //        Debugger.Break();
        //        throw new ArgumentException(message);
        //    }

        //    //
        //    // load WaterSightProject            

        //    // if project is opened from file, update the ID and the DT:
        //    if (loadFromProjectFile)
        //    {
        //        var filePath = ProjectOpenSaveControlModel.ProjectFilePath;
        //        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        //        {
        //            var wsp = WaterSightProject.LoadFromJson(filePath);

        //            if (wsp != null)
        //            {
        //                newProjectControlModel.WSProject = wsp;
        //                newProjectControlModel.ProjectFilePath = ProjectOpenSaveControlModel.ProjectFilePath;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        newProjectControlModel.WSProject = await WaterSightProject.LoadFromWeb(
        //            ws: ProjectOpenSaveControlModel.WS,
        //            dtConfig: ProjectOpenSaveControlModel.SelectedDTConfig);
        //    }

        //    // initialize the  UI and the model based on the selected DT
        //    //newProjectControlModel.DTConfig = projectDT;

        //    await newProjectControlModel.InitializeAsync();
        //    newProjectControl.Initialize(newProjectControlModel);


        //    // Update the selected project
        //    // so that other UI can react
        //    ProjectOpenSaveControlModel.SelectedProject = newProjectControlModel;

        //    var size = newProjectControl.Size;
        //    size.Width += 16;
        //    size.Height += 50;
        //    var centeredForm = new CenteredToolForm(
        //        title: "New Digital Twin",
        //        parentForm: ParentForm,
        //        control: newProjectControl,
        //        size: size);

        //    centeredForm.ShowDialog();

        //    // Update UI
        //    if (File.Exists(newProjectControlModel.ProjectFilePath))
        //        Invoke(() => this.textBoxProjectPath.Text = ProjectOpenSaveControlModel.ProjectFilePath);



        //}

        private async Task SelectedDigitalTwinChangedAsync(int dtId)
        {
            if (this.ProjectOpenSaveControlModel == null)
            {
                Debugger.Break();
                return;
            }

            var dtConfigMap = await this.ProjectOpenSaveControlModel.GetDigitalTwinConfigMapAsync();
            if (dtConfigMap?.TryGetValue(dtId, out var dtConfig) ?? false)
            {
                try
                {
                    var selecteDtConfig = new KeyValuePair<string, DigitalTwinConfig>($"{dtConfig.ID}: {dtConfig.Name}", dtConfig);
                    Invoke(() => this.comboBoxDigitalTwins.SelectedItem = selecteDtConfig);

                    using (new CenterWinDialog(ParentForm))
                    {
                        var message = $"'{dtConfig.ID}: {dtConfig.Name}' is now selected.";
                        MessageBox.Show(this, message, $"Current = {dtConfig.Name}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Log.Debug(message);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"...while setting the active DT");
                    Debugger.Break();
                }
            }
        }

        private void comboBoxDigitalTwins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ProjectOpenSaveControlModel?.WS != null)
            {
                try
                {
                    var selectedDT = (KeyValuePair<string, DigitalTwinConfig>)this.comboBoxDigitalTwins.SelectedItem;

                    if (selectedDT.Value != null)
                    {
                        ProjectOpenSaveControlModel.SelectedDTConfig = selectedDT.Value;
                        ProjectOpenSaveControlModel.WS.UpdateAttributes(
                            id: selectedDT.Value.ID,
                            env: ProjectOpenSaveControlModel.SignInControlModel?.ServerEnvironment ?? Env.Prod);
                    }
                }
                catch (Exception ex)
                {
                    var message = $"...while loading the Digital Twins to the dropdown list... See logs for the details.";
                    Log.Error(ex, message);

                    using (new CenterWinDialog(ParentForm))
                        MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                }

            }
        }



        #endregion

        #region Private Properties
        private DigitalTwinConfig SelectedDTConfig => (DigitalTwinConfig)comboBoxDigitalTwins.SelectedItem;
        #endregion

        #region Public Peroperties
        public ProjectOpenSaveControlModel? ProjectOpenSaveControlModel { get; private set; }
        #endregion
    }
}
