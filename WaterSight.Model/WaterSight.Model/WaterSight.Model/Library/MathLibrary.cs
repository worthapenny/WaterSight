﻿//using Haestad.Domain;
//using OpenFlows.Domain.ModelingElements.Support;
//using Serilog;

//namespace WaterSight.Model.Library;

//public class MathLibrary
//{
//public static double[][] TriangulatedControlPoints(IFieldInfo xField, IFieldInfo yField)
//{
//    var xValues = (xField.Field as IFieldStatistics).GetStatistics(
//        new StatisticType[] {
//                StatisticType.Minimum,
//                StatisticType.Mean,
//                StatisticType.Maximum });

//    var yValues = (yField.Field as IFieldStatistics).GetStatistics(
//        new StatisticType[] {
//                StatisticType.Minimum,
//                StatisticType.Maximum });


//    // create a control triangle points
//    var points = new double[3][] {
//            new double[2] {xValues[0], yValues[0] },
//            new double[2] {xValues[1], yValues[1] },
//            new double[2] {xValues[2], yValues[1] }
//        };


//    Log.Debug($"1: {points[0][0]}, {points[0][1]}");
//    Log.Debug($"2: {points[1][0]}, {points[1][1]}");
//    Log.Debug($"3: {points[2][0]}, {points[2][1]}");

//    return points;
//}
//}
