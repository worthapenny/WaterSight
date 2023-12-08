using NUnit.Framework;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Support;

namespace WaterSight.Web.Test;

[TestFixture]
public class TestBase
{
    #region Static Fields
    public static int TEST_DT_Akshaya_4736 = 4736; // __Test_Bed_Akshaya
    public static int TEST_DT_ID2 = 4827;
    public static int TEST_DT_ID3 = 4828; // Fully Setup
    public static Env TEST_ENV = Env.Qa;
    #endregion

    #region Constructor
    public TestBase() : this(TEST_DT_Akshaya_4736, TEST_ENV)
    {
    }
    public TestBase(int dtID, Env env)
    {
        ActiveEnvironment = env;
        if (ActiveEnvironment == Env.Prod)
        {
            var message = $">>>>> You using {ActiveEnvironment} environment!!! <<<<<";
            Log.Debug(message);
            Debugger.Break();
        }

        var registryPath = env == Env.Prod
                ? @"SOFTWARE\WaterSight\BentleyProdOIDCToken"
                : @"SOFTWARE\WaterSight\BentleyQaOIDCToken";

        RunWaterSightAuthenticator();

        WS = new WS(tokenRegistryPath: registryPath, dtID, -1, ActiveEnvironment);
        WS.Options.EPSGCode = 26956; // Watertown DT
        
    }
    private void RunWaterSightAuthenticator()
    {
        var assemblyPath = @"D:\Development\DotNet\WaterSight\Output\WaterSight.Authenticator\bin\Debug\net6.0\WaterSight.Authenticator.exe";

        var assemblyName = "WaterSight.Authenticator";
        var processes = Process.GetProcessesByName(assemblyName);
        if (!processes.Any())
        {
            try
            {

                var process = new Process();
                process.StartInfo.FileName = assemblyPath;
                var isStarted = process.Start();
                if (!isStarted)
                {
                    Log.Error($"Failed to start the {assemblyName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving build output path: {ex.Message}");
            }

        }
        else
        {
            Log.Information($"Process '{assemblyName}' is already running. ID: {processes.First().Id}");
        }


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
        var vertical = '║'; // (char)186;

        Logger.Debug("");
        Logger.Debug(topLeft + new string(horizontal, width) + topRight);
        Logger.Debug(vertical + dtName.PadRight(width - (width - dtName.Length) / 2).PadLeft(width) + vertical);
        Logger.Debug(vertical + userInfo.PadRight(width - (width - userInfo.Length) / 2).PadLeft(width) + vertical);
        Logger.Debug(bottomLeft + new string(horizontal, width) + bottomRight);
        Logger.Debug("");
    }
    #endregion

    #region Methods

    public void Separator(string name = null) => Logger.Debug($"{new string(Util.Square, 35)} ↑↑↑ {name} ↑↑↑ {new string(Util.Square, 35)}");
    #endregion

    #region Properties
    public WS WS { get; private set; }
    public Env ActiveEnvironment { get; private set; }
    public ILogger Logger => WS.Logger;
    #endregion
}
