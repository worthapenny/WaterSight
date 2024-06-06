using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using WaterSight.UI.App;
using WaterSight.UI.Auth;
using WaterSight.UI.ControlModels;
using WaterSight.UI.Controls;
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


        WaterSight.UI.Support.Logging.Logging.InMemorySink.Logged += (s, e) => WriteToUI(e);

        // Initialize Control Models
        var signInControlModel = new SignInControlModel();
        this.signInControl.Initialize(signInControlModel);
        this.projectOpenSaveControl.Initialize(signInControlModel);
        this.projectOpenSaveControlWaterModelTab.Initialize(signInControlModel);
        
        if(this.projectOpenSaveControlWaterModelTab.ProjectOpenSaveControlModel != null)
            this.projectOpenSaveControlWaterModelTab.ProjectOpenSaveControlModel.IsOffline = true;

        InitializeEvents();

        //this.listBoxActionsRepo.Items.Clear();
        //this.listBoxActionsRepo.Items.Add(new DragableControlBase() { Title = "Dynamic 1", Section = "Excel" });
        //this.listBoxActionsRepo.Items.Add(new DragableControlBase() { Title = "Dynamic 2", Section = "Excel" });

        foreach (var control in flowLayoutPanelActionsRepo.Controls)
        {
            if (control is DragableControlBase dragableControl)
            {
                dragableControl.AddOrRemoveButtonClicked += (s, e) => { AddOrRemoveControl(dragableControl); };
            }
        }

    }

    private void AddOrRemoveControl(DragableControlBase dragableControl)
    {
        if (dragableControl.CanAddControl)
        {
            flowLayoutPanelActions.Controls.Add(dragableControl);
            flowLayoutPanelActionsRepo.Controls.Remove(dragableControl);
        }
        else
        {
            flowLayoutPanelActions.Controls.Remove(dragableControl);
            flowLayoutPanelActionsRepo.Controls.Add(dragableControl);
        }
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

        UIApp.Instance.DigitalTwinChanged += (s, e) => UpdateParentFormTitle();

        //this.dragableControlBase2.DragEventStarted += (s, e) => flowLayoutPanel1.SuspendLayout();
        //this.dragableControlBase2.DragEventEnded += (s, e) => flowLayoutPanel1.ResumeLayout();
    }

    private void UpdateParentFormTitle()
    {
        var wsProject = UIApp.Instance.ActiveProjectModel;
        if (wsProject == null)
            return;

        var envAndName = string.Empty;
        if (UIApp.Instance.WS != null)
            envAndName = $"[{UIApp.Instance.WS.Options.Env}: {UIApp.Instance.WS.UserInfo.Name}]";
                   
        Invoke(() => Text = $"WaterSight - {wsProject.DigitalTwinId}: {wsProject.Name} {envAndName}");
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
                var busyWindow = new BusyWindow();
                busyWindow.Show(this);
                Application.DoEvents();

                try
                {
                    await Invoke(async () =>
                    {
                        EnableControls(true);
                        SetFormTitle($"{SignInControlModel?.Email}");
                        await projectOpenSaveControl.LoadDigitalTwinsAsync();
                    });

                }
                finally
                {
                    busyWindow.Done();
                }

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
                using (new CenterWinDialog(ParentForm))
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
            //this.splitContainerMain.Enabled = enable;
        });
    }
    private void UpdateUI(Action action)
    {
        if (IsHandleCreated)
            Invoke(action);
        else
            action();

    }
    private void WriteToUI(string e)
    {
        try
        {
            richTextBoxLog.Text += e;
            richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
            richTextBoxLog.ScrollToCaret();
        }
        catch
        {
        }
    }

    private void splitContainerMain_Panel1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(typeof(DragableControlBase)) ?? false)
        {
            e.Effect = DragDropEffects.Move;
        }
    }

    private void splitContainerMain_Panel1_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(typeof(DragableControlBase)) ?? false)
        {
            var control = (DragableControlBase)e.Data.GetData(typeof(DragableControlBase));

        }
    }

    private void splitContainerActions_Panel1_DragOver(object sender, DragEventArgs e)
    {
        Debug.Print("sc Over");
    }

    private void splitContainerActions_Panel1_DragDrop(object sender, DragEventArgs e)
    {
        Debug.Print("sc DragDrop");
    }

    private void listBoxActions_DragEnter(object sender, DragEventArgs e)
    {
        Debug.Print($"DragEnter {DateTime.Now.Millisecond}");
        var a = e.Data?.GetData(typeof(string));
    }

    private void listBoxActions_DragDrop(object sender, DragEventArgs e)
    {
        Debug.Print("DragDrop");
    }

    private void listBoxActions_DragLeave(object sender, EventArgs e)
    {
        Debug.Print("DragDrop leave");
    }

    private void listBoxActions_DragOver(object sender, DragEventArgs e)
    {
        //Debug.Print("DragDrop over");

    }

    private SignInControlModel? SignInControlModel => this.signInControl?.SignInControlModel;


}
