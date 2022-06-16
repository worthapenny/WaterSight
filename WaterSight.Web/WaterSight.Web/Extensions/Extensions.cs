using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterSight.Web.Extensions;

public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> GroupAt<T>(this IEnumerable<T> source, int itemsPerGroup)
    {
        for (int i = 0; i < (int)Math.Ceiling((double)source.Count() / itemsPerGroup); i++)
            yield return source.Skip(itemsPerGroup * i).Take(itemsPerGroup);
    }
}


public static class DataFrameExt
{
    public static bool IsEmpty(this DataFrame df)
    {
        return df.Rows.Count == 0;
    }

    public static Tuple<long, int> Shape(this DataFrame df)
    {
        return new Tuple<long, int>(df.Rows.Count, df.Columns.Count);
    }
    public static List<DataFrame> SplitByRows(this DataFrame df, int capacity)
    {
        var dfs = new List<DataFrame>();
        var nValues = new List<int>();

        for (int i = 0; i < df.Rows.Count; i++)
        {
            if (i > 0 && (i % capacity) == 0)
            {
                dfs.Add(df[nValues]);
                nValues.Clear();
                nValues.Add(i);
            }
            else
            {
                nValues.Add(i);
            }
        }
        // add the last remaining values
        dfs.Add(df[nValues]);

        return dfs;
    }
}

