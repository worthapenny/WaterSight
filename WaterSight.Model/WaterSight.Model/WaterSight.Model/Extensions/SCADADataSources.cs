using Haestad.Domain;
using OpenFlows.Water.Domain;
using OpenFlows.Water.Domain.ModelingElements.Components;
using System.Collections.Generic;
using WaterSight.Model.Domain.Scada;
namespace WaterSight.Model.Extensions;

#region Extensions
public static class SCADADataSourceConnectionExtensions
{

    public static List<ISCADADataSourceHistorical> SCADADataSources(this IWaterModelSupport _, IWaterModel waterModel)
    {
        var list = new List<ISCADADataSourceHistorical>();
        foreach (var dataSourceId in waterModel.DomainDataSet.SupportElementManager((int)SupportElementType.ScadaDataSource).ElementIDs())
            list.Add(new SCADADataSource(waterModel, dataSourceId));

        return list;
    }

    public static ISCADADataSourceHistorical SCADADataSource(this IWaterModelSupport _, IWaterModel waterModel)
    {
        return new SCADADataSource(waterModel);
    }
}
#endregion
