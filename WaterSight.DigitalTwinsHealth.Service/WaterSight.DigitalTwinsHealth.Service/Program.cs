using System.ServiceProcess;

namespace WaterSight.DigitalTwinsHealth.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DigitalTwinHealthService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
