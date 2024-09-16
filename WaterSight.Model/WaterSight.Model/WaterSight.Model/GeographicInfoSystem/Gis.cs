//using Haestad.Shapefile;
//using Haestad.Support.Support;
//using OpenFlows.Water.Domain;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;

//namespace WaterSight.Model.GeographicInfoSystem;

//public class Gis
//{
//    #region Constructor
//    public Gis(IWaterModel waterModel)
//    {
//        WaterModel = waterModel;
//    }
//    #endregion

//    #region Public Methods
//    public bool CreatePipeShapefile(
//            string shapefilePath,
//            out List<string> shapeFileNames,
//            string sourceProjectionFilePath = null)
//    {
//        var fields = new List<DBFFieldInfo>();
//        var modelIdField = new DBFFieldInfo()
//        {
//            FieldLength = 16,
//            FieldName = "ModelID",
//            FieldType = DBFFieldType.Numeric
//        };
//        fields.Add(modelIdField);

//        var data = new DataTable();
//        var modelIdDataColumn = new DataColumn(modelIdField.FieldName, typeof(int));
//        data.Columns.Add(modelIdDataColumn);

//        var pipeGeometryList = new List<GeometryPoint[]>();
//        var geometryMap = WaterModel.Network.Pipes.Input.Geometries();
        
//        foreach (var geometryItem in geometryMap)
//        {
//            pipeGeometryList.Add(geometryItem.Value.ToArray());

//            var row = data.NewRow();
//            row[modelIdField.FieldName] = (int)geometryItem.Key;
//            data.Rows.Add(row);
//        }
        

//        var success = Shapefile.CreateShapefile(
//            fullFilePath: shapefilePath,
//            fields: fields,
//            data: data,
//            pipeGeometryList: pipeGeometryList,
//            sourceProjectionFilePath: sourceProjectionFilePath
//            );


//        shapeFileNames = new List<string>();
//        if (success)
//        {
//            var shapefilesDir = Path.GetDirectoryName(shapefilePath);
//            var shapefileName = Path.GetFileNameWithoutExtension(shapefilePath);

//            shapeFileNames.Add(shapefilePath);
//            shapeFileNames.Add(Path.Combine(shapefilesDir, $"{shapefileName}.dbf"));
//            shapeFileNames.Add(Path.Combine(shapefilesDir, $"{shapefileName}.shx"));
//            if (!string.IsNullOrEmpty(sourceProjectionFilePath))
//                shapeFileNames.Add(Path.Combine(shapefilesDir, $"{shapefileName}.prj"));
//        }

//        return success;
//    }
//    #endregion

//    #region Private Properties
//    private IWaterModel WaterModel { get; set; }
//    #endregion
//}
