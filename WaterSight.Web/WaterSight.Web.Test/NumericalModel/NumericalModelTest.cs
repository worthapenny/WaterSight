using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.NumericModels;

namespace WaterSight.Web.Test.NumericalModel;

public class NumericalModelTest : TestBase
{
    #region Constructor
    public NumericalModelTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing NumericalModel Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public NumericModel NumericModel => WS.NumericModel;
    #endregion

    #region Tests
    [Test]
    public async Task NumericalModel_CRUD()
    {
        //
        // Delete existing if any
        //
        var modelDomains = await NumericModel.GetWaterModelDomains();
        var modelDomainWtrg = modelDomains?.Where(m => m.Type == "WaterGems");
        if (modelDomainWtrg?.Any() ?? false)
            foreach (var md in modelDomainWtrg)
            {
                if (md != null)
                    _ = await NumericModel.DeleteWaterModelDomain(md.Id);
            }
        Separator("Done with Delete");


        //
        // Create
        //
        var modelDomainName = $"Test_ModelDomain_{DateTime.Now: yyyyMMdd}";
        var modelDomain = new ModelDomainConfig(
            digitalTwinId: WS.Options.DigitalTwinId,
            epsgCode: "8632",
            name: modelDomainName,
            spinupHours: 0,
            hindcastHours: 10,
            forecastHours: 5
            );
        var id = await NumericModel.AddWaterModelDomain(modelDomain);
        Assert.IsNotNull(id);
        Assert.IsTrue(id > 0);
        Separator($"Created {nameof(NumericModel)}");
        modelDomain.Id = id;
        Separator($"Created, {modelDomainName} model domain");

        //
        // update
        //
        var newForecastHours = 11;
        var newHindcastHours = 22;
        var newSpinupHors = 33;
        modelDomain.ForecastHours = newForecastHours;
        modelDomain.HindcastHours = newHindcastHours;
        modelDomain.SpinUpHours = newSpinupHors;
        var success = await NumericModel.UpdateWaterModelDomain(modelDomain);
        Assert.IsTrue(success);
        Separator($"Updated {nameof(NumericModel)}");

        //
        // read
        //
        var newModelDomains = await NumericModel.GetWaterModelDomains();
        Assert.IsNotNull(newModelDomains);
        Assert.IsTrue(newModelDomains.Count > 0);
        var newModelDomain = newModelDomains.Where(m => m.Name == modelDomainName)?.First() ?? null;
        Assert.AreEqual(newForecastHours, newModelDomain?.ForecastHours);
        Assert.AreEqual(newHindcastHours, newModelDomain?.HindcastHours);
        Assert.AreEqual(newSpinupHors, newModelDomain?.SpinUpHours);
        Separator($"Read {nameof(NumericModel)}");

        //
        // Delete
        //
        var deleted = await NumericModel.DeleteWaterModelDomain(newModelDomain.Id);
        Assert.IsTrue(deleted);
        Separator($"Deleted {nameof(NumericModel)}");
    }

    [Test]
    public async Task UploadWaterModel()
    {
        var zippedModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Setup\Watertown.wtg.sqlite.zip");
        Assert.IsTrue(File.Exists(zippedModelPath));
        Separator($"Zipped model exits");

        var fileInfo = new FileInfo(zippedModelPath);
        var success = await NumericModel.UpdloadZippedWaterModel(fileInfo);
        Assert.IsTrue(success);
        Separator("Model Uploaded");

    }
    #endregion
}



