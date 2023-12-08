using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Customers;

public class Billings : WSItem
{
    #region Constants
    public static int BillingCSVLineCountLimit = 500000;
    #endregion

    #region Constructor
    public Billings(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    
    public async Task<bool> UploadBillingFileAsync(FileInfo fileInfo)
    {
        Logger.Debug($"🆙 About to upload CSV/Excel file for Consumption/Billing.");

        var url = EndPoints.HydStructureMonthlyBillingQDT;

        if (fileInfo.Extension.ToLower().EndsWith("csv"))
            return await WS.PostFile(url, fileInfo,true, "CSV");

        if (fileInfo.Extension.ToLower().Contains("xl"))
            return await WS.PostFile(url, fileInfo, true, "Excel");

        Logger.Error($"Given file extension is not supported. Supported types are, csv and xlsx");
        return false;

    }

    public async Task<bool> DeleteBillingDataAsync()
    {
        var url = EndPoints.HydStructureMonthlyBilling + $"?{EndPoints.Query.DTID}";
        var res = await Request.Delete(url);

        if (res.IsSuccessStatusCode)
            WS.Logger.Debug($"Deleted.");
        else
            WS.Logger.Error($"Failed to delete. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");


        return res.IsSuccessStatusCode;
    }
    #endregion

}