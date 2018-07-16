using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using csharp_prs;

namespace csharp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            // If you get Access violation executing location... remember to switch project type from executable to DLL for D project.
            // Benchmark Time!
            Stopwatch stopwatch = new Stopwatch();

            // Preload file
            byte[] test = File.ReadAllBytes("test.bin");
            Prs.Release(); // We have nothing to release, just want to initialize the wrapper and do a JIT warmup.

            // Benchmark: Compress 1/4 search buffer size and write to file
            stopwatch.Start();
            File.WriteAllBytes("test.prs", Prs.Compress(test, 0x7FF));
            stopwatch.Stop();

            // Actually release memory (check if it works)
            Prs.Release();
            GC.Collect();

            // Show results
            Console.WriteLine("Compress time (wrapper): " + stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
