using System;
using System.Collections.Generic;
using csharp_prs;
using csharp_prs_benchmark.Assets;
using Xunit;

namespace csharp_prs_test
{
    public class Compression
    {
        /// <summary>
        /// Returns the files to test with.
        /// </summary>
        public static IEnumerable<object[]> GetFiles()
        {
            yield return new []{ Assets.Model };
            yield return new []{ Assets.ObjectLayout };
        }

        [Theory]
        [MemberData(nameof(GetFiles))]
        public void Compress(byte[] file)
        {
            byte[] compressed = Prs.Compress(file, 0x1FFF);
            byte[] decompressed = Prs.Decompress(compressed);
            Assert.Equal(file, decompressed);
        }
    }
}
