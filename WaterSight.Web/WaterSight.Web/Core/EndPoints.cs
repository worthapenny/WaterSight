using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI.WebControls;


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
        Options = options;
        Update(apiVersion);
    }
    #endregion

    #region Public Methods
    public void Update(string apiVersion = "v1")
    {
        if (Options.Env == Env.Dev) EnvironmentPrefix = DevPrefix;
        if (Options.Env == Env.Qa) EnvironmentPrefix = QaPrefix;
        if (Options.Env == Env.Prod) EnvironmentPrefix = ProdPrefix;

        Query = new Query(Options);
        ApiVersion = apiVersion;

        Root = $"{Schema}://{EnvironmentPrefix}{SubDomain}{Options.SubDomainSuffix}.{Domain}";
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
    public int DTID => Options.DigitalTwinId;
    public Query Query { get; private set; }
    public string EnvironmentPrefix { get; private set; } = "";
    public string ApiVersion { get; private set; }
    public string Root { get; private set; }
    public string RootApiVersion { get; private set; }
    public Options Options { get; private set; }
    public bool HasPAT => !string.IsNullOrEmpty(Options.PAT);

    // IMS
    public string RootIMS { get; private set; }
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
    // Smart Meters
    public string SmartMeters => $"{RootApiVersion}/SmartMeters";
    public string SmartMetersSmartMeters => $"{SmartMeters}/SmartMeters";
    public string SmartMetersSmartMetersQDT => $"{SmartMetersSmartMeters}?{Query.DTID}";
    public string SmartMetersSmartMetersForId(int id) => $"{SmartMetersSmartMeters}/{id}?{Query.DTID}";
    public string SmartMetersSmartMetersFromFile => $"{SmartMeters}/SmartMetersFromFile";
    public string SmartMetersSmartMetersFromFileQDT => $"{SmartMetersSmartMetersFromFile}?{Query.DTID}&actionId=http://watersight.bentley.com/administration/smart-meters#smartMetersImport";



    //
    // Mailman
    public string Mailman => $"{RootApiVersion}/Mailman";
    public string MailmanSubsGroup => $"{Mailman}/SubscriberGroup";
    public string MailmanSubsGroupQDT => $"{MailmanSubsGroup}?{Query.DTID}";
    public string MailmanSubsGroupQDTGroupId(int? id) => $"{MailmanSubsGroupQDT}&{Query.GroupId(id)}";
    public string MailmanSubsGroupQDtQSubsGroupId(int id) => $"{MailmanSubsGroupQDT}&{Query.SubscriberGroupId(id)}";
    public string MailmanSyncSubscribers => $"{Mailman}/SyncSubscribers";
    public string MailmanSyncSubscribersQDT => $"{MailmanSyncSubscribers}?{Query.DTID}";

    public string MailmanGroupSubscriber => $"{Mailman}/GroupSubscriber";
    public string MailmanGroupSubscriberDTID => $"{MailmanGroupSubscriber}?{Query.DTID}";
    public string MailmanGroupSubscriberDTIDGroupIdSubscriberId(int groupId, int subscriberId) => $"{MailmanGroupSubscriberDTID}&groupId={groupId}&subscriberId={subscriberId}";

    public string MailmanGroupSubscription => $"{Mailman}/GroupSubscription";
    public string MailmanGroupSubscriptionDTID => $"{MailmanGroupSubscription}?{Query.DTID}";
    public string MailmanGroupSubscriptionDTIDSubsTypeSubsKey(string subscriptionType, string subscriptionKey) => $"{MailmanGroupSubscriptionDTID}&subscriptionType={subscriptionType}&subscriptionKey={subscriptionKey}";


    //
    // RTDA
    public string Rtda => $"{RootApiVersion}/RealTimeDataAcquisition";
    public string RtdaTsValues => $"{Rtda}/TimeSeriesValues";
    public string RtdaTsValuesFor(int sensorId) => HasPAT ? $"{RtdaTsValues}/{sensorId}?{Query.UseToken}&{Query.DTID}" : $"{RtdaTsValues}/{sensorId}?{Query.DTID}";
    public string RtdaTsValuesPurge => $"{RtdaTsValues}/LRO?{Query.DTID}&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fsignals%3Fsensorid%3D22571%23purge&beforeDate=&afterDate=";
    public string RtdaTsValuesPurgeFor(int sensorId) => $"{RtdaTsValues}/{sensorId}/LRO?{Query.DTID}&actionId=http%3A%2F%2Fwatersight.bentley.com%2Fadministration%2Fsignals%3Fsensorid%3D22571%23purge&beforeDate=&afterDate=";
    public string RtdaTsValuesWithin(int sensorId, DateTimeOffset startAt, DateTimeOffset endAt, IntegrationType integrationType) => $"{RtdaTsValues}/{sensorId}?{Query.DTID}&{Query.GetStartDateTime(startAt)}&{Query.GetEndDateTime(endAt)}&integrationType={(int)integrationType}";
    public string RtdaTsValuesLatest => $"{RtdaTsValues}/Latest";
    public string RtdaTsValuesMonthlyVolumes => $"{RtdaTsValues}/MonthlyVolumes";
    public string RtdaTsValuesDailyMins => $"{RtdaTsValues}/DailyMinimums";
    public string RtdaSignals => HasPAT ? $"{Rtda}/Signals?{Query.UseToken}" :  $"{Rtda}/Signals";
    public string RtdaSignalsLROQDT => $"{RtdaSignals}/LRO?{Query.DTID}";
    public string RtdaSignalsForQDT(int id, bool LRO = false) => $"{RtdaSignals}/{id}{(LRO ? "/LRO" : "")}?{Query.DTID}";
    public string RtdaSignalsQDT => HasPAT ? $"{RtdaSignals}&{Query.DTID}" : $"{RtdaSignals}?{Query.DTID}";
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
    public string HydStructuresZonesConfidenceRangeForQDT(int id) => $"{HydStructuresZones}/{id}/ConfidenceRange?{Query.DTID}";

    public string HydStructuresPump => $"{HydStructures}/Pump";
    public string HydStructuresPumpQDT => $"{HydStructuresPump}?{Query.DTID}";
    public string HydStructuresPumpForQDT(int id) => $"{HydStructuresPump}?{Query.PumpId(id)}&{Query.DTID}";
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
    public string NumModelingModelDomainUploadOpModelDomainViaBlobStorage => $"{NumModelingModelDomain}/UploadOperationalModelDomainViaBlobStorage";

    public string NumModelingModelElements => $"{NumModeling}/IModelElements";
    public string NumModelingModelElementsAllElements => $"{NumModelingModelElements}/AllElements";
    public string NumModelingParam => $"{NumModeling}/Parameter";
    public string NumModelingParamGetByDomainAndElemType => $"{NumModelingParam}/GetByModelDomainAndElementType";
    public string NumModelingParamResultAttribInfo => $"{NumModelingParam}/ResultAttributeInfos";
    public string NumModelingModelTSD => $"{NumModeling}/ModelTimeSeriesValues";

    public string NumModelingModelRuns => $"{NumModeling}/ModelRuns";
    public string NumModelingModelRunsQDT => $"{NumModelingModelRuns}?{Query.DTID}";
    public string NumModelingModelRunsDataFile(int modelRunId) => $"{NumModelingModelRuns}/{modelRunId}/DataFile";
    public string NumModelingModelRunsDataFileQDT(int modelRunId) => $"{NumModelingModelRunsDataFile(modelRunId)}?{Query.DTID}";

    //
    // Blob Storage
    public string BlobStorage => $"{DT}/BlobStorage";
    public string BlobStorageSas => $"{BlobStorage}/sas";
    public string BlobStorageSasUpload => $"{BlobStorageSas}/upload";
    public string BlobStorageSasUploadQDT => $"{BlobStorageSasUpload}?{Query.DTID}";


    //
    // Digital Twin System Metrics
    public string DTSysMetrics => $"{RootApiVersion}/DigitalTwinSystemMetrics";
    public string DTSysMetricsQueries => $"{DTSysMetrics}/Queries";
    public string DTSysMetricsQueriesQDT => $"{DTSysMetrics}/Queries?{Query.DTID}";


    //
    // Digital Twins
    public string DT => $"{RootApiVersion}/DigitalTwin";
    public string DTSlashId => $"{DT}/{DTID}";
    public string DTConnected => $"{DT}/DigitalTwinsConnected";
    public string DTConnectedQDT => $"{DTConnected}?{Query.DTID}";
    public string DTConnectedQDtName(string name) => $"{DTConnected}?{Query.DTName(name)}";
    public string DTConnectedQDtNameQDtType(string name, int digitalTwinType) => $"{DTConnectedQDtName(name)}&{Query.DTType(digitalTwinType)}";

    public string DTConnectedUser => $"{DTConnected}/User";
    public string DTQuants => $"{DT}/Quantities";
    public string DTQuantsQDT => $"{DTQuants}?{Query.DTID}";

    public string DTIdDisplayUnits => $"{DTSlashId}/DisplayUnits";
    public string DTIdDisplayUnitsOptionsIsTrue => $"{DTIdDisplayUnits}?options=true";

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
    public string DTServiceExpectationsEnergyFromRenewableSources => $"{DTServiceExpectations}/RenewableEnergy";
    public string DTServiceExpectationsCO2EmissionFactor => $"{DTServiceExpectations}/CO2EmissionFactor";
    public string DTServiceExpectationsMaxPressureSet(double pressure, string unit) => $"{DTServiceExpectationsMaxPressure}?{Query.Value(pressure)}&{Query.ValueUnit(unit)}";
    public string DTServiceExpectationsMinPressureSet(double pressure, string unit) => $"{DTServiceExpectationsMinPressure}?{Query.Value(pressure)}&{Query.ValueUnit(unit)}";
    public string DTServiceExpectationsTargetPumpEfficiencySet(double efficiency, string unit) => $"{DTServiceExpectationsMinPumpEfficiency}?{Query.Value(efficiency)}&{Query.ValueUnit(unit)}";
    public string DTServiceExpectationsEnergyFromRenewableSourcesSet(double renewableEnergy, string unit) => $"{DTServiceExpectationsEnergyFromRenewableSources}?{Query.Value(renewableEnergy)}&{Query.ValueUnit(unit)}";
    public string DTServiceExpectationsCO2EmissionFactorSet(double emissionFactor, string unit) => $"{DTServiceExpectationsCO2EmissionFactor}?{Query.Value(emissionFactor)}&{Query.ValueUnit(unit)}";

    //
    // Costs
    public string DTCosts => $"{DT}/{DTID}/Costs";
    public string DTCostsAvgVolumetricProduction => $"{DTCosts}/AvgVolumetricProductionCost";
    public string DTCostsAvgVolumetricTariff => $"{DTCosts}/AvgVolumetricTariff";
    public string DTCostsAvgEnergyCost => $"{DTCosts}/AvgEnergyCost";
    public string DTCostsAvgVolumetricProductionSet(double cost, string unit) => $"{DTCostsAvgVolumetricProduction}?{Query.Value(cost)}&{Query.ValueUnit(unit)}";
    public string DTCostsAvgVolumetricTariffSet(double cost, string unit) => $"{DTCostsAvgVolumetricTariff}?{Query.Value(cost)}&{Query.ValueUnit(unit)}";
    public string DTCostsAvgEnergyCostSet(double cost, string unit) => $"{DTCostsAvgEnergyCost}?{Query.Value(cost)}&{Query.ValueUnit(unit)}";

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


    // 
    // Watchdog
    public string Watchdog => $"{RootApiVersion}/Watchdog";
    public string WatchdogHealthChecks => $"{Watchdog}/HealthChecks";
    public string WatchdogHealthChecksWSServiceFab => $"{WatchdogHealthChecks}/WaterSightServiceFabric";
    public string WatchdogStatus => $"{Watchdog}/status";
    public string WatchdogStatusOnSiteCoord => $"{WatchdogStatus}/onSiteCoord";
    public string WatchdogStatusOnSiteCoordQDT => $"{WatchdogStatus}/onSiteCoord?{Query.DTID}";
    //public string WatchdogStatusOnSiteCoordQDTPeriod(string period = "P7D") => $"{WatchdogStatusOnSiteCoordQDT}&timePeriod={period}";

    public string WatchdogStatusScadaPusher=> $"{WatchdogStatus}/scadaPusher";
    public string WatchdogStatusScadaPusherSummary=> $"{WatchdogStatusScadaPusher}/summary";
    public string WatchdogStatusScadaPusherSummaryQDT => $"{WatchdogStatusScadaPusherSummary}?{Query.DTID}";
    public string WatchdogStatusScadaPusherSummaryQDTPeriod(string period="PT30M") => $"{WatchdogStatusScadaPusherSummary}?{Query.DTID}&timePeriod={period}";
    public string WatchdogStatusScadaPusherLatest => $"{WatchdogStatusScadaPusher}/latest";
    public string WatchdogStatusScadaPusherLatestQDT => $"{WatchdogStatusScadaPusher}/latest?{Query.DTID}";
    public string WatchdogStatusScadaPusherLatestPeriod(string period = "P7D") => $"{WatchdogStatusScadaPusherLatestQDT}&timePeriod={period}";

    public string WatchdogStatusOverview => $"{WatchdogStatus}/overview";
    public string WatchdogStatusOverviewOnSiteCoord => $"{WatchdogStatusOverview}/onSiteCoord";
    public string WatchdogStatusOverviewScadaPusher => $"{WatchdogStatusOverview}/scadaPusher";
    #endregion
}
