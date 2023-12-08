using Haestad.Drawing.Domain;
using Haestad.Framework.Application;
using Haestad.Support.Support;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WaterSight.Model.Support.Network;

[DebuggerDisplay("{ToString()}")]
public class ConnectionTopology
{
    #region Constructor
    public ConnectionTopology()
    {
    }

    public ConnectionTopology(
        IHmIDToObjectDictionary linkIdToStartNodeIdMap,
        IHmIDToObjectDictionary linkIdToStopNodeIdMap,
        IHmIDToObjectDictionary nodeIdToAttachedLinkIdsMap)
    {
        LinkIdToStartNodeIdMap = linkIdToStartNodeIdMap;
        LinkIdToStopNodeIdMap = linkIdToStopNodeIdMap;
        NodeIdToAttachedLinkIdsMap = nodeIdToAttachedLinkIdsMap;
    }

    #endregion

    #region Static Methods
    public static async Task<ConnectionTopology> GetConnectivityAsync(IDomainProject domainProject)
    {
        IHmIDToObjectDictionary linkIdToStartNodeIdMap = null;
        IHmIDToObjectDictionary linkIdToStopNodeIdMap = null;
        IHmIDToObjectDictionary nodeIdToAttachedLinkIdsMap = null;

        //await Task.Run(() =>
        //{
            QuerySupportLibrary.GetTopologyForAllNodesAndLinks(
                domainProject,
                QuerySupportLibrary.GetLinkElementTypeIDs(domainProject),
                out linkIdToStartNodeIdMap,
                out linkIdToStopNodeIdMap,
                out nodeIdToAttachedLinkIdsMap);
        //});


        var connectivity = new ConnectionTopology(
            linkIdToStartNodeIdMap: linkIdToStartNodeIdMap,
            linkIdToStopNodeIdMap: linkIdToStopNodeIdMap,
            nodeIdToAttachedLinkIdsMap: nodeIdToAttachedLinkIdsMap
            );

        return connectivity;
    }

    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"StartNodeMapCount: {LinkIdToStartNodeIdMap.Count}, StopNodeMapCount: {LinkIdToStopNodeIdMap.Count}, NodeToAttachedLinkMapCount: {NodeIdToAttachedLinkIdsMap.Count}";
    }
    #endregion

    #region Public Properties

    public IHmIDToObjectDictionary LinkIdToStartNodeIdMap { get; }
    public IHmIDToObjectDictionary LinkIdToStopNodeIdMap { get; }
    public IHmIDToObjectDictionary NodeIdToAttachedLinkIdsMap { get; }
    #endregion

}
