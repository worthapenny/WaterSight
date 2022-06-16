using System.IO;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.HydrulicStructures;

public class Pump : WSItem
{
    #region Constructor
    public Pump(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods
    public async Task<bool> PostExcelFile(FileInfo fileInfo)
    {
        return await WS.PostFile(EndPoints.HydStructuresPumpsQDT, fileInfo, "Excel");
    }
    #endregion

}
