using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.ExternalService;

namespace WaterSight.Web.Test;

public class PowerBITest: TestBase
{
    #region Constructor
    public PowerBITest()
    //: base(4549, Env.Qa)
    {
        Separator($"----+----+---- Performing Power BI Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public PowerBI PowerBI => WS.PowerBI;
    #endregion

    #region Tests
    [Test, Category("PowerBI"), Category("CRUD")]
    public async Task PowerBI_CRUD()
    {
        // Create Object
        var powerBiConfig = NewPowerBiConfig();
        Assert.IsNotNull(powerBiConfig);


        // Create
        var powerBiConfigCreated = await PowerBI.AddPowerBiConfigAsync(powerBiConfig);
        Assert.IsNotNull(powerBiConfigCreated);
        Assert.IsTrue(powerBiConfigCreated.Id > 0);
        Separator("Done create testing");


        // Update
        var newName = "Power BI from CSharp Updated";
        powerBiConfigCreated.Name = newName;
        var success = await PowerBI.UpdatePowerBiConfig(powerBiConfigCreated);
        Assert.AreEqual(true, success);
        Separator("Done update test");


        // Read
        var powerBiConfigFound = await PowerBI.GetPowerBiConfigsAsync();
        Assert.IsNotNull(powerBiConfigFound);
        Assert.IsNotEmpty(powerBiConfigFound);
        Separator("Done read test");


        // Check if the name updated from above actually happened
        var updatedPowerBI = powerBiConfigFound.Where(pbi => pbi.Name == newName);
        Assert.AreEqual(true, updatedPowerBI.Any());


        // Delete
        if(powerBiConfigCreated != null)
        {
            var deleted = await PowerBI.DeletePowerBiConfig(powerBiConfigCreated);
            Assert.AreEqual(true, deleted);
            Separator("Done delete test");

            // make sure it actually got deleted
            powerBiConfigFound = await PowerBI.GetPowerBiConfigsAsync();
            var deletedItem = powerBiConfigFound.Where(pbi => pbi.Name == newName);
            Assert.AreEqual(false, deletedItem.Any());  
        }

        Separator("Done CRUD tests for Power BI");
    }
    #endregion

    #region Private Methods
    private PowerBiConfig NewPowerBiConfig()
    {
        return new PowerBiConfig(
            name: "Power BI from C#",
            url: "", // MUST be empty
            digitalTwinId: WS.Options.DigitalTwinId,
            navMenuItem: NavMenuItem.NetworkMonitoring);
    }
    #endregion
}
