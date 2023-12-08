using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

namespace WaterSight.Domain;

public class JsonIO
{
    #region Public Methods
    public static bool WriteJsonToFile(object obj, string path)
    {
        var jsonString = JsonConvert.SerializeObject(obj);
        return WriteJsonToFile(jsonString, path);
    }
    public static bool WriteJsonToFile(string jsonContents, string jsonFilePath)
    {
        var stopWatch = Stopwatch.StartNew();
        Log.Information($"About to write json content to file path: {jsonFilePath}");

        var fileInfo = new FileInfo(jsonFilePath);
        if (!fileInfo?.Directory?.Exists ?? false)
        {
            fileInfo?.Directory?.Create();
            Log.Debug($"Json file path parent directory created. Path: {fileInfo.DirectoryName}");
        }

        var success = false;
        try
        {
            File.WriteAllText(jsonFilePath, jsonContents);
            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            success = false;
            var message = $"...while writing to a file, path: {jsonFilePath}";
            Log.Error(ex, message);
        }

        stopWatch.Stop();
        Log.Debug($"[{success}] Wrote json content to file path: {jsonFilePath}. Time taken: {stopWatch.Elapsed}. Success: {success}");

        return success;

    }
    #endregion
}
