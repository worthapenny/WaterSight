using Microsoft.Extensions.Configuration;
using Serilog;
using WaterSight.Authenticator.Auth;

namespace WaterSight.Authenticator.ControlModel;


public class SignInControlModel : IDisposable
{
    #region Constants
    public static int REFRESH_LOGIN_INTERVAL_MINUTES = 30;
    #endregion

    #region Public Events
    public event EventHandler<AuthEvent>? AuthEvent;
    #endregion

    #region Constructor
    public SignInControlModel()
    {
        AuthEvent += (s, e) => AuthEventChanged(e);
    }


    #endregion

    #region Public Methods
    public async Task<bool> SignInOrSignOutAsync()
    {
        if (IsSignedIn)
            return await SignOutAsync();
        else
            return await SignInAsync();
    }
    public async Task<bool> SignInAsync()
    {
        AuthEvent?.Invoke(this, Auth.AuthEvent.LoggingIn);
        Log.Debug("About to Sign in...");

        IsSignedIn = await LoginAsync();

        if (!IsSignedIn)
        {
            Log.Information($"Log in process failed");
            AuthEvent?.Invoke(this, Auth.AuthEvent.LoggingError);
            return IsSignedIn;
        }

        //await LoadUserInfoAsync();
        Log.Information($"[{IsSignedIn}] Logged in status");
        AuthEvent?.Invoke(this, Auth.AuthEvent.LoggedIn);

        return IsSignedIn;
    }

    public async Task<bool> SignOutAsync()
    {
        var success = true;
        try
        {
            AuthEvent?.Invoke(this, Auth.AuthEvent.LoggingOut);
            Log.Debug("About to logout...");

            await OidcClient.LogoutAsync();

            Log.Debug("Successfully logged out");
            AuthEvent?.Invoke(this, Auth.AuthEvent.LoggedOut);
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, $"...while logging out. \n{ex.Message}");
            success = false;
            AuthEvent?.Invoke(this, Auth.AuthEvent.LoggingOutError);
        }

        AuthEvent?.Invoke(this, Auth.AuthEvent.LoggedOut);
        return success;
    }
    public async Task<bool> RefreshAsync()
    {
        var success = false;
        try
        {
            Log.Debug(new string('+', 100));
            AuthEvent?.Invoke(this, Auth.AuthEvent.RefreshStarted);

            var result = await OidcClient.RefreshTokenAsync();
            if (result.IsError)
            {
                Log.Debug($"Error refreshing token: {result.Error}");
                AuthEvent?.Invoke(this, Auth.AuthEvent.RefreshFailed);

                return success;
            }
            else
            {
                TokenState?.Update(result);
                AuthEvent?.Invoke(this, Auth.AuthEvent.RefreshCompleted);

                success = true;
                Log.Debug($"Token refreshed, Length: {AccessToken?.Length}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, $"...while refreshing... \n{ex.Message}");
            AuthEvent?.Invoke(this, Auth.AuthEvent.RefreshFailed);
        }

        Log.Debug(new string('-', 100));
        return success;
    }
    public async Task<bool> LoadUserInfoAsync()
    {
        var success = false;
        try
        {
            var result = await OidcClient.FetchUserInfoAsync();
            if (result.IsError)
            {
                Log.Error($"Error fetching user info: {result.Error}");
                return success;
            }

            var userInfoJson = result.Json;
            var firstName = userInfoJson.GetProperty("given_name").GetString();
            var lastName = userInfoJson.GetProperty("family_name").GetString();
            Name = $"{firstName} {lastName}";
            Email = userInfoJson.GetProperty("email").GetString();

            Log.Debug("User Info:");
            foreach (var claim in result.Claims)
            {
                var type = claim.Type;
                var value = claim.Value;

                if (type.ToLower().Contains("token"))
                    value = $"Length of string in '{type}' is: {value.Length}";

                Log.Debug("{0,-20}: {1}", claim.Type, claim.Value);
            }

            success = true;
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, $"...while fetching user info... \n{ex.Message}");
        }

        return success;
    }
    #endregion

    #region Public IDisposable Methods
    public void Dispose()
    {
        SignInTimer?.Dispose();
    }
    #endregion

    #region Private Methods

    private void AuthEventChanged(AuthEvent e)
    {
        switch (e)
        {
            case Auth.AuthEvent.LoggedIn:
                if (SignInTimer == null)
                {
                    // start a timer thread to get new token every xx minutes
                    if (SignInTimer != null)
                        SignInTimer.Dispose();

                    SignInTimer = new Timer(
                        RefreshLogIn,
                        null,
                        TimeSpan.FromMinutes(REFRESH_LOGIN_INTERVAL_MINUTES),
                        TimeSpan.FromMinutes(REFRESH_LOGIN_INTERVAL_MINUTES));

                    Log.Information($"Timer to refresh token at interval of {REFRESH_LOGIN_INTERVAL_MINUTES} minutes started");
                }

                break;


            case Auth.AuthEvent.LoggingError:
            case Auth.AuthEvent.LoggedOut:
            case Auth.AuthEvent.LoggingOutError:
                TokenState = null;
                IsSignedIn = false;

                SignInTimer?.Dispose();
                SignInTimer = null;
                break;


            default:
                break;
        }
    }
    private DotNetCoreOidcClient GetOidcClient()
    {
        var urlPrefix = "{{env}}";
        var urlPrefixForProd = string.Empty;
        var urlPrefixForQA = "qa-";
        var urlPrefixForDev = "qa-";

        // Get the configurations
        var config = App.GetConfiguration();

        var settings = new OpenIdConnectConfig();
        config.GetSection("OpenIdConnect").Bind(settings);

        if (settings.Authority == null)
            throw new ArgumentNullException(nameof(settings.Authority));

        if (ServerEnvironment == Env.Dev)
            settings.Authority = settings.Authority.Replace(urlPrefix, urlPrefixForDev);

        if (ServerEnvironment == Env.Qa)
            settings.Authority = settings.Authority.Replace(urlPrefix, urlPrefixForQA);

        if (ServerEnvironment == Env.Prod)
            settings.Authority = settings.Authority.Replace(urlPrefix, urlPrefixForProd);


        var client = new DotNetCoreOidcClient(settings);
        return client;
    }
    private async Task<bool> LoginAsync()
    {
        var success = true;

        Log.Debug("Awaiting authorization response from browser...\n");
        var result = await OidcClient.LoginAsync();
        if (result.IsError)
        {
            Log.Debug($"Error Logging in:\n{result.Error}");
            success = false;
            TokenState = null;
        }
        else // logged in
        {
            var gotUserInfo = await LoadUserInfoAsync();

            if (gotUserInfo && Name != null && Email != null)
                Log.Information($"User information loaded, Name: {Name}, Email: {Email}");
            else
                Log.Error($"User information could not be loaded");

            TokenState = OidcClient.TokenState;
        }
        return success;
    }
    private async void RefreshLogIn(object? state)
    {
        Log.Debug($"Passed waiting time of {REFRESH_LOGIN_INTERVAL_MINUTES} minutes, getting refresh token");
        await RefreshAsync();
    }
    #endregion

    #region Public Properties
    public bool IsSignedIn { get; private set; }
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? AccessToken => TokenState?.AccessToken;
    public TokenState? TokenState { get; private set; }
    public DotNetCoreOidcClient OidcClient => _client ??= GetOidcClient();
    public bool IsEURegion { get; set; } = false;
    public Env ServerEnvironment { get; set; } = Env.Prod;
    #endregion

    #region Private Properties
    private System.Threading.Timer? SignInTimer { get; set; }
    #endregion

    #region Field
    private DotNetCoreOidcClient? _client;
    #endregion
}
