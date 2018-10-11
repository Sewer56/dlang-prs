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

import prs.estimate;
import prs.compress;
import prs.decompress;
import std.container.array;
import core.memory;
import std.stdio;
import std.range;
import core.sync.mutex;
import core.stdc.stdlib;
import core.stdc.string;

// Callback to calling code (in CDECL convention) that allocates and copies to their own array from 
// a byte* and length combo.
extern(C) alias CopyMemoryFunctionDefinition = void function(byte*, int);  // D code

/**
	Compresses a supplied byte array.
	Returns the compressed version of the byte array.
	After you are done with using the byte array returned, you should call the exported function
	clearFiles() to dispose of the leftover memory returned to you.

	Params:
	data =					Pointer to the start of the byte array of the file to decompress.
	length =				The length of the byte array.

	searchBufferSize =		(Default: 0x1FFF)

	A value preferably between 0xFF and 0x1FFF that declares how many bytes
	the compressor visit before any specific byte to search for matching patterns.
	Increasing this value compresses the data to smaller filesizes at the expense of compression time.
	Changing this value has no noticeable effect on decompression time.

    allocateMemoryPtr =     A pointer to a CDECL function that copies a region of memory back to its own
                            buffer given a pointer and length.
*/
export extern(C) void externCompress(byte* data, int length, int searchBufferSize, CopyMemoryFunctionDefinition copyMemoryPtr)
{
	byte[] passedData = data[0 .. length];	
	auto compressedData = compress(passedData, searchBufferSize);

    copyMemoryPtr(&compressedData[0], cast(int)compressedData.length);
}

/**
	Decompresses a supplied byte array.
	Returns the decompressed version of the byte array.
	After you are done with using the byte array returned, you should call the exported function
	clearFiles() to dispose of the leftover memory returned to you.

	Params:
	source =				The byte array containing the file or data to compress.
	length =				The length of the byte array.
*/
export extern(C) void externDecompress(byte* data, int length, CopyMemoryFunctionDefinition copyMemoryPtr)
{
	byte[] passedData = data[0 .. length];
	auto decompressedData = decompress(passedData);

    // Malloc and copy byte array.
    copyMemoryPtr(&decompressedData[0], cast(int)decompressedData.length);
}

/**
	Returns the size of a PRS compressed file after decompression, but without performing
    the actual decompression process.

    Useful if in any case, you do not want to decompress the file but want to know its size
    after decompression.

	Params:
	source =				The byte array containing the file or data to compress.
	length =				The length of the byte array.
*/
export extern(C) int externEstimate(byte* data, int length)
{
	byte[] passedData = data[0 .. length];
	int decompressedDataSize = estimate(passedData);

	return decompressedDataSize;
}
