using OpenFlows.Domain.ModelingElements.NetworkElements;
using OpenFlows.Water.Domain.ModelingElements.NetworkElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterSight.Model.Domain
{
    public static class Cluster
    {
        public static Dictionary<string, List<IWaterElement>> ByDistance(List<IWaterElement> nodeIds, double thresholdDistance)
        {
            var groups = new Dictionary<string, List<IWaterElement>>(); //key = groupid, values = element ids
            thresholdDistance *= 2;

            for (int i = 0; i < nodeIds.Count; i++)
            {
                var node = nodeIds[i];
                var coord = (node as IPointNodeInput).GetPoint();
                coord = new Haestad.Support.Support.GeometryPoint(
                    Math.Round(coord.X / thresholdDistance) * thresholdDistance,
                    Math.Round(coord.Y / thresholdDistance) * thresholdDistance);

                var xy = $"{coord.X}, {coord.Y}";

                if (groups.ContainsKey(xy))
                    groups[xy].Add(node);
                else
                    groups.Add(xy, new List<IWaterElement>() { node });

            }

            return groups;
        }

        public static string ByName(List<string> names)
        {
            if(names.Count < 2 )
                return string.Empty;

            var first = names.First();
            var common = new string[] { };

            for (int i = 1; i < names.Count; i++)
            {
                common = CommonString(first, names[i]);
                first = string.Join("", common);
            }

            return string.Join("", common);
        }
        private static string[] CommonString(string left, string right)
        {
            List<string> result = new List<string>();

            string[] rightArray = right.ToCharArray().Select(c => c.ToString()).ToArray();
            string[] leftArray = left.ToCharArray().Select(c => c.ToString()).ToArray();


            result.AddRange(rightArray.Where(r => leftArray.Any(l => l.StartsWith(r))));
            result.AddRange(leftArray.Where(l => rightArray.Any(r => r.StartsWith(l))));

            return result.Distinct().ToArray();
        }
    }
}
