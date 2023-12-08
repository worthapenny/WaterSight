using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace WaterSight.Web.Support
{


    public static class Util
    {
        #region Constancts
        public const char Equal = '=';
        public const char Dot = '.';
        public const char Star = '*';
        public const char Plus = '+';
        public const char XSmall = 'x';
        public const char XBig = 'X';
        public const char Dash = '-';
        public const char Abovescore = '‾';
        public const char Underscore = '_';
        public const char Bullet = '•';
        public const char BulletPlus = '●';
        public const char BulletInverse = '◘';
        public const char BulletWhite = '◦';
        public const char Square = '▪';
        public const char SquarePlus = '■';
        public const char Infinity = '∞';
        public const char OSmall = 'o';
        public const char OBig = 'O';
        #endregion

        #region Public Methods
        // OR => dt.UtcDateTime.ToString("O")
        //public static string ISODateTime(DateTimeOffset dt) => dt.ToString("u").Replace(" ", "T");
        public static Stopwatch StartTimer() => Stopwatch.StartNew();
        public static TimeSpan Elapsed(Stopwatch sw) => sw.Elapsed;

        public static int LogSeparatorSize { get; } = 100;
        public static string LogSeparatorEquals { get; } = new string('=', LogSeparatorSize);
        public static string LogSeparatorDots { get; } = new string('.', LogSeparatorSize);
        public static string LogSeparatorXs { get; } = new string('x', LogSeparatorSize);
        public static string LogSeparatorUnderscores { get; } = new string('_', LogSeparatorSize);
        public static string LogSeparatorDashes { get; } = new string('-', LogSeparatorSize);
        public static string LogSeparatorPluses { get; } = new string('+', LogSeparatorSize);
        public static string LogSeparatorUpperBar { get; } = new string('‾', LogSeparatorSize);
        public static string LogSeparatorBullet { get; } = new string(Bullet, LogSeparatorSize);
        public static string LogSeparatorBulletPlus { get; } = new string(BulletPlus, LogSeparatorSize);
        public static string LogSeparatorBulletInverse { get; }=        new string (BulletInverse, LogSeparatorSize);
        public static string LogSeparatorBulletWhite { get; } = new string(BulletWhite, LogSeparatorSize);
        public static string LogSeparatorSquare { get; } = new string(Square, LogSeparatorSize);
        public static string LogSeparatorSquarePlus { get; } = new string(SquarePlus, LogSeparatorSize);
        public static string LogSeparatorInfinity { get; } = new string(Infinity, LogSeparatorSize);

        public static string LogSeparatorOSmall{ get; } = new string(OSmall, LogSeparatorSize);
        public static string LogSeparatorOBig { get; } = new string(OBig, LogSeparatorSize);

        public static string LogSeparator(char symbol, int repeatSymbol = 100)
        {
            return $"{new string(symbol, repeatSymbol)}";
        }
        public static string LogSeparator(string message, char symbol, int repeatSymbol = 50)
        {
            return $"{new string(symbol, repeatSymbol)}  {message}  {new string(symbol, repeatSymbol)}";
        }
        #endregion

        public static async Task<Dictionary<string, List<object>>> ReadCsvToDataFrameAsync(
            string filePath,
            Dictionary<string, Type> dataTypeMap)
        {
            Dictionary<string, List<object>> dataMap = new Dictionary<string, List<object>>();
            foreach (var kvp in dataTypeMap)
                dataMap.Add(kvp.Key, new List<object>());

            var stopwatch = Util.StartTimer();

            Log.Debug($"About to read CSV file: {filePath}");

            await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        // csvData = List of rows
                        // Each row has List of KVPs for each column
                        var csvData = csv.GetRecords<dynamic>().ToList();
                        Log.Debug($"CSV data read as string in {stopwatch.Elapsed}. Parsing the values to right format...");

                        var emptyValues = new List<object?>() { null, "#VALUE!", "", "null", "Null", "NULL", "NA", "N/A", "n/a", "na" };

                        var counter = 1;
                        foreach (var row in csvData)
                        {
                            foreach (KeyValuePair<string, object> item in row)
                            {
                                var value = item.Value;
                                try
                                {
                                    if (value == null || emptyValues.Contains(value) || string.IsNullOrEmpty(value.ToString()))
                                        value = null;

                                    else
                                        value = Convert.ChangeType(item.Value, dataTypeMap[item.Key]);

                                    dataMap[item.Key].Add(value);
                                }
                                catch (Exception)
                                {
                                    dataMap[item.Key].Add(null);
                                }
                            }

                            if (counter % 1000 == 0)
                            {
                                double percent = (counter * 1.0 / csvData.Count) * 100.0;
                                var complitionTime = TimeSpan.FromSeconds(stopwatch.Elapsed.TotalSeconds / percent * 100);
                                var remainingTime = complitionTime - stopwatch.Elapsed;
                                Log.Debug($"[{counter}/{csvData.Count}] {percent:f2}% parsed. Time-taken: {stopwatch.Elapsed}. Expected duration: {complitionTime}. Done in: {remainingTime}");

                            }
                            counter++;
                        }
                    }
                }
            });

            Log.Debug($"Read and parsed CSV file. Time-taken: {stopwatch.Elapsed}");
            Log.Debug(LogSeparatorDots);
            stopwatch.Stop();

            return dataMap;
        }

        /*public static async Task<DataFrame?> ReadCsvToDataFrameAsync(
            string filePath,
            Dictionary<string, Type> dataTypeMap)
        {
            Log.Debug($"About to read a CSV file: {filePath}");
            var df = new DataFrame();

            using (_ = new TimeWatch("read a CSV file"))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    df = DataFrame.LoadCsvFromString(
                        csvString: content
                        //columnNames: dataTypeMap.Keys.ToArray(),
                        //dataTypes: dataTypeMap.Values.ToArray()
                        );

                    Log.Information($"Read a CSV file, {df.Shape()}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"...while reading CSV file to DataFrame. Path: {filePath}");
                }
                return df;
            }

        }*/



        /*
         * Method below is slower that using CsvHelper
         * 
         public static List<string> GetCsvHeader(
            string filePath,
            int headerRows = 1)
        {
            var df = new DataFrame();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                df = DataFrame.LoadCsv(stream, numberOfRowsToRead: headerRows);
            }

            var columns = df.Columns.Select(c => c.Name).ToList();
            Log.Information($"Number of columns found: {columns.Count}");
            return columns;
        }*/


        //public static List<string> GetCsvHeader(
        //    string filePath)
        //{
        //    var headerRow = new List<string>();
        //    using (var reader = new StreamReader(filePath))
        //    {
        //        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        //        {
        //            csv.Read();
        //            csv.ReadHeader();
        //            headerRow.AddRange(csv.Context.Reader.HeaderRecord);
        //        }

        //        return headerRow;
        //    }
        //}


        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsFileInUse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Log.Debug($"File doesn't exist hence not in use. Path: {filePath}");
                return false;
            }

            bool inUse;
            try
            {
                using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                }
                inUse = false;
                Log.Debug($"File is not in use. Path: {filePath}");
            }
            catch (Exception ex)
            {
                inUse = true;
                Log.Error(ex, $"File IS in USE. Path: {filePath}");
            }

            return inUse;
        }

        #region Helper Classes

    }

    #endregion

}
