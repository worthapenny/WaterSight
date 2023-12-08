using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.UI.Auth;

public enum AuthEvent
{
    LoggingIn =0,
    LoggingOut =1,
    LoggingError =2,
    LoggedIn=3,
    LoggedOut=4,
    LoggingOutError=5,
    RefreshStarted=6,
    RefreshCompleted=7,
    RefreshFailed=8,
}

public delegate EventHandler<AuthEvent> AuthEventHandler();