
using System;
using System.Diagnostics;
using WaterSight.Web.DT;
using WaterSight.Web.Support;

namespace WaterSight.Web.Core;

[DebuggerDisplay("{ToString()}")]
public class Query
{
    #region Constructor
    public Query(Options options)
    {
        Options = options;
    }
    #endregion

    #region Public Methods
    public string GetModelDomainName(string name) => $"modelDomainName={name}";
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Query: {DTID}";
    }
    #endregion

    #region Public Properties / Methods
    public string ActionIdSignalsDelete(string actionName) => $"actionId=http://watersight.bentley.com/administration/signals#{Uri.EscapeDataString(actionName)}";
    public string CurveType(int curveType) => $"curveType={curveType}";
    public string DTID => $"digitalTwinId={Options.DigitalTwinId}";
    public string DTName(string name) => $"digitalTwinName={Uri.EscapeDataString(name)}";
    public string DTType(int digitalTwinType) => $"digitalTwinType={digitalTwinType}";
    public string DTTypeWater => $"digitalTwinType=0";
    public string DomainElementTypeId(int id) => $"domainElementTypeId={id}";
    public string Duration(object value) => $"duration={value}";
    public string ElementId(int id) => $"elementId={id}";
    public string EmergencyEventId(int? id) => $"emergencyEventId={(id == null ? "undefined" : id)}";
    public string EndDateTime(DateTimeOffset at) => $"endDateTime={at.UtcDateTime:O}";
    public string EndMonth => $"startMonth={Options.EndMonth}";
    public string Epsg(int epsg) => $"epsg={epsg}";
    public string EpsgCode(object code) => $"epsgCode={code}";
    public string EventAlertTriggerId(int id) => $"eventAlertTriggerId={id}";
    public string Frequency(object value) => $"frequency={value}";
    public string GetEndDateTime(DateTimeOffset dt) => $"endDateTime={dt.UtcDateTime:O}";
    public string GetEndDateTimeModel(DateTimeOffset dt) => $"endDateTime={dt.UtcDateTime:O}";
    public string GetEndMonth(DateTime dt) => $"startMonth={dt:yyyy-MM}";
    public string GetIntegrationType(IntegrationType integrationType) => $"integrationType={integrationType}";
    public string GetStartDateTime(DateTimeOffset dt) => $"startDateTime={dt.UtcDateTime:O}";
    public string GetStartDateTimeModel(DateTimeOffset dt) => $"startDateTime={dt.UtcDateTime:O}";
    public string GetStartMonth(DateTime dt) => $"startMonth={dt:yyyy-MM}";
    public string GroupId(int id) => $"groupId={id}";
    public string HindcastHours(object value) => $"hindcastHours={value}";
    public string Latitude(double lat) => $"latitude={lat}";
    public string Longitude(double lng) => $"longitude={lng}";
    public string ModelDomainId(int modelDomainId) => $"modelDomainId={modelDomainId}";
    public string ModelDomainName(string name) => $"modelDomainName={Uri.EscapeDataString(name)}";
    public string ModelElementId(int id) => $"modelElementId={id}";
    public string Name(string name) => $"name={Uri.EscapeDataString(name)}";
    public string ParameterFlow => $"parameter=Flow";
    public string ParameterName(string name) => $"parameterName={name}";
    public string PumpId(int id) => $"pumpId={id}";
    public string QuantityUnits(string unit) => $"quantityUnits={Uri.EscapeDataString(unit)}";
    public string SpinupHours(object value) => $"spinupHours={value}";
    public string StartDateTime(DateTimeOffset at) => $"startDateTime={at.UtcDateTime:O}";
    public string SubscriberGroupId(int id) => $"subscriberGroupId={id}";
    public string SubscriberId(int id) => $"subscriberId={id}";
    public string TankCurveId(int id) => $"tankCurveId={id}";
    public string TimeStep(DateTimeOffset at) => $"timeStep={at.UtcDateTime:O}";
    public string Timezone(string timezoneName) => $"timeZoneId={Uri.EscapeDataString(timezoneName)}";
    public string Username(string name) => $"username={Uri.EscapeDataString(name)}";
    public string Value(object value) => $"value={value}";
    public string ValueUnit(string unit) => $"valueUnit={unit}";
    public string VectorType(string dataTypeName) => $"vectorType={Uri.EscapeDataString(dataTypeName)}";
    
    #endregion


    #region Private Properties
    private Options Options { get; set; }
    #endregion
}

