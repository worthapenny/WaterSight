//using Ganss.Excel;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WaterSight.Excel;

//public class ExcelBase
//{
//    #region Constructor
//    public ExcelBase(string filePath)
//    {
//        FilePath = filePath;
//        if (!File.Exists(FilePath))
//            throw new FileNotFoundException(FilePath);

        

//        Xl = new ExcelMapper(filePath);
//    }
//    #endregion

//    #region Protected Properties
//    protected ExcelMapper Xl {  get;  } 
//    #endregion

//    #region Public Properties
//    public string FilePath { get; }    
//    #endregion
//}
