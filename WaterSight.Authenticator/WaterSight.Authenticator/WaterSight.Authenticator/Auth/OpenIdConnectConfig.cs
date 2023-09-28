namespace WaterSight.Authenticator.Auth;

public class OpenIdConnectConfig
{
    #region Public Properties
    public string? Authority { get; set; }

    public string? ClientId { get; set; }

    public Uri? RedirectUri { get; set; }

    public string? Scope { get; set; }
    #endregion
}
