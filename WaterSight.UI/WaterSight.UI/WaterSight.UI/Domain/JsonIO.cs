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
    public static T? LoadFromFile<T>(string fileFullPath)
    {
        var didRead = true;
        var fileContents = string.Empty;

        if (!File.Exists(fileFullPath))
        {
            var message = $"Given file path is not valid. Path: {fileFullPath}";
            var ex = new FileNotFoundException(message);
            Log.Error(ex, message);

            Debugger.Break();
            throw ex;
        }

        try
        {
            Log.Debug($"About to read a file. Path: {fileFullPath}");
            fileContents = File.ReadAllText(fileFullPath);

            Log.Debug($"Read '{fileContents.Length}' content-length from a file. Path: {fileFullPath}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while reading a file. Path: {fileFullPath}");
            didRead = false;
            //Debugger.Break();
        }

        T? retVal = default;
        if (didRead && !string.IsNullOrEmpty(fileContents))
        {
            try
            {
                retVal = JsonConvert.DeserializeObject<T>(fileContents);
                Log.Information($"Deserialized the file contents of length '{fileContents.Length}'. Path: {fileFullPath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"...while deserializing the file contents of length '{fileContents.Length}'. Path: {fileFullPath}");
                Debugger.Break();
            }
        }

        return retVal;
    }
    #endregion
}
