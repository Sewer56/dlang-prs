namespace csharp_prs_interfaces
{
    public interface IPrsInstance
    {
        byte[] Compress(ref byte[] data, int searchBufferSize);
        byte[] Decompress(ref byte[] data);

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
        byte[] Compress(byte[] data, int searchBufferSize);

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <returns></returns>
        byte[] Decompress(byte[] data);

        /// <summary>
        /// Decompresses a supplied array of PRS compressed bytes and
        /// returns a decompressed copy of said bytes.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to decompress.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        /// <returns></returns>
        unsafe byte[] Decompress(byte* data, int dataLength);

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        int Estimate(byte[] data);

        /// <summary>
        /// Decodes the PRS compressed stream and returns the size of the PRS compressed
        /// file, if it were to be decompressed. This operation is approximately 18 times
        /// faster than decompressing and may be useful in some situations.
        /// </summary>
        /// <param name="data">The individual PRS compressed data to get the size of after decompression.</param>
        /// <param name="dataLength">The length of the individual PRS compressed data array to decompress.</param>
        unsafe int Estimate(byte* data, int dataLength);
    }
}