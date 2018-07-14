module prs.exports;

import prs.compress;

/+
/**
	Exists for interoperability purposes with other programming languages
	through the use of exports.
*/
public struct ByteArray
{
	int length;
	void* pointer;	
}

/**
	Compresses a supplied byte array.
	Returns the compressed version of the byte array.

	Params:
	source =				The byte array containing the file or data to compress.

	searchBufferSize =		(Default: 0x1FFF)
	A value preferably between 0xFF and 0x1FFF that declares how many bytes
	the compressor visit before any specific byte to search for matching patterns.
	Increasing this value compresses the data to smaller filesizes at the expense of compression time.
	Changing this value has no noticeable effect on decompression time.

*/
public extern(C) ByteArray externCompress(byte[]* data, int length, int searchBufferSize)
{
	byte[] localdata = *data;
	localdata.length = length;
	return compress(localdata, searchBufferSize);
}
+/