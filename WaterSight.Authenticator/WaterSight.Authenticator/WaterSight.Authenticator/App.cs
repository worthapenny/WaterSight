using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace WaterSight.Authenticator;

public class App
{
    public static string ConfigFileName = "configuration.json";

    public static IConfigurationRoot GetConfiguration()
    {
        var cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var configFilePath = Path.Combine(cwd, ConfigFileName);
        if (!File.Exists(configFilePath))
        {
            var message = $"{ConfigFileName} could not be located at: {configFilePath}";
            var ex = new FileNotFoundException(message);
            Log.Error(ex, message);
            throw ex;
        }

        Log.Debug($"Config file path: {configFilePath}");
        var builder = new ConfigurationBuilder()
                .SetBasePath(cwd)
                .AddJsonFile(ConfigFileName);

        if (builder == null)
        {
            var message = $"Configuration builder couldn't be created";
            var ex = new ApplicationException(message);
            Log.Error(ex, message);
            throw ex;
        }

        var config = builder.Build();
        if (config == null)
        {
            var message = $"Configuration couldn't be built";
            var ex = new ApplicationException(message);
            Log.Error(ex, message);
            throw ex;
        }

        Log.Information($"Configuration file loaded. Path: {configFilePath}");
        return config;
    }
}
