using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.NumericModels;

namespace WaterSight.Web.Test;

[TestFixture, Order(102900), Category("NumericalModel"), Category("Model")]
public class NumericalModelTest : TestBase
{
    #region Constructor
    public NumericalModelTest()
    //: base(4549, Env.Qa) // Akshaya
    : base(3377, Env.Qa) // Watertown
    //: base(179, Env.Prod)
    {
        Logger.Debug($"----+----+---- Performing NumericalModel Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public NumericModel NumericModel => WS.NumericModel;
    #endregion

    #region Tests
    [Test, Category("Simulation Time-steps")]
    public async Task GetSimulationTimeStepsTest()
    {
        var dates = await NumericModel.GetSimulationTimeStepsWaterModel();
        Assert.IsNotNull(dates);
        Assert.IsNotEmpty(dates);

        Separator("Done Simulation Time-steps");
    }

    [Test, Category("CRUD")]
    public async Task NumericalModel_CRUD()
    {
        //
        // Delete existing if any
        //
        var modelDomains = await NumericModel.GetModelDomainsWaterType();
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
        var newModelDomains = await NumericModel.GetModelDomainsWaterType();
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

    [Test, Category("Upload")]
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

    [Test, Category("Elements"), Category("SCADA")]
    public async Task GetTargetElementsWaterModel()
    {
        var map = await NumericModel.GetModelTargetElementsWaterModel();
        Assert.IsNotNull(map);
        Assert.IsTrue(map.Count > 0);
    }

    [Test, Category("Elements"), Category("SCADA")]
    public async Task GetMappedElementsWaterModel()
    {
        var elementId = 49510;
        var mappedElements = await NumericModel.GetMappedScadaElementsWaterModel(elementId);
        Assert.IsNotNull(mappedElements);
        Assert.IsTrue(mappedElements.Count > 0);
    }

    [Test, Category("Elements"), Category("Parameters")]
    [TestCase(WaterDomainElementTypeId.Pipe)]
    [TestCase(WaterDomainElementTypeId.Node)] // Junction
    [TestCase(WaterDomainElementTypeId.Pump)]
    [TestCase(WaterDomainElementTypeId.Tank)]
    [TestCase(WaterDomainElementTypeId.FCV)]
    [TestCase(WaterDomainElementTypeId.GPV)]
    [TestCase(WaterDomainElementTypeId.PBV)]
    [TestCase(WaterDomainElementTypeId.TCV)]
    [TestCase(WaterDomainElementTypeId.PRV)]
    [TestCase(WaterDomainElementTypeId.PSV)]
    [TestCase(WaterDomainElementTypeId.Reservoir)]
    public async Task GetElementParameters(int domainElementTypeId)
    {
        // Get model Domain Name
        var modelDomain = await NumericModel.GetModelDomainsWaterType();
        Assert.IsNotNull(modelDomain);
        Assert.AreEqual(true, modelDomain.Any());

        var parameters = await NumericModel.GetParameters(
            modelDomainName: modelDomain.First().Name,
            domainElementTypeId: domainElementTypeId);

        Assert.IsNotNull(parameters);
        Assert.IsTrue(parameters.Count > 0);
    }

    [Test, Category("Element"), Category("Results")]
    [TestCase(49420, 0)] // Pipe:
    public async Task GetElementResultsAtTime(int elementId, int domainElementTypeId)
    {
        // Get model Domain Name
        var modelDomain = await NumericModel.GetModelDomainsWaterType();
        Assert.IsNotNull(modelDomain);
        Assert.AreEqual(true, modelDomain.Any());

        var results = await NumericModel.GetModelResultsAtTime(
            elementId: elementId,
            domainElementTypeId: domainElementTypeId,
            modelDomainName: modelDomain.First().Name,
            at: DateTimeOffset.UtcNow.AddHours(-1));

        Assert.IsNotNull(results);
        Assert.AreEqual(true, results.ElementFieldResults.Any());
    }
    [Test, Category("Element"), Category("Results"), Category("TSD")]
    [TestCase(49420, "Reaches/flow")]
    public async Task GetElementResultsTSD(int elementId, string parameterName)
    {
        // Get model Domain Name
        var modelDomain = await NumericModel.GetModelDomainsWaterType();
        Assert.IsNotNull(modelDomain);
        Assert.AreEqual(true, modelDomain.Any());

        var results = await NumericModel.GetModelResults(
            elementId: elementId,
            parameterName: parameterName,
            modelDomainName: modelDomain.First().Name,
            startDate: DateTimeOffset.UtcNow.AddDays(-1),
            endDate: DateTimeOffset.UtcNow);

        Assert.IsNotNull(results);
        Assert.AreEqual(true, results.Values.Any());
    }
    #endregion
}



