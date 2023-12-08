using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WaterSight.Model.Generator.Data;

public class EnvironmentalManager
{
    #region Constructor
    public EnvironmentalManager(SCADADataGeneratorOptions options)
    {
        Options = options;
    }
    #endregion

    #region Public Methods
    public double GetEnvironmentalMultiplier(double hoursFromStart)
    {
        // Look up a daily multiplier that is derived from historical weather information, so includes both seasonal and daily fluctuations.
        // Shift any date into an appropriate count from a multi-year repeating cycle, whether that date is before or after the theoretical start of the cycle.
        double currentDayMultiplier = GetDailyEnvironmentalMultiplier(hoursFromStart);
        double nextDayMultiplier = GetDailyEnvironmentalMultiplier(hoursFromStart + 24);

        double dateTimeMultiplier = currentDayMultiplier;

        // Smoothly transition between days so there isn't a sudden jump at midnight.
        double timeOfDayHours = hoursFromStart % 24;
        if (timeOfDayHours > 22.0)
        {
            double ratio = (24 - timeOfDayHours) / 2.0;
            dateTimeMultiplier = (nextDayMultiplier - currentDayMultiplier) * ratio + currentDayMultiplier;
        }

        return dateTimeMultiplier;
    }
    #endregion

    #region Private Methods
    private double GetDailyEnvironmentalMultiplier(double hoursFromStart)
    {
        int daysFromStart = (int)Math.Floor(hoursFromStart / 24.0);

        // Shift any start time to being within the repeating cycle.
        while (daysFromStart < 0)
            daysFromStart += DailyEnvironmentalAdjustment.Count();
        while (daysFromStart >= DailyEnvironmentalAdjustment.Count())
            daysFromStart -= DailyEnvironmentalAdjustment.Count();

        double environmentalVariationMultiplier = 0;
        if (!(Options.DemandVariability is null))
            environmentalVariationMultiplier = Options.DemandVariability.EnvironmentalVariationMultiplier; // Note: this defaults to zero if the user options has a demandVariability element but omits the numeric multiplier.

        double dailyMultiplier = 1.0 + DailyEnvironmentalAdjustment[daysFromStart] * environmentalVariationMultiplier;
        return dailyMultiplier;
    }
    private double[] GetDailyEnvironmentalAdjustmentValues()
    {
        try
        {
            var resourceName = "DailyEnvironmentalAdjustmentFactors.json";
            var jsonString = ReadEmbeddedResource(resourceName);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
            return jsonObject.Factors.ToObject<double[]>();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            var message = $"...while extracting daily env. adjustment factors from embedded resource";
            Log.Error(message, ex);
        }

        return null;
    }
    public string ReadEmbeddedResource(string nameWithExtension)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(nameWithExtension));

        if (resourceName == null) throw new Exception($"{nameWithExtension} could not be found.");

        string data;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
            data = reader.ReadToEnd(); //Make string equal to full file

        return data;
    }
    #endregion

    #region Private Properties
    private SCADADataGeneratorOptions Options { get; }
    private double[] DailyEnvironmentalAdjustment => _dailyEnvironmentalAdjustments ??= GetDailyEnvironmentalAdjustmentValues();
    #endregion

    #region Fields
    private double[] _dailyEnvironmentalAdjustments;
    #endregion
}
