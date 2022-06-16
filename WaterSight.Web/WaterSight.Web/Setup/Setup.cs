using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Setup;

public class Setup : WSItem
{
    #region Constructor
    public Setup(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    public async Task<bool> UploadExcelFile_Sensors(FileInfo excelFileInfo)
    {
        return await WS.Sensor.PostExcelFile(excelFileInfo);
    }

    public async Task<bool> UploadExcelFile_Pumps(FileInfo excelFileInfo)
    {
        return await WS.HydStructure.Pump.PostExcelFile(excelFileInfo);
    }

    public async Task<bool> UploadExcelFile_PumpStations(FileInfo excelFileInfo)
    {
        return await WS.HydStructure.PumpStation.PostExcelFile(excelFileInfo);
    }

    public async Task<bool> UploadExcelFile_Tanks(FileInfo excelFileInfo)
    {
        return await WS.HydStructure.Tank.PostExcelFile(excelFileInfo);
    }

    public async Task<bool> UploadExcelFile_CustomerMeters(FileInfo excelFileInfo)
    {
        return await WS.Customers.Meters.UploadMeterFileAsync(excelFileInfo);
    }

    public async Task<bool> UploadCsvOrExcelFile_Consumption(FileInfo fileInfo)
    {
        return await WS.Customers.Billings.UploadBillingFileAsync(fileInfo);
    }

    #endregion

}
