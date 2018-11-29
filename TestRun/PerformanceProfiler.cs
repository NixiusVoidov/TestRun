using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PerformanceProfiler
{

    class Profiler
    {
        protected static long Frequence = 1;
        protected long StartTime = 0;
        protected long LastTime = 0;
        protected string TestTitle = "";

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public static void Init()
        {
            QueryPerformanceFrequency(out Frequence);
        }

        public void Start(string title)
        {
            TestTitle = title;
            QueryPerformanceCounter(out StartTime);
            LastTime = StartTime;
            Console.WriteLine("Performance Profiler: " + title);
        }

        public void Stage(string text)
        {
            long CurrentTime = 0;
            QueryPerformanceCounter(out CurrentTime);
            double diff = (CurrentTime - LastTime) * 1000.0 / Frequence;
            double diffFromStart = (CurrentTime - StartTime) * 1000.0 / Frequence;
            Console.WriteLine(String.Format("{0}.{1}: {2}msec / {3}msec", TestTitle, text, diff, diffFromStart));
            LastTime = CurrentTime;
        }
    }
}
