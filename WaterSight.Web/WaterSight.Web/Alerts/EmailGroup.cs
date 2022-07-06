using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Alerts;

public class EmailGroup: WSItem
{
    #region Constructor
    public EmailGroup(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public async Task<bool> AddSubscriber(int groupId, int subscriberId)
    {
        var url = EndPoints.MailmanGroupSubscriberDTIDGroupIdSubscriberId(groupId, subscriberId);
        return await WS.PostJson(url, null, false, "Add Subscriber");
    }
    public async Task<bool> RemoveSubscriber(int groupId, int subscriberId)
    {
        var url = EndPoints.MailmanGroupSubscriberDTIDGroupIdSubscriberId(groupId, subscriberId);
        return await WS.DeleteAsync(null, url, "Remove Subscriber");
    }
    
    #region CRUD Operations
    //
    // CREATE
    public async Task<EmailGroupConfig> AddEmailGroupConfig(EmailGroupConfig emailGroup)
    {
        var url = EndPoints.MailmanSubsGroupQDT;
        return await WS.AddAsync<EmailGroupConfig>(emailGroup, url, "Email Group");
    }

    //
    // READ
    public async Task<EmailGroupConfig?> GetEmailGroupConfig(int groupId)
    {
        var url = EndPoints.MailmanSubsGroupQDTGroupId(groupId);
        
        // NOTE: the end-points returns array
        var items = await WS.GetAsync<List<EmailGroupConfig>>(url, null, "Email Group");
        return items.Any() ? items.First() : null;
    }
    public async Task<List<EmailGroupConfig>> GetEmailGroupsConfig()
    {
        var url = EndPoints.MailmanSubsGroupQDTGroupId(null);
        return await WS.GetManyAsync<EmailGroupConfig>(url, "Email Groups");
    }

    //
    // UPDATE
    public async Task<bool> UpdateGroup(EmailGroupConfig mmConfig)
    {
        var url = EndPoints.MailmanSubsGroupQDT;
        return await WS.UpdateAsync(mmConfig.ID, mmConfig, url, "Email Group", true);
    }

    //
    // DELETE
    public async Task<bool> DeleteGroup(EmailGroupConfig mmConfig)
    {
        var url = EndPoints.MailmanSubsGroupQDtQSubsGroupId(mmConfig.ID);
        return await WS.DeleteAsync(mmConfig.ID, url, "Email Group", false);
    }
    public async Task<bool> DeleteGroups()
    {
        var groups = await GetEmailGroupsConfig();
        var success = true;
        foreach (var group in groups)
            success = success & await DeleteGroup(group);
        
        return success;
    }
    #endregion

    #endregion
}

#region Model Classes

[DebuggerDisplay("{ToString()}")]
public class EmailGroupConfig
{
    #region Constructor
    public EmailGroupConfig()
    {
        Subscribers = new List<int>();
        Subscriptions = new List<string>();
    }
    #endregion

    #region Public Properties
    public int DigitalTwinId { get; set; }
    public int ID { get; set; } = 0;
    public string Description { get; set; } = String.Empty;
    public string Name { get; set; }

    /// <summary>
    /// List of Connect User IDs
    /// </summary>
    public List<int> Subscribers { get; set; }// = new List<int>();

    /// <summary>
    /// List of Souces (Sensor, Zones, etc.) IDs
    /// </summary>
    public List<string> Subscriptions { get; set; }// = new List<string>();
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}: {Name}";
    }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class Recipient
{
    #region Constructor
    public Recipient()
    {
    }
    #endregion

    #region Public Properties
    public int DigitalTwinId { get; set; }
    public int ID { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}: {Name} ({Email})";
    }
    #endregion
}
#endregion