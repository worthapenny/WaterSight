using Newtonsoft.Json;
using Serilog;
using WaterSight.Domain;
using WaterSight.Web.Core;
using WaterSight.Web.DT;

namespace WaterSight.UI.Support;

public abstract class ReadWriteBase
{
    #region Constructor
    public ReadWriteBase(
        WS ws,
        DigitalTwinConfig dtConfig //,
        //string projectFilePath
        )
    {
        WS = ws;
        DTConfig = dtConfig;
        //ProjectFilePath = projectFilePath;
    }
    #endregion

    #region Public Methods
    public abstract Task<bool> WaterSightLoadAsync();
    public abstract Task<bool> WaterSightSaveAsync();

    public async Task<T?> FileLoadAsync<T>(string filePath)
    {
        T? fileObject = default;
        try
        {
            var jsonString = await File.ReadAllTextAsync(filePath);
            fileObject = JsonConvert.DeserializeObject<T>(jsonString);
            if(fileObject == null)
                Log.Error($"File parse error. Path: {filePath}");
            
        }
        catch (Exception ex)
        {
            Log.Error(ex, "...while read/parsing the file/file-content");
        }

        return fileObject;
    }
    public SaveFileDialog GetSaveFileDialog()
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "WS Project files (*.ws.json)|*.ws.json";
        saveFileDialog.CheckFileExists = false;
        saveFileDialog.Title = "Create WaterSight Project File";

        return saveFileDialog;
    }
    public async Task<bool> FileSaveAsync(object content, string fullFilePath)
    {
        var success = true;
        if(File.Exists(fullFilePath))
            ProjectFilePath = fullFilePath;
        else
            throw new FileNotFoundException(fullFilePath);

        try
        {
            var jsonContent = JsonConvert.SerializeObject(content, Formatting.Indented);
            await File.WriteAllTextAsync(fullFilePath, jsonContent);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "...while writing to the file");
            success = false;
        }

        return success;
    }

    #endregion

    #region Protected Properties
    protected WS WS { get; }
    protected DigitalTwinConfig DTConfig { get; set; }
    #endregion

    #region Public Properties
    public string? ProjectFilePath { get; private set; }
    #endregion
}
