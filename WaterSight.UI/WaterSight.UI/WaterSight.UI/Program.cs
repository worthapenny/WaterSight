using Serilog;
using WaterSight.UI.Support.Logging;

namespace WaterSight.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Setup logger
            Logging.SetupLogger();

            Application.Run(new Forms.WaterSightParentForm());

            Log.Information($"Application is about to exit");
        }
    }
}