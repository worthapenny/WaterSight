using Haestad.Domain;
using Haestad.SCADA.Domain;
using Haestad.SCADA.Domain.Support;
using Haestad.SCADA.Forms.Domain.Datasources;
using Haestad.Support.Support;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Domain.Scada;



#region Interfaces
public interface ISCADADataSourceHistorical : IElement
{
    public void Add(string label);
    public int LoadSCADATags();
    public TestConnectionResult TestConnection();
    public void CommitChanges();
    void Delete();

    public DatabaseDataSourceType DataSourceType { get; set; }
    public string DataFilePath { get; set; }
    public bool UseConnectionString { get; set; }
    public string ConnectionString { get; set; }
    public string QueryDateTimeDelimiter { get; set; }


    public string SCADADataSourceConnection { get; }
    public string TableName { get; set; }
    public DatabaseDataSourceFormat SourceFormat { get; set; }
    public string SignalColumnName { get; set; }
    public string ValueColumnName { get; set; }
    public string TimestampColumnName { get; set; }
    public string QuestionableColumnName { get; set; }
    public bool IsHistorical { get; set; }
    public bool IsRealTime { get; set; }
    public double TimeToleranceBackwardInMinutes { get; set; }
    public double TimeToleranceForwardInMinutes { get; set; }
    public bool IsZeroGoodQuality { get; set; }


    public bool UseCustomizedSQL { get; set; }
    public string AvailableSignalsSQLQuery { get; set; }
    public string SignalDataSQLQueryOVPR { get; set; }
    public string SignalDataSQLQueryMVPR { get; set; }
    public string DateTimeRangeSQLQuery { get; set; }

    public ISCADASignals Signals { get; }

    public TransformType PumpTransformType { get; set; }
    public RelationalOperator PumpOnOperator { get; set; }
    public double PumpOnRawSignalValue { get; set; }


    public TransformType ValveTransformType { get; set; }
    public RelationalOperator ValveActiveOperator { get; set; }
    public double ValveActiveValue { get; set; }
    public RelationalOperator ValveClosedOperator { get; set; }
    public double ValveClosedValue { get; set; }
}

#endregion

#region Internal Classes

[DebuggerDisplay("{ToString()}")]
internal class SCADADataSource : ISCADADataSourceHistorical
{
    #region Constructor
    public SCADADataSource(IWaterModel waterModel)
    {
        WaterModel = waterModel;
        SupportElementManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource);

        // TODO: Do Lazy Initialization
        InitializeFields();

    }
    public SCADADataSource(IWaterModel waterModel, int dataSourceId)
        : this(waterModel)
    {
        SupportElement = SupportElementManager.Element(dataSourceId);
    }
    #endregion

    #region Public Methods
    public void Add(string label)
    {
        var supportElementManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource);
        var id = supportElementManager.Add();
        SupportElement = supportElementManager.Element(id);
        SupportElement.Label = label;

        Log.Information($"New data source added: {this.IdLabel()}");
    }
    public void Delete()
    {
        var supportElementManager = WaterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource);
        var idLabel = this.IdLabel();

        supportElementManager.Delete(Id);
        Log.Information($"Data source deleted: {idLabel}");
    }
    public int LoadSCADATags()
    {
        var tagCount = 0;
        try
        {
            using (DatabaseDataSource dbDataSource = DataSource)
            {
                UpdateDataSourceProperties(dbDataSource);

                dbDataSource.Connection.Open();

                var tagsFromDB = dbDataSource.GetAvailableSignals().ToList();
                var scadaSignalsManager = WaterModel.Components.SCADASignals(Id);
                foreach (var tag in tagsFromDB)
                {
                    var signal = scadaSignalsManager.Create();
                    signal.SignalLabel = tag.Name;
                    signal.Label = tag.Name;

                    tagCount++;
                }

                Log.Information($"Loaded '{tagCount}' signals from {DataFilePath}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while loading the signals from DataSource");
        }
        finally
        {
            if (DataSource.Connection.IsOpen)
                DataSource.Connection.Close();
        }

        return tagCount;
    }
    public TestConnectionResult TestConnection()
    {
        var connectionResult = DataSource.Connection.TestConnection(null);
        Log.Information($"TestConnection result: {connectionResult.ToString()}");

        return connectionResult;
    }
    public void CommitChanges()
    {
        //
        // Units
        var unitManager = new DatasourceUnitManager(
            SupportElement.Id,
            GetUnitFields(),
            WaterApplicationManager.GetInstance().ParentFormModel.CurrentProject.NumericPresentationManager,
            "Unit");

        // Note: During the construction of DataSourceUnitManager, it initializes
        // with the default fields, hence calling Commit is sufficient
        unitManager.Commit();

        //
        // Transform 
        TransformerElement.Commit();
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        var path = UseConnectionString ? ConnectionString : DataFilePath;
        return $"{Id}: {Label} - [{DataSourceType}]: {path}";
    }
    #endregion

    #region Private Methods
    private void InitializeFields()
    {

        // Database Connection (dialog)
        DatabaseType_DBSourceTypeField = SupportEditField(StandardFieldName.DatabaseDatasourceType);
        DatabaseType_DataSourceField = SupportEditField(StandardFieldName.DatabaseType_Datasource);
        DatabaseType_UseConnectionStringField = SupportEditField(StandardFieldName.DatabaseType_UseCustomConnectionString);
        DatabaseType_ConnectionStringField = SupportEditField(StandardFieldName.DatabaseType_ConnectionString);
        DatabaseType_DateTimeSQLPrefixField = SupportEditField(StandardFieldName.DatabaseType_DateTimeSQLPrefix);
        DatabaseType_DateTimeSQLSufixField = SupportEditField(StandardFieldName.DatabaseType_DateTimeSQLSuffix);

        // Database Source (dialog) -> Tab = Database Source
        ScadaDataSourceField = SupportEditField(StandardFieldName.DatabaseType_DatasourcePath);
        ScadaDataSource_TableNameField = SupportEditField(StandardFieldName.ScadaDataSource_TableName);
        SourceFormatField = SupportEditField(StandardFieldName.SourceFormat);
        ScadaDataSource_SignalNameField = SupportEditField(StandardFieldName.ScadaDataSource_SignalNameField);
        ScadaDataSource_ValueField = SupportEditField(StandardFieldName.ScadaDataSource_ValueField);
        ScadaDataSource_TimeStampField = SupportEditField(StandardFieldName.ScadaDataSource_TimeStampField);
        ScadaDataSource_QuestionableField = SupportEditField(StandardFieldName.ScadaDataSource_QuestionableField);
        ScadaDataSource_SupportsHistoricalDataField = SupportEditField(StandardFieldName.ScadaDataSource_SupportsHistoricalData);
        ScadaDataSource_SupportsRealTimeDataField = SupportEditField(StandardFieldName.ScadaDataSource_SupportsRealtimeData);
        ScadaDataSource_TimeToleranceBackwardField = SupportEditField(StandardFieldName.ScadaDataSource_TimeTolerance);
        ScadaDataSource_TimeToleranceForwardsField = SupportEditField(StandardFieldName.ScadaDataSource_TimeToleranceForwards);
        ScadaDataSource_ZeroIsGoodQualityField = SupportEditField(StandardFieldName.ScadaDataSource_ZeroIsGoodQuality);

        // SQL Statements (dialog) 
        Database_CustomizeSQLStatementsField = SupportEditField(StandardFieldName.Database_CustomizeSQLStatements);
        Database_AvailableSignalsSQLStatementField = SupportEditField(StandardFieldName.Database_AvailableSignalsSQLStatement);
        Database_HistoricalOVPRField = SupportEditField(StandardFieldName.Database_HistoricalOVPR);
        Database_HistoricalMVPRField = SupportEditField(StandardFieldName.Database_HistoricalMVPR);
        Database_DateTimeRangeSQLStatementField = SupportEditField(StandardFieldName.Database_DateTimeRangeSQLStatement);

        // Units Fields
        /* Units fields are initialized and defaults are saved under CommitChanges() method */

    }
    private IEditField SupportEditField(string fieldName)
    {
        return (IEditField)SupportField(fieldName);
    }
    private IField SupportField(string fieldName)
    {
        return SupportElementManager.SupportElementField(fieldName);
    }
    private FieldCollection GetUnitFields()
    {
        var fields = new FieldCollection
        {
            SupportField(StandardFieldName.ScadaUnit_Concentration),
            SupportField(StandardFieldName.ScadaUnit_Demand),
            SupportField(StandardFieldName.ScadaUnit_Flow),
            SupportField(StandardFieldName.ScadaUnit_FlowSetting),
            //SupportField(StandardFieldName.ScadaUnit_Headloss), // Not Supported by the API for some reason 
            SupportField(StandardFieldName.ScadaUnit_HydraulicGrade),
            SupportField(StandardFieldName.ScadaUnit_HydraulicGradeSetting),
            SupportField(StandardFieldName.ScadaUnit_Level),
            SupportField(StandardFieldName.ScadaUnit_Power),
            SupportField(StandardFieldName.ScadaUnit_Pressure),
            SupportField(StandardFieldName.ScadaUnit_RelativeClosure),
            //SupportField(StandardFieldName.ScadaUnit_Velocity) // Not Supported by the API for some reason
        };

        return fields;
    }
    private void UpdateDataSourceProperties(DatabaseDataSource dbDataSource)
    {
        dbDataSource.SourceFormat = SourceFormat;
        dbDataSource.TableName = TableName;
        dbDataSource.SignalNameField = SignalColumnName;
        dbDataSource.SignalValueField = ValueColumnName;
        dbDataSource.SignalDateTimeField = TimestampColumnName;
        dbDataSource.SignalQuestionableField = QuestionableColumnName;
        dbDataSource.TimeToleranceInSecondsBackwards = (int)TimeToleranceBackwardInMinutes * 60;
        dbDataSource.TimeToleranceInSecondsForwards = (int)TimeToleranceForwardInMinutes * 60;

        dbDataSource.AvailableSignalsSelectStatement = AvailableSignalsSQLQuery;
        dbDataSource.HistoricalSelectStatement = this.SourceFormat == DatabaseDataSourceFormat.MultipleValuesPerRow
                        ? SignalDataSQLQueryMVPR : SignalDataSQLQueryOVPR;
        dbDataSource.HistoricalDateTimeRangeSelectStatement = DateTimeRangeSQLQuery;

        dbDataSource.Connection.DateTimeSQLPrefix = QueryDateTimeDelimiter;
        dbDataSource.Connection.DateTimeSQLSuffix = QueryDateTimeDelimiter;

        dbDataSource.CustomizedSQLStatement = dbDataSource.HistoricalSelectStatement;
    }

    private DatabaseDataSource GetNewDataSource()
    {
        var source = (DatabaseDataSource)ScadaFactory.NewDatabaseDataSource(
            scadaDataSourceType: DataSourceType,
            databaseDataSourceFormat: SourceFormat,
            dataSourcePath: DataFilePath);

        if (UseConnectionString)
            source.Connection.ConnectionString = ConnectionString;

        return source;
    }
    private DataSourceTransformerElement GetTransformElement()
    {
        return new DataSourceTransformerElement(WaterModel.DomainDataSet, Id, SupportElementType.ScadaDataSource);
    }
    #endregion

    #region Public Properties
    public string IdLabel => $"{SupportElement.Id}: {SupportElement.Label}";
    public DatabaseDataSourceType DataSourceType
    {
        get => (DatabaseDataSourceType)this.DatabaseType_DBSourceTypeField.GetValue(Id);
        set => this.DatabaseType_DBSourceTypeField.SetValue(Id, value);
    }
    string _dbFilePath;
    public string DataFilePath
    {
        get => _dbFilePath;
        set
        {
            _dbFilePath = value;
            var fileInfo = new FileInfo(value);
            var displayPath = fileInfo.Exists
                ? fileInfo.Name
                : value;

            // this field shows up in the Edit DataSource, "Database Source" dialog
            this.DatabaseType_DataSourceField.SetValue(
                Id,
                UseConnectionString ? ConnectionString : displayPath);

            // this field shows up in "Database Connection" little dialog
            this.ScadaDataSourceField.SetValue(
                Id,
                UseConnectionString ? string.Empty : value);

        }
    }
    public bool UseConnectionString
    {
        get => (bool)this.DatabaseType_UseConnectionStringField.GetValue(Id);
        set => this.DatabaseType_UseConnectionStringField.SetValue(Id, value);
    }
    public string ConnectionString
    {
        get => (string)this.DatabaseType_ConnectionStringField.GetValue(Id);
        set => this.DatabaseType_ConnectionStringField.SetValue(Id, value);
    }
    public string QueryDateTimeDelimiter
    {
        get => (string)this.DatabaseType_DateTimeSQLPrefixField.GetValue(Id);
        set
        {
            this.DatabaseType_DateTimeSQLPrefixField.SetValue(Id, value);
            this.DatabaseType_DateTimeSQLSufixField.SetValue(Id, value);
        }
    }


    public string SCADADataSourceConnection
    {
        get => (string)this.ScadaDataSourceField.GetValue(Id);
        //set => this.ScadaDataSourceField.SetValue(Id, value);
    }
    public string TableName
    {
        get => (string)this.ScadaDataSource_TableNameField.GetValue(Id);
        set => this.ScadaDataSource_TableNameField.SetValue(Id, value);
    }
    public DatabaseDataSourceFormat SourceFormat
    {
        get => (DatabaseDataSourceFormat)this.SourceFormatField.GetValue(Id);
        set => this.SourceFormatField.SetValue(Id, value);
    }
    public string SignalColumnName
    {
        get => (string)this.ScadaDataSource_SignalNameField.GetValue(Id);
        set => this.ScadaDataSource_SignalNameField.SetValue(Id, value);
    }
    public string ValueColumnName
    {
        get => (string)this.ScadaDataSource_ValueField.GetValue(Id);
        set => this.ScadaDataSource_ValueField.SetValue(Id, value);
    }
    public string TimestampColumnName
    {
        get => (string)this.ScadaDataSource_TimeStampField.GetValue(Id);
        set => this.ScadaDataSource_TimeStampField.SetValue(Id, value);
    }
    public string QuestionableColumnName
    {
        get => (string)this.ScadaDataSource_QuestionableField.GetValue(Id);
        set => this.ScadaDataSource_QuestionableField.SetValue(Id, value);
    }
    public bool IsHistorical
    {
        get => (bool)this.ScadaDataSource_SupportsHistoricalDataField.GetValue(Id);
        set => this.ScadaDataSource_SupportsHistoricalDataField.SetValue(Id, value);
    }
    public bool IsRealTime
    {
        get => (bool)this.ScadaDataSource_SupportsRealTimeDataField.GetValue(Id);
        set => this.ScadaDataSource_SupportsRealTimeDataField.SetValue(Id, value);
    }
    public double TimeToleranceBackwardInMinutes
    {
        get => (double)this.ScadaDataSource_TimeToleranceBackwardField.GetValue(Id);
        set => this.ScadaDataSource_TimeToleranceBackwardField.SetValue(Id, value);
    }
    public double TimeToleranceForwardInMinutes
    {
        get => (double)this.ScadaDataSource_TimeToleranceForwardsField.GetValue(Id);
        set => this.ScadaDataSource_TimeToleranceForwardsField.SetValue(Id, value);
    }
    public bool IsZeroGoodQuality
    {
        get => (bool)this.ScadaDataSource_ZeroIsGoodQualityField.GetValue(Id);
        set => this.ScadaDataSource_ZeroIsGoodQualityField.SetValue(Id, value);
    }



    public bool UseCustomizedSQL
    {
        get => (bool)this.Database_CustomizeSQLStatementsField.GetValue(Id);
        set => this.Database_CustomizeSQLStatementsField.SetValue(Id, value);
    }
    public string AvailableSignalsSQLQuery
    {
        get => (string)this.Database_AvailableSignalsSQLStatementField.GetValue(Id);
        set => this.Database_AvailableSignalsSQLStatementField.SetValue(Id, value);
    }
    public string SignalDataSQLQueryOVPR
    {
        get => (string)this.Database_HistoricalOVPRField.GetValue(Id);
        set => this.Database_HistoricalOVPRField.SetValue(Id, value);
    }
    public string SignalDataSQLQueryMVPR
    {
        get => (string)this.Database_HistoricalMVPRField.GetValue(Id);
        set => this.Database_HistoricalMVPRField.SetValue(Id, value);
    }

    public string DateTimeRangeSQLQuery
    {
        get => (string)this.Database_DateTimeRangeSQLStatementField.GetValue(Id);
        set => this.Database_DateTimeRangeSQLStatementField.SetValue(Id, value);
    }


    public TransformType PumpTransformType
    {
        get => (TransformType)this.TransformerElement.PumpTransformType;
        set => this.TransformerElement.PumpTransformType = (int)value;
    }
    public RelationalOperator PumpOnOperator
    {
        get => (RelationalOperator)this.TransformerElement.PumpStatusOnOperator;
        set => this.TransformerElement.PumpStatusOnOperator = (int)value;
    }
    public double PumpOnRawSignalValue
    {
        get => PumpTransformType == TransformType.ThresholdTransform
        ? this.TransformerElement.PumpStatusOnThresholdValue
            : Convert.ToDouble(this.TransformerElement.PumpStatusOnValue);

        set
        {
            if (PumpTransformType == TransformType.ThresholdTransform)
                this.TransformerElement.PumpStatusOnThresholdValue = value;
            else
                this.TransformerElement.PumpStatusOnValue = value.ToString();
        }
    }
    public TransformType ValveTransformType
    {
        get => (TransformType)this.TransformerElement.ValveTransformType;
        set => this.TransformerElement.ValveTransformType = (int)value;
    }
    public RelationalOperator ValveActiveOperator
    {
        get => (RelationalOperator)this.TransformerElement.ValveStatusActiveOperator;
        set => this.TransformerElement.ValveStatusActiveOperator = (int)value;
    }
    public double ValveActiveValue
    {
        get => ValveTransformType == TransformType.ThresholdTransform
            ? this.TransformerElement.ValveStatusActiveThresholdValue
            : Convert.ToDouble(this.TransformerElement.ValveStatusActiveValue);

        set
        {
            if (ValveTransformType == TransformType.ThresholdTransform)
                this.TransformerElement.ValveStatusActiveThresholdValue = value;
            else
                this.TransformerElement.ValveStatusActiveValue = value.ToString();
        }
    }
    public RelationalOperator ValveClosedOperator
    {
        get => (RelationalOperator)this.TransformerElement.ValveStatusClosedOperator;
        set => this.TransformerElement.ValveStatusClosedOperator = (int)value;
    }
    public double ValveClosedValue
    {
        get => ValveTransformType == TransformType.ThresholdTransform
            ? this.TransformerElement.ValveStatusClosedThresholdValue
            : Convert.ToDouble(this.TransformerElement.ValveStatusClosedValue);

        set
        {
            if (ValveTransformType == TransformType.ThresholdTransform)
                this.TransformerElement.ValveStatusClosedThresholdValue = value;
            else
                this.TransformerElement.ValveStatusClosedValue = value.ToString();
        }
    }



    public ISCADASignals Signals
    {
        get => WaterModel.Components.SCADASignals(Id);
    }


    public int Id => SupportElement.Id;
    public string Notes { get { return SupportElement.Notes; } set { SupportElement.Notes = value; } }
    public ModelElementType ModelElementType => (ModelElementType)SupportElement.ModelingElementType;
    public string Label { get { return SupportElement.Label; } set { SupportElement.Label = value; } }

    #endregion

    #region Private Properties
    IWaterModel WaterModel { get; }
    IModelingElement SupportElement { get; set; }
    ISupportElementManager SupportElementManager { get; set; }
    DatabaseDataSource DataSource => /*_dataSource ??=*/ GetNewDataSource();
    DataSourceTransformerElement TransformerElement => _transformElement ??= GetTransformElement();

    Haestad.Domain.IFieldManager FieldManager { get; set; }


    IEditField DatabaseType_DBSourceTypeField { get; set; }
    IEditField DatabaseType_DataSourceField { get; set; }
    IEditField DatabaseType_UseConnectionStringField { get; set; }
    IEditField DatabaseType_ConnectionStringField { get; set; }
    IEditField DatabaseType_DateTimeSQLPrefixField { get; set; }
    IEditField DatabaseType_DateTimeSQLSufixField { get; set; }

    IEditField ScadaDataSourceField { get; set; }
    IEditField ScadaDataSource_TableNameField { get; set; }
    IEditField SourceFormatField { get; set; }
    IEditField ScadaDataSource_SignalNameField { get; set; }
    IEditField ScadaDataSource_ValueField { get; set; }
    IEditField ScadaDataSource_TimeStampField { get; set; }
    IEditField ScadaDataSource_QuestionableField { get; set; }
    IEditField ScadaDataSource_SupportsHistoricalDataField { get; set; }
    IEditField ScadaDataSource_SupportsRealTimeDataField { get; set; }
    IEditField ScadaDataSource_TimeToleranceBackwardField { get; set; }
    IEditField ScadaDataSource_TimeToleranceForwardsField { get; set; }
    IEditField ScadaDataSource_ZeroIsGoodQualityField { get; set; }


    IEditField Database_CustomizeSQLStatementsField { get; set; }
    IEditField Database_AvailableSignalsSQLStatementField { get; set; }
    IEditField Database_HistoricalOVPRField { get; set; }
    IEditField Database_HistoricalMVPRField { get; set; }
    IEditField Database_DateTimeRangeSQLStatementField { get; set; }

    #endregion

    #region Private Field
    DataSourceTransformerElement _transformElement;
    #endregion

}

#endregion