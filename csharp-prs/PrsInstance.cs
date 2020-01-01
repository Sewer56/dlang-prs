using System;
using System.Collections.Generic;
using System.Text;
using csharp_prs_interfaces;

namespace csharp_prs
{
    /// <summary>
    /// A non-static front-end for the static <see cref="Prs"/> class, in case it is useful.
    /// </summary>
    public class PrsInstance : IPrsInstance
    {
        /* Legacy Function Redirects */
        public byte[] Compress(ref byte[] data, int searchBufferSize) => Prs.Compress(data, searchBufferSize);
        public byte[] Decompress(ref byte[] data) => Prs.Decompress(data);

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
        public byte[] Compress(byte[] data, int searchBufferSize) => Prs.Compress(data, searchBufferSize);

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <returns></returns>
        public byte[] Decompress(byte[] data) => Prs.Decompress(data);

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        /// <returns></returns>
        public unsafe byte[] Decompress(byte* data, int dataLength) => Prs.Decompress(data, dataLength);

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        public int Estimate(byte[] data) => Prs.Estimate(data);

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        public unsafe int Estimate(byte* data, int dataLength) => Prs.Estimate(data, dataLength);
    }
}
