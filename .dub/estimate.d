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

module prs.estimate;
import std.container.array;
import std.typecons;

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
    Specifies the current offset from the start of the file used during decompression.
*/
private int pointer = 0;

/**
    Returns the size of a PRS encoded file by performing the decompression of a file
    without actually copying any data for speed. This may be useful for the hacking of
    games whereby you may want to ensure the game has enough space to decompress a specific
    PRS encoded stream but don't want to actually decompress the stream yourself.

    Params:
    source = An array of bytes read containing a PRS compressed file or structure.
*/
public int estimate(ref byte[] source)
{
	// Initialize variables.
	pointer = 0;
	controlByte = readByte(source);
	currentBitPosition = 0;
	int fileSize = 0;

	// Endlessly iterate over the file until the end of file signature/opcode special combo is hit.
	while (true)
	{
		// Test for Direct Byte (Opcode 1)
		if (retrieveControlBit(source) == 1)
		{
			// Increment direct byte.
			pointer += 1;
			fileSize += 1;
			continue;
		}

		// Opcode 1 failed, now testing for Opcode 0X
		// Test for Opcode 01
		if (retrieveControlBit(source) == 1)
		{
			// Append size of long copy, break if it's end of file.
			if (decodeLongCopy(source, fileSize))
				break;
		}
		// Do Opcode 00
		else
		{
			decodeShortCopy(source, fileSize);
		}
	}

	// Return back
	return fileSize;
}

/**
    Decodes a PRS encoded longjump (after the opcode)
    and appends the size of the longjump onto the file size.

    Returns true if this is the special end of file
    opcode, else false.

    Params:
    source = Source array to PRS shortcopy from.
    destination = Destination array to perform the copy operation to.
*/
public bool decodeLongCopy(ref byte[] source, ref int fileSize)
{
	// Obtain the offset and size packed combination.
	int offset	=	readByte(source);		// Dlang will return negative number, e.g. ff ff ff 88 when only 88 is read because of signed bytes being default.
	                                        // Readbyte has been modified to return unsigned only
	offset		|=	readByte(source) << 8;	   

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
		length = readByte(source); 
		length += 1;
	} 
	else							   // Otherwise Mode 2 (Long Copy Short)
	{ length += 2; }				   // Offset length by 2 a packed.

	// LZ77 Write to Destination
    fileSize += length;
	return false;
}

/**
    Decodes a PRS encoded shortjump (after the opcode)
    and appends the size of the shortjump onto the file size.

    Params:
    source = Source array to PRS shortcopy from.
    destination = Destination array to perform the copy operation to.
*/
public void decodeShortCopy(ref byte[] source, ref int fileSize)
{
	// Use a shorter variable name for simplification. (Compiler will optimize this out in release mode)
	int length = 0;

	// Get our length for the jump.
	length =  length		| retrieveControlBit(source); // The second bit comes first.
	length =  length << 1;								  // Small hint to the compiler.
	length =  length		| retrieveControlBit(source); // Then the first bit.

	// Offset the value back by 2.
	length += 2;

	// Obtain the offset.
	int offset = readByte(source) | -0x100; // -0x100 converts from `256 - positive offset` to `negative offset`
	                                        // We lost our sign when we originally wrote the offset.

	// LZ77 Write to Destination
    fileSize += length;
}

/**
    Reads a byte from the location specified by the module local
    variable pointer and automatically increments the pointer value.
*/
public ubyte readByte(ref byte[] source)
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
public int retrieveControlBit(ref byte[] source)
{
	// Once we are exhausted out of bits, we need to read the next one from our stream.
	if (currentBitPosition >= 8)
	{
		// Get new controlByte and reset bit position.
		controlByte = readByte(source);
		currentBitPosition = 0;
	}

	// Retrieve our return value and pre-shift control byte for our next return value.
	int returnValue = controlByte & 0x01; // Select first bit.
	controlByte = controlByte >> 1;

	// Read the next bit next time.
	currentBitPosition += 1;

	return returnValue;
}