using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using WaterSight.WaterSightDataCollector.Support;
using WaterSight.WaterSightDataCollector.Support.Logging;

namespace WaterSight.WaterSightDataCollector.DB;

public class DatabaseManager : IDisposable
{
    #region Constants
    public static string SqlServerHostName = "";// "NAOU300282\\SQLSMSDN";
    public static string SqlServerDatabaseName = ""; // "WS_AwMoSTL";
    public static string Table_Name_Data = ""; // "AwMoSTL";
    public static string Col_Name_Timestamp = "Timestamp";
    public static string Col_Name_Tag = "Tag";
    public static string Col_Name_Value = "Value";

    #endregion

    #region Public Methods
    public static async Task<DateTimeOffset?> GetLatestDateTimeAsync(string tag)
    {

        if (string.IsNullOrEmpty(Table_Name_Data))
            throw new ApplicationException($"SQL Server data-table name cannot be blank");

        var query = @$"SELECT TOP 1 {Col_Name_Timestamp} FROM {Table_Name_Data} WHERE {Col_Name_Tag} = '{tag}' ORDER BY {Col_Name_Timestamp} DESC";
        Log.Debug($"Query: {query}");

        DateTimeOffset? at = null;
        OpenConnection();

        // wait for connection to be established
        while (Connection.State != ConnectionState.Open)
            await Task.Delay(100);


        // Create a command with the SQL query and associate it with the connection
        using var command = new SqlCommand(query, Connection);
        command.CommandTimeout = 3 * 60;
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var value = reader[0];
            Log.Debug($"Latest datetime in db: '{value}' for tag: '{tag}'");

            at = value is DBNull || value == null
                ? null
                : DateTimeOffset.Parse($"{value}");
        }

        Log.Debug($"Latest datetime for tag '{tag}' from the DB is: '{at}'");
        return at;
    }
    public static async Task<List<TSD>> ReadDatabaseForTSD(
        string tag,
        DateTimeOffset from,
        DateTimeOffset to
        )
    {
        if (string.IsNullOrEmpty(Table_Name_Data))
            throw new ApplicationException($"SQL Server data-table name cannot be blank");

        Log.Debug($"'✍️ Abut to collect data from {Table_Name_Data} table for {tag}, time range ['{from}', '{to}']");
        var startTime = Stopwatch.StartNew();

        var query = $"SELECT * FROM {Table_Name_Data} WHERE Tag = '{tag}' and Timestamp >= '{from}' and Timestamp < '{to}';";
        var data = new List<TSD>();
        try
        {

            var connection = await NewConnectionAsync();
            using var conn = connection;
            using var command = new SqlCommand();
            command.CommandText = $"SELECT * FROM {Table_Name_Data} WHERE Tag = '{tag}' and Timestamp >= '{from}' and Timestamp < '{to}';";
            command.Connection = connection;
            command.CommandTimeout = 3 * 60;

            Log.Debug($"Query: {command.CommandText}");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new TSD()
                {
                    Tag = Convert.ToString(reader["Tag"]) ?? string.Empty,
                    At = (DateTimeOffset)reader["Timestamp"],
                    Value = Convert.ToDouble(reader["Value"])
                };

                data.Add(row);
            }
        }
        catch (Exception ex)
        {
            var message = $"...while reading data from {Table_Name_Data} table for {tag}, time range ['{from}', '{to}']";
            Log.Error(ex, message);
            Debugger.Break();
        }

        Log.Debug($"✅ [🕒 {startTime.Elapsed}] Done collecting data from {Table_Name_Data} table for {tag}, time range ['{from}', '{to}']");
        LogLibrary.Separate_Dot();
        return data;
    }
    public static async Task<bool> WriteToDatabaseAsync(List<TSD> data)
    {
        if (string.IsNullOrEmpty(Table_Name_Data))
            throw new ApplicationException($"SQL Server data-table name cannot be blank");

        Log.Debug($"'✍️ Abut to write '{data.Count}' rows into the {Table_Name_Data} table for {data.First().Tag}");
        var startTime = Stopwatch.StartNew();

        var success = true;
        try
        {
            // Create datable of the data
            var dataTable = TSD.NewDataTable();
            data.ForEach(d => d.AddDataRow(dataTable));

            // Write data to the database
            using var newConnection = await NewConnectionAsync();


            using var bulkCopy = new SqlBulkCopy(newConnection);
            bulkCopy.BulkCopyTimeout = 5 * 60;

            bulkCopy.DestinationTableName = Table_Name_Data;
            await bulkCopy.WriteToServerAsync(dataTable);
        }
        catch (Exception ex)
        {
            var message = $"...while inserting data to the {Table_Name_Data} table. Error: {ex.Message}";
            Log.Error(ex, message);
            success = false;
            Debugger.Break();
        }

        Log.Debug($"✅ [🕒 {startTime.Elapsed}] Database updated with {data.Count} new records into the {Table_Name_Data} table");
        LogLibrary.Separate_Dot();

        return success;
    }

    public static void CloseSqliteConnection()
    {
        if (_connection != null)
        {
            _connection.Close();
            Log.Information($"Connection {_connection.State}");
            _connection = null;
            LogLibrary.Separate_Dot();
        }
    }

    public static async Task<SqlConnection> NewConnectionAsync()
    {
        if(string.IsNullOrEmpty(SqlServerHostName))
            throw new ApplicationException($"SQL Server host name cannot be blank");

        if(string.IsNullOrEmpty(SqlServerDatabaseName))
            throw new ApplicationException($"SQL Server database name cannot be blank");

        var connectionString = @$"Data Source={SqlServerHostName};Initial Catalog={SqlServerDatabaseName};Integrated Security=True;MultipleActiveResultSets=true;";
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        Log.Debug($"Connection String: {connectionString}");
        Log.Information($"Connection Status: {connection.State}. DB: {SqlServerDatabaseName}, Host: {SqlServerHostName}");

        return connection;
    }

    public void Dispose()
    {
        CloseSqliteConnection();
        _connection?.Dispose();
        _connection = null;
    }

    #endregion

    #region Private Methods
    private static string GetConnectionString()
    {
        var connectionString = @$"Data Source={SqlServerHostName};Initial Catalog={SqlServerDatabaseName};Integrated Security=True;MultipleActiveResultSets=true;";
        return connectionString;
    }
    private static SqlConnection GetSqlConnection()
    {
        if (_connection == null)
            _connection = OpenConnection();

        return _connection;
    }
    private static SqlConnection OpenConnection()
    {
        if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
        {

            var connectionString = @$"Data Source={SqlServerHostName};Initial Catalog={SqlServerDatabaseName};Integrated Security=True;MultipleActiveResultSets=true;";
            _connection = new SqlConnection(connectionString);
            _connection.Open();

            Log.Information($"Connection opened to '{SqlServerHostName}'");

        }
        return _connection;
    }
    #endregion

    #region Public Static Properties
    public static string ConnectionString => _connectionString ??= GetConnectionString();
    public static SqlConnection Connection => _connection ??= GetSqlConnection();
    #endregion

    #region Fields
    private static string? _connectionString = null;
    private static SqlConnection? _connection;
    #endregion
}
