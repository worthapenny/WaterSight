//using OpenFlows.Domain.ModelingElements;
//using System.Collections.Generic;
//using System.Linq;

//namespace WaterSight.Model.Extensions;

//public static class ListExtensions
//{
//    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
//    {
//        return source
//            .Select((x, i) => new { Index = i, Value = x })
//            .GroupBy(x => x.Index / chunkSize)
//            .Select(x => x.Select(v => v.Value).ToList())
//            .ToList();
//    }

//    public static IEnumerable<T> Unique<T>(this IEnumerable<T> source) where T: IElement
//    {
//        return source.GroupBy(s => s.Id).Select(g => g.FirstOrDefault());
//    }
//}
