using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Domain;
using WaterSight.WaterSightDataCollector.Support.Logging;
using WaterSight.Web.Core;

namespace WaterSight.WaterSightDataCollector.Test;

[TestFixture]
public class DataLoaderTestFixture
{
    
    #region Private Properties
    private int DigitalTwinId => (int)DtID.QaAwMoStl;
    private string DigitalTwinName => DtInfo.DtName_QaAwMoStl;
    private Env DigitalTwinEnv => Env.Qa;


    private WS WS { get; set; }
    private DataLoader DataLoader { get; set; }
    #endregion

    #region Setup
    [OneTimeSetUp]
    public void OneTimeSetup()
    {

        DataLoader = new DataLoader(
            dtID: DigitalTwinId,
            dtName: DigitalTwinName,
            env: DigitalTwinEnv);
        Assert.That(DataLoader, Is.Not.Null);

        WS = DataLoader.NewWaterSightInstance();
        Assert.That(WS, Is.Not.Null);

        //var sourceDigitalTwinName = "ProdAwMoStl";
        //DatabaseManager.DatabasePath = Path.Combine(AppConstants.AppDir, sourceDigitalTwinName, $"{sourceDigitalTwinName}.db");
        //Assert.That(File.Exists(DatabaseManager.DatabasePath), Is.True);
        //Log.Information($"Source DB: {DatabaseManager.DatabasePath}");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Log.Information($"TearDown started");
        LogLibrary.Separate_BulletPlus();

        DatabaseManager.CloseSqliteConnection();

        LogLibrary.Separate_BulletInverse();
    }
    #endregion

    #region Tests
    [Test]
    public async Task LoadData_Sensor_Test()
    {
        Assert.That(DataLoader, Is.Not.Null);

        await DataLoader.LoadDataAsync();
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

    //    LogLibrary.Separate_Star();
    //    Log.Debug(new string('*', 5) + $": {DigitalTwinName} [{DigitalTwinId}]");
    //    Log.Debug(new string('*', 5) + $": Env = {DigitalTwinEnv}");
    //    LogLibrary.Separate_Star();

    //    return ws;
    //}

    #endregion

    #region Fields
    private WS? _ws = null;
    #endregion
}
