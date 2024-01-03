using WaterSight.WaterSightDataCollector.DB;
using WaterSight.WaterSightDataCollector.Domain;
using WaterSight.Web.Core;

namespace WaterSight.WaterSightDataCollector;

public class Program
{

    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        // Report the arguments list
        Console.WriteLine($"Args: " + string.Join(", ", args));

        if (args.Length != 7)
        {
            Console.WriteLine($"Argument length is not right. Must be: dtId, dtName, env, collect/load, sqlServerHost, sqlServerDbName, sqlServerTableName");
            return 1;
        }

        var dtID = Convert.ToInt32(args[0]);    // DT ID
        var dtName = args[1];   // DT Name
        var envStr = args[2];   // Environment [prod, qa, dev]
        var action = args[3];   // [collect, load]
        var sqlServerHost = args[4];            // SQL Server Host address
        var sqlServerDatabaseName = args[5];    // SQL Server Database Name
        var sqlServerTableName = args[6];       // SQL Server Table Name in the above database


        // DT ID
        if (dtID == 0)
        {
            await Console.Out.WriteLineAsync("Digital twin ID cannot be 0.");
            return 1;
        }

        // DT Name
        if (string.IsNullOrEmpty(dtName))
        {
            await Console.Out.WriteLineAsync("Digital twin name cannot be blank.");
            return 1;
        }

        // Environment
        if (string.IsNullOrEmpty(envStr))
        {
            await Console.Out.WriteLineAsync("Digital twin environment cannot be blank.");
            return 1;
        }

        // Action
        if (string.IsNullOrEmpty(action))
        {
            await Console.Out.WriteLineAsync("Action (collect/load) cannot be blank.");
            return 1;
        }

        // SQL Server Host Address
        if (string.IsNullOrEmpty(sqlServerHost))
        {
            await Console.Out.WriteLineAsync("SQL Server host cannot be blank.");
            return 1;
        }

        // SQL Server Database Name
        if (string.IsNullOrEmpty(sqlServerDatabaseName))
        {
            await Console.Out.WriteLineAsync("SQL Server Database name cannot be blank.");
            return 1;
        }

        // SQL Server Table Name
        if (string.IsNullOrEmpty(sqlServerTableName))
        {
            await Console.Out.WriteLineAsync("SQL Server table name cannot be blank.");
            return 1;
        }


        var env = Env.Prod;
        if (envStr.ToLower() == "qa")
            env = Env.Qa;
        else if (envStr.ToLower() == "dev")
            env = Env.Dev;


        // Set the SQL Server Information
        DatabaseManager.SqlServerHostName = sqlServerHost;
        DatabaseManager.SqlServerDatabaseName = sqlServerDatabaseName;
        DatabaseManager.Table_Name_Data = sqlServerTableName;


        // collect the data
        if (action == "collect")
        {
            var dataCollector = new DataCollector(
            dtID: dtID,
            dtName: dtName,
            env: env);

            await dataCollector.CollectDataAndWriteToDatabaseAsync();
        }

        // load the data
        else if (action == "load")
        {
            var dataLoader = new DataLoader(
                dtID: dtID,
                dtName: dtName,
                env: env);

            await dataLoader.LoadDataAsync();
        }


        return 0;

    }
}