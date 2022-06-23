using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.HydrulicStructures;

public class PumpStation : WSItem
{
    #region Constructor
    public PumpStation(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Properties
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        return await WS.PostFile(EndPoints.HydStructuresPumpingStationsQDT, fileInfo, true, "Excel");
    }
    #endregion
}
