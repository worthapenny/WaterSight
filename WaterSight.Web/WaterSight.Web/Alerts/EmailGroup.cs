using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace WaterSight.Web.Alerts;

public class EmailGroup: WSItem
{
    #region Constructor
    public EmailGroup(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public async Task<bool> SyncSybscribers(List<Recipient> recipients)
    {
        var url = EndPoints.MailmanSysnSubscribersQDT;
        return await WS.PostJson(url, recipients, false, "Sync Subscriber");
    }

    #region CRUD Operations
    //
    // READ
    public async Task<EmailGroupConfig> GetEmailGroupConfig(int groupId)
    {
        var url = EndPoints.MailmanSubsGroupQDTGroupId(groupId);
        return await WS.GetAsync<EmailGroupConfig>(url, null, "Email Group");
    }
    public async Task<List<EmailGroupConfig>> GetEmailGroupConfigs()
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
        var groups = await GetEmailGroupConfigs();
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
    }
    #endregion

    #region Public Properties
    public int DigitalTwinId { get; set; }
    public int ID { get; set; } = 0;
    public string Description { get; set; } = String.Empty;
    public string Name { get; set; }
    public List<int> Subscribers { get; set; } = new List<int>();
    public List<string> Subscriptions { get; set; } = new List<string>();
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