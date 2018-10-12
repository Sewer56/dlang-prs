using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csharp_prs
{
    /// <summary>
    /// Provides a means by which the underlying D language library's functions may be accessed.
    /// </summary>
    public static class Prs
    {
        /* Function redirects to X64/X86 DLL */
        private static Action<byte[], int, int, CopyArrayFunction>  _compressFunction;
        private static Action<byte[], int, CopyArrayFunction>       _decompressFunction;
        private static Action<IntPtr, int, CopyArrayFunction>       _decompressFunctionAlt;
        private static Func<byte[], int, int>                       _estimateFunction;
        private static Func<IntPtr, int, int>                       _estimateFunctionAlt;

        /* Defines a C# function which fills a managed array using a pointer and length. */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CopyArrayFunction(IntPtr dataPtr, int length);

        /* Static Initializer */
        static Prs()
        {
            if (IntPtr.Size == 4)
            {
                _compressFunction = Prs32.Compress;
                _decompressFunction = Prs32.Decompress;
                _decompressFunctionAlt = Prs32.Decompress;
                _estimateFunction = Prs32.Estimate;
                _estimateFunctionAlt = Prs32.Estimate;

            }
            else if (IntPtr.Size == 8)
            {
                _compressFunction = Prs64.Compress;
                _decompressFunction = Prs64.Decompress;
                _decompressFunctionAlt = Prs64.Decompress;
                _estimateFunction = Prs64.Estimate;
                _estimateFunctionAlt = Prs64.Estimate;
            }
            else
            {
                throw new NotSupportedException("Prs compression library is only compiled for X86 and X86_64 architectures.");
            }
        }

        /* Legacy Function Redirects */

        public static byte[] Compress(ref byte[] data, int searchBufferSize) => Compress(data, searchBufferSize);
        public static byte[] Decompress(ref byte[] data) => Decompress(data);

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
        public static byte[] Compress(byte[] data, int searchBufferSize)
        {
            byte[] resultantBytes = new byte[0];
            void AllocateArrayImpl(IntPtr dataPtr, int length)
            {
                resultantBytes = new byte[length];
                Marshal.Copy(dataPtr, resultantBytes, 0, length);
            }

            _compressFunction(data, data.Length, searchBufferSize, AllocateArrayImpl);
            return resultantBytes;
        }

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            byte[] resultantBytes = new byte[0];
            void AllocateArrayImpl(IntPtr dataPtr, int length)
            {
                resultantBytes = new byte[length];
                Marshal.Copy(dataPtr, resultantBytes, 0, length);
            }

            _decompressFunction(data, data.Length, AllocateArrayImpl);
            return resultantBytes;
        }

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        /// <returns></returns>
        public static unsafe byte[] Decompress(byte* data, int dataLength)
        {
            byte[] resultantBytes = new byte[0];
            void AllocateArrayImpl(IntPtr dataPtr, int length)
            {
                resultantBytes = new byte[length];
                Marshal.Copy(dataPtr, resultantBytes, 0, length);
            }

            _decompressFunctionAlt((IntPtr)data, dataLength, AllocateArrayImpl);
            return resultantBytes;
        }

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        public static int Estimate(byte[] data)
        {
            return _estimateFunction(data, data.Length);
        }

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        public static unsafe int Estimate(byte* data, int dataLength)
        {
            return _estimateFunctionAlt((IntPtr)data, dataLength);
        }
    }
}
