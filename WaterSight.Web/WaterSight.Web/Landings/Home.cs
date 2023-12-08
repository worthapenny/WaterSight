using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Landings;

public class Home : WSItem
{
    #region Constructor
    public Home(WS ws) : base(ws)
    {
    }
    #endregion

    #region Public Methods

    #region Sensor CRUD operation
    // Create
    // Nothing to create here


    // Read
    public async Task<List<StatQuery>?> GetStatQueries()
    {
        var url = EndPoints.DTSysMetricsQueriesQDT;
        List<StatQuery> queries = await WS.GetAsync<List<StatQuery>>(url, null, "StatQuery");
        return queries;
    }


    // Update
    // Nothing to update here

    // Delete
    // Nothing to delete here
    #endregion

    public List<StatQueryValue> GetSensorCount(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorCount); }
    public List<StatQueryValue> GetSensorCountNoData(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorCountNoData); }
    public List<StatQueryValue> GetSensorCountPartialData(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorCountPartialData); }
    public List<StatQueryValue> GetSensorCountOK(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorCountOK); }
    public List<StatQueryValue> GetPumpCount(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdPumpCount); }
    public List<StatQueryValue> GetInefficientPumps(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdInefficientPumps); }
    public List<StatQueryValue> GetAveragePumpEfficiency(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdAveragePumpEfficiency); }
    public List<StatQueryValue> GetEnergyConsumed(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyConsumed); }
    public List<StatQueryValue> GetEnergyInefficiency(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyInefficiency); }
    public List<StatQueryValue> GetEnergyInefficiencyCost(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyInefficiencyCost); }
    public List<StatQueryValue> GetEnergyTarget(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyTarget); }
    public List<StatQueryValue> GetEnergyDifference(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyDifference); }
    public List<StatQueryValue> GetEnergyDifferenceCost(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdEnergyDifferenceCost); }
    public List<StatQueryValue> GetMinPumpEfficiency(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdMinPumpEfficiency); }
    public List<StatQueryValue> GetTankCount(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdTankCount); }
    public List<StatQueryValue> GetTankHighLevelAlarms(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdTankHighLevelAlarms); }
    public List<StatQueryValue> GetTankLowLevelAlarms(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdTankLowLevelAlarms); }
    public List<StatQueryValue> GetTankSystemStorage(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdTankSystemStorage); }
    public List<StatQueryValue> GetAvailableTankSystemStorage(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdAvailableTankSystemStorage); }
    public List<StatQueryValue> GetSucceededSimulationRuns(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSucceededSimulationRuns); }
    public List<StatQueryValue> GetTotalSimulationRuns(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdTotalSimulationRuns); }
    public List<StatQueryValue> GetActiveEventCount(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdActiveEventCount); }
    public List<StatQueryValue> GetNewEventCount(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdNewEventCount); }
    public List<StatQueryValue> GetWaterBalanceRL(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceRL); }
    public List<StatQueryValue> GetWaterBalanceRW(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceRW); }
    public List<StatQueryValue> GetWaterBalanceAL(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceAL); }
    public List<StatQueryValue> GetWaterBalanceUAC(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceUAC); }
    public List<StatQueryValue> GetWaterBalanceWL(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceWL); }
    public List<StatQueryValue> GetWaterBalanceWLCost(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWaterBalanceWLCost); }
    public List<StatQueryValue> GetZoneMaxRL(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdZoneMaxRL); }
    public List<StatQueryValue> GetZoneMaxAL(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdZoneMaxAL); }
    public List<StatQueryValue> GetZoneMaxMNF(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdZoneMaxMNF); }
    public List<StatQueryValue> GetTestQuery(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdtestQuery); }
    public List<StatQueryValue> GetSensorsMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorsMetricsTile); }
    public List<StatQueryValue> GetHydraulicStructuresMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdHydraulicStructuresMetricsTile); }
    public List<StatQueryValue> GetGISMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdGISMetricsTile); }
    public List<StatQueryValue> GetZonesMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdZonesMetricsTile); }
    public List<StatQueryValue> GetCustomersMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdCustomersMetricsTile); }
    public List<StatQueryValue> GetModelDomainMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdModelDomainMetricsTile); }
    public List<StatQueryValue> GetPowerBIMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdPowerBIMetricsTile); }
    public List<StatQueryValue> GetAlertingMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdAlertingMetricsTile); }
    public List<StatQueryValue> GetPersonalAccessTokenMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdPersonalAccessTokenMetricsTile); }
    public List<StatQueryValue> GetWorkOrderMetricsTile(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdWorkOrderMetricsTile); }
    public List<StatQueryValue> GetSensorStatus(List<StatQuery> queries) { return GetQueryValues<object>(queries, StatQuery.QueryIdSensorStatus); }
    public List<StatQueryValue> GetSmartMetersMetricsTile(List<StatQuery> queries) { return GetQueryValues<int>(queries, StatQuery.QueryIdSmartMetersMetricsTile); }
    public List<StatQueryValue> GetTotalCarbonFootprint(List<StatQuery> queries) { return GetQueryValues<double>(queries, StatQuery.QueryIdTotalCarbonFootprint); }
    public List<StatQueryValue> GetSensorCountOffline(List<StatQuery> queries) { return GetQueryValues<int>(queries, StatQuery.QueryIdSensorCountOffline); }

    #endregion

    #region Private Methods
    public List<StatQueryValue> GetQueryValues<T>(List<StatQuery> queries, string queryId)
    {
        var queriesCheck = queries.Where(q => q.QueryId == queryId);
        return queriesCheck.Any() ? queriesCheck.First().Values : default;
    }
    #endregion


}

#region Model Class
public class StatQuery
{
    #region Constants
    public const string QueryIdSensorCount = "SensorCount";
    public const string QueryIdSensorCountNoData = "SensorCountNoData";
    public const string QueryIdSensorCountPartialData = "SensorCountPartialData";
    public const string QueryIdSensorCountOK = "SensorCountOK";
    public const string QueryIdPumpCount = "PumpCount";
    public const string QueryIdInefficientPumps = "InefficientPumps";
    public const string QueryIdAveragePumpEfficiency = "AveragePumpEfficiency";
    public const string QueryIdEnergyConsumed = "EnergyConsumed";
    public const string QueryIdEnergyInefficiency = "EnergyInefficiency";
    public const string QueryIdEnergyInefficiencyCost = "EnergyInefficiencyCost";
    public const string QueryIdEnergyTarget = "EnergyTarget";
    public const string QueryIdEnergyDifference = "EnergyDifference";
    public const string QueryIdEnergyDifferenceCost = "EnergyDifferenceCost";
    public const string QueryIdMinPumpEfficiency = "MinPumpEfficiency";
    public const string QueryIdTankCount = "TankCount";
    public const string QueryIdTankHighLevelAlarms = "TankHighLevelAlarms";
    public const string QueryIdTankLowLevelAlarms = "TankLowLevelAlarms";
    public const string QueryIdTankSystemStorage = "TankSystemStorage";
    public const string QueryIdAvailableTankSystemStorage = "AvailableTankSystemStorage";
    public const string QueryIdSucceededSimulationRuns = "SucceededSimulationRuns";
    public const string QueryIdTotalSimulationRuns = "TotalSimulationRuns";
    public const string QueryIdActiveEventCount = "ActiveEventCount";
    public const string QueryIdNewEventCount = "NewEventCount";
    public const string QueryIdWaterBalanceRL = "WaterBalanceRL";
    public const string QueryIdWaterBalanceRW = "WaterBalanceRW";
    public const string QueryIdWaterBalanceAL = "WaterBalanceAL";
    public const string QueryIdWaterBalanceUAC = "WaterBalanceUAC";
    public const string QueryIdWaterBalanceWL = "WaterBalanceWL";
    public const string QueryIdWaterBalanceWLCost = "WaterBalanceWLCost";
    public const string QueryIdZoneMaxRL = "ZoneMaxRL";
    public const string QueryIdZoneMaxAL = "ZoneMaxAL";
    public const string QueryIdZoneMaxMNF = "ZoneMaxMNF";
    public const string QueryIdtestQuery = "testQuery";
    public const string QueryIdSensorsMetricsTile = "SensorsMetricsTile";
    public const string QueryIdHydraulicStructuresMetricsTile = "HydraulicStructuresMetricsTile";
    public const string QueryIdGISMetricsTile = "GISMetricsTile";
    public const string QueryIdZonesMetricsTile = "ZonesMetricsTile";
    public const string QueryIdCustomersMetricsTile = "CustomtersMetricsTile";
    public const string QueryIdModelDomainMetricsTile = "ModelDomainMetricsTile";
    public const string QueryIdPowerBIMetricsTile = "PowerBIMetricsTile";
    public const string QueryIdAlertingMetricsTile = "AlertingMetricsTile";
    public const string QueryIdPersonalAccessTokenMetricsTile = "PersonalAccessTokenMetricsTile";
    public const string QueryIdWorkOrderMetricsTile = "WorkOrderMetricsTile";
    public const string QueryIdSensorStatus = "SensorStatus";
    public const string QueryIdSmartMetersMetricsTile = "SmartMetersMetricsTile";
    public const string QueryIdTotalCarbonFootprint = "TotalCarbonFootprint";
    public const string QueryIdSensorCountOffline = "SensorCountOffline";

    #endregion

    #region Public Properties
    public string QueryId { get; set; }
    public string Description { get; set; }
    public string Period { get; set; }
    public string Units { get; set; }
    public string LastStatus { get; set; }
    public int NumValues { get; set; }
    public int RefreshInterval { get; set; }
    public int StaleTime { get; set; }
    public List<StatQueryValue> Values { get; set; }
    #endregion
}

public class StatQueryValue
{
    public DateTimeOffset TimeStamp { get; set; }
    public object Value { get; set; }
    public string ValueDescription { get; set; }
    public int Code { get; set; }
    public int? ComputationPoints { get; set; }
}
#endregion
