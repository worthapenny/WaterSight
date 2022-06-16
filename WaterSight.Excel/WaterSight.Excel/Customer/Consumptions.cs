using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace WaterSight.Excel.Customer;

[DebuggerDisplay("Count: {ConsumptionItemsList.Count}")]
public class Consumptions
{
    #region Constructor
    public Consumptions(string csvFilePath)
    {
        CsvFilePath = csvFilePath;
        ConsumptionItemsList = new List<ConsumptionItem>();
    }
    #endregion

    #region Public Methods

    public bool Save()
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        return Save(csvConfig);
    }

    public bool Save(CsvConfiguration csvConfig)
    {
        bool success = true;
        try
        {
            Log.Debug($"About to write to a CSV file. Path {CsvFilePath}");

            using (var writer = new StreamWriter(CsvFilePath))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                csv.WriteRecords(ConsumptionItemsList);
            }

            Log.Debug($"Wrote to a CSV file. Path {CsvFilePath}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while writing to a CSV file. Path: {CsvFilePath}"); success = false;
            success = false;
        }

        return success;
    }
    public async Task<bool> SaveAsync(CsvConfiguration csvConfig)
    {
        return await Task.Run(() => Save(csvConfig));
    }
    public async Task<bool> SaveAsync()
    {
        return await Task.Run(() => Save());
    }
    #endregion

    #region Public Properties
    public string CsvFilePath { get; set; }
    public List<ConsumptionItem> ConsumptionItemsList { get; set; }
    #endregion
}

[DebuggerDisplay("{ToString()}")]
public class ConsumptionItem
{
    #region Constructor
    public ConsumptionItem()
    {
    }
    #endregion

    #region Public Properties
    [Index(0), Name("MeterId")]
    public string MeterId { get; set; }


    [Index(1), Name("BillingMonth")]
    public string BillingMonth => $"{BillingDateTime:yyyy-MM}-15";


    [Ignore]
    public DateTime BillingDateTime { get; set; }


    [Index(2), Name("Value")]
    public double? Value { get; set; }


    [Index(3), Name("Unit")]
    public string UnitString { get; set; }

    #endregion


    #region Overridden Methods
    public override string ToString()
    {
        return $"{MeterId} Value: {Value} {UnitString} On: {BillingMonth}";
    }
    #endregion
}