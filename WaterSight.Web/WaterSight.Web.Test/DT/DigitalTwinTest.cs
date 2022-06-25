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

        // Check against current DT
        var currectDt = twins.Where(dt => dt.ID == WS.Options.DigitalTwinId).FirstOrDefault();
        Assert.IsNotNull(currectDt);
        Assert.AreEqual(WS.Options.DigitalTwinId, currectDt.ID);
        Separator("Read Digital Twins");

        //
        // UPDATE
        // - description
        var newDescription = $"Test Description {DateTime.Now:u}";
        currectDt.Description = newDescription;
        var updated = await DigitalTwin.UpdateDigitalTwinDescriptionAsync(currectDt);
        Assert.AreEqual(true, updated);

        // Check if the description indeed get updated
        var currentDTConfig = await DigitalTwin.GetDigitalTwinAsync(WS.Options.DigitalTwinId);
        Assert.IsNotNull(currentDTConfig);
        Assert.AreEqual(newDescription, currentDTConfig.Description);

        Separator("Updated Description");

        //// update - Avatar (Image)
        //var imageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Images\TestMtEverest.jpg");
        //var uploaded = await DigitalTwin.UpdateDigitalTwinAvatarAsync(imageFilePath, currectDt);
        //Assert.AreEqual(true, uploaded);
        //Separator("Updated Avatar");

        //
        // CRAETE
        // Create Object
        var newDT = new DigitalTwinWaterCreateOptions($"DT_Created_From_CSharp_{DateTime.Now:yyyyMMddhhmmss}");
        newDT.Goals.Add(DigitalTwinCreateOptions.GoalsCapitalPlanning);
        newDT.Goals.Add(DigitalTwinCreateOptions.GoalsTransmissionMains);

        var newDtConfig = await DigitalTwin.AddDigitalTwinAsync(newDT);
        Assert.IsNotNull(newDtConfig);
        Assert.IsTrue(newDtConfig.ID > 0);

        // update goals
        newDT.Goals.Remove(DigitalTwinCreateOptions.GoalsCapitalPlanning);
        newDT.Goals.Remove(DigitalTwinCreateOptions.GoalsTransmissionMains);
        updated = await DigitalTwin.UpdateDigitalTwinGoalAsync(newDtConfig.ID, newDT);
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
