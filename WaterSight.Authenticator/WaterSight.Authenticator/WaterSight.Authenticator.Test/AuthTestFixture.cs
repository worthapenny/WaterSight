using WaterSight.Authenticator.ControlModel;

namespace WaterSight.Authenticator.Test;

public class AuthTestFixture
{

    [Test]
    public async Task TestQAAuthenticationAsync()
    {
        var signInControlModel = new SignInControlModel();
        signInControlModel.ServerEnvironment = Auth.Env.Dev;

        Assert.That(signInControlModel.OidcClient.Settings.Authority.StartsWith("https://qa-ims"), Is.True);

        var loggedIn = await signInControlModel.SignInAsync();
        Assert.That(loggedIn, Is.True);
        Assert.That(signInControlModel.IsSignedIn, Is.True);

        Assert.That(signInControlModel.TokenState, Is.Not.Null);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.AccessToken), Is.False);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.RefreshToken), Is.False);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.IdentityToken), Is.False);
    }


    [Test]
    public async Task TestProdAuthenticationAsync()
    {
        var signInControlModel = new SignInControlModel();
        signInControlModel.ServerEnvironment = Auth.Env.Prod;

        Assert.That(signInControlModel.OidcClient.Settings.Authority.StartsWith("https://ims"), Is.True);

        var loggedIn = await signInControlModel.SignInAsync();
        Assert.That(loggedIn, Is.True);
        Assert.That(signInControlModel.IsSignedIn, Is.True);

        Assert.That(signInControlModel.TokenState, Is.Not.Null);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.AccessToken), Is.False);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.RefreshToken), Is.False);
        Assert.That(string.IsNullOrEmpty(signInControlModel.TokenState.IdentityToken), Is.False);
    }
}