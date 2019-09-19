using System;
using BenchmarkDotNet.Running;
using csharp_prs;
using csharp_prs_benchmark.Tests;

namespace csharp_prs_benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Model>();
            BenchmarkRunner.Run<ObjectLayout>();

            var files = new FileBenchmark[] { new Model(), new ObjectLayout() };
            foreach (var file in files)
                PrintCompressedSizes(file);
        }

        private static void PrintCompressedSizes(FileBenchmark benchmark)
        {
            foreach (var size in benchmark.WindowSizes)
            {
                byte[] compressed = benchmark.Compress(size);
                Console.WriteLine($"Benchmark: {benchmark} | Compressed Size: {compressed.Length} | Window Size: {size}");
            }
        }
    }
}
