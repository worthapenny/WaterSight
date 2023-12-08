using OpenFlows.Domain.ModelingElements;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System.Collections.Generic;

namespace WaterSight.Model.Support;


public static class OFWComparer
{
    public static ElementComparer<IWaterElement> WaterElement = waterElementComparer;
    static ElementComparer<IWaterElement> waterElementComparer = new ElementComparer<IWaterElement>();


    public static ElementComparer<ISCADAElement> SCADAElement = scadaElementComparer;
    static ElementComparer<ISCADAElement> scadaElementComparer = new ElementComparer<ISCADAElement>();
}

public class ElementComparer<T> : IEqualityComparer<T> where T : IElement
{    
    public bool Equals(T element1, T element2)
    {
        if (ReferenceEquals(element1, element2))
        {
            return true;
        }
        if (ReferenceEquals(element1, null) ||
            ReferenceEquals(element2, null))
        {
            return false;
        }
        return element1.Id == element2.Id;
    }

    public int GetHashCode(T element)
    {
        return element == null ? 0 : element.Id.GetHashCode();
    }
}
