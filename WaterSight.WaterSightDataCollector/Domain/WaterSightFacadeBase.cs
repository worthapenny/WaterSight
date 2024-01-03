using Serilog;
using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Support.Logging;
using WaterSight.Web.Core;

namespace WaterSight.WaterSightDataCollector.Domain;

public abstract class WaterSightFacadeBase
{
    #region Constructor
    public WaterSightFacadeBase(int dtID, string dtName, Env env)
    {
        DigitalTwinID = dtID;
        Env = env;
        DigitalTwinName = dtName;

        AppConstants.DigitalTwinName = DigitalTwinName;
    }
    #endregion

    #region Protected Methods
    public WS NewWaterSightInstance()
    {
        var tokenRegistryPath = Env == Env.Prod
                ? @"SOFTWARE\WaterSight\BentleyProdOIDCToken"
                : @"SOFTWARE\WaterSight\BentleyQaOIDCToken";

        // Setup Logger
        Logger.Instance.SetupLogger();
        Log.Information($"DigitalTwin ID: {DigitalTwinID}");
        Log.Information($"DigitalTwin Name: {DigitalTwinName}");
        Log.Information($"DigitalTwin Environment: {Env}");

        // DatabaseName
        //Log.Information($"Database path: {DatabaseManager.DatabasePath}");
        LogLibrary.Separate_OBig();



        // Update the Timeout to 3 minutes
        // as it might take a long time to collect the data
        Request.Timeout = TimeSpan.FromMinutes(10);


        var ws = new WS(
            tokenRegistryPath: tokenRegistryPath,
            digitalTwinId: DigitalTwinID,
            epsgCode: -1,
            env: Env,
            logger: Logger.Instance.LoggerConfig
            );

        Log.Debug($"WaterSight instance is created");
        return ws;
    }
    #endregion


    #region Protected Properties
    protected int DigitalTwinID { get; }
    protected Env Env { get; }
    protected string DigitalTwinName { get; }
    protected WS WS => _ws ??= NewWaterSightInstance();
    #endregion


    #region Fields
    private WS? _ws;
    #endregion
}
