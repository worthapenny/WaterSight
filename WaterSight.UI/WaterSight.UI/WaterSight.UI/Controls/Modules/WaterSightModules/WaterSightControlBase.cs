using WaterSight.Domain;
using WaterSight.UI.App;

namespace WaterSight.UI.Controls.Modules.WaterSightModules;

public partial class WaterSightControlBase : DragableControlBase
{
    #region Constructor
    public WaterSightControlBase()
    {
        InitializeComponent();
    }
    #endregion

    #region Overridden Methods
    protected override void InitializeEvents()
    {
        CheckBoxUseAnalysisDir.CheckedChanged += (s, e) => UseAnalysisDirChanged(CheckBoxUseAnalysisDir.Checked);
        base.InitializeEvents();
    }

    protected override void EnableControls(bool enable)
    {
        base.EnableControls(enable);
        CheckBoxUseAnalysisDir.Enabled = enable;
    }
    protected virtual void UseAnalysisDirChanged(bool useAnalysisDir)
    {
    }
    #endregion

    #region Private Methods
    private string GetWaterSightDir()
    {
        if (UIApp.Instance.ActiveProjectModel == null)
            return string.Empty;

        return UIApp.Instance.ActiveProjectModel.Folders.ProjectDir;
    }
    #endregion

    #region Public Properties
    public bool UseAnalysisDir
    {
        get
        {
            var useAnalysisDir = CheckBoxUseAnalysisDir.Checked;
            UseAnalysisDirChanged(useAnalysisDir);
            return useAnalysisDir;
        }
    }
    #endregion

    #region Protected Properties
    protected string WaterSightDir => GetWaterSightDir();
    #endregion

    #region Fields
    //private string? _waterSightDir;
    #endregion
}
