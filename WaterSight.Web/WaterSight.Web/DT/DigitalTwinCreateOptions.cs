using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.Web.DT;

public class DigitalTwinWaterCreateOptions : DigitalTwinCreateOptions
{
    #region Constructor
    public DigitalTwinWaterCreateOptions(string digitalTwinName) : base(DigitalTwinType.Water)
    {
        DigitalTwinName = digitalTwinName;
    }
    #endregion

    #region Public Properties
    public override string DigitalTwinName { get; protected set; }

    #endregion
}

public class DigitalTwinSewerCreateOptions : DigitalTwinCreateOptions
{
    #region Constructor
    public DigitalTwinSewerCreateOptions(string digitalTwinName) : base(DigitalTwinType.Sewer)
    {
        DigitalTwinName= digitalTwinName;
    }
    #endregion

    #region Protected Methods
    //protected override void AddGoals(DigitalTwinType digitalTwinType)
    //{
    //    base.AddGoals(digitalTwinType);
    //}
    #endregion

    #region Public Properties
    public override string DigitalTwinName { get; protected set; }
    #endregion

}


[DebuggerDisplay("{ToString()}")]
public abstract class DigitalTwinCreateOptions
{
    #region Constants
    public const string GoalsCapitalPlanning = "Capital Planning";
    public const string GoalsDistributionSystems = "Distribution Systems";
    public const string GoalsTransmissionMains = "Transmission Mains";
    public const string GoalsStormWater = "Storm Water";
    public const string GoalsSewerDrainage = "Sewer Drainage";
    public const string GoalsFloodWarning = "Flood Warning";

    private const string DigitalTwinTypeWater = "Water Supply";
    private const string DigitalTwinTypeSewer = "Water Collection";
    #endregion

    #region Constructor
    public DigitalTwinCreateOptions(DigitalTwinType digitalTwinType)
    {
        DigitalTwinType = digitalTwinType;
        SetDigitalTwinTypeName(digitalTwinType);
        AddGoals(digitalTwinType);
        
    }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{DigitalTwinName} [{DigitalTwinType}: {DigitalTwinTypeName}]";
    }
    #endregion

    #region Private Methods
    private void SetDigitalTwinTypeName(DigitalTwinType digitalTwinType)
    {
        switch (digitalTwinType)
        {
            case DigitalTwinType.Water:
                DigitalTwinTypeName = DigitalTwinTypeWater;
                break;

            case DigitalTwinType.Sewer:
                DigitalTwinTypeName = DigitalTwinTypeSewer;
                break;

            default:
                break;
        }
    }
    protected virtual void AddGoals(DigitalTwinType digitalTwinType)
    {
        Goals.Clear();

        switch (digitalTwinType)
        {
            case DigitalTwinType.Water:
                //Goals.Add(GoalsCapitalPlanning);
                Goals.Add(GoalsDistributionSystems);
                //Goals.Add(GoalsTransmissionMains);
                break;

            case DigitalTwinType.Sewer:
                //Goals.Add(GoalsCapitalPlanning);
                //Goals.Add(GoalsStormWater);
                Goals.Add(GoalsSewerDrainage);
                //Goals.Add(GoalsFloodWarning);
                break;

            default:
                break;
        }
    }
    #endregion

    #region Public Properties
    [JsonIgnore]
    public abstract string DigitalTwinName { get;  protected set; }
    public DigitalTwinType DigitalTwinType { get; set; } = DigitalTwinType.Water;
    public List<string> Goals { get; set; } = new List<string>();

    [JsonProperty("TypeDisplayName")]
    public string DigitalTwinTypeName { get;  set; }
    #endregion
}
