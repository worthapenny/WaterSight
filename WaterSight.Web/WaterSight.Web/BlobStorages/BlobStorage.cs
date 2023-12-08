using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.BlobStorages;

public  class BlobStorage: WSItem
{

    #region Constructor
    public BlobStorage(WS ws):base(ws)
    {        
    }
    #endregion

    #region Public Methods
    public async Task<string> GetStorageTokenUrlAsync()
    {
        var url = EndPoints.BlobStorageSasUploadQDT;
        var tokenUrl = await WS.GetAsync<string>(url, null, "Blob Storage Token");
        return tokenUrl;
    }
    #endregion
}
