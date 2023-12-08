using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.UI;

public class App
{

    public static IConfigurationRoot GetConfiguration()
    {
        var cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var configFilePath = Path.Combine(cwd, "configuration.json");
        if (!File.Exists(configFilePath))
        {
            var message = $"Configuration.json could not be located at: {configFilePath}";
            var ex = new FileNotFoundException(message);
            Log.Error(ex, message);
            throw ex;
        }

        var builder = new ConfigurationBuilder()
                .SetBasePath(cwd)
                .AddJsonFile("configuration.json");

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

        return config;
    }
}
