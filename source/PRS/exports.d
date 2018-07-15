/*
	PRS-R (dlang-prs)

	A high performance custom compressor/decompressor for SEGA's
	PRS compression format used since the Sega Saturn.
	Copyright (C) 2018  Sewer. Sz (Sewer56)

	PRS-R is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	PRS-R is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

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