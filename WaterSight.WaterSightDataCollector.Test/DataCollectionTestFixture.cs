using Serilog;
using System.Reflection;
using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Domain;
using WaterSight.WaterSightDataCollector.Support.Logging;
using WaterSight.Web.Core;

namespace WaterSight.WaterSightDataCollector.Test;

[TestFixture]
public class Tests
{
    #region Constants

    private const string BlankSCADADataFileName = "BlankSCADAData.db";
    #endregion

    #region Private Properties
    private int DigitalTwinId => (int)DtID.ProdAwMoStl;
    private string DigitalTwinName => DtInfo.DtName_ProdAwMoStl;
    private Env DigitalTwinEnv => Env.Prod;


    private WS WS { get; set; }
    private DataCollector DataCollector { get; set; }
    #endregion



    #region Setup
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DataCollector = new DataCollector(
            dtID: DigitalTwinId,
            dtName: DigitalTwinName,
            env: DigitalTwinEnv);

        WS = DataCollector.NewWaterSightInstance();

        //var exePath = Assembly.GetExecutingAssembly().Location;
        //var exeDir = Path.GetDirectoryName(exePath);

        //if (exeDir == null)
        //    throw new ApplicationException($"Could not determine the current exe file location");


        //// Copy the file from exe location to the digital twin folder
        //var sqliteFilePathAtExeDir = Path.Combine(exeDir, BlankSCADADataFileName);
        //Assert.That(File.Exists(sqliteFilePathAtExeDir), Is.True);

        //// Delete the database if any
        //if (File.Exists(DatabaseManager.DatabasePath))
        //{
        //    //File.Delete(DatabaseManager.DatabasePath);
        //    //Log.Information($"Database deleted. Path: {DatabaseManager.DatabasePath}");
        //}

        //// Now copy a brand new empty one
        //if (!File.Exists(DatabaseManager.DatabasePath))
        //    File.Copy(sqliteFilePathAtExeDir, DatabaseManager.DatabasePath);
        //Assert.That(File.Exists(DatabaseManager.DatabasePath), Is.True);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Log.Information($"TearDown started");
        LogLibrary.Separate_BulletPlus();

        DatabaseManager.CloseSqliteConnection();

        //File.Delete(DatabaseManager.DatabasePath);
        LogLibrary.Separate_BulletInverse();
    }


    #endregion

    #region Tests


    [Test]
    public async Task ConnectionTest()
    {
        var conn = DatabaseManager.Connection.State;
        Assert.That(conn, Is.EqualTo(System.Data.ConnectionState.Open));

        var latestDateTime = await DatabaseManager.GetLatestDateTimeAsync("one");
        Assert.That(latestDateTime, Is.Null);
    }

    [Test]
    public async Task CollectSensorDataTest()
    {
        Assert.That(DataCollector, Is.Not.Null);

        var sensorsConfig = await WS.Sensor.GetSensorsConfigAsync();
        Assert.That(sensorsConfig, Is.Not.Empty);

        var sensorConfig = sensorsConfig[1];
        Assert.That(sensorConfig, Is.Not.Null);
        Log.Information($"Testing on Sensor: {sensorConfig}");


        var from = DateTimeOffset.Now.AddDays(-90);
        var to = DateTimeOffset.Now.AddDays(-80);

        // Collect data for 10 days
        var data = await DataCollector.CollectSensorDataAsync(
            sensorConfig: sensorConfig,
            from: from,
            to: to,
            IntegrationType.Raw);

        Assert.That(data, Is.Not.Null);

        //
        // Save the data to the local DB
        var didWrite = await DatabaseManager.WriteToDatabaseAsync(data);
        Assert.That(didWrite, Is.True);
    }

    [Test]
    public async Task CollectSensorsDataTest()
    {
        Assert.That(DataCollector, Is.Not.Null);
               

        // Get list of sensors from WaterSight
        var WS = DataCollector.NewWaterSightInstance();
        var sensorsConfig = await WS.Sensor.GetSensorsConfigAsync();
        Assert.That(sensorsConfig, Is.Not.Empty);

        // Pull the sensors (to collect the data on them0
        sensorsConfig = sensorsConfig
            //.Take(1)
            .TakeLast(250)
            .ToList();

        // Collect and write
        await DataCollector.CollectDataAndWriteToDatabaseAsync(
            sensorsConfig: sensorsConfig,
            type: IntegrationType.Raw,
            degreeOfParallelism: 5);
    }

    [Test]
    public async Task EntryPointTest()
    {
        var args = new[]
        {
            $"{DigitalTwinId}",
            $"{DigitalTwinName}",
            $"prod",
            "collect"
        };

        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0));
    }
    #endregion


    #region  Helper Methods

    //private WS NewWS()
    //{
    //    var registryPath = DigitalTwinEnv == Env.Prod
    //            ? @"SOFTWARE\WaterSight\BentleyProdOIDCToken"
    //            : @"SOFTWARE\WaterSight\BentleyQaOIDCToken";

    //    var ws = new WS(
    //        tokenRegistryPath: registryPath,
    //        digitalTwinId: DigitalTwinId,
    //        epsgCode: -1,
    //        env: DigitalTwinEnv,
    //        logger: Logger.Instance.LoggerConfig,
    //        subDomainSuffix: "",
    //        restToken: null,
    //        pat: null,
    //        logFilesDir: AppConstants.AppLogDir);

    //    return ws;
    //}

    #endregion


    #region Fields
    private WS? _ws = null;
    #endregion
}
