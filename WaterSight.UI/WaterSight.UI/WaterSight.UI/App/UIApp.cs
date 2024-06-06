using WaterSight.UI.ControlModels;
using WaterSight.Web.Core;

namespace WaterSight.UI.App;

public class UIApp
{

    public event EventHandler<int>? DigitalTwinChanged;

    #region Singleton Pattern
    private static UIApp? instance;
    private UIApp() { }
    public static UIApp Instance => instance ??= new UIApp();
    #endregion

    #region Public Properties

    public int ActiveDiitialTwinId
    {
        get => _digitalTwinId;
        set { 
            _digitalTwinId = value; 
            if(WS != null)
                WS.Options.DigitalTwinId = value;

            DigitalTwinChanged?.Invoke(this, value); }
    }
    public WS? WS { get; set; }
    public NewProjectControlModel? ActiveProjectModel { get; set; }
    public string? ActiveProjectFilePath { get; set; }
    #endregion

    #region Fields

    private int _digitalTwinId;
    #endregion
}
