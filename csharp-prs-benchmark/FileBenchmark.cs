using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using csharp_prs;

namespace csharp_prs_benchmark
{
    public class FileBenchmark
    {
        public IEnumerable<int> WindowSizes => new[] { 0xFF, 0x7FF, 0x1FFF };

        [ParamsSource(nameof(WindowSizes))]
        public int WindowSize { get; set; }

        public byte[] FileData { get; private set; }
        public byte[] FileDataCompressed { get; private set; }

        public FileBenchmark(byte[] fileData)
        {
            FileData = fileData;
            FileDataCompressed = Prs.Compress(FileData, 0x1FFF);
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Executed once per each WindowSize
            FileDataCompressed = Prs.Compress(FileData, WindowSize);
        }

        /* Compress / Decompress */
        [Benchmark]
        public byte[] Compress() => Compress(WindowSize);

        [Benchmark]
        public byte[] Decompress() => Prs.Decompress(FileDataCompressed);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public byte[] Compress(int windowSize) => Prs.Compress(FileData, windowSize);
    }
}
