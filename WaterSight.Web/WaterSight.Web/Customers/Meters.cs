using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
namespace WaterSight.Web.Customers;

public class Meters : WSItem
{
    #region Constructor
    public Meters(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public async Task<bool> UploadMeterFileAsync(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload Excel file for Customer Meters.");

        var url = EndPoints.HydStructureConsumptionPointsQDT;
        var res = await Request.PostFile(url, fileInfo);

        if (res.IsSuccessStatusCode)
        {
            Logger.Information($"Meter file uploaded. Path: {fileInfo.FullName}. Text: {await res.Content.ReadAsStringAsync()}");
            return true;
        }
        else
        {
            Logger.Error($"Failed to upload. Path: {fileInfo.FullName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
            return false;
        }
    }

    public async Task<bool> DeleteMetersDataAsync()
    {
        var url = EndPoints.HydStructureConsumptionPointsQDT;
        var res = await Request.Delete(url);

        if (res.IsSuccessStatusCode)
            Logger.Debug($"Deleted.");
        else
            Logger.Error($"Failed to delete. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");


        return res.IsSuccessStatusCode;
    }
    #endregion
}
