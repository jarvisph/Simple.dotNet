using System.Diagnostics;

namespace Simple.Core.Helper
{
    public class StopwatchHelper
    {
        private static Stopwatch _stopwatch;
        public static Stopwatch Stopwatch
        {
            get
            {
                if (_stopwatch == null)
                    _stopwatch = new Stopwatch();
                return _stopwatch;
            }
        }
        public static void StartTime()
        {
            Stopwatch.Start();
        }
        public static void StopTime()
        {
            Stopwatch.Reset();
        }
        public static long Milliseconds
        {
            get
            {
                return Stopwatch.ElapsedMilliseconds;
            }
        }
        public static long Seconds
        {
            get
            {
                return Milliseconds / 1000;
            }
        }
    }
}
