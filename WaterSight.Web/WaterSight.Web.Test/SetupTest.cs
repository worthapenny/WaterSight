using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Test;

public class SetupTest : TestBase
{
    #region Constructor
    public SetupTest()
        //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Setup Related Tests ----+----+----");
    }
    #endregion

    #region Tests
    [Test]
    public async Task Upload_Config_ExcelFile()
    {
        var excelFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Setup\Watertown_Configuration.xlsx");
        Assert.IsTrue(File.Exists(excelFilePath));
        Logger.Debug($"Found Excel file at: {excelFilePath}");
        Separator();

        // Delete all existing
        var deleted = await WS.Sensor.DeleteSensorsConfigAsync();
        Assert.IsTrue(deleted);
        Logger.Debug($"Deleted all sensor config");
        Separator();

        // Sensors
        var success = await WS.Sensor.PostExcelFile(new FileInfo(excelFilePath));
        Assert.IsTrue(success);
        Logger.Debug($"Uploaded {excelFilePath}");
        Separator();


        // Get created sensors
        var sensors = await WS.Sensor.GetSensorsConfigAsync();
        Assert.AreEqual(37, sensors.Count);
        Logger.Debug($"Sensors count between Excel and server matched");
        Separator();

    }
    #endregion
}
