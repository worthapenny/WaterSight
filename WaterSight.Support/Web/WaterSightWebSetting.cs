using Newtonsoft.Json;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Support.Web;


public class WaterSightWebSetting
{
    public async Task<WaterSightWebSetting> LoadFromWeb(WS ws)
    {
        var instance = new WaterSightWebSetting();
        instance.Info = await Info.LoadFromDtConfigAsync(ws);
        instance.Geo = await Geo.LoadFromWebAsync(ws);
        instance.Units = await ws.Settings.Units.GetAllUnits();
        instance.Operations = await Operations.LoadFromWebAsync(ws);
        return instance;
    }

    public Info Info { get; set; } = new Info();

    public Geo Geo { get; set; } = new Geo();

    public Operations Operations { get; set; } = new Operations();

    public UnitSystem UnitSystem { get; set; } = UnitSystem.US;

    [JsonIgnore]
    public List<UnitsConfig?> Units { get; set; } = new List<UnitsConfig?>();
}
