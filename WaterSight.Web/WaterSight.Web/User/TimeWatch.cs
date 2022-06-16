//using Serilog;
//using System;
//using System.Diagnostics;

//namespace WaterSight.Web.User
//{
//    public class TimeWatch : IDisposable
//    {
//        public TimeWatch(string actionDescription)
//        {
//            ActionDescription = actionDescription;
//            Start = Watch.Elapsed;
//        }

//        public void Dispose()
//        {
//            TimeSpan elapsed = Watch.Elapsed - Start;
//            Log.Debug($"Time-taken to {ActionDescription}: {elapsed}");
//        }

//        public string ActionDescription { get; }
//        public Stopwatch Watch { get; } = new Stopwatch();
//        public TimeSpan Start { get; }
//    }
//}
