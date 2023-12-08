using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

namespace WaterSight.MonitoredFileDataPusher.IO;


public class JsonIO
{
    public static bool WriteToFile(string fileFullPath, object data, Formatting formatting = Formatting.Indented)
    {
        var fileInfo = new FileInfo(fileFullPath);

        var parentDirectory = fileInfo.Directory;
        if (parentDirectory == null)
            throw new DirectoryNotFoundException($"Directory for given file path is not valid. File path: {fileFullPath}");

        if (!parentDirectory.Exists)
            parentDirectory.Create();

        var jsonString = JsonConvert.SerializeObject(data, formatting);
        return WriteToFile(fileFullPath, jsonString);
    }
    public static bool WriteToFile(string fileFullPath, string data)
    {
        var didWrite = true;

        try
        {
            Log.Debug($"About to write '{data.Length}' content-length to a file. Path: {fileFullPath}");
            File.WriteAllText(fileFullPath, data);

            Log.Information($"Wrote '{data.Length}' content-length to a file. Path: {fileFullPath}");
        }
        catch (Exception ex)
        {
            Debugger.Break();
            Log.Error(ex, $"...while writing to the Json file. Path: {fileFullPath}");
            didWrite = false;
        }

        return didWrite;
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
                Debugger.Break();
                Log.Error(ex, $"...while deserializing the file contents of length '{fileContents.Length}'. Path: {fileFullPath}");
            }
        }

        return retVal;
    }
}
