
using OpenFlows.Water.Domain.ModelingElements.Components;
using System.Diagnostics;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Support.Data;

[DebuggerDisplay("{ToString()}")]
public class DemandWithPattern
{
    #region Constructor
    public DemandWithPattern()
    {        
    }
    public DemandWithPattern(double baseDemand, IPattern pattern)
    {
        Demand = baseDemand;
        Pattern = pattern;
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Demand: {Demand}, Pattern: {Pattern.IdLabel}";
    }
    #endregion

    #region Public Properties
    public double Demand { get; set; }
    public IPattern Pattern { get; set; }
    #endregion
}
