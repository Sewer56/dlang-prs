using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using csharp_prs;

namespace csharp_prs_benchmark
{
    public class FileBenchmark
    {
        [Params(0xFF, 0x7FF, 0x1FFF)]
        public int WindowSize  { get; set; }

        public byte[] FileData { get; private set; }
        public byte[] FileDataCompressed { get; private set; }

        public FileBenchmark(byte[] fileData)
        {
            FileData = fileData;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            // Executed once per each WindowSize
            FileDataCompressed = Prs.Compress(FileData, WindowSize); 
        }

        /* Compress / Decompress */
        [Benchmark]
        public byte[] Compress()
        {
            return Prs.Compress(FileData, WindowSize);
        }

        /*
        [Benchmark]
        public byte[] Decompress()
        {
            return Prs.Decompress(FileDataCompressed);
        }
        */
    }
}
