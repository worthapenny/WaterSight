using System;
using System.Diagnostics;
using System.Web;


namespace WaterSight.Web.Core;

[DebuggerDisplay("{ToString()}")]
public class EndPoints
{
    public const string Schema = "https";
    public const string SubDomain = "connect-watersight";
    public const string ImsSubDomain = "ims";
    public const string Domain = "bentley.com";
    public const string Api = "api";
    public const string ProdPrefix = "";
    public const string QaPrefix = "qa-";
    public const string DevPrefix = "dev-";

    #region Constructor
    public EndPoints(Options options, string apiVersion = "v1")
    {
        if (options.Env == Env.Prod) EnvironmentPrefix = ProdPrefix;
        if (options.Env == Env.Qa) EnvironmentPrefix = QaPrefix;
        if (options.Env == Env.Dev) EnvironmentPrefix = DevPrefix;

        DTID = options.DigitalTwinId;
        Query = new Query(options);
        ApiVersion = apiVersion;

        Root = $"{Schema}://{EnvironmentPrefix}{SubDomain}.{Domain}";
        RootApiVersion = $"{Root}/{Api}/{apiVersion}";

        RootIMS = $"{Schema}://{EnvironmentPrefix}{ImsSubDomain}.{Domain}";
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{RootApiVersion}";
    }
    #endregion

    #region Public ReadOnly Properties
    public int DTID { get; }
    public Query Query { get; }
    public string EnvironmentPrefix { get; } = "";
    public string ApiVersion { get; }
    public string Root { get; }
    public string RootApiVersion { get; }


    // IMS
    public string RootIMS { get; }
    public string ImsConnect => $"{RootIMS}/connect";
    public string ImsUserInfo => $"{ImsConnect}/userinfo";


    //
    // User
    public string UserInfo => $"{RootApiVersion}";

    //
    // Alerting
    public string Alerting => $"{RootApiVersion}/Alerting";
    public string AlertingAlerts => $"{Alerting}/Alerts";
    public string AlertingConfigs => $"{Alerting}/Configurations";
    public string AlertingConfigsQDT => $"{AlertingConfigs}?{Query.DTID}";
    public string AlertingConfigsForId(int id) => $"{AlertingConfigs}/{id}?{Query.DTID}";

    //
    // RTDA
    public string Rtda => $"{RootApiVersion}/RealTimeDataAcquisition";
    public string RtdaTsValues => $"{Rtda}/TimeSeriesValues";
    public string RtdaTsValuesFor(int sensorId) => $"{RtdaTsValues}/{sensorId}?{Query.DTID}";
    public string RtdaTsValuesPurge => $"{RtdaTsValues}/LRO?{Query.DTID}&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fsignals%3Fsensorid%3D22571%23purge&beforeDate=&afterDate=";
    public string RtdaTsValuesPurgeFor(int sensorId) => $"{RtdaTsValues}/{sensorId}/LRO?{Query.DTID}&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fsignals%3Fsensorid%3D22571%23purge&beforeDate=&afterDate=";
    public string RtdaTsValuesWithin(int sensorId, DateTimeOffset startAt, DateTimeOffset endAt, IntegrationType integrationType) => $"{RtdaTsValues}/{sensorId}?{Query.DTID}&{Query.GetStartDateTime(startAt)}&{Query.GetEndDateTime(endAt)}&integrationType={(int)integrationType}";
    public string RtdaTsValuesLatest => $"{RtdaTsValues}/Latest";
    public string RtdaTsValuesMonthlyVolumes => $"{RtdaTsValues}/MonthlyVolumes";
    public string RtdaTsValuesDailyMins => $"{RtdaTsValues}/DailyMinimums";
    public string RtdaSignals => $"{Rtda}/Signals";
    public string RtdaSignalsLROQDT => $"{RtdaSignals}/LRO?{Query.DTID}";
    public string RtdaSignalsForQDT(int id, bool LRO = false) => $"{RtdaSignals}/{id}{(LRO ? "/LRO" : "")}?{Query.DTID}";
    public string RtdaSignalsConfigQDT => $"{RtdaSignals}?{Query.DTID}";
    public string RtdaSignalsFile => $"{RtdaSignals}/File";
    public string RtdaSignalsFileQDT => $"{RtdaSignalsFile}?{Query.DTID}";
    public string RtdaSettingsTile => $"{Rtda}/SettingsTile";


    //
    // Patterns
    public string RtdaPatternWeeks => $"{Rtda}/PatternWeeks";
    public string RtdaPatternWeeksQDT => $"{RtdaPatternWeeks}?{Query.DTID}";
    public string RtdaPatternWeeksForQDT(int id) => $"{RtdaPatternWeeks}/{id}?{Query.DTID}";

    //
    // Geographic Feature
    public string GeoFeatures => $"{RootApiVersion}/GeographicFeatures";
    public string GeoFeaturesShpProps => $"{GeoFeatures}/GetShpFilePropertiesList";
    public string GeoFeaturesShpPropsQDT => $"{GeoFeaturesShpProps}?{Query.DTID}";

    public string GeoFeaturesVectorData => $"{GeoFeatures}/VectorData";
    public string GeoFeaturesVectorDataQDT => $"{GeoFeaturesVectorData}?{Query.DTID}";
    public string GeoFeaturesVectorDataQDTVectorType(string name) => $"{GeoFeaturesVectorDataQDT}&{Query.VectorType(name)}";
    public string GeoFeaturesVectorDataQDTVectorTypeLRO(string name) => $"{GeoFeaturesVectorDataQDT}&{Query.VectorType(name)}&actionId=http://watersight.bentley.com/administration/gis?gisdatatype={name}";
    public string GeoFeaturesVectorDataTypes => $"{GeoFeatures}/VectorDataTypes";
    public string GeoFeaturesVectorDataTypesQDTVectorType(string dataTypeName) => $"{GeoFeaturesVectorDataTypes}?{Query.DTID}&{Query.VectorType(dataTypeName)}";


    //
    // Hydraulic Structures
    public string HydStructures => $"{RootApiVersion}/HydraulicStructures";
    public string HydStructuresMNF => $"{HydStructures}/MinimumNightlyFlow";
    public string HydStructuresZones => $"{HydStructures}/Zones";
    public string HydStructuresZonesQDT => $"{HydStructuresZones}?{Query.DTID}";
    public string HydStructuresZonesForQDT(int id) => $"{HydStructuresZones}/{id}?{Query.DTID}";
    public string HydStructuresPump => $"{HydStructures}/Pump";
    public string HydStructuresPumpQDT => $"{HydStructuresPump}?{Query.DTID}";
    public string HydStructuresPumpForQDT(int id) => $"{HydStructuresPump}/{Query.PumpId(id)}&{Query.DTID}";
    public string HydStructuresPumps => $"{HydStructures}/Pumps";
    public string HydStructuresPumpsQDT => $"{HydStructuresPumps}?{Query.DTID}";

    public string HydStructuresPumpingStation => $"{HydStructures}/PumpingStation";
    public string HydStructuresPumpingStations => $"{HydStructures}/PumpingStations";
    public string HydStructuresPumpingStationsQDT => $"{HydStructuresPumpingStations}?{Query.DTID}";
    public string HydStructuresTank => $"{HydStructures}/Tank";
    public string HydStructuresTankQDT => $"{HydStructuresTank}?{Query.DTID}";
    public string HydStructuresTanks => $"{HydStructures}/Tanks";
    public string HydStructuresTanksQDT => $"{HydStructuresTanks}?{Query.DTID}";
    public string HydStructuresTankForQDT(int id) => $"{HydStructuresTank}?tankId={id}&{Query.DTID}";
    public string HydStructuresTankCurve => $"{HydStructures}/TankCurve";
    public string HydStructuresTankCurveQDT => $"{HydStructures}/TankCurve?{Query.DTID}";
    public string HydStructuresTankCurveQDTCurveType(int curveType = 0) => $"{HydStructuresTankCurveQDT}&{Query.CurveType(curveType)}";
    public string HydStructuresTankCurveForQDT(int id) => $"{HydStructuresTankCurveQDT}&{Query.TankCurveId(id)}";
    public string HydStructureTankCurves => $"{HydStructures}/TankCurves";
    public string HydStructureTankTurnover => $"{HydStructures}/TankTurnover";
    public string HydStructureMassBalances => $"{HydStructures}/MassBalances";
    public string HydStructureMassBalancesCompute => $"{HydStructureMassBalances}/Compute";
    public string HydStructureMassBalancesComputeQDT => $"{HydStructureMassBalancesCompute}?{Query.DTID}";
    public string HydStructureConsumptionPoints => $"{HydStructures}/ConsumptionPoints";
    public string HydStructureConsumptionPointsQDT => $"{HydStructureConsumptionPoints}?{Query.DTID}";
    public string HydStructureMonthlyBilling => $"{HydStructures}/MonthlyBilling";
    public string HydStructureMonthlyBillingQDT => $"{HydStructureMonthlyBilling}?{Query.DTID}";

    //
    // Numerical Modeling
    public string NumModeling => $"{RootApiVersion}/NumericalModelling";

    public string NumModelingElements => $"{NumModeling}/Elements";
    public string NumModelingElementsResults => $"{NumModelingElements}/Results";
    public string NumModelingScadaElement => $"{NumModeling}/ScadaElement";
    public string NumModelingScadaElementModelElements => $"{NumModelingScadaElement}/ModelElements";
    public string NumModelingScadaElementModelElementsQDT => $"{NumModelingScadaElementModelElements}?{Query.DTID}";
    public string NumModelingScadaElementScadaElements => $"{NumModelingScadaElement}/ScadaElements";
    public string NumModelingScadaElementScadaElementsQDT => $"{NumModelingScadaElementScadaElements}?{Query.DTID}";
    public string NumModelingModelDomain => $"{NumModeling}/ModelDomain";
    public string NumModelingModelDomainDomainsLRO(int modelDomainId) => $"{NumModelingModelDomain}/ModelDomainsLRO?{Query.ModelDomainId(modelDomainId)}&{Query.DTID}";
    public string NumModelingModelDomainDomains => $"{NumModelingModelDomain}/ModelDomains";
    public string NumModelingModelDomainDomainsQDT => $"{NumModelingModelDomainDomains}?{Query.DTID}";
    public string NumModelingModelDomainDomainsQDTQDomainId(int domainId) => $"{NumModelingModelDomainDomains}?{Query.DTID}&{Query.ModelDomainId(domainId)}";
    public string NumModelingModelDomainQDT => $"{NumModelingModelDomain}?{Query.DTID}";

    public string NumModelingModelDomainTimeInstanceLastModelRun => $"{NumModelingModelDomain}/TimeInstantsLatestModelRun";

    public string NumModelingModelDomainUploadOpModelDomain => $"{NumModelingModelDomain}/UploadOperationalModelDomain";
    public string NumModelingModelElements => $"{NumModeling}/IModelElements";
    public string NumModelingModelElementsAllElements => $"{NumModelingModelElements}/AllElements";
    public string NumModelingParam => $"{NumModeling}/Parameter";
    public string NumModelingParamGetByDomainAndElemType => $"{NumModelingParam}/GetByModelDomainAndElementType";
    public string NumModelingParamResultAttribInfo => $"{NumModelingParam}/ResultAttributeInfos";
    public string NumModelingModelTSD => $"{NumModeling}/ModelTimeSeriesValues";



    //
    // Digital Twins
    public string DT => $"{RootApiVersion}/DigitalTwin";
    public string DTConnected => $"{DT}/DigitalTwinsConnected";
    public string DTConnectedQDT => $"{DTConnected}?{Query.DTID}";
    public string DTConnectedQDtName(string name) => $"{DTConnected}?{Query.DTName(name)}";
    public string DTConnectedQDtNameQDtType(string name, int digitalTwinType) => $"{DTConnectedQDtName(name)}&{Query.DTType(digitalTwinType)}";
    
    public string DTConnectedUser => $"{DTConnected}/User";
    public string DTQuants => $"{DT}/Quantities";
    public string DTQuantsQDT => $"{DTQuants}?{Query.DTID}";

    public string DTGuid => $"{DT}/GUID";
    public string DTGoals(int dtId) => $"{DT}/{dtId}/Goals";

    public string DTAvatar => $"{DT}/Avatar";
    public string DTAvatarQDT => $"{DTAvatar}?{Query.DTID}";

    public string DTDigitalTwins => $"{DT}/DigitalTwins";
    public string DTDigitalTwinsQDT => $"{DTDigitalTwins}?{Query.DTID}";
    
    public string DTMetaDeta => $"{DT}/Metadata";
    public string DTDigitalTwinsGuid => $"{DTDigitalTwins}/GUID";
    
    public string DTPowerBI => $"{DT}/PowerBI";
    public string DTPowerBIUrl => $"{DTPowerBI}/Url";
    public string DTPowerBIUrlQDT => $"{DTPowerBI}/Url?{Query.DTID}";

    //
    // Template
    public string DTTemplate => $"{DT}/Templates";
    public string DTTemplateConfig => $"{DTTemplate}/Configuration";
    public string DTTemplateBilling => $"{DTTemplate}/Billing";
    public string DTTemplateCurrentConfig => $"{DTTemplate}/CurrentConfiguration";

    //
    // Units
    public string DTQuantities => $"{DT}/Quantities";
    public string DTQuantitiesUnitQDTQQuant(string unitName, string unitValue) => $"{DTQuantities}/{unitName}?{Query.DTID}&{Query.QuantityUnits(unitValue)}";

    //
    // Service Expectation
    public string DTServiceExpectations => $"{DT}/{DTID}/ServiceExpectations";
    public string DTServiceExpectationsMaxPressure => $"{DTServiceExpectations}/MaxPressure";
    public string DTServiceExpectationsMinPressure => $"{DTServiceExpectations}/MinPressure";
    public string DTServiceExpectationsMinPumpEfficiency => $"{DTServiceExpectations}/MinPumpEfficiency";
    public string DTServiceExpectationsMaxPressureSet(double pressure) => $"{DTServiceExpectationsMaxPressure}?{Query.Value(pressure)}";
    public string DTServiceExpectationsMinPressureSet(double pressure) => $"{DTServiceExpectationsMinPressure}?{Query.Value(pressure)}";
    public string DTServiceExpectationsTargetPumpEfficiencySet(double efficiency) => $"{DTServiceExpectationsMinPumpEfficiency}?{Query.Value(efficiency)}";

    //
    // Costs
    public string DTCosts => $"{DT}/{DTID}/Costs";
    public string DTCostsAvgVolumetricProduction => $"{DTCosts}/AvgVolumetricProductionCost";
    public string DTCostsAvgVolumetricTariff => $"{DTCosts}/AvgVolumetricTariff";
    public string DTCostsAvgEnergyCost => $"{DTCosts}/AvgEnergyCost";
    public string DTCostsAvgVolumetricProductionSet(double cost) => $"{DTCostsAvgVolumetricProduction}?{Query.Value(cost)}";
    public string DTCostsAvgVolumetricTariffSet(double cost) => $"{DTCostsAvgVolumetricTariff}?{Query.Value(cost)}";
    public string DTCostsAvgEnergyCostSet(double cost) => $"{DTCostsAvgEnergyCost}?{Query.Value(cost)}";

    //
    // Coordinate Systems
    public string DTCoordinateSystem => $"{DT}/ComponentCoordinateSystems";
    public string DTCoordinateSystemQDT => $"{DTCoordinateSystem}?{Query.DTID}";
    public string DTCoordinateSystemQDTSet(string name, int epsg) => $"{DTCoordinateSystemQDT}&{Query.Name(name)}&{Query.Epsg(epsg)}";

    //
    // Timezone(s)
    public string DTTimezones => $"{DT}/{DTID}/Timezones";
    public string DTTimezone => $"{DT}/{DTID}/Timezone";
    public string DTTimezoneSet(string timezone) => $"{DTTimezone}?{Query.Timezone(timezone)}";

    //
    // Coordinates
    public string DTCoordinates => $"{DT}/{DTID}/Coordinates";
    public string DTCoordinatesQDT => $"{DTCoordinates}?{Query.DTID}";
    public string DTCoordinatesQDTSet(double lat, double lng) => $"{DTCoordinatesQDT}&{Query.Latitude(lat)}&{Query.Longitude(lng)}";
    
    #endregion
}
