using Haestad.Domain;
using Haestad.SCADA.Domain;
using Haestad.Support.Support;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaterSight.Model.Domain;

#region Extensions

public static class SCADADataSourceConnectionExtensions
{
    public static List<ISCADADataSource> SCADADataSources(this IWaterModelSupport _, IWaterModel waterModel)
    {
        var list = new List<ISCADADataSource>();
        foreach (var dataSourceId in waterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource).ElementIDs())
            list.Add(new SCADADataSource(waterModel, dataSourceId));

        return list;
    }
}
#endregion

#region Interfaces
public interface ISCADADataSource : IElement
{
    public ISCADASignals Signals { get; }
    public ISCADADataSourceConnection Connection { get; }
    public string TableName { get; }
    public DatabaseDataSourceFormat SourceFormat { get; }
    public string SignalNameField { get; }
    public string ValueField { get; }
    public string TimeStampField { get; }
    public string QuestionalbeField { get; }
    public bool IsHistorical { get; }
    public double TimeToleranceBackward { get; }
    public double TimeToleranceForward { get; }
    public bool IsZeroGoodQuality { get; }
    public string AvailableSignalsSQLStatement { get; }
    public string SignalDataSQLStatement { get; }
    public string DateTimeRangeSQLStatement { get; }
    public bool IsCustomizedSQL { get; }
}

public interface ISCADADataSourceConnection
{
    public string ConnectionLabel { get; }
    public DatabaseDataSourceType DataSourceType { get; }
    public string DataSource { get; }
    public string ConnectionString { get; }
}
#endregion

#region Internal Classes

[DebuggerDisplay("{ToString()}")]
internal class SCADADataSource : ISCADADataSource
{
    #region Constructor
    public SCADADataSource(IWaterModel waterModel, int dataSourceId)
    {
        WaterModel = waterModel;

        // TODO: Do Lazy Initialization
        InitializeFields();
        SupportElement = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource).Element(dataSourceId);
        Connection = new SCADADataSourceConnection(waterModel, SupportElement);
    }
    #endregion

    #region Private Methods
    private void InitializeFields()
    {
        var fieldManager = WaterModel.DomainDataSet.FieldManager;
        ScadaDataSource_TableName = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_TableName, (int)SupportElementType.ScadaDataSource);
        SourceFormatField = fieldManager.SupportElementField(StandardFieldName.SourceFormat, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_SignalNameField = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_SignalNameField, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_ValueField = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_ValueField, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_TimeStampField = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_TimeStampField, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_QuestionableField = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_QuestionableField, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_SupportsHistoricalData = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_SupportsHistoricalData, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_TimeTolerance = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_TimeTolerance, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_TimeToleranceForwards = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_TimeToleranceForwards, (int)SupportElementType.ScadaDataSource);
        ScadaDataSource_ZeroIsGoodQuality = fieldManager.SupportElementField(StandardFieldName.ScadaDataSource_ZeroIsGoodQuality, (int)SupportElementType.ScadaDataSource);
        Database_AvailableSignalsSQLStatement = fieldManager.SupportElementField(StandardFieldName.Database_AvailableSignalsSQLStatement, (int)SupportElementType.ScadaDataSource);
        Database_HistoricalOVPR = fieldManager.SupportElementField(StandardFieldName.Database_HistoricalOVPR, (int)SupportElementType.ScadaDataSource);
        Database_DateTimeRangeSQLStatement = fieldManager.SupportElementField(StandardFieldName.Database_DateTimeRangeSQLStatement, (int)SupportElementType.ScadaDataSource);
        Database_CustomizeSQLStatements = fieldManager.SupportElementField(StandardFieldName.Database_CustomizeSQLStatements, (int)SupportElementType.ScadaDataSource);

    }
    #endregion

    #region Private Properties
    IWaterModel WaterModel { get; }
    IModelingElement SupportElement { get; }
IField ScadaDataSource_TableName { get; set; }
    IField SourceFormatField { get; set; }
    IField ScadaDataSource_SignalNameField { get; set; }
    IField ScadaDataSource_ValueField { get; set; }
    IField ScadaDataSource_TimeStampField { get; set; }
    IField ScadaDataSource_QuestionableField { get; set; }
    IField ScadaDataSource_SupportsHistoricalData { get; set; }
    IField ScadaDataSource_TimeTolerance { get; set; }
    IField ScadaDataSource_TimeToleranceForwards { get; set; }
    IField ScadaDataSource_ZeroIsGoodQuality { get; set; }
    IField Database_AvailableSignalsSQLStatement { get; set; }
    IField Database_HistoricalOVPR { get; set; }
    IField Database_DateTimeRangeSQLStatement { get; set; }
    IField Database_CustomizeSQLStatements { get; set; }

    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"SCADA DataSource: {Id}: {Label}";
    }
    #endregion

    #region Public Properties
    public ISCADASignals Signals => WaterModel.Components.SCADASignals(Id);
    public ISCADADataSourceConnection Connection { get; }
    public string TableName => (string)ScadaDataSource_TableName.GetValue(Id);
    public DatabaseDataSourceFormat SourceFormat => (DatabaseDataSourceFormat)SourceFormatField.GetValue(Id);
    public string SignalNameField => (string)ScadaDataSource_SignalNameField.GetValue(Id);
    public string ValueField => (string)ScadaDataSource_ValueField.GetValue(Id);
    public string TimeStampField => (string)ScadaDataSource_TimeStampField.GetValue(Id);
    public string QuestionalbeField => (string)ScadaDataSource_QuestionableField.GetValue(Id);
    public bool IsHistorical => (bool)ScadaDataSource_SupportsHistoricalData.GetValue(Id);
    public double TimeToleranceBackward => (double)ScadaDataSource_TimeToleranceForwards.GetValue(Id);
    public double TimeToleranceForward => (double)ScadaDataSource_TimeTolerance.GetValue(Id);
    public bool IsZeroGoodQuality => (bool)ScadaDataSource_ZeroIsGoodQuality.GetValue(Id);
    public string AvailableSignalsSQLStatement => (string)Database_AvailableSignalsSQLStatement.GetValue(Id);
    public string SignalDataSQLStatement => (string)Database_HistoricalOVPR.GetValue(Id);
    public string DateTimeRangeSQLStatement => (string)Database_DateTimeRangeSQLStatement.GetValue(Id);
    public bool IsCustomizedSQL => (bool)Database_CustomizeSQLStatements.GetValue(Id);



    public int Id => SupportElement.Id;
    public string Notes { get { return SupportElement.Notes; } set { SupportElement.Notes = value; } }
    public ModelElementType ModelElementType => (ModelElementType)SupportElement.ModelingElementType;
    public string Label { get { return SupportElement.Label; } set { SupportElement.Label = value; } }
    string ILabeled.Label => SupportElement.Label;
    #endregion
}


[DebuggerDisplay("{ToString()}")]
internal class SCADADataSourceConnection : ISCADADataSourceConnection
{
    #region Constructor
    public SCADADataSourceConnection(IWaterModel waterModel, IModelingElement element)
    {
        WaterModel = waterModel;
        Element = element;

        // TODO: Do Lazy Initialization
        InitializeFields();
    }

    #endregion

    #region Private Methods
    private void InitializeFields()
    {
        var fieldManager = WaterModel.DomainDataSet.FieldManager;
        DatabaseType_Datasource = fieldManager.SupportElementField(StandardFieldName.DatabaseType_Datasource, (int)SupportElementType.ScadaDataSource);
        DatabaseDatasourceTypeField = fieldManager.SupportElementField(StandardFieldName.DatabaseDatasourceType, (int)SupportElementType.ScadaDataSource);
        DatabaseType_DatasourcePath = fieldManager.SupportElementField(StandardFieldName.DatabaseType_DatasourcePath, (int)SupportElementType.ScadaDataSource);
        DatabaseType_ConnectionString = fieldManager.SupportElementField(StandardFieldName.DatabaseType_ConnectionString, (int)SupportElementType.ScadaDataSource);
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ConnectionLabel}";
    }
    #endregion

    #region Public Properties

    public string ConnectionLabel => (string)DatabaseType_Datasource.GetValue(Element.Id);
    public DatabaseDataSourceType DataSourceType => (DatabaseDataSourceType)DatabaseDatasourceTypeField.GetValue(Element.Id);
    public string DataSource => (string)DatabaseType_DatasourcePath.GetValue(Element.Id);
    public string ConnectionString => (string)DatabaseType_ConnectionString.GetValue(Element.Id);

    #endregion

    #region Private Properties
    IWaterModel WaterModel { get; }
    IModelingElement Element { get; }
    IField DatabaseType_Datasource { get; set; }
    IField DatabaseDatasourceTypeField { get; set; }
    IField DatabaseType_DatasourcePath { get; set; }
    IField DatabaseType_ConnectionString { get; set; }

    #endregion
}

#endregion