using OpenFlows.Water.Domain.ModelingElements.Components;
using System.Diagnostics;
using WaterSight.Model.Extensions;

namespace WaterSight.Model.Support.Data;

[DebuggerDisplay ("ToString()")]
public class UnitDemandWithPattern
{
    #region Constructor
    public UnitDemandWithPattern()
    {
    }

    public UnitDemandWithPattern(IUnitDemandLoad unitDemand, double numberOfUnitDemands, IPattern pattern)
    {
        UnitDemand = unitDemand;
        NumberOfUnitDemands = numberOfUnitDemands;
        Pattern = pattern;
    }

    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"Unit Demand Type: {UnitDemand.UnitDemandType}, # Demand: {NumberOfUnitDemands},  Pattern: {Pattern.IdLabel()}";
    }
    #endregion

    #region Public Properties
    public IUnitDemandLoad UnitDemand { get; }
    public double NumberOfUnitDemands { get; set; }
    public IPattern Pattern { get; set; }
    #endregion
}
