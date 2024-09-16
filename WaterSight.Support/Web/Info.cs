using WaterSight.Web.Core;

namespace WaterSight.Support.Web;

public class Info
{
    public async Task<Info> LoadFromDtConfigAsync(WS ws)
    {
        var dt = await ws.DigitalTwin.GetDigitalTwinAsync();

        if (dt != null)
        {
            Name = dt.Name;
            Description = dt.Description;
            DTID = dt.ID;
        }
        return this;
    }


    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DTID { get; set; }
    public string ShortName { get; set; } = string.Empty;


}