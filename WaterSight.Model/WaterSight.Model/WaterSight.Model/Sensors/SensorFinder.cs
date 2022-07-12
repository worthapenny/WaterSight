using Haestad.Mapping.Support;
using WaterSight.Model.Domain;
using WaterSight.Model.Extensions;
using OpenFlows.Water.Application;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Model.Support;
using Haestad.Framework.Application;
using OpenFlows.Domain.ModelingElements.NetworkElements;

namespace WaterSight.Model.Sensors;

public class SensorFinder
{
    #region Constructor
    public SensorFinder(IWaterModel waterModel)
    {
        WaterModel = waterModel;
    }
    #endregion

    #region Public Methods
    //public async Task<List<Sensor>> SearchFeatureAsync(IDomainProject domainProject, SensorFinderOptions options, bool isDuplicateOK = false)
    //{
    //    var connectivity = await Library.GetConnectivityAsync(domainProject);

    //    var sensors = new List<Sensor>();
    //    sensors.AddRange(SerachTanks(connectivity, options));
    //    //sensors.AddRange(await SearchPumpsAsync(connectivity, options));
    //    //sensors.AddRange(SearchValves(connectivity, options));
    //    //sensors.AddRange(await SearchReservoirsAsync(connectivity, options));

    //    // Drop the duplicates
    //    if (!isDuplicateOK)
    //    {
    //        var count = sensors.Count();
    //        sensors = sensors.GroupBy(s => new { s.NetworkElement.Id, s.SensorType, s.IsDirection, s.IsDirectionOutwards })
    //                        .Select(s => s.FirstOrDefault()).ToList();
    //        Log.Information($"Duplicate sensors values are dropped, before count {count}, after coulnt {sensors.Count}");
    //    }

    //    return sensors;

    //}

    public async Task<List<Sensor>> SearchElementsAsync(SensorFinderOptions options, bool isDuplicateOK = false)
    {        
        var sensors = new List<Sensor>();
        sensors.AddRange(SearchTanks(options));
        sensors.AddRange(await SearchPumpsAsync(options));
        sensors.AddRange(SearchValves(options));
        sensors.AddRange(SearchReservoirs(options));

        // Drop the duplicates
        if (!isDuplicateOK)
        {
            var count = sensors.Count();
            sensors = sensors.GroupBy(s => new { s.NetworkElement.Id, s.SensorType, s.IsDirection, s.IsDirectionOutwards })
                            .Select(s => s.FirstOrDefault()).ToList();
            Log.Information($"Duplicate sensors values are dropped, before count {count}, after coulnt {sensors.Count}");
        }

        return sensors;
    
    }
    public List<Sensor> SearchTanks(ConnectionTopology conntivity, SensorFinderOptions options)
    {
        Log.Debug($"Searching possible sensors for tanks...");

        var a = conntivity.NodeIdToAttachedLinkIdsMap[WaterModel.Network.Tanks.ElementIDs().First()];

        var sensors = new List<Sensor>();
        var sw = new Stopwatch();
        sw.Start();

        var tankElements = WaterModel.Network.Tanks.Elements();
        Sensor sensor;
        if (options.TankLevel)
        {
            foreach (var tank in tankElements)
            {
                sensor = new Sensor(SensorType.Level, tank, tank, SCADATargetAttribute.TankLevel);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {tank.IdLabel(true)} = {sensor}");
            }
        }

        if (options.TankFlow)
        {
            tankElements.ForEach(tank =>
            {


                var connectedLinks = WaterModel.Network.ConnectedElements(WaterModel, tank);
                connectedLinks.ForEach(l =>
                {
                    sensor = new Sensor(SensorType.Flow, l as IWaterElement, tank, SCADATargetAttribute.Discharge);
                    sensors.Add(sensor);
                    Log.Debug($"Sensor found for connected link of {tank.IdLabel(true)} = {sensor}");
                });
            });
        }

        sw.Stop();
        Log.Information($"Time taken: {sw.Elapsed} for {tankElements.Count} tanks. Sensor found: {sensors.Count}");
        Log.Debug(new string('.', 100));

        return sensors;
    }
    public List<Sensor> SearchTanks(SensorFinderOptions options)
    {
        Log.Debug($"Searching possible sensors for tanks...");

        var sensors = new List<Sensor>();
        var sw = new Stopwatch();
        sw.Start();

        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
        var tankElements = WaterModel.Network.Tanks.Elements(isActiveState);


        Sensor sensor;

        foreach (var tank in tankElements)
        {
            var connectedScadaElements = tank.GetConnectedSCADAElements(WaterModel);


            if (options.TankLevel)
            {
                sensor = new Sensor(SensorType.Level, tank, tank, SCADATargetAttribute.TankLevel);
                var tankLevelScadaElementCheck = connectedScadaElements
                    .Where(e => e.Input?.TargetAttribute == SCADATargetAttribute.TankLevel);

                if (tankLevelScadaElementCheck.Any())
                    sensor = new Sensor(tankLevelScadaElementCheck.First(), SensorType.Level);

                sensors.Add(sensor);
                Log.Debug($"Sensor found for {tank.IdLabel(true)} = {sensor}");
            }

            if (options.TankFlow)
            {
                var connectedLinks = WaterModel.Network.ConnectedElements(WaterModel, tank);
                connectedLinks.ForEach(l =>
                {
                    sensor = new Sensor(SensorType.Flow, l as IWaterElement, tank, SCADATargetAttribute.Discharge);
                    sensors.Add(sensor);
                    Log.Debug($"Sensor found for connected link of {tank.IdLabel(true)} = {sensor}");
                });
            }

        }

        sw.Stop();
        Log.Information($"Time taken: {sw.Elapsed} for {tankElements.Count} tanks. Sensor found: {sensors.Count}");
        Log.Debug(new string('.', 100));

        return sensors;
    }
    public async Task<List<Sensor>> SearchPumpsAsync(SensorFinderOptions options)
    {
        Log.Debug($"Searching possible sensors for pumps...");
        var sw = new Stopwatch();
        sw.Start();

        var sensors = new List<Sensor>();

        // VSPBs are not supported

        // Regular Pumps
        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
        var pumps = WaterModel.Network.Pumps.Elements(isActiveState);

        foreach (var pump in pumps)
        {
            Sensor sensor;

            // on pump itself
            if (options.PumpStatus)
            {
                sensor = new Sensor(SensorType.Status, pump, pump, SCADATargetAttribute.PumpStatus);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }

            // on pump itself
            if (options.PumpPower)
            {
                sensor = new Sensor(SensorType.Power, pump, pump, SCADATargetAttribute.WirePower);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }

            // on pump itself
            if (options.PumpSpeedFactor)
            {
                sensor = new Sensor(SensorType.PumpSpeed, pump, pump, SCADATargetAttribute.PumpSetting);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }

            // on downstream link
            var downstreamLink = pump.Input.DownstreamLink;
            if (options.PumpDischargePipeFlow && downstreamLink != null)
            {
                sensor = new Sensor(SensorType.Flow, downstreamLink as IWaterElement, pump, SCADATargetAttribute.Discharge);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }

            // on downstream node
            if (options.PumpDischargeNodePressure)
            {
                sensor = new Sensor(SensorType.Pressure, (downstreamLink as IPipeInput).StopNode as IWaterElement, pump, SCADATargetAttribute.Pressure);
                sensor.IsDirection = true;
                sensor.IsDirectionOutwards = true;
                sensor.UpdateLabel();
                sensor.GetTagName();
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }

            // on upstream node
            if (options.PumpSuctionNodePressure)
            {
                var inLinksCheck = await WaterModel.Network.IncomingLinksAsync(WaterModel, pump);
                if (!inLinksCheck.Any())
                {
                    Log.Warning($"A pump must have an incoming link. If it does then geometry can be improved. Pump: {pump.IdLabel()}");
                    var connectedLinks = WaterModel.Network.ConnectedElements(WaterModel, pump);
                    connectedLinks.Remove(pump.Input.DownstreamLink);
                    inLinksCheck = new List<IWaterElement>() { connectedLinks.First() as IWaterElement };
                }

                var upstreamLink = inLinksCheck.First();
                sensor = new Sensor(SensorType.Pressure, (upstreamLink as IPipeInput).StartNode as IWaterElement, pump, SCADATargetAttribute.Pressure);
                sensor.IsDirection = true;
                sensor.IsDirectionOutwards = false;
                sensor.UpdateLabel();
                sensor.GetTagName();
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {pump.IdLabel(true)} = {sensor}");
            }


        }

        Log.Information($"Time taken: {sw.Elapsed} for {pumps.Count} pumps. Sensor found: {sensors.Count}");
        sw.Stop();
        Log.Debug(new string('.', 100));

        return sensors;
    }
    public List<Sensor> SearchValves(SensorFinderOptions options)
    {
        Log.Debug($"Searching possible sensors for valves...");
        var sw = new Stopwatch();
        sw.Start();

        var sensors = new List<Sensor>();

        var valves = new List<IWaterElement>();
        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;

        valves.AddRange(WaterModel.Network.FCVs.Elements(isActiveState));
        valves.AddRange(WaterModel.Network.PRVs.Elements(isActiveState));
        valves.AddRange(WaterModel.Network.PSVs.Elements(isActiveState));
        valves.AddRange(WaterModel.Network.PBVs.Elements(isActiveState));
        if (options.TCVs) valves.AddRange(WaterModel.Network.TCVs.Elements(isActiveState));
        if (options.GPVs) valves.AddRange(WaterModel.Network.GPVs.Elements(isActiveState));

        foreach (var valve in valves)
        {
            Sensor sensor;

            // on valve itself
            if (options.ValveFlow)
            {
                sensor = new Sensor(SensorType.Flow, valve, valve, SCADATargetAttribute.Discharge);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {valve.IdLabel(true)} = {sensor}");

            }

            if (options.ValveStatus)
            {

                sensor = new Sensor(SensorType.Status, valve, valve, SCADATargetAttribute.ValveStatus);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {valve.IdLabel(true)} = {sensor}");
            }

            if (options.ValveDownstreamPressure)
            {
                sensor = new Sensor(SensorType.Pressure, valve, valve, SCADATargetAttribute.PressureOut);
                sensors.Add(sensor);
                sensor.IsDirection = true;
                sensor.IsDirectionOutwards = true;
                sensor.UpdateLabel();
                sensor.GetTagName();
                if (valve is IPressureReducingValve)
                    sensor.TargetAttribute = SCADATargetAttribute.PressureValveSetting;

                Log.Debug($"Sensor found for {valve.IdLabel(true)} = {sensor}");
            }

            if (options.ValveUpstreamPressure)
            {
                sensor = new Sensor(SensorType.Pressure, valve, valve, SCADATargetAttribute.PressureIn);
                sensors.Add(sensor);
                sensor.IsDirection = true;
                sensor.IsDirectionOutwards = false;
                sensor.UpdateLabel();
                sensor.GetTagName();
                if (valve is IPressureSustainingValve)
                    sensor.TargetAttribute = SCADATargetAttribute.PressureValveSetting;

                Log.Debug($"Sensor found for {valve.IdLabel(true)} = {sensor}");
            }
        }

        Log.Information($"Time taken: {sw.Elapsed} for {valves.Count} valves. Sensor found: {sensors.Count}");
        sw.Stop();
        Log.Debug(new string('.', 100));

        return sensors;
    }

    public List<Sensor> SearchReservoirs(SensorFinderOptions options)
    {
        Log.Debug($"Searching possible sensors for reservoirs...");
        var sw = new Stopwatch();
        sw.Start();

        var sensors = new List<Sensor>();

        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
        var reservoirs = WaterModel.Network.Reservoirs.Elements(isActiveState);

        foreach (var res in reservoirs)
        {
            Sensor sensor;
            if (options.ReservoirHead)
            {
                sensor = new Sensor(SensorType.HydraulicGrade, res, res, SCADATargetAttribute.HydraulicGrade);
                sensors.Add(sensor);
                Log.Debug($"Sensor found for {res.IdLabel(true)} = {sensor}");

            }

            if (options.ReservoirFlow)
            {
                var connectedLinkCheck = WaterModel.Network.ConnectedElements(WaterModel, res);
                if (connectedLinkCheck.Any())
                {
                    var connectedLink = connectedLinkCheck.First();
                    sensor = new Sensor(SensorType.Flow, connectedLink as IWaterElement, res, SCADATargetAttribute.Discharge);
                    sensors.Add(sensor);
                    Log.Debug($"Sensor found for {res.IdLabel(true)} = {sensor}");
                }
            }
        }

        sw.Stop();
        Log.Information($"Time taken: {sw.Elapsed} for {reservoirs.Count} reservoirs. Sensors count: {sensors.Count}");
        Log.Debug(new string('.', 100));

        return sensors;
    }
    #endregion


    #region Public Properties
    public IWaterModel WaterModel { get; }
    #endregion

    #region Private Properties
    #endregion
}
