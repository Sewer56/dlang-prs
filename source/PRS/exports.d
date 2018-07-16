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
import prs.decompress;
import std.container.array;
import core.memory;
import std.stdio;
import std.range;
import core.sync.mutex;

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
	Used for storing references to individual arrays of bytes which contain
	our compressed files to be used by external libraries.
*/
public Array!(Array!byte) files;

/**
	Controls accesses to the array of byte arrays containing data 
*/
public shared Mutex mutex;

/**
	Compresses a supplied byte array.
	Returns the compressed version of the byte array.
	After you are done with using the byte array returned, you should call the exported function
	addGCRoot() with the pointer to re-add for garbage collection.

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
	addFile(compressedData);

 	return ByteArray(cast(int)compressedData.length, &compressedData[0]);
}

/**
	Clears the arrays used for temporary storage from memory.
	More specifically, removes the array instances that were passed into your C/C++/C# etc. code
	from memory.
*/
export extern(C) void clearFiles()
{
	// Conditional mutex creation.
	if (mutex is null)
	{ enableMutex(); }

	mutex.lock_nothrow();

	// Destroys each of the stored compressed/files and empties their list.
	for (int x = 0; x < files.length; x++)
	{ destroy(files[x]); }

	files.clear();
	mutex.unlock_nothrow();
}

/**
	Adds an array of bytes into the array of
	the array of bytes storing data for external C/C++/C# code.

	Params:
	file = The file to add to the array of files.
*/
public void addFile(ref Array!byte file)
{
	// Conditional mutex creation.
	if (mutex is null)
	{ enableMutex(); }

	mutex.lock_nothrow();
	files.insert(file);
	mutex.unlock_nothrow();
}

/**
	Enables the dlang garbage collector and sets up a new mutex object.
*/
public void enableMutex()
{
	GC.enable;
	mutex = new shared Mutex();
}

/**
	Decompresses a supplied byte array.
	Returns the decompressed version of the byte array.
	After you are done with using the byte array returned, you should call the exported function
	addGCRoot() with the pointer to re-add for garbage collection.

	Params:
	source =				The byte array containing the file or data to compress.
	length =				The length of the byte array.
*/
export extern(C) ByteArray externDecompress(byte* data, int length)
{
	byte[] passedData = data[0 .. length];
	auto decompressedData = decompress(passedData);
	addFile(decompressedData);

	return ByteArray(cast(int)decompressedData.length, &decompressedData[0]);
}