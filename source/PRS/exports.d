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

/**
	A simple structure that stores a length and a pointer to the data of a generic array.
	Intended for exports, made for easy interoperability - actually identical to the current
	implementation of byte[] in D.

	Users of the library should copy the byte array in their own programming language 
	to a native array upon receiving it and then call clear() to clear this list of pointers, 
	permitting Garbage Collection.
*/
public struct ByteArray 
{ 
	int length; 
	void* pointer;
	
	this(int length, void* pointer)
	{
		this.length = length;
		this.pointer = pointer;
	}
}


/**
    Enables the dlang garbage collector and sets up a new mutex object.
*/
export extern(C) void initialize()
{
	GC.enable;
}

/**
    Exposes the free function belonging to the C standard library.
*/
export extern(C) void nativeFree(void* memoryLocation)
{
    free(memoryLocation);
}

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

*/
export extern(C) ByteArray externCompress(byte* data, int length, int searchBufferSize)
{
	byte[] passedData = data[0 .. length];	
	auto compressedData = compress(passedData, searchBufferSize);

    // Malloc and copy byte array.
    void* memoryLocation = malloc(compressedData.length);
    memcpy(memoryLocation, &compressedData[0], compressedData.length);

 	return ByteArray(cast(int)compressedData.length, memoryLocation);
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
export extern(C) ByteArray externDecompress(byte* data, int length)
{
	byte[] passedData = data[0 .. length];
	auto decompressedData = decompress(passedData);

    // Malloc and copy byte array.
    void* memoryLocation = malloc(decompressedData.length);
    memcpy(memoryLocation, &decompressedData[0], decompressedData.length);

	return ByteArray(cast(int)decompressedData.length, memoryLocation);
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
