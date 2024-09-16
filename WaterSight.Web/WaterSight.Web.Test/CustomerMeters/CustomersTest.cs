using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Customers;

namespace WaterSight.Web.Test;

[TestFixture, Order(106000), Category("CustomerMeters")]
public class CustomersTest : TestBase
{
    #region Constructor
    public CustomersTest()
    //: base(4549, Env.Qa)
    //: base(233, Env.Prod)
    {
        Logger.Debug($"----+----+---- Performing Customers Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Meters Meters => WS.Customers.Meters;
    public Billings Billings => WS.Customers.Billings;
    #endregion

    #region Tests

    [Test, Order(106001)]
    public async Task MeterData_Upload()
    {
        var excelFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Setup\Watertown_Configuration.xlsx");
        FileAssert.Exists(excelFilePath);

        var uploaded = await Meters.UploadMeterFileAsync(new FileInfo(excelFilePath));
        Assert.That(uploaded, Is.True);
        Separator("Uploaded");
    }


    [Test, Order(106002)]
    public async Task Billing_Upload()
    {
        // Excel Upload
        // There is some weirdness going on with time
        // So wait 5 seconds
        await Task.Delay(5 * 1000);
        var excelFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\Setup\Watertown_Configuration.xlsx");
        var uploaded = await Billings.UploadBillingFileAsync(new FileInfo(excelFilePath));
        Assert.That(uploaded, Is.True);
        Separator("Uploaded Excel");

        // CSV Upload
        var csvData = "" +
            "ID,Billing month,StatQueryValue,Units\n" +
            "C-5-28,2020-May-01,1,hft³\n" +
            "C-5-29,2020-May-02,4,hft³";

        var csvFilePath = Path.GetTempFileName() + ".csv";
        File.WriteAllText(csvFilePath, csvData);

        uploaded = await Billings.UploadBillingFileAsync(new FileInfo(csvFilePath));
        Assert.That(uploaded, Is.True);
        Separator("Uploaded CSV");

        // delete csvFile
        File.Delete(csvFilePath);
        Assert.AreEqual(false, File.Exists(csvFilePath));
        Separator("CSV deleted");

        // Delete Billings data
        var deleted = await WS.Customers.Billings.DeleteBillingDataAsync();
        Assert.IsTrue(deleted);
        Separator("Deleted");

    }

    [Test, Order(106009)]
    public async Task Billing_Delete()
    {
        var deleted = await Billings.DeleteBillingDataAsync();
        Assert.IsTrue(deleted);
        Separator("Billing Deleted");
    }


    [Test, Order(106010)]
    public async Task Meter_Delete()
    {
        var deleted = await Meters.DeleteMetersDataAsync();
        Assert.IsTrue(deleted);
        Separator("Meter Deleted");
    }

    #endregion
}
