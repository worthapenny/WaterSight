using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using WaterSight.Authenticator.Auth;
using WaterSight.Authenticator.ControlModel;
using WaterSight.Authenticator.Support;

namespace WaterSight.Authenticator;

public class Program
{

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    public static async Task Main(string[] args)
    {
        // redirect console output to parent process;
        // must be before any calls to Console.WriteLine()
        AttachConsole(ATTACH_PARENT_PROCESS);
        Console.WriteLine("About to setup logger");

        //
        // Logging
        //
        string logTemplate = "{Timestamp:dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate:logTemplate)
            .CreateLogger();

        Log.Information("Logging started for console");

        //
        // Make sure Admin rights are there
        if (!IsAdministrator())
        {
            Log.Information("Restarting as admin");
            var runningWithAdmin = StartAsAdmin(Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe"), args);

            if (runningWithAdmin)
            {
                Log.Information($"App is now running with admin in another instance. Exiting this one.");
                Environment.Exit(0);
            }
        }

        //
        // register for the auth events
        SignInControlModel.AuthEvent += (s, e) => AuthStateChanged(e);

        //
        // User Options
        //
        var intervalInMinutes = ParseArguments(args, out Env env);


        // Apply user options
        SignInControlModel.ServerEnvironment = env;
        SignInControlModel.REFRESH_LOGIN_INTERVAL_MINUTES = intervalInMinutes;
        //SignInControlModel.REFRESH_LOGIN_INTERVAL_MINUTES = 2;

        //
        // Start the process (of logging in)
        // 
        var signedIn = await SignInControlModel.SignInAsync();
        if (!signedIn)
            Log.Error($"Singin failed, auto refresh is not initialized");


        // keep the main thread alive
        Console.ReadLine();
    }

    #region Private Methods
    private static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        var hasAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

        Log.Information($"Is app running with admin rights: {hasAdmin}");
        return hasAdmin;
    }

    private static bool StartAsAdmin(string fileName, string[] args)
    {
        var proc = new Process
        {
            StartInfo =
            {
                FileName = fileName,
                UseShellExecute = true,
                Verb = "runas",
            }

        };


        // add arguments
        foreach (var arg in args)
            proc.StartInfo.ArgumentList.Add(arg);

        bool started;
        try
        {
            started = proc.Start();
            Log.Debug($"Attempted to start this app with Admin rights. Started: {started}, Name: {proc.ProcessName}, PID: {proc.Id}");
        }
        catch (Exception ex)
        {
            started = false;
            Log.Error(ex, $"...while starting the app with Admin rights");
        }

        return started;
    }

    private static int ParseArguments(string[] args, out Env env)
    {
        var intervalInMinutes = 30; // ever 30 minutes a new token will be generated
        env = Env.Prod;

        var argsLength = args.Length;
        Log.Debug($"Length of arguments = {args.Length} and args are: {string.Join(", ", args)}");

        if (argsLength > 0)
        {
            var firstArgs = args.Length == 1 ? args[1] : "30";
            var secondArgs = args.Length == 2 ? args[2] : "prod";

            try
            {
                intervalInMinutes = Convert.ToInt16(firstArgs);
                if (secondArgs.ToLower() == "qa" || secondArgs.ToLower() == "dev")
                    env = Env.Qa;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"...while parsing the arguments");
            }
        }

        return intervalInMinutes;
    }

    private static void AuthStateChanged(AuthEvent e)
    {
        switch (e)
        {
            case AuthEvent.LoggedIn:
                Log.Information($"Logged In. Email: {SignInControlModel.Email}. Token Length: {SignInControlModel.AccessToken?.Length}");
                WinRegistry.UpdateRegistry(SignInControlModel.AccessToken, SignInControlModel.ServerEnvironment == Env.Prod);
                break;

            case AuthEvent.RefreshCompleted:
                Log.Information($"Refreshed finished");
                WinRegistry.UpdateRegistry(SignInControlModel.AccessToken, SignInControlModel.ServerEnvironment == Env.Prod);
                break;

        }
    }

    #endregion

    #region Private Properties
    private static SignInControlModel SignInControlModel { get; set; } = new SignInControlModel();
    #endregion
}