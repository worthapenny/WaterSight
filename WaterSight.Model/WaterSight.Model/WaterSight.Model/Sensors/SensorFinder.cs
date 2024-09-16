//using Haestad.Framework.Application;
//using Haestad.Network;
//using Haestad.Support.Support;
//using OpenFlows.Domain.ModelingElements.NetworkElements;
//using OpenFlows.Water.Domain;
//using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
//using Serilog;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using WaterSight.Model.Extensions;
//using WaterSight.Model.Library;
//using WaterSight.Model.Support.Network;

//namespace WaterSight.Model.Sensors;



//public class SensorFinder
//{
//    #region Constructor
//    public SensorFinder(IWaterModel waterModel)
//    {
//        WaterModel = waterModel;
//    }
//    #endregion


//    #region Public Methods  
//    public List<Sensor> DropDuplicates(List<Sensor> sensors)
//    {
//        // Same network element and for the same attribute
//        var uniqueSensors = sensors.GroupBy(s => new {s.NetworkElementId, s.TargetAttribute})
//            .Select(s => s.FirstOrDefault())
//            .ToList();

//        Log.Information($"Duplicate sensors [Same element, Same attribute] values are dropped, before count {sensors.Count()}, after count {uniqueSensors.Count}");
//        var newCount  = uniqueSensors.Count;

//        // Same network element, same type, same direction
//        uniqueSensors = sensors.GroupBy(s => new { s.NetworkElement.Id, s.SensorType, s.IsDirectional, s.IsDirectionOutward })
//            .Select(s => s.FirstOrDefault())
//            .ToList();

//        Log.Information($"Duplicate sensors [Same element, Same type, in same direction] values are dropped, before count {newCount}, after count {uniqueSensors.Count}");

//        return uniqueSensors;
//    }

//    public async Task<List<Sensor>> SearchElementsAsync(IDomainProject project, SensorFinderOptions options, bool shouldDropDuplicates = true)
//    {
//        LogLibrary.Separate_StartGroup();
//        var topology = await BuildNetworkCacheAsync(project);

//        var sensors = new List<Sensor>();
//        sensors.AddRange(SearchTanks(topology, options));
//        sensors.AddRange(await SearchPumpsAsync(topology, options));
//        sensors.AddRange(SearchValves(options));
//        sensors.AddRange(SearchReservoirs(options));

       
//        if (shouldDropDuplicates)
//            sensors = DropDuplicates(sensors);

//        // If Label is empty, tag will be empty
//        // So, fill such with the NetworkElementId
//        var sensorsWithEmptyTagCheck = sensors.Where(s => string.IsNullOrEmpty(s.TagName));
//        foreach (var emptyTagSensor in sensorsWithEmptyTagCheck)
//        {
//            emptyTagSensor.TagName = $"NoLabel__{emptyTagSensor.NetworkElementId}";
//            Log.Information($"Sensor with no tag got updated. Sensor: {emptyTagSensor}");
//        }

//        LogLibrary.Separate_EndGroup();

//        return sensors;

//    }

//    public List<Sensor> SearchTanks(ConnectionTopology topology, SensorFinderOptions options)
//    {
//        Log.Debug($"Searching possible sensors for tanks...");

//        var sensors = new List<Sensor>();
//        var sw = new Stopwatch();
//        sw.Start();

//        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
//        var tankElements = WaterModel.Network.Tanks.Elements(isActiveState);


//        Sensor sensor;

//        foreach (var tank in tankElements)
//        {

//            if (options.IncludeTankLevel)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Level, tank, tank, SCADATargetAttribute.TankLevel);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for tank {tank.IdLabel} is {sensor}");
//            }

//            if (options.IncludeTankFlow)
//            {
//                if (!topology.NodeIdToAttachedLinkIdsMap.Contains(tank.Id))
//                    throw new ProjectException($"No connected elements found for tank {tank.IdLabel}");

//                var connectedLinkIdsObject = topology.NodeIdToAttachedLinkIdsMap[tank.Id];
//                if (connectedLinkIdsObject == null)
//                {
//                    Log.Warning($"No connected nodes found for tank: {tank}");
//                    continue;
//                }

//                // Get the connected pipes to the given tank
//                var connectedPipes = ((HmIDCollection)connectedLinkIdsObject)
//                        .Cast<int>()
//                        .Select(id => WaterModel.Element(id) as IPipe)
//                        .ToList();


//                var isDirectional = false;
//                var isDirectionOutwards = false;

//                connectedPipes.ForEach(l =>
//                {
//                    sensor = new Sensor(WaterModel, SensorType.Flow, l, tank, SCADATargetAttribute.Discharge);
//                    sensors.Add(sensor);

//                    // When two pipes are connected, see if we can identify incoming and outgoing
//                    if (connectedPipes.Count > 1)
//                    {
//                        // For two pipes connected to a tank
//                        if (connectedPipes.Count > 1 && connectedPipes.Count <= 2)
//                        {
//                            Log.Debug($"Two pipes are connected to {tank.IdLabel} tank");

//                            // First Pipe
//                            var pipe = connectedPipes.First();
//                            var stopNodeFirstPipe = pipe.Input.StopNode;
//                            if (stopNodeFirstPipe.Id == tank.Id)
//                            {
//                                isDirectional = true;
//                                isDirectionOutwards = false;
//                                Log.Debug($"Inflow for tank {tank.IdLabel} is to pipe {pipe.IdLabel}");
//                            }

//                            // Second Pipe
//                            pipe = connectedPipes.Last();
//                            var startNodeSecondPipe = pipe.Input.StartNode;
//                            if (startNodeSecondPipe.Id == tank.Id)
//                            {
//                                isDirectional = true;
//                                isDirectionOutwards = true;
//                                Log.Debug($"Outflow for tank {tank.IdLabel} is to pipe {pipe.IdLabel}");
//                            }

//                        }
//                        else
//                        {
//                            Log.Warning($"Tank {tank.IdLabel} has more than two pipes connected. Generated tags could be duplicate.");
//                        }
//                    }


//                    if (isDirectional)
//                    {
//                        sensor.IsDirectional = isDirectional;
//                        sensor.IsDirectionOutward = isDirectionOutwards;
//                        sensor.UpdateTagName();
//                        sensor.UpdateLabel();
//                    }

//                    Log.Verbose($"Sensor found for connected link of {tank.IdLabel} = {sensor}");
//                });
//            }

//        }

//        sw.Stop();
//        Log.Information($"Time taken: {sw.Elapsed} for {tankElements.Count} tanks. Sensor found: {sensors.Count}");
//        Log.Debug(new string('.', 100));

//        return sensors;
//    }

//    public async Task<List<Sensor>> SearchPumpsAsync(ConnectionTopology topology, SensorFinderOptions options)
//    {
//        Log.Debug($"Searching possible sensors for pumps...");
//        var sw = new Stopwatch();
//        sw.Start();

//        var sensors = new List<Sensor>();

//        // VSPBs are not supported

//        // Regular Pumps
//        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
//        var pumps = WaterModel.Network.Pumps.Elements(isActiveState);

//        foreach (var pump in pumps)
//        {
//            Sensor sensor;

//            // on pump itself [STATUS]
//            if (options.IncludePumpStatus)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Status, pump, pump, SCADATargetAttribute.PumpStatus);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }

//            // on pump itself [POWER]
//            if (options.IncludePumpPower)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Power, pump, pump, SCADATargetAttribute.WirePower);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }

//            // on pump itself [SPEED]
//            if (options.IncludePumpSpeedFactor)
//            {
//                sensor = new Sensor(WaterModel, SensorType.PumpSpeed, pump, pump, SCADATargetAttribute.PumpSetting);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }

//            // on downstream link
//            var downstreamLink = pump.Input.DownstreamLink;
//            if (options.IncludePumpDischargePipeFlow && downstreamLink != null)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Flow, downstreamLink as IWaterElement, pump, SCADATargetAttribute.Discharge);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }

//            // on downstream node
//            if (options.IncludePumpDischargeNodePressure)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Pressure, (downstreamLink as IPipeInput).StopNode as IWaterElement, pump, SCADATargetAttribute.Pressure);
//                sensor.IsDirectional = true;
//                sensor.IsDirectionOutward = true;
//                sensor.UpdateLabel();
//                sensor.UpdateTagName();
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }

//            // on upstream node
//            if (options.IncludePumpSuctionNodePressure)
//            {
//                if (topology.LinkIdToStopNodeIdMap.Contains(pump.Id))
//                {
//                    Log.Warning($"");
//                }

//                var incomingLink = topology.LinkIdToStopNodeIdMap[pump.Id];


//                var inLinksCheck = await WaterModel.Network.IncomingLinksAsync(WaterModel, pump);
//                if (!inLinksCheck.Any())
//                {
//                    Log.Warning($"A pump must have an incoming link. If it does then geometry can be improved. Pump: {pump.IdLabel}");
//                    var connectedLinks = pump.ConnectedAdjacentElements(WaterModel);
//                    connectedLinks.Remove(pump.Input.DownstreamLink as IWaterElement);
//                    inLinksCheck = new List<IWaterElement>() { connectedLinks.First() as IWaterElement };
//                }

//                var upstreamLink = inLinksCheck.First();
//                sensor = new Sensor(WaterModel, SensorType.Pressure, (upstreamLink as IPipeInput).StartNode as IWaterElement, pump, SCADATargetAttribute.Pressure);
//                sensor.IsDirectional = true;
//                sensor.IsDirectionOutward = false;
//                sensor.UpdateLabel();
//                sensor.UpdateTagName();
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {pump.IdLabel} = {sensor}");
//            }


//            // Common Downstream Node (for the whole station)

//            // Common Upstream Node (for the whole station)

//            // Common Downstream Flow (for the whole station)

//            // Common Upstream Flow (for the whole station)



//        }

//        Log.Information($"Time taken: {sw.Elapsed} for {pumps.Count} pumps. Sensor found: {sensors.Count}");
//        sw.Stop();
//        Log.Debug(new string('.', 100));

//        return sensors;
//    }

//    /*    public Sensor[] SearchDownstreamNodeAndLinkOnPumpsAsync(SensorFinderOptions options, ConnectionTopology topology)
//        {
//            var project = WaterApplicationManager.GetInstance()?.ParentFormModel?.CurrentProject as IDomainProject ?? null;
//            if (project == null)
//                throw new ProjectException("To find the common node, the water model must be opened from application layers.");


//            var pumps = WaterModel.Network.Pumps.Elements();
//            foreach (var pump in pumps)
//            {

//            }


//            // Let's trace the network to find a common node and a link
//            var downstreamLinks = new List<int>();
//            var downstreamNodes = new List<int>();



//            // For some reason the results are not reliable when doing the 
//            // tracing from pump. Hence finding the downstream node then
//            // performing the tracking
//            var downNodeId = Network.GetEdgeStopId(pumpInput.DownstreamLink.Id);

//            (WaterModel.Element(downNodeId) as IWaterElement)
//                       .TraceDownStream(Network, out downstreamNodes, out downstreamLinks);







//            var nodeSensor = new Sensor();
//            var linkSensor = new Sensor();

//            return new Sensor[] { nodeSensor, linkSensor };
//        }*/

//    public List<Sensor> SearchValves(SensorFinderOptions options)
//    {
//        Log.Debug($"Searching possible sensors for valves...");
//        var sw = new Stopwatch();
//        sw.Start();

//        var sensors = new List<Sensor>();

//        var valves = new List<IWaterElement>();
//        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;

//        valves.AddRange(WaterModel.Network.FCVs.Elements(isActiveState));
//        valves.AddRange(WaterModel.Network.PRVs.Elements(isActiveState));
//        valves.AddRange(WaterModel.Network.PSVs.Elements(isActiveState));
//        valves.AddRange(WaterModel.Network.PBVs.Elements(isActiveState));
//        if (options.IncludeTCVs) valves.AddRange(WaterModel.Network.TCVs.Elements(isActiveState));
//        if (options.IncludeGPVs) valves.AddRange(WaterModel.Network.GPVs.Elements(isActiveState));

//        foreach (var valve in valves)
//        {
//            Sensor sensor;

//            // on valve itself
//            if (options.IncludeValveFlow)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Flow, valve, valve, SCADATargetAttribute.Discharge);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {valve.IdLabel} = {sensor}");

//            }

//            if (options.IncludeValveStatus)
//            {

//                sensor = new Sensor(WaterModel, SensorType.Status, valve, valve, SCADATargetAttribute.ValveStatus);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {valve.IdLabel} = {sensor}");
//            }

//            if (options.IncludeValveDownstreamPressure)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Pressure, valve, valve, SCADATargetAttribute.PressureOut);
//                sensors.Add(sensor);
//                sensor.IsDirectional = true;
//                sensor.IsDirectionOutward = true;
//                sensor.UpdateLabel();
//                sensor.UpdateTagName();

//                Log.Verbose($"Sensor found for {valve.IdLabel} = {sensor}");
//            }

//            if (options.IncludeValveUpstreamPressure)
//            {
//                sensor = new Sensor(WaterModel, SensorType.Pressure, valve, valve, SCADATargetAttribute.PressureIn);
//                sensors.Add(sensor);
//                sensor.IsDirectional = true;
//                sensor.IsDirectionOutward = false;
//                sensor.UpdateLabel();
//                sensor.UpdateTagName();

//                Log.Verbose($"Sensor found for {valve.IdLabel} = {sensor}");
//            }
//        }

//        Log.Information($"Time taken: {sw.Elapsed} for {valves.Count} valves. Sensor found: {sensors.Count}");
//        sw.Stop();
//        Log.Debug(new string('.', 100));

//        return sensors;
//    }

//    public List<Sensor> SearchReservoirs(SensorFinderOptions options)
//    {
//        Log.Debug($"Searching possible sensors for reservoirs...");
//        var sw = new Stopwatch();
//        sw.Start();

//        var sensors = new List<Sensor>();

//        var isActiveState = options.ActiveElementsOnly ? ElementStateType.Active : ElementStateType.Inactive;
//        var reservoirs = WaterModel.Network.Reservoirs.Elements(isActiveState);

//        foreach (var res in reservoirs)
//        {
//            Sensor sensor;
//            if (options.IncludeReservoirHead)
//            {
//                sensor = new Sensor(WaterModel, SensorType.HydraulicGrade, res, res, SCADATargetAttribute.HydraulicGrade);
//                sensors.Add(sensor);
//                Log.Verbose($"Sensor found for {res.IdLabel} = {sensor}");

//            }

//            if (options.IncludeReservoirFlow)
//            {
//                var connectedLinkCheck = res.ConnectedAdjacentElements(WaterModel);
//                if (connectedLinkCheck.Any())
//                {
//                    var connectedLink = connectedLinkCheck.First();
//                    sensor = new Sensor(WaterModel, SensorType.Flow, connectedLink as IWaterElement, res, SCADATargetAttribute.Discharge);
//                    sensors.Add(sensor);
//                    Log.Verbose($"Sensor found for {res.IdLabel} = {sensor}");
//                }
//            }
//        }

//        sw.Stop();
//        Log.Information($"Time taken: {sw.Elapsed} for {reservoirs.Count} reservoirs. Sensors count: {sensors.Count}");
//        Log.Debug(new string('.', 100));

//        return sensors;
//    }
//    #endregion


//    #region Private Methods
//    private async Task<ConnectionTopology> BuildNetworkCacheAsync(IDomainProject project)
//    {
//        return await ConnectionTopology.GetConnectivityAsync(project);
//    }
//    #endregion

//    #region Public Properties
//    public IWaterModel WaterModel { get; }
//    #endregion


//    #region Private Properties
//    private HmiNetwork Network
//    {
//        get
//        {
//            if (_network == null)
//                _network = new Haestad.NetworkBuilder.Water.IdahoNetworkBuilder(WaterModel.DomainDataSet).CreateNetwork();

//            return _network;
//        }
//    }
//    #endregion

//    #region Fields
//    HmiNetwork _network;
//    #endregion
//}
