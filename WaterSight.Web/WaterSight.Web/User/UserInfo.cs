using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.User;

public class UserInfo : WSItem
{
    #region Constructor
    public UserInfo(WS ws) : base(ws)
    {
    }
    #endregion


    #region Public Methods

    public async Task<UserInfoWeb?> GetUserInfoAsync()
    {
        var url = EndPoints.ImsUserInfo;
        return await WS.GetAsync<UserInfoWeb>(
            url: url,
            id: null,
            typeName: "User Info");
    }

    #endregion
}


#region Model
public class UserInfoWeb
{
    public string? sub { get; set; }
    public string? ultimate_site { get; set; }
    public string? org { get; set; }
    public string? usage_country_iso { get; set; }
    public int auth_time { get; set; }
    public string? name { get; set; }
    public string? preferred_username { get; set; }
    public string? org_name { get; set; }
    public string? given_name { get; set; }
    public string? family_name { get; set; }
    public string? email { get; set; }
    public string? sid { get; set; }

    public override string ToString()
    {
        return $"{given_name} {family_name}, {email}";
    }
}
#endregion