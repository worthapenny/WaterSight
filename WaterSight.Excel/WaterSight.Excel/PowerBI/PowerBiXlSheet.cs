using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WaterSight.Excel.PowerBI;


[DebuggerDisplay("Count: {PowerBIItemsList.Count}")]
public class PowerBiXlSheet:ExcelSheetBase
{
    #region Constructor
    public PowerBiXlSheet(string excelFilePath)
        : base(ExcelSheetName.PowerBI, excelFilePath)
    {
        PowerBIItemsList = new List<PowerBiItem>();
    }
    #endregion

    #region Public Methods
    public void LoadFromExcel()
    {
        var excelMapper = new ExcelMapper(base.FilePath);
        PowerBIItemsList = excelMapper.Fetch<PowerBiItem>(base.SheetName).ToList();
    }
    #endregion

    #region Public Properties
    public List<PowerBiItem> PowerBIItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class PowerBiItem
{
    #region Constants
    public const char MenuEntryIntSeparator = ':';
    #endregion

    #region Constructor
    public PowerBiItem()
    {
    }
    public PowerBiItem(string displayName, string menuEntryStr, string url)
    {
        DisplayName = displayName;
        MenuEntryStr = menuEntryStr;
        Url = url;
    }

    #endregion

    #region Public Properties
    [Column(1, "Display Name")]
    public string DisplayName { get; set; }

    [Column(2, "Menu Entry")]
    public string MenuEntryStr { get; set; }

    [Ignore]
    public int MenuEntry
    {
        get
        {
            var menuItem = 0; // Power BI
            var menuItemKeyValuePair = MenuEntryStr.Split(MenuEntryIntSeparator);
            if (menuItemKeyValuePair.Length == 2)
            {
                menuItem = Convert.ToInt16(menuItemKeyValuePair.First());
            }
            else
            {
                throw new InvalidOperationException($"Menu Entry of '{MenuEntryStr}' is not in correct format. Correct format is 'nn:TextValue'.");
            }

            return menuItem;
        }
    }


    [Column(3, "Url")]
    public string Url { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName} in {MenuEntryStr}";
    }
    #endregion
}
