using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaterSight.Web.Core;

namespace WaterSight.Web.Settings;
public class PatternWeeks : WSItem
{

    #region Constructor
    public PatternWeeks(WS ws) : base(ws)
    {
    }
    #endregion

    #region CRUD Operations
    //
    // CREATE
    public async Task<PatternWeekConfig?> AddPatternWeekConfigAsync(PatternWeekConfig pattern)
    {
        var url = EndPoints.RtdaPatternWeeksQDT;
        int? id = await WS.AddAsync<int?>(pattern, url, "PatternWeek");
        if (id.HasValue)
        {
            pattern.ID = id.Value;

            // to get other ids, call the pattern get
            pattern = await GetPatternWeekConfigAsync(id.Value);
            return pattern;
        }

        return null;
    }

    //
    // READ
    public async Task<PatternWeekConfig?> GetPatternWeekConfigAsync(int id)
    {
        var url = EndPoints.RtdaPatternWeeksForQDT(id);
        PatternWeekConfig? patternConfig = await WS.GetAsync<PatternWeekConfig>(url, id, "PatternWeek");
        return patternConfig;
    }
    public async Task<List<PatternWeekConfig?>> GetPatternWeeksConfigAsync()
    {
        var url = EndPoints.RtdaPatternWeeksQDT;
        return await WS.GetManyAsync<PatternWeekConfig>(url, "PatternWeeks");
    }
    public async Task<PatternWeekConfig> GetDefaultPatternWeekConfigAsync()
    {
        var allPatterns = await GetPatternWeeksConfigAsync();
        return allPatterns.Where(p => p.IsDefault).First();
    }

    //
    // UPDATE
    public async Task<bool?> UpdatePatternWeekConfigAsync(PatternWeekConfig pattern)
    {
        var url = EndPoints.RtdaPatternWeeksForQDT(pattern.ID);
        return await WS.UpdateAsync(pattern.ID, pattern, url, "PatternWeek");
    }

    //
    // DELETE
    public async Task<bool> DeletePatternWeekConfigAsync(int patternWeekId)
    {
        var url = EndPoints.RtdaPatternWeeksForQDT(patternWeekId);
        return await WS.DeleteAsync(patternWeekId, url, "PatternWeek");
    }
    public async Task<bool> DeletePatternWeeksConfigAsync()
    {
        Logger.Verbose("About to delete all the patterns...");
        var url = EndPoints.RtdaPatternWeeksQDT;
        return await WS.DeleteManyAsync(url, "PatternWeek");
    }

    #endregion


}


[DebuggerDisplay("{ToString()}")]
public class DayOfWeekGroup
{
    #region Constructor
    public DayOfWeekGroup()
    {
    }
    #endregion

    #region Public Properties

    public int ID { get; set; }
    public int DayOfWeek { get; set; }
    public int GroupNumber { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}:{DayOfWeek}, Group: {GroupNumber}";
    }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class PatternWeekConfig
{
    #region Constructor
    public PatternWeekConfig()
    {
    }
    #endregion

    #region Public Properties
    public int ID { get; set; }
    public int DigitalTwinID { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public List<DayOfWeekGroup> DaysOfWeek { get; set; }
    #endregion

    #region Overridden Methods
    public override string ToString()
    {
        return $"{ID}: {Name}, DT: {DigitalTwinID}, Count: {DaysOfWeek.Count}";
    }
    #endregion
}