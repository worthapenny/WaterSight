using Ganss.Excel;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Excel.Sensor;

[DebuggerDisplay("{ToString()}")]
public class PossibleSensorsXlSheet : ExcelSheetBase
{
    #region Constructor
    public PossibleSensorsXlSheet(string excelFilePath)
        : this(ExcelSheetName.PossibleSensors, excelFilePath)
    {
    }
    public PossibleSensorsXlSheet(string excelFilePath, string sheetName)
        : base(sheetName, excelFilePath)
    {
        PossibleSensorsItemsList = new List<PossibleSensorItem>();
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Sheet: {SheetName} Possible sensors count = {PossibleSensorsItemsList.Count}";
    }
    #endregion

    #region Public Properties
    public List<PossibleSensorItem> PossibleSensorsItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class PossibleSensorItem
{
    #region Constructor
    public PossibleSensorItem()
    {
    }
    public PossibleSensorItem(
        string sensorType,
        string tagName,
        string displayName,
        string attribute,
        string sourceElementIdLabel,
        string targetElmentIdLabel)
    {
        SensorType = sensorType;
        TagName = tagName;
        DisplayName = displayName;
        Attribute = attribute;
        SourceElementIdLabel = sourceElementIdLabel;
        TargetElementIdLabel = targetElmentIdLabel;
    }

    #endregion

    #region Public Properties
    public string SensorType { get; set; }
    public string TagName { get; set; }
    public string DisplayName { get; set; }
    public string SourceElementIdLabel { get; set; }
    public string TargetElementIdLabel { get; set; }

    [Ignore]
    public string Attribute { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DisplayName} [{Attribute}]";
    }
    #endregion
}