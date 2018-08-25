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

module prs.decompress;
import std.container.array;
import std.typecons;
import prs.estimate;
import std.algorithm.mutation;

/**
    Stores the current instance of the control byte which will have various variable length
    codes appended to it in order to instruct the decoder what to do next.
*/
private byte controlByte;

/**
    Defines the index of the next control bit of the active control byte that is
    to be modified by the compressor in question.	
*/
private int currentBitPosition = 0;

/**
    Specifies the current offset from the start of the file used during compression.
*/
private int pointer = 0;

private int destinationPointer = 0;
byte[] destination;
byte[] source;

/**
    Decompresses a supplied array of PRS compressed bytes and
    returns a decompressed copy of said bytes.

    Params:
    source = An array of bytes read containing a PRS compressed file or structure.
*/
public byte[] decompress(ref byte[] dataSource)
{
    // Get file size of file to be decompressed.
    source = dataSource;

	// Initialize variables.
	pointer = 0;
	controlByte = readByte();
	currentBitPosition = 0;
	destination = new byte[source.length * 4]; // Safe estimate.

	// Endlessly iterate over the file until the end of file signature/opcode special combo is hit.
	while (true)
	{
        // If it was not safe then, well, we have to :/
        if (destinationPointer + 256 > destination.length)
            destination.length *= 4;

		// Test for Direct Byte (Opcode 1)
		if (retrieveControlBit() == 1)
		{
			// Write direct byte.
			destination[destinationPointer] = readByte();   
            destinationPointer += 1;
            continue;
		}

		// Opcode 1 failed, now testing for Opcode 0X
		// Test for Opcode 01
		if (retrieveControlBit() == 1)
		{
			// Write long copy, break if it's end of file.
			if (writeLongCopy())
				break;
		}
		// Do Opcode 00
		else
		{
			writeShortCopy();
		}
	}

	// Return back
	return destination[0 .. destinationPointer];
}

/**
    Decodes a PRS encoded longjump (after the opcode)
    and writes the result into the destination array.

    Returns true if this is the special end of file
    opcode, else false.

    Params:
    source = Source array to PRS shortcopy from.
    destination = Destination array to perform the copy operation to.
*/
public bool writeLongCopy()
{
	// Obtain the offset and size packed combination.
	int offset	=	readByte();		// Dlang will return negative number, e.g. ff ff ff 88 when only 88 is read because of signed bytes being default.
	                                // Readbyte has been modified to return unsigned only
	offset		|=	readByte() << 8;	   

	// Check for decompression end condition.
	if (offset == 0)
	{ return true; }

	// Separate the size from the offset and calculate the actual offset.
	int length = offset & 0b111;
	offset = (offset >> 3) | -0x2000;	// When packing, we have lost the contents of the initial bits when left shifting which make
	                                    // our offset negative (8192 - offset = actual offset)
	                                    // Here we simply re-add those bits back to get our actual offset.

	// Check if Mode 3 (Long Copy Large)
	if (length == 0)
	{ 
		// Get length from next byte and increment.
		length = readByte(); 
		length += 1;
	} 
	else							   // Otherwise Mode 2 (Long Copy Short)
	{ length += 2; }				   // Offset length by 2 a packed.

	// LZ77 Write to Destination
	lz77Copy(length, offset);
	return false;
}

/**
    Decodes a PRS encoded shortjump (after the opcode)
    and writes the result into the destination array.

    Params:
    source = Source array to PRS shortcopy from.
    destination = Destination array to perform the copy operation to.
*/
public void writeShortCopy()
{
	// Use a shorter variable name for simplification. (Compiler will optimize this out in release mode)
	int length = 0;

	// Get our length for the jump.
	length =  length		| retrieveControlBit(); // The second bit comes first.
	length =  length << 1;								  // Small hint to the compiler.
	length =  length		| retrieveControlBit(); // Then the first bit.

	// Offset the value back by 2.
	length += 2;

	// Obtain the offset.
	int offset = readByte() | -0x100; // -0x100 converts from `256 - positive offset` to `negative offset`
	                                  // We lost our sign when we originally wrote the offset.

	// LZ77 Write to Destination
	lz77Copy(length, offset);
}

/**
    Copies bytes from the source array to the destination array with the specified length
    and offset. The final byte index of the destination is used for declaring the position from which
    the look behind operation is performed.
*/
public void lz77Copy(int length, int offset)
{
	// Contains the pointer to the destination array from which to perform the look back for bytes to copy from.
	int copyStartPosition = destinationPointer + offset; // offset is negative

    // Minimal vector optimizations.
    if (copyStartPosition + length < destinationPointer)
    {
        destination[destinationPointer .. destinationPointer + length] = destination[copyStartPosition .. copyStartPosition + length];
    }
    else 
    {
        for (int x = 0; x < length; x++)
            destination[destinationPointer + x] = destination[copyStartPosition + x];      
    }

    destinationPointer += length;
}

/**
    Reads a byte from the location specified by the module local
    variable pointer and automatically increments the pointer value.
*/
pragma(inline, true)
public ubyte readByte()
{
	ubyte returnValue = source[pointer];
	pointer += 1;
	return returnValue;
}

/**
    Retrieves the next control bit inside of the currently
    set controlByte. Fetches the next controlByte and reads
    the first bit if the current controlByte is exhausted.
*/
public int retrieveControlBit()
{
	// Once we are exhausted out of bits, we need to read the next one from our stream.
	if (currentBitPosition >= 8)
	{
		// Get new controlByte and reset bit position.
		controlByte = readByte();
		currentBitPosition = 0;
	}

	// Retrieve our return value and pre-shift control byte for our next return value.
	int returnValue = controlByte & 0x01; // Select first bit.
	controlByte = controlByte >> 1;

	// Read the next bit next time.
	currentBitPosition += 1;

	return returnValue;
}