using WaterSight.UI.Auth;
using WaterSight.UI.ControlModels;
using WaterSight.Web.Core;

namespace WaterSight.UI.Controls;

public partial class SignInControl : UserControl
{
    #region Constructor

    public SignInControl()
    {
        InitializeComponent();
    }
    #endregion



    #region Public Methods
    public void Initialize(SignInControlModel signInControlModel)
    {
        SignInControlModel = signInControlModel;
        SignInControlModel.AuthEvent += SignInControlModel_AuthEvent;

        this.radioButtonDev.Checked = false;
        this.radioButtonQA.Checked = false;
        this.radioButtonProd.Checked = true;
        this.buttonSignIn.Click += async (s, e) => await SignInControlModel.SignInOrSignOutAsync();
    }
    #endregion

    #region Private Methods

    private void SignInControlModel_AuthEvent(object? sender, AuthEvent e)
    {
        switch (e)
        {
            case AuthEvent.LoggingIn:
                ChangeSignInButtonText("Signing In...");
                EnableRadioButtons(false);                
                break;

            case AuthEvent.LoggingOut:
                ChangeSignInButtonText("Signing Out...");
                EnableRadioButtons(false);
                break;

            case AuthEvent.LoggingError:
                MessageBox.Show(this, "Logging in was not successful. Please review the log or try again", "Failed to sign in.", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                ChangeSignInButtonText("Sign In...");
                break;

            case AuthEvent.LoggedIn:
                ChangeSignInButtonText("Sign Out");
                EnableRadioButtons(false);
                break;

            case AuthEvent.LoggedOut:
                ChangeSignInButtonText("Sign In");
                EnableRadioButtons(true);
                break;

            case AuthEvent.LoggingOutError:
                ChangeSignInButtonText("Err Sign Out");
                MessageBox.Show(this, "Logging out was not successful. Please review the log or try again", "Failed to sign out.", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                break;

            case AuthEvent.RefreshStarted:
                ChangeSignInButtonText("Refreshing...");
                //SynchronizationContext.Post(_ =>
                //    this.buttonSignIn.Text = "Refreshing...", null);
                break;


            case AuthEvent.RefreshCompleted:
                ChangeSignInButtonText("Refreshed");
                //SynchronizationContext.Post(_ =>
                //    this.buttonSignIn.Text = "Sign Out", null);
                break;


            case AuthEvent.RefreshFailed:
                ChangeSignInButtonText("Refresh Failed");
                //SynchronizationContext.Post(_ =>
                //    this.buttonSignIn.Text = "Refresh Failed", null);
                break;


            default:
                throw new ArgumentException($"Unhandled Auth Event of {e}");
        }

        Application.DoEvents();
    }

    private void ChangeSignInButtonText(string text)
    {
        Invoke(() => { this.buttonSignIn.Text = text; });
    }

    private void EnableRadioButtons(bool enable)
    {
        this.radioButtonDev.Enabled = enable;
        this.radioButtonQA.Enabled = enable;
        this.radioButtonProd.Enabled = enable;
        this.checkBoxEuRegion.Enabled = enable;
    }
    

    private void radioButtonProd_CheckedChanged(object sender, EventArgs e)
    {
        if (SignInControlModel != null)
            SignInControlModel.ServerEnvironment = Env.Prod;
    }

    private void radioButtonQA_CheckedChanged(object sender, EventArgs e)
    {
        if (SignInControlModel != null)
            SignInControlModel.ServerEnvironment = Env.Qa;
    }

    private void radioButtonDev_CheckedChanged(object sender, EventArgs e)
    {
        if (SignInControlModel != null)
            SignInControlModel.ServerEnvironment = Env.Dev;
    }

    private void checkBoxEuRegion_CheckedChanged(object sender, EventArgs e)
    {
        if (SignInControlModel != null)
            SignInControlModel.IsEURegion = this.checkBoxEuRegion.Checked;
    }


    #endregion

    #region Public Properties
    public SignInControlModel? SignInControlModel { get; set; }
    public SynchronizationContext SynchronizationContext { get; set; } = new SynchronizationContext();
    #endregion
}
