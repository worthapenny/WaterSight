using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Customers;

public class Billings : WSItem
{
    #region Constructor
    public Billings(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public int BillingCSVLineCountLimit = 100000;

    public async Task<bool> UploadBillingFileAsync(FileInfo fileInfo)
    {
        Logger.Debug($"About to upload CSV/Excel file for Consumption/Billing.");

        var url = EndPoints.HydStructureMonthlyBillingQDT;

        if (fileInfo.Extension.ToLower().EndsWith("csv"))
            return await WS.PostFile(url, fileInfo,true, "CSV");

        if (fileInfo.Extension.ToLower().Contains("xl"))
            return await WS.PostFile(url, fileInfo, true, "Excel");

        Logger.Error($"Given file extension is not supported. Supported types are, csv and xlsx");
        return false;

        // TODO

        //var fileStream = new FileStream(filePath, FileMode.Open);
        //var billingDF = DataFrame.LoadCsv(fileStream);

        //var billingDFs = billingDF.SplitByRows(BillingCSVLineCountLimit);

        //var count = 1;
        //foreach (var df in billingDFs)
        //{
        //    var stream = new MemoryStream();
        //    DataFrame.WriteCsv(df, stream);

        //    var res = await Request.PostStream(url, new StreamContent(stream));

        //    if (res.IsSuccessStatusCode)
        //        WS.Logger.Debug($"[{count}/{billingDFs.Count}] Consumption file uploaded: {filePath}");
        //    else
        //    {
        //        WS.Logger.Error($"[{count}/{billingDFs.Count}] Failed to upload the file: {filePath}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
        //        return false;
        //    }
        //}
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