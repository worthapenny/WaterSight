using Ganss.Excel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Excel.Extensions;

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
        public bool SheetExists(string name)
        {
            var exists = this.Xl.FetchSheetNames().Where(n => n == name).Any();
            return exists; 
        }
        public List<T> Read<T>()
        {
            if (!SheetExists(this.SheetName))
            {
                Log.Error($"Given sheet '{this.SheetName}' does not exists in the give file. Path: {this.FilePath}");
                Debugger.Break();
            }

            var items = this.Xl.Fetch<T>(this.FilePath, SheetName);
            return new List<T>(items);
        }

        public bool Save<T>(List<T> data)
        {
            var success = true;
            try
            {
                Log.Debug($"About to write to an Excel sheet {SheetName}. File: {FilePath}");
                Xl.Save(FilePath, data, SheetName);

                Log.Information($"Updated '{SheetName}' excel sheet. File: {FilePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"...while writing to the Excel file. {FilePath}");
                success = false;
            }

            return success;
        }
        public async Task<bool> SaveAsync<T>(List<T> data)
        {
            var success = true;
            Log.Debug($"About to write to an Excel sheet {SheetName}. File: {FilePath}");
            try
            {
                var xlMapper = File.Exists(FilePath) ? new ExcelMapper(FilePath) : new ExcelMapper();                
                await xlMapper.SaveAsync(FilePath, data, SheetName).TimeoutAfter(new TimeSpan(0, 0, 30), "save to Excel");

                Log.Debug($"Wrote to an Excel sheet {SheetName}. File: {FilePath}");

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
