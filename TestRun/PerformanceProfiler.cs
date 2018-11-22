using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PerformanceProfiler
{

    class Profiler
    {
        protected static long Frequence = 1;
        protected static long StartTime = 0;
        protected static long LastTime = 0;
        protected static string TestTitle = "";

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        public static void Init()
        {
            QueryPerformanceFrequency(out Frequence);
        }

        public static void Start(string title)
        {
            TestTitle = title;
            QueryPerformanceCounter(out StartTime);
            LastTime = StartTime;
            Console.WriteLine("Performance Profiler: ", title);
        }

        public static void Stage(string text)
        {
            long CurrentTime = 0;
            QueryPerformanceCounter(out CurrentTime);
            double diff = (CurrentTime - LastTime) * 1000.0 / Frequence;
            double diffFromStart = (CurrentTime - StartTime) * 1000.0 / Frequence;
            Console.WriteLine(String.Format("{0}: {1}sec / {2}sec", text, diff, diffFromStart));
            LastTime = CurrentTime;
        }
    }
}
