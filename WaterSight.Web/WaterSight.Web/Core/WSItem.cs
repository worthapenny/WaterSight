using Serilog;

namespace WaterSight.Web.Core
{
    public abstract class WSItem
    {
        #region Constructor
        public WSItem(WS ws)
        {
            WS = ws;
        }
        #endregion

        #region Public Properties
        public WS WS { get; }
        public EndPoints EndPoints => WS.EndPoints;
        public ILogger Logger => WS.Logger;
        #endregion
    }
}
