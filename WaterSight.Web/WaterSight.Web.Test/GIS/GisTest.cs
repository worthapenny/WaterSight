using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Support;

namespace WaterSight.Web.Test.GIS;


public class GisTest : TestBase
{
    #region Constructor
    public GisTest()
        //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing GIS Related Tests ----+----+----");
    }
    #endregion

    #region Tests
    //
    // Pipes
    //
    [Test, Order(1)]
    public async Task UploadZippedPipeShpe()
    {
        var pipeZipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles/GIS/TestPipes.zip");
        Assert.IsTrue(File.Exists(pipeZipFilePath));

        var success = await WS.GIS.UploadPipeZippedShpFile(pipeZipFilePath);
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);
    }
    [Test, Order(2)]
    public async Task DeleteZippedPipeShapefile()
    {
        var success = await WS.GIS.DeletePipeZippedShpFile();
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);
    }

    //
    // Zones
    //
    [Test, Order(3)]
    public async Task UploadPressureZoneShapefile()
    {
        var zoneZipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles/GIS/TestPressureZones.zip");
        Assert.IsTrue(File.Exists(zoneZipFilePath));

        var success = await WS.GIS.UploadPressureZoneZippedShpFile(zoneZipFilePath);
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);

    }
    [Test, Order(4)]
    public async Task DeleteZippedZoneShapefile()
    {
        var success = await WS.GIS.DeletePressureZoneZippedShpFile();
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);
    }

    //
    // Any shapefile
    //
    [Test, Order(5)]
    public async Task UploadAnyShapefile()
    {
        var myDataType = "MyType"; // this name MUST match with below test
        var anyShapefilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles/GIS/TestPressureZones.zip");
        Assert.IsTrue(!File.Exists(myDataType));

        var success = await WS.GIS.UploadAnyZippedShpFile(anyShapefilePath, myDataType);
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);
    }
    [Test, Order(6)]
    public async Task DeleteAnyShapefile()
    {
        var myDataType = "MyType"; // this name MUST match with above test
        var success = await WS.GIS.DeleteAnyZippedShpFile(myDataType);
        Assert.IsTrue(success);

        Logger.Debug(Util.LogSeparatorDashes);
    }
    #endregion
}

