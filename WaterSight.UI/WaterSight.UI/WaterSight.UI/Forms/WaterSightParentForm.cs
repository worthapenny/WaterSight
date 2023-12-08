using Serilog;
using WaterSight.UI.Auth;
using WaterSight.UI.ControlModels;
using WaterSight.UI.Forms.Support;
using WaterSight.UI.Support;
using WaterSight.Web.Core;

namespace WaterSight.UI.Forms;

public partial class WaterSightParentForm : Form
{
    public WaterSightParentForm()
    {
        InitializeComponent();

        EnableControls(false);
        Load += async (s, e) => await AfterFormLoadEventAsync();
    }

    private async Task AfterFormLoadEventAsync()
    {
        //await Task.Run(async () =>
        //{
        //    //await Task.Delay(100);


        //});

        // Initialize Control Models
        var signInControlModel = new SignInControlModel();
        this.signInControl.Initialize(signInControlModel);
        this.projectOpenSaveControl.Initialize(signInControlModel);

        InitializeEvents();
    }



    private void InitializeEvents()
    {
        if (this.signInControl?.SignInControlModel == null)
        {
            Log.Error($"{nameof(this.signInControl.SignInControlModel)}' cannot be null.");
            return;
        }


        this.signInControl.SignInControlModel.AuthEvent += async (s, e) =>
        {
            await AuthEventChangedAsync(e);
        };
    }
    private void InitializeVisually()
    {

    }

    private async Task AuthEventChangedAsync(AuthEvent e)
    {
        EnableControls(false);

        switch (e)
        {
            case AuthEvent.LoggingIn:
                SetFormTitle($"Signing In...");
                break;

            case AuthEvent.LoggingOut:
                SetFormTitle($"Signing Out...");
                break;

            case AuthEvent.LoggingError:
                SetFormTitle($"Signing Error...");
                break;

            case AuthEvent.LoggedIn:
                SetFormTitle($"{SignInControlModel?.Email}");
                var busyWindow = new BusyWindow();
                busyWindow.Show(this);
                try
                {
                    await projectOpenSaveControl.LoadDigitalTwinsAsync();
                }
                finally
                {
                    busyWindow.Done();
                }
                EnableControls(true);
                break;

            case AuthEvent.LoggedOut:
                SetFormTitle($"Logged Out");
                break;

            case AuthEvent.LoggingOutError:
                SetFormTitle($"LoggingOut Error");
                break;

            case AuthEvent.RefreshStarted:
                SetFormTitle($"Refreshing...");
                break;
            case AuthEvent.RefreshCompleted:
                EnableControls(true);
                break;

            case AuthEvent.RefreshFailed:
                SetFormTitle($"Refreshing Token Failed...");
                break;

            default:
                MessageBox.Show(this, $"Unsupported Auth Event {e}", "Error");
                break;
        }
    }

    private void SetFormTitle(string title)
    {
        var selectedEnv = Enum.GetName(typeof(ServerEnvironment), SignInControlModel?.ServerEnvironment ?? Env.Prod);
        var ws = "WaterSight";
        var text = $"{selectedEnv} - {ws} - {title}";

        UpdateUI(() => Text = text);
    }

    private void EnableControls(bool enable)
    {
        UpdateUI(() =>
        {
            this.projectOpenSaveControl.Enabled = enable;
            this.splitContainerMain.Enabled = enable;
        });
    }
    private void UpdateUI(Action action)
    {
        if (IsHandleCreated)
            Invoke(action);
        else
            action();

    }

    private SignInControlModel? SignInControlModel => this.signInControl?.SignInControlModel;


}
