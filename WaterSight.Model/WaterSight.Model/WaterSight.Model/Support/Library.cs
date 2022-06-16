using Haestad.Drawing.Domain;
using Haestad.Framework.Application;
using Haestad.Support.Support;
using System.Threading.Tasks;

namespace WaterSight.Model.Support;

public static class Library
{
    public static async Task<ConnectionTopology> GetConnectivityAsync(IDomainProject domainProject)
    {
        var connectivity = new ConnectionTopology();

        await Task.Run(() => {
            IHmIDToObjectDictionary linkIdToStartNodeIdMap = null;
            IHmIDToObjectDictionary linkIdToStopnodeIdMap = null;
            IHmIDToObjectDictionary nodeIdToAttachedLinkIdsmap = null;


            QuerySupportLibrary.GetTopologyForAllNodesAndLinks(
                domainProject,
                new HmIDCollection { },
                out linkIdToStartNodeIdMap,
                out linkIdToStopnodeIdMap,
                out nodeIdToAttachedLinkIdsmap);
        });

        return connectivity;
    }
}
