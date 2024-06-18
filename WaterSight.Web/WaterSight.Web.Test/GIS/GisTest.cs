using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Test;


[TestFixture, Order(103000), Category("GIS")]
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
    [Test, Order(103010)]
    public async Task UploadZippedPipeShapefile()
    {
        // Check if something already exits
        var shapefileProps = await WS.GIS.GetShapefileProperties();
        var pipeGisItem = shapefileProps.Where(p => p.Name == GIS.GIS.PIPES_TYPE_NAME);
        if(pipeGisItem.Any())
        {
            var deleted = await WS.GIS.DeletePipeZippedShpFile();
            Assert.AreEqual(true, deleted);
        }

        var pipeZipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\GIS\TestPipes.zip");
        Assert.IsTrue(File.Exists(pipeZipFilePath));

        var success = await WS.GIS.UploadPipeZippedShpFile(pipeZipFilePath);
        Assert.IsTrue(success);

        Separator("Pipe Uploaded");
    }

    [Test, Order(103020), Category("Shapefile")]
    public async Task GetShapefilePropsTest()
    {
        var shapefileProps = await WS.GIS.GetShapefileProperties();
        Assert.IsNotNull(shapefileProps);

        Separator("Read Shp Properties");
    }

    [Test, Order(103030)]
    public async Task DeleteZippedPipeShapefile()
    {
        var success = await WS.GIS.DeletePipeZippedShpFile();
        Assert.IsTrue(success);

        Separator("Delted Pipe Shp");
    }

    //
    // Zones
    //
    [Test, Order(103040)]
    public async Task UploadPressureZoneShapefile()
    {

        // Check if something already exits
        var shapefileProps = await WS.GIS.GetShapefileProperties();
        var zoneGisItem = shapefileProps.Where(p => p.Name == GIS.GIS.ZONES_TYPE_NAME);
        if (zoneGisItem.Any())
        {
            var deleted = await WS.GIS.DeletePressureZoneZippedShpFile();
            Assert.AreEqual(true, deleted);
        }

        var zoneZipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\GIS\TestPressureZones.zip");
        Assert.IsTrue(File.Exists(zoneZipFilePath));

        var success = await WS.GIS.UploadPressureZoneZippedShpFile(zoneZipFilePath);
        Assert.IsTrue(success);

        Separator("Uploaded zone shp");

    }
    [Test, Order(103050)]
    public async Task DeleteZippedZoneShapefile()
    {
        var success = await WS.GIS.DeletePressureZoneZippedShpFile();
        Assert.IsTrue(success);

        Separator("Deleted zone shp");
    }

    //
    // Any shapefile
    //
    [Test, Order(103060)]
    public async Task UploadAnyShapefile()
    {
        var myDataType = $"MyType_{DateTime.Now:yyyyMMdd}"; // this name MUST match with below test
        // TODO: Check if myDataType exists
        // if it does exits, delete it first
        var anyShapefilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles/GIS/CriticalCustomers_mini.zip");
        Assert.IsTrue(File.Exists(anyShapefilePath));

        var success = await WS.GIS.UploadAnyZippedShpFile(anyShapefilePath, myDataType);
        Assert.IsTrue(success);

        Separator("Uploaded Any shp");
    }
    [Test, Order(103070)]
    public async Task DeleteAnyShapefile()
    {
        var myDataType = $"MyType_{DateTime.Now:yyyyMMdd}"; // this name MUST match with above test
        var success = await WS.GIS.DeleteAnyZippedShpFile(myDataType);
        Assert.IsTrue(success);

        Separator("Deleted any shp");
    }


    #endregion
}

