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
        } 
    }
}
