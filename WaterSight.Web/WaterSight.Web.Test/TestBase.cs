using NUnit.Framework;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Test;

public class TestBase
{
    #region Static Fields
    public static int TEST_DT_ID = 4736;
    public static Env TEST_ENV = Env.Qa;
    #endregion

    #region Constructor
    public TestBase() : this(TEST_DT_ID, TEST_ENV)
    {
    }
    public TestBase(int dtID, Env env)
    {
        if (env == Env.Prod)
        {
            var message = $">>>>> You using {env.ToString()} environment!!! <<<<<";
            Debugger.Break();
        }

        var registryPath = env == Env.Prod
            ? @"SOFTWARE\WaterSight\BentleyProdOIDCToken"
            : @"SOFTWARE\WaterSight\BentleyQaOIDCToken";

        WS = new WS(tokenRegistryPath: registryPath, dtID, env);
        //WS.Options.EPSGCode = 26956; // Watertown DT
    }
    #endregion

    #region Only One Time Setup
    [OneTimeSetUp]
    public async Task OneTimeSetupAsync()
    {
        var dt = await WS.DigitalTwin.GetDigitalTwinAsync(WS.Options.DigitalTwinId);

        var dtName = dt?.ID + ": " + dt?.Name + $" [{WS.Options.Env.ToString().ToUpper()}]";
        var userInfo = (await this.WS.UserInfo.GetUserInfoAsync())?.ToString() ?? "";

        var width = 100;

        var topLeft = '╔'; // (char)201;
        var topRight = '╗'; // (char)187;
        var bottomLeft = '╚'; // (char)200;
        var bottomRight = '╝'; // (char)188;
        var horizontal = '═'; // (char)205;
        var vertial = '║'; // (char)186;

        Logger.Debug("");
        Logger.Debug(topLeft + new string(horizontal, width) + topRight);
        Logger.Debug(vertial + dtName.PadRight(width - (width - dtName.Length) / 2).PadLeft(width) + vertial);
        Logger.Debug(vertial + userInfo.PadRight(width - (width - userInfo.Length) / 2).PadLeft(width) + vertial);
        Logger.Debug(bottomLeft + new string(horizontal, width) + bottomRight);
        Logger.Debug("");
    }
    #endregion

    #region Methods

    public void Separator(string name = null) => Logger.Debug($"{new string('.', 35)} {name} {new string('.', 35)}");
    #endregion

    #region Properties
    public WS WS { get; private set; }
    public ILogger Logger => WS.Logger;
    #endregion
}
