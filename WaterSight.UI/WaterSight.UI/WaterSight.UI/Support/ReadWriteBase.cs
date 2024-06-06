//using Newtonsoft.Json;
//using Serilog;
//using System.Diagnostics;
//using WaterSight.Domain;
//using WaterSight.Web.Core;
//using WaterSight.Web.DT;

//namespace WaterSight.UI.Support;

//public abstract class ReadWriteBase
//{
//    #region Constructor
//    public ReadWriteBase(
//        WS ws //,
//        //DigitalTwinConfig dtConfig //,
//        //string projectFilePath
//        )
//    {
//        WS = ws;
//        //DTConfig = dtConfig;
//        //ProjectFilePath = projectFilePath;
//    }
//    #endregion

//    #region Public Methods
//    public abstract Task<bool> WaterSightLoadAsync();
//    public abstract Task<bool> WaterSightSaveAsync(WaterSightProject wsProject);

//    public async Task<T?> FileLoadAsync<T>(string filePath)
//    {
//        T? fileObject = default;
//        try
//        {
//            var jsonString = await File.ReadAllTextAsync(filePath);
//            fileObject = JsonConvert.DeserializeObject<T>(jsonString);
//            if (fileObject == null)
//                Log.Error($"File parse error. Path: {filePath}");

//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex, "...while read/parsing the file/file-content");
//        }

//        return fileObject;
//    }

    

//    #endregion

//    #region Protected Properties
//    #endregion

//    #region Public Properties
//    public WS WS { get; }
//    public DigitalTwinConfig? DTConfig { get; set; }
//    public string? ProjectFilePath { get; set; }
//    #endregion
//}
