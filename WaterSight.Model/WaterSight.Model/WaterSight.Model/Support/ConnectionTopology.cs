using Haestad.Support.Support;

namespace WaterSight.Model.Support;

public class ConnectionTopology
{    
    public IHmIDToObjectDictionary LinkIdToStartNodeIdMap { get; }
    public  IHmIDToObjectDictionary LinkIdToStopNodeIdMap { get; }
    public  IHmIDToObjectDictionary NodeIdToAttachedLinkIdsMap { get; }
}
