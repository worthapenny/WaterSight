using WaterSight.Domain;
using WaterSight.Excel;
using WaterSight.UI.App;
using WaterSight.UI.Extensions;
using WaterSight.UI.Forms.Support;
using WaterSight.Web.Support;

namespace WaterSight.UI.Controls.Modules.WaterSightModules;

public enum ExcelCategory
{
    Sensors,
    Tanks,
    Pumps,
    Consumption,
    Zones,
    Alerts,
    PowerBI,
}

public partial class ExcelUploadControl : WaterSightControlBase
{
    #region Constructor
    public ExcelUploadControl()
    {
        InitializeComponent();
    }
    #endregion

    #region Overridden Methods
    protected override void InitializeEvents()
    {
        base.InitializeEvents();

        comboBoxExcelCategory.SelectedIndexChanged += (s, e) => ExcelCategoryChanged();
        CheckBoxUseAnalysisDir.CheckedChanged += (s, e) => UpdateExcelFilePath();
    }
    protected override void InitializeVisually()
    {
        base.InitializeVisually();
        UpdateExcelFilePath();
        BindExcelCategoryToCombobox();
    }

    protected override void EnableControls(bool enable)
    {
        labelExcelCategory.Enabled = enable;
        comboBoxExcelCategory.Enabled = enable;
        checkBoxDeleteExistingItems.Enabled = enable;
        labelLocalPath.Enabled = enable;
        textBoxXlFilePath.Enabled = enable;

        base.EnableControls(enable);
    }
    protected override void ValidateInput()
    {
        var messages = new List<string>();
        var prefix = $"{Section} | {SelectedExcelCategory} |";

        if (string.IsNullOrEmpty(SelectedExcelFilePath))
            messages.Add($"{prefix} Excel file path can not be empty");

        if (!File.Exists(SelectedExcelFilePath))
            messages.Add($"{prefix} Invlaid path. Path: '{SelectedExcelFilePath}'");

        else if (Util.IsFileInUse(SelectedExcelFilePath))
            messages.Add($"{prefix} File is in use!");

        if (messages.Count > 0)
        {
            ProgressBarControl.Visible = true;
            ProgressBarControl.Maximum = ProgressBarControl.Value = 1;
            ProgressBarControl.SetState(ProgrssBarState.Error);

            var message = $"Validation Messages: \n";
            message += string.Join(", ", messages);

            using (new CenterWinDialog(ParentForm))
                MessageBox.Show(this, message, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            ProgressBarControl.Visible = false;
            ProgressBarControl.Minimum = ProgressBarControl.Value = 0;
            ProgressBarControl.SetState(ProgrssBarState.Normal);
        }

        
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods    

    private void BindExcelCategoryToCombobox()
    {
        if (comboBoxExcelCategory.DataSource != null) return;
        comboBoxExcelCategory.DataSource = Enum.GetValues(typeof(ExcelCategory));
    }
    private void ExcelCategoryChanged()
    {
        SelectedExcelCategory = (ExcelCategory)comboBoxExcelCategory.SelectedItem;
        UpdateExcelFilePath();
    }
    private void UpdateExcelFilePath()
    {
        string xlFilePath = "No project is active";
        textBoxXlFilePath.Text = xlFilePath;

        if (UIApp.Instance.ActiveProjectFilePath == null) return;

        var xlFileNames = new ExcelFileNames(WaterSightDir);

        switch (SelectedExcelCategory)
        {
            case ExcelCategory.Sensors:
                xlFilePath = xlFileNames.SensorsExcelPath;
                break;

            case ExcelCategory.Tanks:
                xlFilePath = xlFileNames.TanksExcelPath;
                break;

            case ExcelCategory.Pumps:
                xlFilePath = xlFileNames.PumpsExcelPath;
                break;

            case ExcelCategory.Consumption:
                xlFilePath = xlFileNames.CustomerMeterExcelPath;
                break;

            case ExcelCategory.Zones:
                xlFilePath = xlFileNames.ZonesExcelPath;
                break;

            case ExcelCategory.Alerts:
                xlFilePath = xlFileNames.AlertsExcelPath;
                break;

            case ExcelCategory.PowerBI:
                xlFilePath = xlFileNames.PowerBIExcelPath;
                break;

            default:
                throw new Exception($"Unknown Excel category");
        }

        textBoxXlFilePath.Text = xlFilePath;
        SelectedExcelFilePath = xlFilePath;
    }
    #endregion


    #region Public Properties
    public ExcelCategory SelectedExcelCategory { get; private set; }
    public string? SelectedExcelFilePath { get; private set; }

    //public bool UseAnalysisDir
    //{
    //    get => checkBoxUseAnalysisDir.Checked;
    //    set => checkBoxUseAnalysisDir.Checked = value;
    //}
    public bool DeleteExistingServerItems
    {
        get => checkBoxDeleteExistingItems.Checked;
        set => checkBoxDeleteExistingItems.Checked = value;
    }
    public string ExcelFilePath
    {
        get => textBoxXlFilePath.Text;
        set => textBoxXlFilePath.Text = value;
    }
    public bool IsEnabled => CheckBoxEnableAction.Checked;
    #endregion
}
