using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;
using WaterSight.UI.ControlModels;

namespace WaterSight.UI.App;

public class AppConfig
{
    public static event EventHandler<NewProjectControlModel?>? ProjectSelectionChanged;

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

    public static NewProjectControlModel? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            ProjectSelectionChanged?.Invoke(null, value);
        }
    }

    private static NewProjectControlModel? _selectedProject;
}
