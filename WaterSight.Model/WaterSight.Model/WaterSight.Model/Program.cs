using Haestad.Support.Library;
using OpenFlows.Application;
using OpenFlows.Water.Application;
using System;
using WaterSight.Model.Forms;

namespace WaterSight.Model;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static int Main()
    {
        WaterApplicationManager.SetApplicationManager(new WaterApplicationManager());
        WaterApplicationManager.GetInstance().SetParentFormSurrogateDelegate(
            new ParentFormSurrogateDelegate((fm) =>
            {
                return new WaterAppParentForm(fm);
            }));

        WaterApplicationManager.GetInstance().Start();

        ///In this case, the call to Start() is a blocking call (uses ApplicationMode.Run) so this log entry doesn't show up until the application is shutdown.
        TraceLibrary.WriteLine(HmTraceLevel.Release, $"OpenFlows.IApplicationManager::IsStarted: {WaterApplicationManager.GetInstance().IsStarted}");

        WaterApplicationManager.GetInstance().Stop();

        TraceLibrary.WriteLine(HmTraceLevel.Release, $"OpenFlows.IApplicationManager::IsStarted: {WaterApplicationManager.GetInstance().IsStarted}");

        return 0;
    }
}
