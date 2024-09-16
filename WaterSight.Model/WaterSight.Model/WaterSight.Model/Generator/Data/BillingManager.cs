using Haestad.Domain;
using Haestad.Support.Units;
using OpenFlows.Water.Domain.ModelingElements.CalculationOptions;
using OpenFlows.Water.Domain.ModelingElements.Components;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Serilog;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using System.Diagnostics;
using Niraula.Extensions.Water.Support;

namespace WaterSight.Model.Generator.Data;

public class BillingManager
{
    #region Constructor
    public BillingManager(IWaterModel waterModel, SCADADataGeneratorOptions options, Randomizer randomizer, OutputManager outputManager, PatternManager patternManager)
    {
        Options = options;
        WaterModel = waterModel;

        Randomizer = randomizer;
        OutputManager = outputManager;
        PatternManager = patternManager;

        DemandUnit = WaterModel.Units.NetworkUnits.Junction.DemandUnit.GetUnit();
        FlowUnit = WaterModel.Units.NetworkUnits.Pipe.FlowUnit.GetUnit();
        VolumeUnit = WaterModel.Units.NetworkUnits.Tank.VolumeUnit.GetUnit();
    }
    #endregion

    #region Public Methods
    public bool RecordBilling()
    {
        Log.Debug("Recording customer bill results.");

        if (OutputManager.CustomerBillResults is null)
        {
            Log.Information("No Customer Bill table defined for export. Billing records will be skipped.");
            return true;
        }

        GetDates();

        GetDemandAdjustments();

        GetDemandElements();

        InitializeTimeIndexDictionary();

        UpdateCustomerBilledAndConsumptionRecords();

        Log.Information("Completed recording customer bill results.");
        return true;
    }

    public bool PopulateBillingTable()
    {
        Log.Debug("Populating billing table.");
        if (OutputManager.CustomerBillResults is null)
        {
            Log.Information("No Customer Bill table defined for export. Billing records will be skipped.");
            return true;
        }

        var success = false;
        try
        {
            OutputManager.CustomerBillResults.BeginLoadData();

            foreach (Tuple<int, DateTime> key in CumulativeMonthlyBills_kgal.Keys)
            {
                IWaterElement element = (IWaterElement)WaterModel.Element(key.Item1);
                double cumulativeVolume = CumulativeMonthlyBills_kgal[key];

                // TODO: This is currently configured for monthly bills. Can expand this to allow for irregular billing cycles.
                DateTime billStartDate = key.Item2;
                DateTime billEndDate = billStartDate.AddMonths(1).AddDays(-1);

                IZone zone = GetElementZone(element);
                string zoneLabel = String.Empty;
                if (zone != null)
                    zoneLabel = zone.Label;

                string billingID = element.Id.ToString();

                if (element is ICustomerMeter)
                {
                    string customerBillingID = ((ICustomerMeter)element).Manager.InputFields.FieldByName(StandardFieldName.CustomerNode_BillingID).GetValue<string>(element.Id);
                    if (!(customerBillingID == string.Empty))
                        billingID = customerBillingID;
                }

                DataRow row = OutputManager.CustomerBillResults.NewRow();

                row[Options.Output.BillingDataOutputOptions.BillingDataOutputTable.CustomerIDColumnName] = billingID;
                row[Options.Output.BillingDataOutputOptions.BillingDataOutputTable.BillMonthColumnName] = billStartDate;
                row[Options.Output.BillingDataOutputOptions.BillingDataOutputTable.BillVolumeColumnName] = cumulativeVolume;
                row[Options.Output.BillingDataOutputOptions.BillingDataOutputTable.BillUnitsColumnName] = "kgal (U.S.)";
                row[Options.Output.BillingDataOutputOptions.BillingDataOutputTable.BillZoneColumnName] = zoneLabel;

                OutputManager.CustomerBillResults.Rows.Add(row);
            }

            success = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...while loading billing data to DataTable";
            Log.Error(message, ex);
            success = false;
        }
        finally
        {
            OutputManager.CustomerBillResults.EndLoadData();
        }

        Log.Information($"Populated billing table.");
        return success;
    }
    #endregion

    #region Private Methods
    //// These methods are declared as Internal rather than Private to make it easier to develop unit tests.
    /// https://stackoverflow.com/questions/9122708/unit-testing-private-methods-in-c-sharp
    private void GetDates()
    {
        Log.Debug("Getting date range.");
        DatesInSimulation = new List<DateTime>();

        DateTime previousDate = WaterModel.ActiveScenario.Options.SimulationStartDate.Date;
        DatesInSimulation.Add(previousDate);
        double duration = WaterModel.ActiveScenario.Options.Duration;
        int numDays = (int)Math.Floor(duration / 24.0);

        for (int i = 1; i < numDays; i++)
        {
            DateTime nextDate = previousDate.AddDays(1);
            previousDate = nextDate;
            DatesInSimulation.Add(nextDate);
        }

        Log.Information("Got date range.");
    }

    private void GetDemandAdjustments()
    {
        Log.Debug("Getting demand adjustments.");
        if (WaterModel.ActiveScenario.Options.DemandAdjustments == DemandAdjustmentsType.Active)
            DemandAdjustments = WaterModel.ActiveScenario.Options.ActiveDemandAdjustments.Get();
        else
            DemandAdjustments = null;

        if (WaterModel.ActiveScenario.Options.UnitDemandAdjustments == UnitDemandAdjustmentType.Active)
            UnitDemandAdjustments = WaterModel.ActiveScenario.Options.ActiveUnitLoadDemandAdjustments.Get();
        else
            UnitDemandAdjustments = null;

        if (WaterModel.ActiveScenario.Options.CalculationType == CalculationType.SCADAConnectAnalysis)
        {
            // TODO <sjac>: Note that currently the OpenFlows API does not expose the SCADAConnect Demand Adjustments and Unit Demand Adjustments.
            Log.Warning("NOTE: The active scenario is a SCADAConnect Scenario. If SCADAConnect Demand Adjustments or Unit Demand Adjustments are active, they will not be reflected in the billed consumption.");
        }

        Log.Information("Got demand adjustments.");
    }

    private void GetDemandElements()
    {
        DemandElements.Clear(); // Note: rather than instantiating once at the beginning, we are refreshing the list once per simulation so that we don't break if a future task turns an element inactive midway through a set of simulations.
        Log.Debug("Getting demand elements.");
        foreach (ICustomerMeter element in WaterModel.Network.CustomerMeters.Elements( ElementStateType.Active))
            DemandElements.Add(element);

        foreach (IJunction element in WaterModel.Network.Junctions.Elements(ElementStateType.Active))
        {
            if (element.Input.DemandCollection.Count == 0 && element.Input.UnitDemandLoadCollection.Count == 0)
                continue;

            // If the only demands are omitted demands (such as leaks/fire flow), then skip.
            int billedDemandCount = element.Input.UnitDemandLoadCollection.Count;
            IDemands demandCollection = element.Input.DemandCollection.Get();
            foreach (IDemand demand in demandCollection)
            {
                if (demand.DemandPattern is null)
                    billedDemandCount += 1;
                else if (!demand.DemandPattern.Notes.Contains(PatternManager.OmitFromBillsStr))
                    billedDemandCount += 1;
            }
            if (billedDemandCount == 0)
                continue;

            DemandElements.Add(element);
        }
        foreach (IHydrant element in WaterModel.Network.Hydrants.Elements(ElementStateType.Active))
        {
            if (element.Input.DemandCollection.Count == 0 && element.Input.UnitDemandLoadCollection.Count == 0)
                continue;

            DemandElements.Add(element);
        }
        foreach (ITank element in WaterModel.Network.Tanks.Elements(ElementStateType.Active))
        {
            if (element.Input.DemandCollection.Count == 0 && element.Input.UnitDemandLoadCollection.Count == 0)
                continue;
            DemandElements.Add(element);
        }

        Log.Information($"Got demand elements.");
    }

    private void InitializeTimeIndexDictionary()
    {
        Log.Debug("Initializing time index dictionary.");
        DateTimeToTimeIndex = new Dictionary<DateTime, int>();

        DateTime startDate = WaterModel.ActiveScenario.Options.SimulationStartDate;

        double[] timestepsInSeconds = WaterModel.ActiveScenario.TimeStepsInSeconds;

        for (int i = 0; i < timestepsInSeconds.Count(); i++)
        {
            double secondsFromStart = timestepsInSeconds[i];
            DateTime dateTime = startDate.AddSeconds(secondsFromStart);
            DateTimeToTimeIndex[dateTime] = i;
        }

        Log.Information("Initialized time index dictionary.");
    }

    private void UpdateCustomerBilledAndConsumptionRecords()
    {
        Log.Debug("Updating bill and consumption records.");
        foreach (IWaterElement element in DemandElements)
        {
            foreach (DateTime date in DatesInSimulation)
            {
                double totalDemand = CalculateAdjustedTotalDemand(element, date);
                double totalDemand_GPD = WaterModel.Units.ConvertValue(totalDemand, DemandUnit, Unit.GallonsPerDay);
                double elementTotalDailyVolume_kgal = WaterModel.Units.ConvertValue(totalDemand_GPD, Unit.Gallons, Unit.ThousandGallons);

                // TODO: This is currently configured for monthly bills. Can enhance this to allow for irregular billing periods.
                DateTime billStartDate = new DateTime(date.Year, date.Month, 1);
                Tuple<int, DateTime> key = new Tuple<int, DateTime>(element.Id, billStartDate);
                if (CumulativeMonthlyBills_kgal.ContainsKey(key))
                {
                    CumulativeMonthlyBills_kgal[key] = CumulativeMonthlyBills_kgal[key] + elementTotalDailyVolume_kgal;
                }
                else
                    CumulativeMonthlyBills_kgal[key] = elementTotalDailyVolume_kgal;
            }
        }

        Log.Information("Updated bill and consumption records.");
    }

    private double CalculateAdjustedTotalDemand(IWaterElement element, DateTime date)
    {
        double totalAdjustedBaseDemand = 0;

        var demandDetails = GetElementDemandDetails(element);
        var unitDemandDetails = GetElementUnitDemandDetails(element);

        foreach (DemandDetail detail in demandDetails)
        {
            totalAdjustedBaseDemand += GetAdjustedDemand(element, detail.BaseDemand, detail.DemandPattern, date);
        }

        foreach (UnitDemandDetail unitDetail in unitDemandDetails)
        {
            totalAdjustedBaseDemand += GetAdjustedUnitDemand(element, unitDetail.UnitDemand, unitDetail.NumUnitDemands, unitDetail.DemandPattern, date);
        }

        return totalAdjustedBaseDemand;
    }

    private double GetAdjustedDemand(IWaterElement element, double baseFlow, IPattern pattern, DateTime date)
    {
        double patternMultiplier;
        if (pattern is null)
        {
            patternMultiplier = 1.0;
        }
        else
        {
            if (pattern.Notes.Contains(PatternManager.OmitFromBillsStr))
                return 0;
            patternMultiplier = PatternManager.GetAverageMultiplierForDay(pattern, date);
        }

        double adjustedDemand = baseFlow;

        adjustedDemand *= patternMultiplier;

        if (!(DemandAdjustments is null))
        {
            foreach (IActiveDemandAdjustment demandAdjustment in DemandAdjustments)
            {
                // Note: Demand adjustments are applied in order, and are all applied prior to being multiplied by the pattern.
                if (!DemandAdjustmentApplies(element, pattern, demandAdjustment))
                    continue;

                if (demandAdjustment.Operation == AdjustmentOperationType.Add)
                    adjustedDemand += demandAdjustment.Value;
                else if (demandAdjustment.Operation == AdjustmentOperationType.Subtrace)
                    adjustedDemand -= demandAdjustment.Value;
                else if (demandAdjustment.Operation == AdjustmentOperationType.Multiply)
                    adjustedDemand *= demandAdjustment.Value;
                else if (demandAdjustment.Operation == AdjustmentOperationType.Divide)
                    adjustedDemand /= demandAdjustment.Value;
                else if (demandAdjustment.Operation == AdjustmentOperationType.Set)
                    adjustedDemand = demandAdjustment.Value;
            }
        }

        return adjustedDemand;
    }

    private double GetAdjustedUnitDemand(IWaterElement element, IUnitDemandLoad unitDemandLoad, double numUnitDemands, IPattern pattern, DateTime date)
    {
        if (unitDemandLoad is null)
            return 0;

        double adjustedDemand = unitDemandLoad.UnitDemand * numUnitDemands;

        double patternMultiplier = PatternManager.GetAverageMultiplierForDay(pattern, date);

        adjustedDemand *= patternMultiplier;

        if (!(UnitDemandAdjustments is null))
        {
            foreach (IActiveUnitDemandAdjustment unitDemandAdjustment in UnitDemandAdjustments)
            {
                // Note: Demand adjustments are applied in order, and are all applied prior to being multiplied by the pattern.
                if (!UnitDemandAdjustmentApplies(element, unitDemandLoad, unitDemandAdjustment))
                    continue;

                if (unitDemandAdjustment.Operation == AdjustmentOperationType.Add)
                    adjustedDemand += unitDemandAdjustment.Value;
                else if (unitDemandAdjustment.Operation == AdjustmentOperationType.Subtrace)
                    adjustedDemand -= unitDemandAdjustment.Value;
                else if (unitDemandAdjustment.Operation == AdjustmentOperationType.Multiply)
                    adjustedDemand *= unitDemandAdjustment.Value;
                else if (unitDemandAdjustment.Operation == AdjustmentOperationType.Divide)
                    adjustedDemand /= unitDemandAdjustment.Value;
                else if (unitDemandAdjustment.Operation == AdjustmentOperationType.Set)
                    adjustedDemand = unitDemandAdjustment.Value;
            }
        }

        return adjustedDemand;
    }

    private bool DemandAdjustmentApplies(IWaterElement element, IPattern pattern, IActiveDemandAdjustment adjustment)
    {
        // NOTE: If this method of checking scope is slow, an alternate method may work: adjustment.Scope.Get().Contains(element.Id)
        if (!(adjustment.Scope is null))
            if (!adjustment.Scope.Elements().Contains(element))
                return false;
        // TODO <sjac>: Currently there is no way in the OpenFlowsAPI to distinguish between an adjustment that does not specify a pattern and an adjustment that specifies the 'Fixed' pattern. 
        //      They are both treated as null patterns. Therefore any adjustment applied to Fixed patterns will be treated as if it applies to all patterns.
        if (!(adjustment.DemandPattern is null))
            if (pattern is null || adjustment.DemandPattern.Id != pattern.Id)
                return false;
        return true;
    }

    private bool UnitDemandAdjustmentApplies(IWaterElement element, IUnitDemandLoad unitDemandLoad, IActiveUnitDemandAdjustment adjustment)
    {
        // NOTE: If this method of checking scope is slow, an alternate method may work: adjustment.Scope.Get().Contains(element.Id)
        if (!(adjustment.Scope is null))
            if (!adjustment.Scope.Elements().Contains(element))
                return false;
        if (!(adjustment.UnitLoadDemand is null))
            if (adjustment.UnitLoadDemand.Id != unitDemandLoad.Id)
                return false;
        return true;
    }

    private IZone GetElementZone(IWaterElement element)
    {
        if (ElementZones.ContainsKey(element.Id))
            return ElementZones[element.Id];

        IZone zone = null;
        if (element is IJunction)
        {
            zone = ((IJunction)element).Input.Zone;
        }
        else if (element is IHydrant)
        {
            zone = ((IHydrant)element).Input.Zone;
        }
        else if (element is ITank)
        {
            zone = ((ITank)element).Input.Zone;
        }
        else if (element is ICustomerMeter)
        {
            // TODO <sjac>: This is a hacky workaround, as the OpenFlowsAPI currently doesn't have a direct way to get the associated Zone for a customer meter.
            zone = (IZone)((ICustomerMeter)element).Input.AssociatedElement.GetType().GetProperty("Zone").GetValue(((ICustomerMeter)element).Input.AssociatedElement, null);
        }
        else
        {
            throw new NotImplementedException();
        }

        ElementZones[element.Id] = zone;
        return zone;
    }

    //internal void UpdateElementBillRecords(IWaterDomainElement element, double elementTotalAverageDailyFlow_GPD, DateTime date, IZone zone)
    //{
    //    if (VolumeUnit is null)
    //        VolumeUnit = Model.Units.NetworkUnits.Tank.VolumeFormatter.DisplayUnit;
    //    double elementTotalDailyVolume = Model.Units.ConvertValue(elementTotalAverageDailyFlow_GPD, Unit.Gallons, VolumeUnit);

    //    DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
    //    DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

    //    // Update Customer Bills
    //    // TODO <sjac>: This can be expanded to allow monthly billing periods or randomized billing periods of roughly X number of days with a user option.
    //    UpdateCustomerBillsRow(element, date, startOfMonth, endOfMonth, elementTotalDailyVolume, zone);
    //}

    //internal void UpdateCustomerBillsRow(IWaterDomainElement element, DateTime date, DateTime billStartDate, DateTime billEndDate, double totalDailyVolume, IZone zone)
    //{
    //    string query = $"{element.Id}, {billStartDate.ToShortDateString()}";
    //    DataRow row = FindRow(OutputManager.CustomerBillResults, query);

    //    if (row is null)
    //    {
    //        string zoneLabel = String.Empty;
    //        if (zone != null)
    //            zoneLabel = zone.Label;

    //        row = OutputManager.CustomerBillResults.NewRow();
    //        if (element is ICustomerMeter)
    //        {
    //            string billingID = (string)((ICustomerMeter)element).Manager.FieldManager.Field(StandardFieldName.CustomerNode_BillingID).GetValue(element.Id);
    //            if (!(billingID == string.Empty))
    //                row[OutputManager.CustomerIDColumn] = $"\"{billingID}\""; // Add double quotes around text field, so that if the Billing ID contains commas it will still be treated as one row by CSV interpreters.
    //            else
    //                row[OutputManager.CustomerIDColumn] = element.Id;
    //        }
    //        else
    //            row[OutputManager.CustomerIDColumn] = element.Id;
    //        row[OutputManager.CustomerStartColumn] = billStartDate;
    //        row[OutputManager.CustomerEndColumn] = billEndDate;
    //        row[OutputManager.CustomerVolumeColumn] = totalDailyVolume;
    //        row[OutputManager.CustomerZoneColumn] = zoneLabel;

    //        OutputManager.CustomerBillResults.Rows.Add(row);

    //        AddRowToFinder(OutputManager.CustomerBillResults, query, row);
    //    }
    //    else
    //    {
    //        double billVolume = (double)row[OutputManager.CustomerVolumeColumn];
    //        billVolume += totalDailyVolume;
    //        row[OutputManager.CustomerVolumeColumn] = billVolume;
    //    }
    //}

    //internal DataRow FindRow(DataTable table, string query)
    //{
    //    // Note: a dictionary has better performance than a DataTable.Select query.
    //    if (TableRowFinders is null)
    //        TableRowFinders = new Dictionary<string, Dictionary<string, DataRow>>();

    //    Dictionary<string, DataRow> rowFinder;
    //    if (TableRowFinders.ContainsKey(table.TableName))
    //        rowFinder = TableRowFinders[table.TableName];
    //    else
    //    {
    //        rowFinder = new Dictionary<string, DataRow>();
    //        TableRowFinders[table.TableName] = rowFinder;
    //    }

    //    if (rowFinder.ContainsKey(query))
    //        return rowFinder[query];
    //    else
    //        return null;
    //}

    private List<DemandDetail> GetElementDemandDetails(IWaterElement element)
    {
        int id = element.Id;
        if (ElementDemandDetails.ContainsKey(id))
            return ElementDemandDetails[id];

        var detailList = new List<DemandDetail>();

        if (element is ICustomerMeter customerMeter)
        {
            DemandDetail detail = new DemandDetail(customerMeter.Input.BaseDemand, customerMeter.Input.DemandPattern);
            detailList.Add(detail);
        }
        else if (element is IJunction junction)
        {
            foreach (IDemand demand in junction.Input.DemandCollection.Get())
            {
                DemandDetail detail = new DemandDetail(demand.BaseFlow, demand.DemandPattern);
                detailList.Add(detail);
            }
        }
        else if (element is IHydrant hydrant)
        {
            foreach (IDemand demand in hydrant.Input.DemandCollection.Get())
            {
                DemandDetail detail = new DemandDetail(demand.BaseFlow, demand.DemandPattern);
                detailList.Add(detail);
            }
        }
        else if (element is ITank tank)
        {
            foreach (IDemand demand in tank.Input.DemandCollection.Get())
            {
                DemandDetail detail = new DemandDetail(demand.BaseFlow, demand.DemandPattern);
                detailList.Add(detail);
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        ElementDemandDetails[id] = detailList;
        return detailList;
    }

    private List<UnitDemandDetail> GetElementUnitDemandDetails(IWaterElement element)
    {
        int id = element.Id;
        if (ElementUnitDemandDetails.ContainsKey(id))
            return ElementUnitDemandDetails[id];

        var detailList = new List<UnitDemandDetail>();

        if (element is ICustomerMeter customerMeter)
        {
            UnitDemandDetail detail = new UnitDemandDetail(customerMeter.Input.UnitDemand, customerMeter.Input.NumberOfUnitDemands, customerMeter.Input.UnitDemandPattern);
            detailList.Add(detail);
        }
        else if (element is IJunction junction)
        {
            foreach (IUnitLoadDemand demand in junction.Input.UnitDemandLoadCollection.Get())
            {
                UnitDemandDetail detail = new UnitDemandDetail(demand.UnitDemandLoad, demand.NumberOfLoadingUnits, demand.UnitDemandPattern);
                detailList.Add(detail);
            }
        }
        else if (element is IHydrant hydrant)
        {
            foreach (IUnitLoadDemand demand in hydrant.Input.UnitDemandLoadCollection.Get())
            {
                UnitDemandDetail detail = new UnitDemandDetail(demand.UnitDemandLoad, demand.NumberOfLoadingUnits, demand.UnitDemandPattern);
                detailList.Add(detail);
            }
        }
        else if (element is ITank tank)
        {
            foreach (IUnitLoadDemand demand in tank.Input.UnitDemandLoadCollection.Get())
            {
                UnitDemandDetail detail = new UnitDemandDetail(demand.UnitDemandLoad, demand.NumberOfLoadingUnits, demand.UnitDemandPattern);
                detailList.Add(detail);
            }
        }

        ElementUnitDemandDetails[id] = detailList;
        return detailList;
    }

    //internal void AddRowToFinder(DataTable table, string query, DataRow row)
    //{
    //    Dictionary<string, DataRow> rowFinder = TableRowFinders[table.TableName];
    //    rowFinder[query] = row;
    //}
    #endregion

    #region Private Properties
    private SCADADataGeneratorOptions Options { get; }
    private IWaterModel WaterModel { get; }

    private Randomizer Randomizer { get; }
    private OutputManager OutputManager { get; }
    private PatternManager PatternManager { get; }
    private IActiveDemandAdjustments DemandAdjustments { get; set; }
    private IActiveUnitDemandAdjustments UnitDemandAdjustments { get; set; }

    private Unit FlowUnit { get; set; }
    private Unit DemandUnit { get; set; }
    private Unit VolumeUnit { get; set; }

    private Dictionary<int, IZone> ElementZones { get; } = new Dictionary<int, IZone>();
    private List<DateTime> DatesInSimulation { get; set; } = new List<DateTime>();
    private List<IWaterElement> DemandElements { get; } = new List<IWaterElement>();
    private Dictionary<DateTime, int> DateTimeToTimeIndex { get; set; } = new Dictionary<DateTime, int>();
    private Dictionary<int, List<DemandDetail>> ElementDemandDetails { get; } = new Dictionary<int, List<DemandDetail>>();
    private Dictionary<int, List<UnitDemandDetail>> ElementUnitDemandDetails { get; } = new Dictionary<int, List<UnitDemandDetail>>();
    private Dictionary<Tuple<int, DateTime>, double> CumulativeMonthlyBills_kgal { get; } = new Dictionary<Tuple<int, DateTime>, double>();

    #endregion

    #region Internal Structs
    internal struct DemandDetail
    {
        internal DemandDetail(double baseDemand, IPattern demandPattern)
        {
            BaseDemand = baseDemand;
            DemandPattern = demandPattern;
        }
        internal double BaseDemand { get; }
        internal IPattern DemandPattern { get; }
    }

    internal struct UnitDemandDetail
    {
        internal UnitDemandDetail(IUnitDemandLoad unitDemand, double numUnitDemands, IPattern demandPattern)
        {
            UnitDemand = unitDemand;
            NumUnitDemands = numUnitDemands;
            DemandPattern = demandPattern;
        }
        internal IUnitDemandLoad UnitDemand { get; }
        internal double NumUnitDemands { get; }
        internal IPattern DemandPattern { get; }
    }
    #endregion
}
