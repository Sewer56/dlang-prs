using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csharp_prs_benchmark.Assets
{
    public static class Assets
    {
        public static byte[] Model => File.ReadAllBytes("Assets/Model.bin");
        public static byte[] ObjectLayout => File.ReadAllBytes("Assets/ObjectLayout.bin");
    }
}
