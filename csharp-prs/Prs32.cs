using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace csharp_prs
{
    internal class Prs32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("dlang-prs32.dll",
            EntryPoint = "externCompress",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern void Compress(byte[] data, int length, int searchBufferSize, Prs.CopyArrayFunction copyArrayFunction);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("dlang-prs32.dll",
            EntryPoint = "externDecompress",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern void Decompress(byte[] data, int length, Prs.CopyArrayFunction copyArrayFunction);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("dlang-prs32.dll",
            EntryPoint = "externDecompress",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern void Decompress(IntPtr data, int length, Prs.CopyArrayFunction copyArrayFunction);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("dlang-prs32.dll",
            EntryPoint = "externEstimate",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int Estimate(byte[] data, int length);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("dlang-prs32.dll",
            EntryPoint = "externEstimate",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        public static extern int Estimate(IntPtr data, int length);
    }
}
