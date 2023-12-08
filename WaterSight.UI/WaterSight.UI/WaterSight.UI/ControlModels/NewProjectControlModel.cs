using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Domain;
using WaterSight.UI.Support;
using WaterSight.Web.Core;
using WaterSight.Web.DT;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WaterSight.UI.ControlModels;

public class NewProjectControlModel : ReadWriteBase
{
    #region Constructor
    public NewProjectControlModel(
        WS ws,
        DigitalTwinConfig dtConfig)
        : base(ws, dtConfig)
    {
        WSProject = new WaterSightProject();
    }
    #endregion

    #region Public Overridden Methods
    public override Task<bool> WaterSightLoadAsync()
    {
        throw new NotImplementedException();
    }

    public override Task<bool> WaterSightSaveAsync()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Public Methods
   
    public async Task InitializeAsync()
    {
        WSProject = await WaterSightProject.LoadFromWebAsync(WS, DTConfig);
    }
    #endregion

    #region Public Properties
    public WaterSightProject WSProject { get; set; }

    public string DTInfo => $"{DTConfig?.ID}: {DTConfig?.Name}";
    #endregion
}
