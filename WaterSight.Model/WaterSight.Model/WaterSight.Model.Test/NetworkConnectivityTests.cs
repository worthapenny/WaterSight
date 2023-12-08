using Haestad.NetworkBuilder.Water;

using NUnit.Framework;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Model.Extensions;
using WaterSight.Model.Sensors;
using WaterSight.Model.Support.Network;

namespace WaterSight.Model.Test;

public class NetworkConnectivityTests : OpenFlowsWaterTestFixtureBase
{
    #region Setup/Teardown
    protected override void OneTimeSetupImpl()
    {
    }
    #endregion

    #region Tests
    [Test]
    public void Connectivity_Tests()
    {
        ModelFilePath = @"d:\Development\Data\ModelData\Samples\Example5.wtg";

        int id;
        id = 307; // Example 5

        var element = id.WaterElement(WaterModel);
        Assert.That(element, Is.Not.Null);

        var nodeIds = new List<int>();
        var linkIds = new List<int>();
        var nodes = new List<IWaterElement>();
        var links = new List<IWaterElement>();


        var networkBuilder = new IdahoNetworkBuilder(WaterModel.DomainDataSet);
        var network = networkBuilder.CreateNetwork();

        if (!WaterModel.ActiveScenario.HasResults)
            WaterModel.ActiveScenario.Run();

        //
        // Trace DownStream
        //
        element.TraceDownStream(network, out nodeIds, out linkIds, WaterModel.ActiveScenario.HasResults);

        nodes = nodeIds.Select(id => id.WaterElement(WaterModel)).ToList();
        links = linkIds.Select(id => id.WaterElement(WaterModel)).ToList();

        Log.Debug($"DownStream nodes: {string.Join(", ", nodes.Select(n => n.IdLabel()))}");
        Log.Debug($"DownStream links: {string.Join(", ", links.Select(n => n.IdLabel()))}");

        //
        // Trace UpSteam
        // 
        element.TraceUpStream(network, out nodeIds, out linkIds, WaterModel.ActiveScenario.HasResults);

        nodes = nodeIds.Select(id => id.WaterElement(WaterModel)).ToList();
        links = linkIds.Select(id => id.WaterElement(WaterModel)).ToList();

        Log.Debug($"UpStream nodes: {string.Join(", ", nodes.Select(n => n.IdLabel()))}");
        Log.Debug($"UpStream links: {string.Join(", ", links.Select(n => n.IdLabel()))}");


        //
        // Connected Adjacent Nodes (starting from node)
        // 
        var nodeId = 140; // Example 5
        var node = nodeId.WaterElement(WaterModel);
        var connectedNodes = node.ConnectedAdjacentNodes(WaterModel);
        Log.Debug($"Connected adjacent nodes from {node.IdLabel()}: {string.Join(", ", connectedNodes.Select(n => n.IdLabel()))}");

        var connectedNodesByExtraLevel1 = node.ConnectedAdjacentNodes(WaterModel, 2);
        Log.Debug($"Connected adjacent + 1 depth nodes from {node.IdLabel()}: {string.Join(", ", connectedNodesByExtraLevel1.Select(n => n.IdLabel()))}");


        //
        // Connected Adjacent Links (starting from link)
        // 
        var linkId = 452; // Example 5
        var link = linkId.WaterElement(WaterModel);
        var connectedLinks = link.ConnectedAdjacentLinks(WaterModel);
        Log.Debug($"Connected adjacent links from {link.IdLabel()}: {string.Join(", ", connectedLinks.Select(n => n.IdLabel()))}");

        var connectedLinksByExtraLevel1 = link.ConnectedAdjacentLinks(WaterModel, 2);
        Log.Debug($"Connected adjacent + 1 depth links from {link.IdLabel()}: {string.Join(", ", connectedLinksByExtraLevel1.Select(n => n.IdLabel()))}");

    }
    #endregion
}
