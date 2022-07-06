using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Alerts;
using WaterSight.Web.Core;

namespace WaterSight.Web.Test;

[TestFixture, Order(105100), Category("Email Groups")]
public class EmailGroupTest : TestBase
{
    #region Constructor
    public EmailGroupTest()
    //: base(4549, Env.Qa)
    {
        Separator($"----+----+---- Performing Email Groups Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public EmailGroup EmailGroup => WS.Alert.EmailGroup;
    #endregion

    #region Tets

    [Test, Category("CRUD")]
    public async Task EmailGroup_CRUD()
    {
        // add a sensor so that the SensorConfig list is not empty
        var sensorTest = new SensorsTest();
        var sensorCreated = await WS.Sensor.AddSensorConfigAsync(await sensorTest.NewSensorConfigAsync());
        Assert.That(sensorCreated.ID, Is.GreaterThan(0));

        // Connect Client Id (User's Id)
        var connectClientId = ActiveEnvironment == Env.Prod ? 234 : 2645;

        // create EmailGroup object
        var emailGroup = NewEmailGroupConfig(/*sensorCreated.ID, connectClientId*/);
        Assert.That(emailGroup, Is.Not.Null);

        
        // Check if item with same name exists, and delete if so
        var emailGroupsFound = await WS.Alert.EmailGroup.GetEmailGroupsConfig();
        if (emailGroupsFound != null)
        {
            var emailGroupCheck = emailGroupsFound.Where(g => g.Name == emailGroup.Name);
            if (emailGroupCheck.Any())
                _ = await WS.Alert.EmailGroup.DeleteGroup(emailGroupCheck.First());
        }


        //
        // CREATE
        var emailGroupCreated = await WS.Alert.EmailGroup.AddEmailGroupConfig(emailGroup);
        Assert.That(emailGroupCreated, Is.Not.Null);
        Assert.That(emailGroupCreated.ID, Is.GreaterThan(0));
        Separator("Done create testing");

        // Add user/person
        var userAdded = await WS.Alert.EmailGroup.AddSubscriber(
            groupId: emailGroupCreated.ID,
            subscriberId: connectClientId);
        Assert.That(userAdded, Is.True);

        // Remove user/person
        var userRemoved = await WS.Alert.EmailGroup.RemoveSubscriber(
            groupId: emailGroupCreated.ID,
            subscriberId: connectClientId);
        Assert.That(userRemoved, Is.True);
        Separator("Done add/remove users testing");

        //
        // READ
        var emailGroupFound = await WS.Alert.EmailGroup.GetEmailGroupConfig(emailGroupCreated.ID);
        Assert.That(emailGroupFound, Is.Not.Null);
        Separator("Done read, single testing");

        emailGroupsFound = await WS.Alert.EmailGroup.GetEmailGroupsConfig();
        Assert.That(emailGroupsFound, Is.Not.Null);
        Assert.That(emailGroupsFound, Is.Not.Empty);
        Separator("Done read, many items testing");

        //
        // UPDATE
        var newName = emailGroupCreated.Name + "__Updated";
        emailGroupCreated.Name = newName;
        var updated = await WS.Alert.EmailGroup.UpdateGroup(emailGroupCreated);
        Assert.That(updated, Is.True);

        emailGroupFound = await WS.Alert.EmailGroup.GetEmailGroupConfig(emailGroupCreated.ID);
        Assert.That(emailGroupFound, Is.Not.Null);
        Assert.That(newName, Is.EqualTo(emailGroupFound.Name));
        Separator("Done update testing");

        //
        // DELETE
        var deleted = await WS.Alert.EmailGroup.DeleteGroup(emailGroupCreated);
        Assert.That(deleted, Is.True);
        Separator("Done delete, single item, testing");

        var deletedMany = await WS.Alert.EmailGroup.DeleteGroups();
        Assert.IsTrue(deletedMany);
        Separator("Done delete, many items, testing");

        // Delete created sensor as well
        var deletedSensor = await WS.Sensor.DeleteSensorConfigAsync(sensorCreated.ID);
        Assert.AreEqual(true, deletedSensor);
    }
    #endregion

    #region Private Methods
    public EmailGroupConfig NewEmailGroupConfig(/*int sensorID, int connectUserID*/)
    {
        var group = new EmailGroupConfig()
        {
            DigitalTwinId = WS.Options.DigitalTwinId,
            Description = "Test email group from C#",
            Name = "TestEmailGroup"
        };

        /*
        group.Subscribers.Add(connectUserID);
        group.Subscriptions.Add(sensorID.ToString());*/

        return group;
    }
    #endregion
}
