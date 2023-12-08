using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaterSight.Web.Core;
using WaterSight.Web.Settings;

namespace WaterSight.Web.Test;

[TestFixture, Order(100100), Category("Settings"), Category("Units")]
public class UnitsTest : TestBase
{
    #region Constructor
    public UnitsTest()
    //: base(4549, Env.Qa)
    {
        Logger.Debug($"----+----+---- Performing Units Related Tests ----+----+----");
    }
    #endregion

    #region Properties
    public Units Units => WS.Settings.Units;
    #endregion

    #region Tests
    [Test]
    public async Task SupportedUnits_Test()
    {
        var supportedUnits = await Units.GetAllUnits();
        Assert.NotNull(supportedUnits);
        Assert.IsTrue(supportedUnits.Count > 0);
        Separator("Pulled Supported Units");

        // Create unit classes like Pressure, Volume etc.
        // Only run when needed
        // Copy the output from "Output" window of VS into Units.cs file
        if (!true)
            CreateUnitClasses(supportedUnits);
    }
    [Test]
    public async Task All_Units_Test()
    {
        Assert.True(await Units.SetPressureUnit(Pressure.GPa));
        Assert.True(await Units.SetLengthUnit(Length.kly));
        Assert.True(await Units.SetVolumeUnit(Volume.bbl));
        Assert.True(await Units.SetEnergyUnit(Energy.GJ));
        Assert.True(await Units.SetWaterAgeUnit(Duration.micro_s));
        Assert.True(await Units.SetTemperatureUnit(Temperature.degree_Rø));
        Assert.True(await Units.SetCurrencyUnit(Currency.Euro));
        Assert.True(await Units.SetFlowUnit(VolumeFlow.dm_cubed_per_min));
        Assert.True(await Units.SetAreaUnit(Area.dm_squared));
        Assert.True(await Units.SetPowerUnit(Power.GJ_per_h));
        Assert.True(await Units.SetVelocityUnit(Speed.cm_per_min));
        Assert.True(await Units.SetMassConcentrationUnit(MassConcentration.cg_per_dL));
        Assert.True(await Units.SetRatioUnit(Ratio.percent_dot));
        Assert.True(await Units.SetWatherUnit(Weather.Metric_SI));
        Separator("Individual POSTs");

        var supportedUnits = await Units.GetAllUnits();
        Assert.True(supportedUnits.Where(u => u?.Name == "Pressure").First()?.Units == Pressure.GPa);
        Assert.True(supportedUnits.Where(u => u?.Name == "Length").First()?.Units == Length.kly);
        Assert.True(supportedUnits.Where(u => u?.Name == "Volume").First()?.Units == Volume.bbl);
        Assert.True(supportedUnits.Where(u => u?.Name == "Energy").First()?.Units == Energy.GJ);
        Assert.True(supportedUnits.Where(u => u?.Name == "Duration").First()?.Units == Duration.micro_s);
        Assert.True(supportedUnits.Where(u => u?.Name == "Temperature").First()?.Units == Temperature.degree_Rø);
        Assert.True(supportedUnits.Where(u => u?.Name == "Currency").First()?.Units == Currency.Euro);
        Assert.True(supportedUnits.Where(u => u?.Name == "VolumeFlow").First()?.Units == VolumeFlow.dm_cubed_per_min);
        Assert.True(supportedUnits.Where(u => u?.Name == "Area").First()?.Units == Area.dm_squared);
        Assert.True(supportedUnits.Where(u => u?.Name == "Power").First()?.Units == Power.GJ_per_h);
        Assert.True(supportedUnits.Where(u => u?.Name == "Speed").First()?.Units == Speed.cm_per_min);
        Assert.True(supportedUnits.Where(u => u?.Name == "MassConcentration").First()?.Units == MassConcentration.cg_per_dL);
        Assert.True(supportedUnits.Where(u => u?.Name == "Ratio").First()?.Units == Ratio.percent_dot);
        Assert.True(supportedUnits.Where(u => u?.Name == "Weather").First()?.Units == Weather.Metric_SI);
        Separator("Individual GETs");

    }


    private void CreateUnitClasses(List<UnitsConfig> supportedUnits)
    {
        var classTemplate = "public static class {className}\n";
        classTemplate += "{\n";
        classTemplate += "{classProperties}\n";
        classTemplate += "}\n";
        var propertyTemplate = "\tpublic static string {propertyName} => \"{propertyValue}\";";

        var unitsClass = new StringBuilder();
        foreach (var supportedUnit in supportedUnits)
        {
            var sb = new StringBuilder();
            var unitClass = classTemplate.Replace("{className}", supportedUnit?.Name);
            var units = new List<string>();

            foreach (var unit in supportedUnit?.UnitsList.Distinct())
            {
                if (string.IsNullOrWhiteSpace(unit))
                    continue;

                var propName = unit
                        .Replace(" ", "_")
                        .Replace("/", "_per_")
                        .Replace("²", "_squared_")
                        .Replace("³", "_cubed_")
                        .Replace("%", "percent")
                        .Replace("(", "_")
                        .Replace(")", "_")
                        .Replace(".", "")
                        .Replace("·", "_")
                        .Replace("-", "_")
                        .Replace("µ", "_micro_")
                        .Replace("$", "USDoller")
                        .Replace("€", "Euro")
                        .Replace("£", "Pound")
                        .Replace("°", "_degree_")
                        .Replace("⊙", "_dp_")
                        //.Replace("in", "inch")
                        .Replace("‰", "_percent_dot_")
                        .Replace("__", "_");

                // 'in' is reserved word so replace with inch
                propName = Regex.Replace(propName, @"\bin\b", "inch");

                if (propName.StartsWith("_"))
                    propName = propName.TrimStart('_');
                if (propName.EndsWith("_"))
                    propName = propName.TrimEnd('_');




                var prop = propertyTemplate
                    .Replace("{propertyName}", propName)
                    .Replace("{propertyValue}", unit);

                sb.AppendLine(prop);
            }

            unitClass = unitClass.Replace("{classProperties}", sb.ToString());

            unitsClass.AppendLine(string.Empty);
            unitsClass.AppendLine($"#region {supportedUnit.Name}");
            unitsClass.AppendLine(unitClass);
            unitsClass.AppendLine("#endregion");
        }

        var tempUnitClassFilePath = Path.GetTempFileName();
        File.WriteAllText(tempUnitClassFilePath, unitsClass.ToString());
        Logger.Information($"Classes generated at: {tempUnitClassFilePath}");
    }
    #endregion
}


