using Ganss.Excel;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WaterSight.Excel
{
    public abstract class ExcelSheetBase
    {
        #region Constructor
        public ExcelSheetBase(string sheetName, string filePath)
        {
            SheetName = sheetName;
            FilePath = filePath;

            Xl = File.Exists(filePath) ? new ExcelMapper(filePath) : new ExcelMapper();
        }
        #endregion

        #region Public Methods
        public List<T> Read<T>()
        {
            var items = this.Xl.Fetch<T>(this.FilePath, SheetName);
            return new List<T>(items);
        }

        public void Save<T>(List<T> data)
        {
            Log.Debug($"About to write to an Excel sheet {SheetName}. File: {FilePath}");
            Xl.Save(FilePath, data, SheetName);

            Log.Information($"Updated '{SheetName}' excel sheet. File: {FilePath}");
        }
        public async Task<bool> SaveAsync<T>(List<T> data)
        {
            var success = true;
            Log.Debug($"About to write to an Excel sheet {SheetName}. File: {FilePath}");
            try
            {
                var xlMapper = File.Exists(FilePath) ? new ExcelMapper(FilePath) : new ExcelMapper();
                await xlMapper.SaveAsync(FilePath, data, SheetName);

                Log.Debug($"Updated '{SheetName}' excel sheet. File: {FilePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"...while saving the data to an Excel file. Path: {FilePath}");
                success = false;
            }

            return success;
        }

        #endregion

        #region Public Properties
        public string SheetName { get; set; }
        public string FilePath { get; protected set; }
        #endregion

        #region Protected Properties
        protected ExcelMapper Xl { get; set; }
        #endregion
    }
}
