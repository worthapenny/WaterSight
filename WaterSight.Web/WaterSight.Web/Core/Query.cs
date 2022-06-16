
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
    public string DTID => $"digitalTwinId={Options.DigitalTwinId}";
    public string DTName(string name) => $"digitalTwinName={Uri.EscapeDataString(name)}";
    public string DTType(int digitalTwinType) => $"digitalTwinType={digitalTwinType}";

    public string GetStartDateTime(DateTimeOffset dt) => $"startDateTime={Util.ISODateTime(dt)}";
    public string GetStartDateTimeModel(DateTimeOffset dt) => $"startDateTime={Util.ISODateTime(dt)}";

    public string EndDateTime => $"endDateTime={Options.EndAt}";
    public string GetEndDateTime(DateTimeOffset dt) => $"endDateTime={Util.ISODateTime(dt)}";
    public string GetEndDateTimeModel(DateTimeOffset dt) => $"endDateTime={Util.ISODateTime(dt)}";
    public string GetStartMonth(DateTime dt) => $"startMonth={dt:yyyy-MM}";
    public string EndMonth => $"startMonth={Options.EndMonth}";

    public string GetEndMonth(DateTime dt) => $"startMonth={dt:yyyy-MM}";

    public string GetIntegrationType(IntegrationType integrationType) => $"integrationType={integrationType}";
    public string VectorType(string dataTypeName) => $"vectorType={Uri.EscapeDataString(dataTypeName)}";
    public string ParameterFlow => $"parameter=Flow";

    public string DTTypeWater => $"digitalTwinType=0";
    public string ModelDomainId(int modelDomainId) => $"modelDomainId={modelDomainId}";
    public string Username(string name) => $"username={Uri.EscapeDataString(name)}";
    public string ModelDomainName(string name) => $"modelDomainName={Uri.EscapeDataString(name)}";
    public string EpsgCode(object code) => $"epsgCode={code}";
    public string Frequency(object value) => $"frequency={value}";
    public string Duration(object value) => $"duration={value}";
    public string SpinupHours(object value) => $"spinupHours={value}";
    public string HindcastHours(object value) => $"hindcastHours={value}";
    public string QuantityUnits(string unit) => $"quantityUnits={Uri.EscapeDataString(unit)}";
    public string Value(object value) => $"value={value}";
    //public string ValueUnit(string unit) => $"valueUnit={unit}";

    public string Name(string name) => $"name={Uri.EscapeDataString(name)}";
    public string Epsg(int epsg) => $"epsg={epsg}";
    public string Latitude(double lat) => $"latitude={lat}";
    public string Longitude(double lng) => $"longitude={lng}";

    public string ActionIdSignalsDelete(string actionName) => $"actionId=http://watersight.bentley.com/administration/signals#{Uri.EscapeDataString(actionName)}";

    public string CurveType(int curveType) => $"curveType={curveType}";
    public string TankCurveId(int id) => $"tankCurveId={id}";

    public string EventAlertTriggerId(int id) => $"eventAlertTriggerId={id}";
    public string Timezone(string timezoneName) => $"timeZoneId={Uri.EscapeDataString(timezoneName)}";

    #endregion


    #region Private Properties
    private Options Options { get; set; }
    #endregion
}

