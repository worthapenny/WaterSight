using System.Collections.Generic;

namespace WaterSight.Web.Core;

public static class Payload
{
    public static Dictionary<string, object?> P0F015M => new Dictionary<string, object?>() {
            {"confidencePercentiles",  new int[]{ } },
            {"confidenceHistoricalRange", "P0D"},
            {"forecastPeriod", "POD"},
            {"resamplingInterval", "PT15M"},
            {"statisticPercentiles", new int[5, 50, 95]},
            {"maximumGapSize", "PT15M"},
            {"fillWithPattern", false},
            {"dirtySignal", true},
            {"integrationPercentile", null},
            {"isDryWeatherPattern", false}
        };
}
