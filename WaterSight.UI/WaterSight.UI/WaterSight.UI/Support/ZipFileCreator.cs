using Serilog;
using System.IO.Compression;

namespace WaterSight.UI.Support;

public static class ZipFileCreator
{
    /// <summary>
    /// Create a ZIP file of the files provided.
    /// </summary>
    /// <param name="fileName">The full path and name to store the ZIP file at.</param>
    /// <param name="files">The list of files to be added.</param>
    public static async Task<bool> CreateZipFileAsync(string fileName, IEnumerable<string> files)
    {
        var success = true;
        // Create and open a new ZIP file
        var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
        try
        {
            await Task.Run(() =>
            {
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        success = false;
                        Log.Error($"Given file path is invalid. Path: {file}");
                        break;
                    }

                    // Add the entry for each file
                    zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
                }

            });

            // Dispose of the object when we are done            
            zip.Dispose();

        }
        catch (System.Exception ex)
        {
            success = false;
            Log.Error(ex, "...while creating a compressed file");

            try
            {
                zip.Dispose();
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch
            {
            }
        }

        Log.Information($"[{success}] Zip file created. Path: {fileName}");
        return success;
    }
}
