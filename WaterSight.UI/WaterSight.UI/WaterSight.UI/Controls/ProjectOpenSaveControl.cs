using Serilog;
using System.Runtime.CompilerServices;
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
            buttonNewProject.Click += async (s, e) => await LoadNewProjectControlAsync();
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
                MessageBox.Show(this, "Failed to load Digital Twin list. See log for more info.", "DT load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Private Methods

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
                ProjectOpenSaveControlModel.ProjectFilePath = openFileDialog.FileName;
                Log.Information($"Selected project file: {ProjectOpenSaveControlModel.ProjectFilePath}");



                Invoke(() => this.textBoxProjectPath.Text = ProjectOpenSaveControlModel.ProjectFilePath);

            }
            else
            {
                Log.Debug($"User canceled the OpenFileDialog box");
            }
        }

        private async Task LoadNewProjectControlAsync()
        {
            if (ProjectOpenSaveControlModel == null
                || ProjectOpenSaveControlModel?.WS == null)
            {
                Log.Error($"'{nameof(Initialize)}' must be called first.");
                return;
            }

            if(ProjectOpenSaveControlModel?.SelectedDTConfig == null)
            {
                var message = $"Digital Twin has not been selected, would you still like to continue?";
                var dialogResult = MessageBox.Show(this, message, "No Digital Twin selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            //var saveFileDialog = ProjectOpenSaveControlModel.GetSaveFileDialog();
            //if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    ProjectOpenSaveControlModel.ProjectFilePath = saveFileDialog.FileName;
            //    Log.Information($"Selected project file: {ProjectOpenSaveControlModel.ProjectFilePath}");

            //    //this.textBoxProjectPath.Text = new FileInfo(ProjectOpenSaveControlModel.ProjectFilePath)?.Directory?.FullName;
            //    Invoke(() => this.textBoxProjectPath.Text = ProjectOpenSaveControlModel.ProjectFilePath);


            //    //var newProjectControl = new NewProjectControl();
            //    //var newProjectControlModel = new NewProjectControlModel(
            //    //    ws: ProjectOpenSaveControlModel.WS,
            //    //    dtConfig: ProjectOpenSaveControlModel.SelectedDTConfig,
            //    //    projectFilePath: ProjectOpenSaveControlModel.ProjectFilePath);

            //    //await newProjectControlModel.InitializeAsync();

            //    //newProjectControl.Initialize(newProjectControlModel);

            //    //var centeredForm = new CenteredToolForm(
            //    //    title: "New Digital Twin",
            //    //    parentForm: ParentForm,
            //    //    control: newProjectControl,
            //    //    size: new Size(400, 450));

            //    //centeredForm.ShowDialog();
            //}
            //else
            //{
            //    Log.Debug($"User canceled the SaveFileDialog box");
            //}

            var newProjectControl = new NewProjectControl();
            var newProjectControlModel = new NewProjectControlModel(
                ws: ProjectOpenSaveControlModel.WS,
                dtConfig: ProjectOpenSaveControlModel.SelectedDTConfig);

            await newProjectControlModel.InitializeAsync();

            newProjectControl.Initialize(newProjectControlModel);

            var centeredForm = new CenteredToolForm(
                title: "New Digital Twin",
                parentForm: ParentForm,
                control: newProjectControl,
                size: new Size(400, 450));

            centeredForm.ShowDialog();

            if(File.Exists(newProjectControlModel.ProjectFilePath) )
                Invoke(() => this.textBoxProjectPath.Text = ProjectOpenSaveControlModel.ProjectFilePath);

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
                    MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                }

            }
        }

        #endregion






        #region Public Peroperties
        public ProjectOpenSaveControlModel? ProjectOpenSaveControlModel { get; private set; }
        #endregion
    }
}
