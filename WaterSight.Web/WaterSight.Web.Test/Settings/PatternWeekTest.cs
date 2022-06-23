using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;
using WaterSight.Web.Support;
using WaterSight.Web.Test;

namespace WaterSight.Web.Test;


[TestFixture, Order(100600), Category("Settings"), Category("ParameterWeek")]
public class PatternWeeksTest : TestBase
{
    #region Constructor
    public PatternWeeksTest()
    //: base(4549, Env.Qa)
    //:base(244, Env.Prod)
    {
        Logger.Debug($"----+----+---- Performing Pattern Weeks Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public WaterSight.Web.Settings.PatternWeeks PatternWeeks => WS.Settings.PatternWeeks;
    #endregion

    #region Tests
    [Test]
    public async Task PatternWeeks_CRUD()
    {
        // Create Object
        var newPatternWeek = NewPatternWeekConfig();
        Assert.IsNotNull(newPatternWeek);

        // Create
        var patternWeek = await PatternWeeks.AddPatternWeekConfigAsync(newPatternWeek);

        Assert.IsNotNull(patternWeek);
        Assert.IsTrue(patternWeek.ID > 0);
        Assert.AreEqual(newPatternWeek.Name, patternWeek.Name);
        Logger.Debug("Done create testing");
        Logger.Debug(Util.LogSeparatorDots);


        // Update
        var newName = "New Pattern Week Name";
        patternWeek.Name = newName;
        var success = await PatternWeeks.UpdatePatternWeekConfigAsync(patternWeek);
        Assert.AreEqual(true, success);
        Logger.Debug("Done update testing");
        Logger.Debug(Util.LogSeparatorDots);


        // Read
        var patternWeekFound = await PatternWeeks.GetPatternWeekConfigAsync(patternWeek.ID);
        Assert.IsNotNull(patternWeekFound);
        Assert.AreEqual(patternWeekFound.ID, patternWeek.ID);
        Assert.AreEqual(patternWeekFound.Name, newName);
        Logger.Debug("Done read, single item, testing");
        Logger.Debug(Util.LogSeparatorDots);


        // Delete
        var deleted = await PatternWeeks.DeletePatternWeekConfigAsync(patternWeek.ID);
        Assert.AreEqual(true, deleted);
        Logger.Debug("Done delete, single item, testing");
        Logger.Debug(Util.LogSeparatorDots);


        // Delete All
        var deletedAll = await PatternWeeks.DeletePatternWeeksConfigAsync();
        Assert.AreEqual(true, deletedAll);
        Logger.Debug("Done delete , all items, testing");
        Logger.Debug(Util.LogSeparatorDots);


        // Read All (Default should still be there even after Delete all)
        var patternWeeks = await PatternWeeks.GetPatternWeeksConfigAsync();
        Assert.IsNotNull(patternWeeks);
        Assert.IsTrue(patternWeeks.Count > 0);
        Logger.Debug("Done read, all items, testing");
        Logger.Debug(Util.LogSeparatorDots);


        Logger.Debug("Done with CRUD tests");
        Logger.Debug(Util.LogSeparatorEquals);
    }

    private PatternWeekConfig NewPatternWeekConfig()
    {
        var patternWeek = new PatternWeekConfig();
        patternWeek.Name = "Test Pattern Configuration";
        patternWeek.IsDefault = false;
        patternWeek.DigitalTwinID = WS.Options.DigitalTwinId;
        patternWeek.ID = 0;
        patternWeek.DaysOfWeek = new List<DayOfWeekGroup>()
        {
            new DayOfWeekGroup(){ID=0, DayOfWeek=0,GroupNumber=1},
            new DayOfWeekGroup(){ID=0, DayOfWeek=1,GroupNumber=0},
            new DayOfWeekGroup(){ID=0, DayOfWeek=2,GroupNumber=0},
            new DayOfWeekGroup(){ID=0, DayOfWeek=3,GroupNumber=0},
            new DayOfWeekGroup(){ID=0, DayOfWeek=4,GroupNumber=0},
            new DayOfWeekGroup(){ID=0, DayOfWeek=5,GroupNumber=0},
            new DayOfWeekGroup(){ID=0, DayOfWeek=6,GroupNumber=1},
        };

        return patternWeek;
    }
    #endregion
}
