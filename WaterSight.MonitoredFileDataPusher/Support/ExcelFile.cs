using ExcelDataReader;
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.MonitoredFileDataPusher.Support;

public class ExcelFile
{
    #region Constructor
    public ExcelFile(string xlFilePath)
    {
        if (!File.Exists(xlFilePath))
            throw new FileNotFoundException(xlFilePath);

        XlFilePath = xlFilePath;
    }
    #endregion

    #region Public Methods
    public IEnumerable<DataTable> Read(string sheetName = "")
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using (var stream = File.Open(XlFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    if (reader.Name == sheetName || string.IsNullOrEmpty(sheetName))
                    {
                        // Load the content of the sheet into a DataTable
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        yield return dt;
                    }
                } while (reader.NextResult());
            }
        }
    }

    public void ReadExcel(string sheetName)
    {
        var data = new ExcelMapper(XlFilePath).Fetch();
        foreach (var d in data)
        {

        }
    }
    #endregion


    #region Private Properties
    private string XlFilePath;
    #endregion
}
