using Newtonsoft.Json;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace WaterSight.Model.Generator;


public enum SupportedFileType
{
    CSV = 0,
    //Excel=1,
    SQLite = 2,
    //Access=3,
    SqlServer = 4,
}

[DebuggerDisplay("For: {Model}")]
public class SCADADataGeneratorOptions
{
    #region Public Methods
    public string ToJson(bool formatted)
    {
        var indented = formatted ? Formatting.Indented : Formatting.None;
        return JsonConvert.SerializeObject(this, indented);
    }
    #endregion

    public Model Model { get; set; } = new Model();

    public int RandomSeed { get; set; }

    public Input Input { get; set; } = new Input();

    public Output Output { get; set; } = new Output();

    public List<SensorNode> SensorElements { get; set; } = new List<SensorNode>();

    public DemandVariability DemandVariability { get; set; } = new DemandVariability();

    public List<ChangingPumpNode> ChangingPumps { get; set; } = new List<ChangingPumpNode>();

    public MeterError MeterError { get; set; } = new MeterError();

    public List<RandomDemandNode> RandomDemandNodes { get; set; } = new List<RandomDemandNode>();

    public List<ContinualDemandNode> ContinualDemandNodes { get; set; } = new List<ContinualDemandNode>();
}

[DebuggerDisplay("[{ActiveScenarioLabel}] {ModelFilePath}")]
public class Model
{
    public string ModelFilePath { get; set; }
    public string ScenarioToUseLabel { get; set; }
    public bool SaveEachSimulation { get; set; } = false;
    public SimulationOptions SimulationOptions { get; set; } = new SimulationOptions();
}

[DebuggerDisplay("[{Start} - [End]] @ {TimeStepsHours} Hrs at {DaysPerSimulation} days")]
public class SimulationOptions
{
    public double TimeStepsHours { get; set; } = 0.25;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int DaysPerSimulation { get; set; } = 10;
}

[DebuggerDisplay("Input: {Ami}")]
public class Input
{
    public Ami Ami { get; set; } = new Ami();
}

[DebuggerDisplay("Ami Enabled: {Enable}")]
public class Ami
{
    public bool Enable { get; set; } = false;
    public AmiTsdSourceOptions DataSourceOptions { get; set; } = new AmiTsdSourceOptions();
}

[DebuggerDisplay("Source: {DataSource}, DB: {DatabaseName}, Table: {TableName}")]
public class TimeSeriesDataSourceOptions
{
    public string DataSource { get; set; } = @"localhost\sqlexpress";
    public string DatabaseName { get; set; } = "Database";
    public string TableName { get; set; } = "SCADAData";
    public string TimestampColumnName { get; set; } = "Timestamp";
    public string ValueColumnName { get; set; } = "Value";
}


public class AmiTsdSourceOptions : TimeSeriesDataSourceOptions
{
    public string CustomerIDColumnName { get; set; } = "CustomerID";
}

[DebuggerDisplay("<Output Options>")]
public class Output
{
    public TsdOutputOptions TsdOutputOptions { get; set; } = new TsdOutputOptions();
    public BillingDataOutputOptions BillingDataOutputOptions { get; set; } = new BillingDataOutputOptions();
}


[DebuggerDisplay("[{OutputFileType.ToString()}] {TableName} Path: {OutputFileName}")]
public class OutputOptions
{
    public string GetExtension()
    {
        switch (OutputFileType)
        {
            case SupportedFileType.CSV:
                return "csv";

            case SupportedFileType.SQLite:
                return "sqlite";

            default:
            case SupportedFileType.SqlServer:
                return "";
        }
    }

    [Browsable(false)]
    public SupportedFileType OutputFileType { get; set; } = SupportedFileType.SQLite;

    public string OutputFullFilePath => Path.Combine(OutputDir, $"{FileNameWithoutExt}.{GetExtension()}");

    public string OutputDir { get; set; }
    public string FileNameWithoutExt { get; set; } = "GeneratedTimeSeriesData";
    public string OptionsJsonFilePath => Path.Combine(OutputDir, $"TsdGenerationOptions.json");
    public string SCADAElementsJsonFilePath => Path.Combine(OutputDir, "SCADAElements.json");

    public bool Enable { get; set; } = true;

    public bool ClearAllData { get; set; } = true;

    public string TableName { get; set; } = "TimeSeriesData";

}

public class TsdOutputOptions : OutputOptions
{
    public TsdTable TsdTable { get; set; } = new TsdTable();
    public bool TagContainsUnit { get; set; } = false;

    public bool AddGeneratedDataSourceToSCADASignals { get; set; } = true;
    public bool CreateSCADAElements { get; set; } = true;
    public double SCADAElementLocationOffset { get; set; } = 10;
    public bool TransformPumpRawData { get; set; } = false;
    public int PumpOnValue { get; set; } = 1;
    public bool TransformValveRawData { get; set; } = false;
    public int ValveActiveValue { get; set; } = 1;
    public int ValveClosedValue { get; set; } = 0;
}


[DebuggerDisplay("{SignalColumnName}: {ValueColumnName} @ {TimestampColumnName}")]
public class TsdTable
{
    public string TimestampColumnName { get; set; } = "Timestamp";

    public string TagColumnName { get; set; } = "Tag";

    public string ValueColumnName { get; set; } = "Value";
}


public class BillingDataOutputOptions : OutputOptions
{
    public BillingDataOutputOptions()
    {
        OutputFileType = SupportedFileType.CSV;
        TableName = "BillingData";
    }

    public BillingDataOutputTable BillingDataOutputTable { get; set; } = new BillingDataOutputTable();
}

public class BillingDataOutputTable /*: OutputTable*/
{
    public string CustomerIDColumnName { get; set; } = "ID";

    public string BillMonthColumnName { get; set; } = "BillingMonth";

    public string BillVolumeColumnName { get; set; } = "Volume";

    public string BillUnitsColumnName { get; set; } = "Units";

    public string BillZoneColumnName { get; set; } = "Zone";
}


[DebuggerDisplay("Id: {Id}, Outage: {OutagePercentChance} %, OutageDuration: {OutageDurationHours} Hrs, OratageVariabiligy: {OutageDurationPercentVariability} %")]
public class SensorNode
{
    #region Constructor
    public SensorNode()
    {
    }
    public SensorNode(int id, SCADATargetAttribute attribute)
        : this(id, attribute, 0, 0, 0)
    {
    }

    public SensorNode(int id, SCADATargetAttribute attribute, double outagePercentChance, double outageDurationHours, double outageDurationPercentVariability) :
        this()
    {
        Id = id;
        TargetAttribute = attribute;
        OutagePercentChance = outagePercentChance;
        OutageDurationHours = outageDurationHours;
        OutageDurationPercentVariability = outageDurationPercentVariability;
    }

    #endregion

    public int Id { get; set; }

    public SCADATargetAttribute TargetAttribute { get; set; } = SCADATargetAttribute.Pressure;

    public double OutagePercentChance { get; set; } = 0;

    public double OutageDurationHours { get; set; } = 0;

    public double OutageDurationPercentVariability { get; set; } = 0;
}


[DebuggerDisplay("MaxPatternOffset: {MaxPatternOffsetHours} Hrs, HourlyVariation: {HourlyPercentVariation} %, Env. Variation: {EnvironmentalVariationMultiplier}")]
public class DemandVariability
{

    public double MaxPatternOffsetHours { get; set; } = 1;

    public double HourlyPercentVariation { get; set; } = 5;

    public double EnvironmentalVariationMultiplier { get; set; } = 1;
}


[DebuggerDisplay("Id: {Id}, [{StartA}, {StartB}, {StartC}], [{EndA}, {EndB}, {EndC}], [{DischargeUnit}, {HeadUnit}]")]
public class ChangingPumpNode
{
    public int Id { get; set; }

    public double StartA { get; set; }

    public double StartB { get; set; }

    public double StartC { get; set; }

    public double EndA { get; set; }

    public double EndB { get; set; }

    public double EndC { get; set; }

    public string DischargeUnit { get; set; }

    public string HeadUnit { get; set; }
}


[DebuggerDisplay("FlowErr: {FlowMeterErrorPercent} %, PressErr: {PressureMeterErrorPercent} %, PowerErr: {PowerMeterErrorPercent} %, LevelErr: {LevelMeterErrorPercent} %")]
public class MeterError
{

    public double FlowMeterErrorPercent { get; set; } = 0;

    public double PressureMeterErrorPercent { get; set; } = 0;

    public double PowerMeterErrorPercent { get; set; } = 0;

    public double LevelMeterErrorPercent { get; set; } = 0;
}


[DebuggerDisplay("[{Type}] Id: {Id} DailyChange: {DailyPercentChance} % @ {FlowRateGPM} gpm at {FlowVariabilityPercent} %")]
public class RandomDemandNode
{

    public int Id { get; set; }

    public string Type { get; set; } // file / leak // for documentation only

    public double DailyPercentChance { get; set; } = 7.0; // file = 7.0, leak = 3.0

    public double FlowRateGPM { get; set; } // 1500 gpm for fire, 100 gpm for leak

    public double FlowVariabilityPercent { get; set; } // 10 for file, 50 for leak

    public double LinearGrowthDurationHours { get; set; } // 0 for fire, 240 for leak

    public double ConstantDurationHours { get; set; } // 3 for fire, 60 for leak

    public double MinimumGapBetweenEventsHours { get; set; } = 120;

    public double DurationVariabilityPercent { get; set; } = 50;

    public string Description { get; set; } // where the even /type is happening...
}



[DebuggerDisplay("[{Type}] Id: {Id} @ {FlowRateGPM} gpm, Des: {Description}")]
public class ContinualDemandNode
{
    #region Constructor
    public ContinualDemandNode()
    {
    }
    public ContinualDemandNode(int id, double flowRateGPM, string type = "flow/leak", string description = "not given")
    {
        Id = id;
        Type = type;
        FlowRateGPM = flowRateGPM;
        Description = description;
    }
    #endregion

    public int Id { get; set; }

    public string Type { get; set; } = "leak"; // for documentation only

    public double FlowRateGPM { get; set; }

    public string Description { get; set; } // for documentation only
}

