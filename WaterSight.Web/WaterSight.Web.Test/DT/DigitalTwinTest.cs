using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.DT;
using WaterSight.Web.Support;

namespace WaterSight.Web.Test;


[TestFixture, Order(107000), Category("DigitalTwin")]
public class DigitalTwinTest : TestBase
{
    #region Constructor
    public DigitalTwinTest()
    //: base(4549, Env.Qa)
    //:base(119, Env.Prod)
    {
        Separator($"----+----+---- Performing Digital Twin Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public DigitalTwin DigitalTwin => WS.DigitalTwin;
    #endregion

    #region Tests
    [Test]
    public async Task DigitalTwin_CRUD()
    {
        Logger.Debug(Util.LogSeparatorPluses);
        Separator("CRUD Digital Twins");

        //
        // READ
        // Get Digital Twins
        var twins = await DigitalTwin.GetDigitalTwinsAsync();
        Assert.IsTrue(twins.Count > 0);

        // CREATE
        var newDTName = "My New DT";
        var newDT = await DigitalTwin.AddWaterDigitalTwinAsync(newDTName);
        Assert.That(newDT, Is.Not.Null);
        Assert.That(newDT.Name, Is.EqualTo(newDTName));


        // Check against current DT
        var currentDt = twins.Where(dt => dt.ID == WS.Options.DigitalTwinId).FirstOrDefault();
        Assert.IsNotNull(currentDt);
        Assert.AreEqual(WS.Options.DigitalTwinId, currentDt.ID);
        Separator("Read Digital Twins");

        //
        // UPDATE
        // - description
        var newDescription = $"Test Description {DateTime.Now:u}";
        currentDt.Description = newDescription;
        var updated = await DigitalTwin.UpdateDigitalTwinNameAndDescriptionAsync(currentDt.Name, newDescription);
        Assert.AreEqual(true, updated);

        // Check if the description indeed get updated
        var currentDTConfig = await DigitalTwin.GetDigitalTwinAsync();
        Assert.IsNotNull(currentDTConfig);
        Assert.AreEqual(newDescription, currentDTConfig.Description);

        Separator("Updated Description");

        //// update - Avatar (Image)
        //var imageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Images\TestMtEverest.jpg");
        //var uploaded = await DigitalTwin.UpdateDigitalTwinAvatarAsync(imageFilePath, currentDt);
        //Assert.AreEqual(true, uploaded);
        //Separator("Updated Avatar");

        //
        // CRAETE
        // Create Object
        var newDTCreateOptions = new DigitalTwinWaterCreateOptions($"DT_Created_From_CSharp_{DateTime.Now:yyyyMMddhhmmss}");
        newDTCreateOptions.Goals.Add(DigitalTwinCreateOptions.GoalsCapitalPlanning);
        newDTCreateOptions.Goals.Add(DigitalTwinCreateOptions.GoalsTransmissionMains);

        var newDtConfig = await DigitalTwin.AddDigitalTwinAsync(newDTCreateOptions);
        Assert.IsNotNull(newDtConfig);
        Assert.IsTrue(newDtConfig.ID > 0);

        // update goals
        newDTCreateOptions.Goals.Remove(DigitalTwinCreateOptions.GoalsCapitalPlanning);
        newDTCreateOptions.Goals.Remove(DigitalTwinCreateOptions.GoalsTransmissionMains);
        updated = await DigitalTwin.UpdateDigitalTwinGoalAsync(newDtConfig.ID, newDTCreateOptions);
        Assert.AreEqual(true, updated);

        Separator("Created Digital Twin");


        // Delete 
        var deleted = await DigitalTwin.DeleteDigitalTwinAsync(newDtConfig.ID);
        Assert.AreEqual(true, deleted);

        Separator("Deleted Digital Twin");

        // Done
        Logger.Debug(Util.LogSeparatorXs);
    }

    #endregion
}
