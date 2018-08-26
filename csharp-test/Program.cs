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

            // Benchmark: Compress 1/4 search buffer size and write to file
            stopwatch.Start();
            byte[] compressed = Prs.Compress(ref test, 0x7FF);
            stopwatch.Stop();

            // Decompressed
            File.WriteAllBytes("zcompressed.prs", compressed);
            byte[] decompressed = Prs.Decompress(ref compressed);
            File.WriteAllBytes("zdecompressed.prs", decompressed);

            // Actually release memory (check if it works)
            GC.Collect();

            // Show results
            Console.WriteLine("Compress time (wrapper): " + stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
