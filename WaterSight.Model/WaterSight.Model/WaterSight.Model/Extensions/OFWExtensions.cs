using Haestad.Domain;
using Haestad.Support.Library;
using WaterSight.Model.Domain;
using OpenFlows.Domain.ModelingElements;
using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenFlows.Water.Domain.ModelingElements.Components;
using Haestad.Network;
using Haestad.Network.Algorithm;
using Haestad.Support.Support;
using System.IO;
using WaterSight.Model.Support;
using System.Diagnostics;
using Serilog;
using System.Drawing.Printing;
using Haestad.Drawing.Domain;
using Haestad.Framework.Support;
using Haestad.Support.Units;
using System.Xml.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace WaterSight.Model.Extensions;

//public static class OFWExtensions
//{


//    #region IWaterModel Related
//    public static Curves Curves(this IWaterModel model)
//    {
//        return new Curves(model);
//    }
//    public static bool DeleteDwhFile(this IWaterModel waterModel)
//    {
//        var dwhFilePath = $"{waterModel.ModelInfo.Filename}.dwh";
//        if (File.Exists(dwhFilePath))
//            File.Delete(dwhFilePath);

//        return !File.Exists(dwhFilePath);
//    }
//    #endregion

//    #region IWaterNetwork




//    public static async Task<List<IWaterElement>> IncomingLinksAsync(this IWaterNetwork network, IWaterModel waterModel, IWaterElement element)
//    {
//        var incomingIds = new List<IWaterElement>();
//        //await Task.Run(() =>
//        //{
//        if (!(element is IBaseLinkInput))
//        {
//            var ids = (waterModel.DomainDataSet as IDomainDataSetSearch)?.GetIncomingLinkIDsToNode(element.Id);
//            incomingIds.AddRange(from id in ids?.ToArray()
//                                 select waterModel.Element(id) as IWaterElement);
//        }
//        //});

//        return incomingIds;
//    }
//    #endregion

//    #region IDirectoredNode Related
//    public static IWaterElement UpstreamLink(this IBaseDirectedNodeInput element, IWaterModel waterModel)
//    {
//        var downStreamLink = element.DownstreamLink;
//        var connectedLinks = ((IWaterElement)element).ConnectedAdjacentElements(waterModel);
//        connectedLinks.RemoveAll(l => l.Id == downStreamLink.Id);
//        return connectedLinks.First();
//    }
//    public static void ConnectedUpAndDownElements(
//        this IBaseDirectedNodeInput element,
//        IWaterModel waterModel,
//        out IWaterElement upLink,
//        out IWaterElement upNode,
//        out IWaterElement downLink,
//        out IWaterElement downNode)
//    {
//        var downStreamLink = (IWaterElement)element.DownstreamLink;
//        var connectedLinks = ((IWaterElement)element).ConnectedAdjacentElements(waterModel);
//        connectedLinks.RemoveAll(l => l.Id == downStreamLink.Id);
//        upLink = connectedLinks.FirstOrDefault();
//        downLink = downStreamLink;

//        var elementAndDownNode = downStreamLink.ConnectedAdjacentNodes(waterModel);
//        elementAndDownNode.RemoveAll(n => n.Id == ((IElement)element).Id);
//        downNode = elementAndDownNode.FirstOrDefault();

//        var eitherNodes = ((IWaterElement)element).ConnectedAdjacentNodes(waterModel);
//        eitherNodes.RemoveAll(n => n.Id == elementAndDownNode.FirstOrDefault().Id);
//        upNode = eitherNodes.First();
//    }
//    #endregion

//    #region IBaseLinkInput Related
//    public static double SlopeAngle(this IBaseLinkInput linkInput, out double degrees)
//    {
//        var points = linkInput.GetPoints();
//        var firstPoint = points.First();
//        var lastPoint = points.Last();
//        degrees = MathLibrary.CalculateAngleOfLine(firstPoint, lastPoint);
//        var slopeAngle = degrees * Math.PI / 180;
//        return slopeAngle;
//    }
//    public static GeometryPoint MidPoint(this IBaseLinkInput linkInput)
//    {
//        var points = linkInput.GetPoints();

//        return MathLibrary.GetPointAtDistanceIntoPolyline(
//            points: points.ToArray(),
//            distance: linkInput.Length / 2,
//            segmentNumber: out var _);
//    }
//    #endregion

//    #region Components
//    public static ISupportElement? GetDataSouce(this IWaterModelSupport component, string label, IWaterModel waterModel)
//    {
//        var dds = waterModel.DomainDataSet;
//        var manager = dds.SupportElementManager((int)SupportElementType.ScadaDataSource);

//        var idCheck = manager.Elements().Cast<ISupportElement>().Where(e => e.Label == label);
//        return idCheck.Any() ? idCheck.First() : null;
//    }
//    #endregion

//    #region ISCADASignal
//    public static string TagForWaterSight(this ISCADASignal signal)
//    {
//        if (signal != null)
//            return signal.IsDerived ? signal.Label : signal.SignalLabel;

//        return string.Empty;
//    }
//    #endregion

//    #region IWaterElement

//    public static List<IWaterElement> ConnectedAdjacentElements(this IWaterElement element, IWaterModel waterModel)
//    {
//        var dds = waterModel.DomainDataSet as IDomainDataSetSearch;
//        var elements = new List<IWaterElement>();

//        // for links it's easy
//        if (element is IBaseLinkInput)
//        {
//            var link = element as IBaseLinkInput;
//            elements.Add(link.StartNode as IWaterElement);
//            elements.Add(link.StopNode as IWaterElement);
//        }

//        // for nodes
//        else
//        {
//            // Incoming links
//            foreach (var linkId in dds.GetIncomingLinkIDsToNode(element.Id))
//            {
//                var incomingLink = waterModel.Element(linkId);
//                if (!elements.Contains(incomingLink))
//                    elements.Add(incomingLink as IWaterElement);
//            }

//            // Outgoing links
//            foreach (var linkId in dds.GetOutcomingLinkIDsFromNode(element.Id))
//            {
//                var outgoingLink = waterModel.Element(linkId);
//                if (!elements.Contains(outgoingLink))
//                    elements.Add(outgoingLink as IWaterElement);
//            }
//        }

//        return elements;
//    }
//    public static List<IWaterElement> ConnectedAdjacentNodes(this IWaterElement node, IWaterModel waterModel)
//    {
//        var connectedElements = node.ConnectedAdjacentElements(waterModel);

//        // for pipes it's easy
//        if (node is IBaseLinkInput)
//            return connectedElements;

//        // for nodes, the connected elements will be pipes
//        // look for all connected nodes
//        var connectedNodes = new List<IWaterElement>();
//        foreach (IBaseLinkInput link in connectedElements)
//        {
//            connectedNodes.Add((IWaterElement)link.StartNode);
//            connectedNodes.Add((IWaterElement)link.StopNode);
//        }

//        connectedNodes.RemoveAll(n => n.Id == node.Id);
//        return connectedNodes;
//    }

//    public static List<IWaterElement> ConnectedAdjacentNodes(this IWaterElement node, IWaterModel waterModel, int depth)
//    {
//        var nodes = new List<IWaterElement>();
//        nodes.AddRange(ConnectedAdjacentNodes(node, waterModel));
//        for (int i = 1; i < depth; i++)
//        {
//            var newNodes = new List<IWaterElement>();
//            foreach (var newNode in nodes)
//                newNodes.AddRange(ConnectedAdjacentNodes(newNode, waterModel));

//            nodes.AddRange(newNodes);
//        }

//        return nodes.Unique().ToList();
//    }

//    public static List<IWaterElement> ConnectedAdjacentLinks(this IWaterElement link, IWaterModel waterModel)
//    {
//        var startNode = (IWaterElement)((IBaseLinkInput)link).StartNode;
//        var startNodeLinks = startNode.ConnectedAdjacentElements(waterModel);

//        var stopNode = (IWaterElement)((IBaseLinkInput)link).StopNode;
//        var stopNodeLinks = stopNode.ConnectedAdjacentElements(waterModel);

//        var connectedLinks = new List<IWaterElement>();
//        connectedLinks.AddRange(startNodeLinks);
//        connectedLinks.AddRange(stopNodeLinks);

//        connectedLinks.RemoveAll(l => l.Id == link.Id);
//        return connectedLinks;
//    }

//    public static List<IWaterElement> ConnectedAdjacentLinks(this IWaterElement link, IWaterModel waterModel, int depth)
//    {
//        var links = new List<IWaterElement>();
//        links.AddRange(ConnectedAdjacentLinks(link, waterModel));
//        for (int i = 1; i < depth; i++)
//        {
//            var newLinks = new List<IWaterElement>();
//            foreach (var newLink in links)
//                newLinks.AddRange(ConnectedAdjacentLinks(newLink, waterModel));

//            links.AddRange(newLinks);
//        }

//        return links.Unique().ToList();
//    }

//    public static IEnumerable<ISCADAElement> ConnectedSCADAElements(this IWaterElement element, IWaterModel waterModel)
//    {
//        var filters = new FilterContextCollection
//            {
//                {
//                    waterModel.Network.SCADAElements.InputFields.FieldByLabel("Model Element").Field,
//                    ComparisonOperator.EqualTo,
//                    element.Id
//                }
//            };

//        return waterModel.Network.SCADAElements.SelectElements(new SortContextCollection(), filters);
//    }
//    public static IEnumerable<ISCADAElement> ConnectedSCADAElements(
//        this IWaterElement element,
//        IWaterModel waterModel,
//        IEnumerable<string> attributes)
//    {
//        var ses = ConnectedSCADAElements(element, waterModel);
//        return ses.Where(se =>
//        {
//            var attribute = se.Input.TargetAttribute.ToString().ToLower();
//            return attributes.Contains(attribute)
//                && se.Input.HistoricalSignal != null
//                && se.Input.IsActive;
//        });
//    }
//    public static IEnumerable<ISCADAElement> ConnectedSCADAElementsFlowType(
//        this IWaterElement element,
//        IWaterModel waterModel)
//    {
//        var ses = ConnectedSCADAElements(element, waterModel, new[] { "flow", "discharge" });
//        return ses;
//    }
//    public static IEnumerable<ISCADAElement> ConnectedSCADAElementsPressureLevelOrGradeType(this IWaterElement element, IWaterModel waterModel)
//    {
//        var ses = ConnectedSCADAElements(element, waterModel, new[] { "pressure", "tanklevel", "hydraulicgrade" });
//        return ses;
//    }
//    public static IEnumerable<ISCADAElement> ConnectedSCADAElementsPressureType(this IWaterElement element, IWaterModel waterModel)
//    {
//        var ses = ConnectedSCADAElements(element, waterModel, new[] { "pressure" });
//        return ses;
//    }
//    //public static void TraceConnected(
//    //    this IWaterElement element,
//    //    HmiNetwork network,
//    //    IWaterModel waterModel,
//    //    out List<IWaterElement> outNodes,
//    //    out List<IWaterElement> outLinks,
//    //    int limitElementsTo = 10)
//    //{
//    //    if (element is not IPointNodeInput)
//    //    {
//    //        Debugger.Break();
//    //        throw new ArgumentException($"Given {element} must be of node type");
//    //    }

//    //    var dfsSearch = new HmiDfsSearch(network);
//    //    dfsSearch.StartVertex = element.Id;
//    //    dfsSearch.ScanWholeGraph = false;
//    //    dfsSearch.StoreNonTreeEdges = true;
//    //    dfsSearch.Run();

//    //    var nodeIds = dfsSearch.GetSearchedVertexes().GetEnumerator();
//    //    var linkIds = dfsSearch.GetSearchedEdges().GetEnumerator();

//    //    outNodes = new List<IWaterElement>();
//    //    outLinks = new List<IWaterElement>();

//    //    for (int i = 0; i < limitElementsTo; i++)
//    //    {
//    //        if (nodeIds.MoveNext())
//    //            outNodes.Add((IWaterElement)waterModel.Element(nodeIds.Current));

//    //        if (linkIds.MoveNext())
//    //            outLinks.Add((IWaterElement)waterModel.Element(linkIds.Current));

//    //    }

//    //}

//    //public static void TraceConnectedMagnetically(
//    //    this IWaterElement element,
//    //    IWaterModel waterModel,
//    //    out List<IWaterElement> outNodesA,
//    //    out List<IWaterElement> outLinksA,
//    //    int limitElementsTo = 10)
//    //{
//    //    var outNodes = new List<IWaterElement>();
//    //    var outLinks = new List<IWaterElement>();

//    //    if (limitElementsTo <= 0)
//    //    {
//    //        outNodesA = new List<IWaterElement>();
//    //        outLinksA = new List<IWaterElement>();
//    //        return;
//    //    }

//    //    if (element is IPointNodeInput)
//    //    {
//    //        var connectedLinks = element.ConnectedAdjacentElements(waterModel);
//    //        outLinks.AddRange(connectedLinks);

//    //        limitElementsTo--;
//    //        foreach (var link in connectedLinks)
//    //        {
//    //            TraceConnectedMagnetically(link, waterModel, out outNodes, out outLinks, limitElementsTo);
//    //            outLinks.AddRange(outLinks);
//    //            outNodes.AddRange(outNodes);
//    //        }
//    //    }
//    //    else if (element is IBaseLinkInput)
//    //    {
//    //        var connectedNodes = element.ConnectedAdjacentElements(waterModel);
//    //        outNodes.AddRange(connectedNodes);

//    //        limitElementsTo--;
//    //        foreach (var node in connectedNodes)
//    //        {
//    //            TraceConnectedMagnetically(node, waterModel, out outNodes, out outLinks, limitElementsTo);
//    //            outLinks.AddRange(outLinks);
//    //            outNodes.AddRange(outNodes);
//    //        }
//    //    }

//    //    outNodesA = outNodesA == null ? new List<IWaterElement>() : outNodes;
//    //    outLinksA = outLinks == null ? new List<IWaterElement>(): outLinks;
//    //}

//    //public void test(IWaterModel waterModel)
//    //{
//    //    var queryParam = new QueryParameter("TraceUpStream", "TraceUpStream", typeof(int), Unit.None, "",  )
//    //    var query = new PredefinedQueries();
//    //    query.ExecuteQuery(
//    //         Haestad.Mapping.DrawingQueryType.FindTraceUpstream,
//    //         null,
//    //         waterModel.ActiveScenario.Id,
//    //         new IQueryParameter[] { },
//    //         null, // message handler
//    //         null, // progress indicator
//    //         out string errorMessage);
//    //}

//    public static void TraceUpStream(this IWaterElement element, HmiNetwork network, out List<int> nodeIds, out List<int> linkIds, bool hasResults)
//    {

//        nodeIds = new List<int>();
//        linkIds = new List<int>();

//        HmIDCollection nodes;
//        HmIDCollection links; ;


//        if (hasResults)
//        {
//            var tracerResult = new HmiFlowResultsUpstreamSearch(network);
//            tracerResult.StartNode = element.Id;
//            tracerResult.Run();
//            nodes = tracerResult.UpstreamNodes;
//            links = tracerResult.UpstreamPipes;
//        }
//        else
//        {
//            var tracer = new HmiWaterUpstreamSearch(network);
//            tracer.StartNode = element.Id;
//            tracer.Run();
//            nodes = tracer.UpstreamNodes;
//            links = tracer.UpstreamPipes;
//        }

//        for (int i = 0; i < nodes.Count; i++)
//            nodeIds.Add(nodes[i]);

//        for (int i = 0; i < links.Count; i++)
//            linkIds.Add(links[i]);
//    }
//    public static void TraceDownStream(this IWaterElement element, HmiNetwork network, out List<int> nodeIds, out List<int> linkIds, bool hasResults)
//    {
//        nodeIds = new List<int>();
//        linkIds = new List<int>();

//        HmIDCollection nodes;
//        HmIDCollection links;

//        if (hasResults)
//        {
//            var tracerResult = new HmiFlowResultsDownstreamSearch(network);
//            tracerResult.StartNode = element.Id;
//            tracerResult.Run();
//            nodes = tracerResult.DownstreamNodes;
//            links = tracerResult.DownstreamPipes;
//        }
//        else
//        {
//            var tracer = new HmiWaterDownstreamSearch(network);
//            tracer.StartNode = element.Id;
//            tracer.Run();
//            nodes = tracer.DownstreamNodes;
//            links = tracer.DownstreamPipes;
//        }

//        for (int i = 0; i < nodes.Count; i++)
//            nodeIds.Add(nodes[i]);

//        for (int i = 0; i < links.Count; i++)
//            linkIds.Add(links[i]);
//    }


//    //public static void TraceConnectedNodes(
//    //    this IWaterElement element,
//    //    IWaterModel waterModel,
//    //    List<IWaterElement> connectedNodes)
//    //{

//    //    var newNodes = element.ConnectedAdjacentNodes(waterModel);
//    //    connectedNodes.AddRange(newNodes);

//    //    foreach (var node in newNodes)
//    //        node.TraceConnectedNodes(waterModel, connectedNodes);

//    //}



//    //public static void TraceConnectedLinks(
//    // this IWaterElement element,
//    // IWaterModel waterModel,
//    // List<IWaterElement> connectedLinks,
//    // int depth = 5)
//    //{
//    //    if (depth <= 0) return;

//    //    var newLinks = element.ConnectedAdjacentLinks(waterModel);
//    //    connectedLinks.AddRange(newLinks);

//    //    foreach (var links in newLinks)
//    //        links.TraceConnectedNodes(waterModel);

//    //}

//    //public static void TraceConnectedElements(
//    //    this IWaterElement element,
//    //    IWaterModel waterModel,
//    //    List<IWaterElement> connectedElements,
//    //    int depth = 5)
//    //{
//    //    if (depth <= 0) return;

//    //    var newElements = element.ConnectedAdjacentElements(waterModel);
//    //    connectedElements.AddRange(newElements);

//    //    foreach (var newElement in newElements)
//    //        newElement.TraceConnectedElements(waterModel, connectedElements, depth - 1);
//    //}


//    public static GeometryPoint GetMidCenterPoint(this IWaterElement element)
//    {
//        if (element is IBaseLinkInput)
//            return (element as IBaseLinkInput).MidPoint();

//        if (element is IBasePolygonInput)
//            return MathLibrary.CalculatePolygonCenterPoint(
//                (element as IBasePolygonInput).GetRings()[0]);

//        if (element is IPointNodeInput)
//            return (element as IPointNodeInput).GetPoint();

//        return new GeometryPoint();
//    }
//    #endregion

//    #region IElement
//    public static string IdLabel(this IElement element, bool quoted = false, char quote = '"')
//    {
//        if (!quoted)
//            return $"{element.Id}: {element.Label}";
//        else
//            return $"{quote}{element.Id}: {element.Label}{quote}";
//    }
//    public static IWaterElement WaterElement(this IElement element)
//    {
//        return (IWaterElement)element;
//    }
//    #endregion

//    #region ICustomerMeter
//    public static IZone? Zone(this ICustomerMeter cm)
//    {
//        return ((IWaterZoneableNetworkElementInput)cm.Input.AssociatedElement)?.Zone;
//    }
//    public static Dictionary<int, List<ICustomerMeter>> ZoneSummary(this ICustomerMeters _, IWaterNetwork waterNetwork)
//    {
//        var map = waterNetwork.CustomerMeters.Elements()
//            .GroupBy(cm => cm.Zone()==null? -1: cm.Zone().Id)
//            .ToDictionary(g => g.Key, g=>g.ToList());

//        return map;
//    }

//    #endregion

//    #region Int
//    public static IWaterElement WaterElement(this int id, IWaterModel waterModel)
//    {
//        return (IWaterElement)waterModel.Element(id);
//    }
//    #endregion
//}