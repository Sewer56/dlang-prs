using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AdvancedDLSupport;

namespace csharp_prs
{
    /// <summary>
    /// Provides a means by which the underlying D language library's functions may be accessed.
    /// Use the interface <see cref="_compressor"/> to interact with the library.
    /// </summary>
    public static class Prs
    {
        private const string LibraryName32 = "dlang-prs32";
        private const string LibraryName64 = "dlang-prs64";
        private static IDlangPrs _compressor;

        /// <summary>
        /// Compresses a supplied byte array.
        /// Returns the compressed version of the byte array.
        /// </summary>
        /// <param name="data">The byte array containing the file or data to compress.</param>
        /// <param name="searchBufferSize">(Default = 0x1FFF)
        /// A value preferably between 0xFF and 0x1FFF that declares how many bytes
        /// the compressor visit before any specific byte to search for matching patterns.
        /// Increasing this value compresses the data to smaller filesizes at the expense of compression time.
        /// Changing this value has no noticeable effect on decompression time.</param>
        /// <returns></returns>
        public static byte[] Compress(ref byte[] data, int searchBufferSize)
        {
            ByteArray byteArray = _compressor.externCompress(data, data.Length, searchBufferSize);
            return byteArray.GetBytes();
        }

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <returns></returns>
        public static byte[] Decompress(ref byte[] data)
        {
            ByteArray byteArray = _compressor.externDecompress(data, data.Length);
            return byteArray.GetBytes();
        }

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        public static int Estimate(ref byte[] data)
        {
            return _compressor.externEstimate(data, data.Length);
        }

        static Prs()
        {
            if (IntPtr.Size == 4)
            {
                _compressor = NativeLibraryBuilder.Default.ActivateInterface<IDlangPrs>(LibraryName32);
            }
            else
            {
                _compressor = NativeLibraryBuilder.Default.ActivateInterface<IDlangPrs>(LibraryName64);
            }
            
            _compressor.initialize();
        }


        /// <summary>
        /// Simple struct defining a native byte array for interoperability
        /// with dlang-prs.
        /// </summary>
        public struct ByteArray
        {
            public int Length;
            public IntPtr Pointer;

            /// <summary>
            /// Creates a "native", C# garbage collected
            /// byte array and copies the data returned from native code into it.
            /// </summary>
            /// <returns></returns>
            public byte[] GetBytes()
            {
                byte[] byteArray = new byte[Length];
                Marshal.Copy(Pointer, byteArray, 0, Length);

                _compressor.nativeFree(Pointer);
                return byteArray;
            }
        }
    }

    /// <summary>
    /// Represents the exports provided by the D library dlang-prs.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IDlangPrs
    {
        Prs.ByteArray externCompress(byte[] data, int length, int searchBufferSize);
        Prs.ByteArray externDecompress(byte[] data, int length);
        int externEstimate(byte[] data, int length);
        void initialize();
        void nativeFree(IntPtr memoryPointer);
    }
}
