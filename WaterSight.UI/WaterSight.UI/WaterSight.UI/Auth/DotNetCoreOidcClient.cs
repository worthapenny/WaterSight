using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using WaterSight.UI.Browser;

namespace WaterSight.UI.Auth;

public class DotNetCoreOidcClient
{
    #region Constructor
    public DotNetCoreOidcClient(OpenIdConnectConfig settings)
    {
        Settings = settings;

        authority = settings.Authority;
        httpClient = new HttpClient();

        var oidcClientOptions = new OidcClientOptions
        {
            Authority = Settings.Authority,
            ClientId = Settings.ClientId,
            RedirectUri = settings.RedirectUri.ToString(),
            //Scope = "openid email profile organization global_roles feature_tracking openflows-watersight offline_access context-registry-service rbac-service itwins:read itwins:modify imodels:modify",
            Scope = Settings.Scope,
            Browser = new SystemBrowser(Settings.RedirectUri.Port, Settings.RedirectUri.AbsolutePath),
            LoadProfile = false,
        };

        Client = new OidcClient(oidcClientOptions);
    }
    #endregion

    #region Public Methods
    public async Task<LoginResult> LoginAsync()
    {
        var loginResult = await Client.LoginAsync(new LoginRequest() { BrowserDisplayMode = DisplayMode.Hidden, BrowserTimeout = 3000 });
        TokenState = new TokenState(loginResult);
        return loginResult;
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync()
    {
        if (TokenState == null)
            throw new InvalidOperationException("Must Login First");

        var result = await Client.RefreshTokenAsync(TokenState.RefreshToken);
        TokenState.Update(result);
        return result;
    }

    public async Task<UserInfoResponse> FetchUserInfoAsync()
    {
        if (TokenState == null)
            throw new InvalidOperationException("Must Login First");

        var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(authority);

        return await httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = discoveryDoc.UserInfoEndpoint,
            Token = TokenState.AccessToken
        });
    }

    public async Task LogoutAsync()
    {
        if (TokenState == null)
            throw new InvalidOperationException("Must Login First");

        var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(authority);

        var tokenRevocationRequest = new TokenRevocationRequest
        {
            Address = discoveryDoc.RevocationEndpoint,
            ClientId = Client.Options.ClientId,
            ClientSecret = Client.Options.ClientSecret,
        };

        tokenRevocationRequest.Token = TokenState.AccessToken;
        var accessTokenRevoke = await httpClient.RevokeTokenAsync(tokenRevocationRequest);

        tokenRevocationRequest.Token = TokenState.RefreshToken;
        var refreshTokenRevoke = await httpClient.RevokeTokenAsync(tokenRevocationRequest);

        if (accessTokenRevoke.IsError || refreshTokenRevoke.IsError)
        {
            throw new InvalidOperationException($"Access Token Revoke: {accessTokenRevoke.Error}, Refresh Token Revoke: {refreshTokenRevoke.Error}");
        }
    }
    #endregion

    #region Public Properties
    public OidcClient Client { get; }

    public TokenState? TokenState { get; private set; }

    public OpenIdConnectConfig Settings { get; private set; }
    #endregion

    #region Fields
    private readonly string authority;
    private HttpClient httpClient;
    #endregion
}
