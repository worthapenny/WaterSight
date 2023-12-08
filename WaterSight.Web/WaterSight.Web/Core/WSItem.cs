using Ganss.Excel;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WaterSight.Web.Core
{
    public abstract class WSItem
    {
        #region Constructor
        public WSItem(WS ws, string name = "")
        {
            WS = ws;
            Name = name;
        }
        #endregion

        #region Public Methods
        protected async Task<bool> WriteToExcelAsync<T>(
           IEnumerable<T> data,
           string filePath,
           string sheetName,
           ExcelMapper excelMapper = null)
        {
            var success = true;
            Log.Debug($"About to write to an Excel sheet {sheetName}. File: {filePath}");
            try
            {
                excelMapper ??= new ExcelMapper();
                await excelMapper.SaveAsync(filePath, data, sheetName);
                Log.Debug($"Updated '{sheetName}' excel sheet. File: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"...while saving the data to an Excel file. Path: {filePath}");
                success = false;
            }

            return success;
        }
        #endregion

        #region Public Properties
        public WS WS { get; }
        public string Name { get; }
        public EndPoints EndPoints => WS.EndPoints;
        public ILogger Logger => WS.Logger;
        #endregion
    }
}
