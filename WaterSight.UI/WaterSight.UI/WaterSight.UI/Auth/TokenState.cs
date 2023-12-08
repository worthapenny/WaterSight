using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System.Security.Claims;

namespace WaterSight.UI.Auth;

public class TokenState
{
    #region Constructor
    public TokenState(LoginResult loginResult)
    {
        Update(loginResult);
    }
    #endregion

    #region Public Methods
    public LoginResult Update(LoginResult result)
    {
        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;
        IdentityToken = result.IdentityToken;
        AccessTokenExpiration = result.AccessTokenExpiration;
        User = result.User;
        return result;
    }

    public RefreshTokenResult Update(RefreshTokenResult result)
    {
        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;
        IdentityToken = result.IdentityToken;
        AccessTokenExpiration = DateTime.Now + TimeSpan.FromSeconds(result.ExpiresIn);
        return result;
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Access Token: {AccessToken}\n\rRefresh Token: {RefreshToken}\n\rIdentityToken: {IdentityToken}\n\r" +
            $"Access Token Expiration: {AccessTokenExpiration}";
    }
    #endregion

    #region Public Properties
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public string IdentityToken { get; set; }

    public DateTimeOffset AccessTokenExpiration { get; set; }

    public ClaimsPrincipal User { get; set; }
    #endregion
}
