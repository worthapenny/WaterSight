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

namespace WaterSight.Model.Extensions;

public static class OFWExtensions
{

    #region IWaterModel
    public static Curves Curves(this IWaterModel model)
    {
        return new Curves(model);
    }
    
    #endregion

    #region IWaterNetwork

    public static List<IElement> ConnectedElements(this IWaterNetwork network, IWaterModel waterModel, IWaterElement element)
    {
        var dds = waterModel.DomainDataSet as IDomainDataSetSearch;
        var elements = new List<IElement>();

        // for links it's easy
        if (element is IBaseLinkInput)
        {
            var link = element as IBaseLinkInput;
            elements.Add(link.StartNode);
            elements.Add(link.StopNode);
        }

        // for nodes
        else
        {
            // Incoming links
            foreach (var linkId in dds.GetIncomingLinkIDsToNode(element.Id))
            {
                var incomingLink = waterModel.Element(linkId);
                if (!elements.Contains(incomingLink))
                    elements.Add(incomingLink);
            }

            // Outgoing links
            foreach (var linkId in dds.GetOutcomingLinkIDsFromNode(element.Id))
            {
                var outgoingLink = waterModel.Element(linkId);
                if (!elements.Contains(outgoingLink))
                    elements.Add(outgoingLink);
            }
        }

        return elements;
    }

    public static async Task<List<IWaterElement>> IncomingLinksAsync(this IWaterNetwork network, IWaterModel waterModel, IWaterElement element)
    {
        var incomingIds = new List<IWaterElement>();
        await Task.Run(() =>
        {
            if (!(element is IBaseLinkInput))
            {
                var ids = (waterModel.DomainDataSet as IDomainDataSetSearch)?.GetIncomingLinkIDsToNode(element.Id);
                incomingIds.AddRange(from id in ids?.ToArray()
                                     select waterModel.Element(id) as IWaterElement);
            }
        });

        return incomingIds;
    }
    #endregion

    #region Components
    public static ISupportElement? GetDataSouce(this IWaterModelSupport component, string label, IWaterModel waterModel)
    {
        var dds = waterModel.DomainDataSet;
        var manager = dds.SupportElementManager((int)SupportElementType.ScadaDataSource);

        var idCheck = manager.Elements().Cast<ISupportElement>().Where(e => e.Label == label);
        return idCheck.Any() ? idCheck.First():  null;
    }
    #endregion

    #region ISCADASignal
    public static string TagForWaterSight(this ISCADASignal signal)
    {
        if (signal != null)
            return signal.IsDerived ? signal.Label : signal.SignalLabel;

        return string.Empty;
    }
    #endregion

    #region IWaterElement
    public static IEnumerable<ISCADAElement> GetConnectedSCADAElements(this IWaterElement element, IWaterModel waterModel)
    {
        //return waterNetwork.SCADAElements.Elements().Where(se =>
        //    se.Input.TargetElement.Id == element.Id);
        var filters = new FilterContextCollection
            {
                {
                    waterModel.Network.SCADAElements.InputFields.FieldByLabel("Model Element").Field,
                    ComparisonOperator.EqualTo,
                    element.Id
                }
            };

        return waterModel.Network.SCADAElements.SelectElements(new SortContextCollection(), filters);

    }
    public static void TraceUpStream(this IWaterElement element, HmiNetwork network, out List<int> outNodes, out List<int> outLinks)
    {
        if (element is IBaseLinkInput)
            throw new ArgumentException($"Given element is of wrong type '{element.WaterElementType}'. Only node type elements are supported");

        outNodes = new List<int>();
        outLinks = new List<int>();
        
        var tracer = new HmiWaterUpstreamSearch(network);
        tracer.StartNode = element.Id;
        tracer.Run();

        var nodes = tracer.UpstreamNodes;
        for (int i = 0; i < nodes.Count; i++)
            outNodes.Add(nodes[i]);

        var links = tracer.UpstreamPipes;
        for (int i = 0; i < links.Count; i++)
            outLinks.Add(links[i]);
    }
    public static void TraceDownStream(this IWaterElement element, HmiNetwork network, out List<int> outNodes, out List<int> outLinks)
    {
        if (element is IBaseLinkInput)
            throw new ArgumentException($"Given element is of wrong type '{element.WaterElementType}'. Only node type elements are supported");

        outNodes = new List<int>();
        outLinks = new List<int>();

        var tracer = new HmiWaterDownstreamSearch(network);
        tracer.StartNode = element.Id;
        tracer.Run();

        var nodes = tracer.DownstreamNodes;
        for (int i = 0; i < nodes.Count; i++)
            outNodes.Add(nodes[i]);

        var links = tracer.DownstreamPipes;
        for (int i = 0; i < links.Count; i++)
            outLinks.Add(links[i]);
    }
    public static GeometryPoint GetMidCenterPoint(this IWaterElement element)
    {
        if (element is IBaseLinkInput)
            return (element as IBaseLinkInput).MidPoint();

        if (element is IBasePolygonInput)
            return MathLibrary.CalculatePolygonCenterPoint(
                (element as IBasePolygonInput).GetRings()[0]);

        if (element is IPointNodeInput)
            return (element as IPointNodeInput).GetPoint();

        return new GeometryPoint();
    }
    #endregion

    #region IBaseLinkInput
    public static double SlopeAngle(this IBaseLinkInput linkInput)
    {
        var points = linkInput.GetPoints();
        var firstPoint = points.First();
        var lastPoint = points.Last();
        var slopeAngle = MathLibrary.CalculateAngleOfLine(firstPoint, lastPoint);
        slopeAngle = slopeAngle * Math.PI / 180;
        return slopeAngle;
    }
    public static GeometryPoint MidPoint(this IBaseLinkInput linkInput)
    {
        var points = linkInput.GetPoints();

        return MathLibrary.GetPointAtDistanceIntoPolyline(
            points: points.ToArray(),
            distance: linkInput.Length / 2,
            segmentNumber: out var _);
    }
    #endregion

    #region IElement
    public static string IdLabel(this IElement element, bool quoted = false, char quote = '"')
    {
        if (!quoted)
            return $"{element.Id}: {element.Label}";
        else
            return $"{quote}{element.Id}: {element.Label}{quote}";
    }
    #endregion
}
