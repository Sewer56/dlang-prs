
# Changelog
	19-Sep-2019: Performance tweak adjusted. Compressor now produces optimally compressed files. Files are now same size as original game files.
	19-Sep-2019: Increased compression speed by up to 50%.
	25-Aug-2018: Testing methodology changed to not include file write times in compression/decompression. All benchmarks have been retaken.
	24-Aug-2018: Decompression speed of dlang-prs improved by up to 33%.
	24-Aug-2018: prs-util [X64] added to benchmarks.


A C# wrapper for this library is also available on [NuGet](https://www.nuget.org/packages/csharp-prs/).

# Notice

After finishing this project, I have learned to witness that apparently this also exists, https://github.com/playegs/prs which meant my documentation work was (partially) for naught, nonetheless - I am still publishing this regardless as another alternative, unchanged from the original text below. Have fun ^-^.

# A Brief History

    It's 2018 and PRS; a rather commonly used non-supertrivial compression format has either
    never seen a piece of documentation or the documentation was lost through time.
    
    This has happened despite the first decompressor appearing around in the early 2000s (Nemesis' prsdec),
    which (did not?) appear to have a release of the source code and also shipped an anti-compressing
    compressor (basically an encoder that produced format compatible files, albeit larger as it did not attempt compression).
    
    A few years later, one named fuzziqer has also produced a compressor and decompressor combo, this time with a source release
    and a compressor which did actual compression. While the source did not ship with a license, and the author stated that 
    using the source freely was fine as long as he would have been credited for the compression and decompression code,
    the source was not very well documented - as to truly describe the format and was not trivial to follow.
    
    Aaaand well... FraGag ported fuzziqer's utility to .NET and that's about it... nothing...

# Why?

    Well, after a good while, the speed of the .NET implementation of PRS Compression/Decompression
    has started to bother me when using existing modding tools for Sonic Heroes and Sonic Adventure,
    two of SEGA published titles which have used this compression scheme employed since the era of the
    failed SEGA Saturn.
    
    Quite frankly, I thought that they could do with just a tiiny bit of optimization and/or perhaps
    simply more options for compression intensity - but the moment I started researching... well.. above.
    
    No documentation, only one piece of not yet fully optimized, uncommented source code and a port of it...
    and not much more - which surprised me especially given how much these libraries have been used over the
    overall course of time.
    
    I originally only planned to write a PRS Compressor/Decompressor in D as a small challenge, and to learn
    a bit of the programming language by writing my first program with it... However after I've seen the whole situation,
    and began to see possible optimizations as I dug through the non-trivial to understand source code - I thought
    it would just be straight up better to document the format entirely, while providing a new, better implementation
    for the communities.

# PRS Specifics: Summary

    PRS is a LZ77 based compression algorithm with some RLE shenanigans whereby the file is split into blocks 
    consisting of control bytes + data.
        
    (ctrlByte, byte[]) (ctrlByte, byte[]) (ctrlByte, byte[]) 
    
    The control bytes contain variable length codes stored as bits (max length: 2 bits) which tell the decoder how
    to treat the individual bytes of data up till the next control byte. They are read in a right to left order,
    i.e. with `controlByte & 1` and bitshifted right in order to acquire the next entry.
    
    With that said, each block has an undefined data length in that there are no headers in any of the blocks in question.
    As variable length codes are added onto the control bytes from right to left, the control byte is written,
    following by the data related to it once the individual control byte is full.
    
    The variable length codes themselves consist of 3 modes, a direct byte copy, short copy and two variants of long copy as is 
    to be seen below using run length encoding (size & offset). The dictionary from which the copying is performed is actually
    the last N bytes of data, hence this is also an algorithm using a sliding window for compression.

# PRS Specifics: Control Byte Variable Length Codes
    The following is a listing of the various variable length codes packed into the individual control bytes:
    
    Mode 0 (Direct Byte)           : 1
    Mode 1 (Short Copy)            : 00XX           (XX stores the size)
    Mode 2 (Long Copy, Small Size) : 01
    Mode 3 (Long Copy, Large Size) : 01             (This is not a typo, you will learn why later)
    
    The individual variable length codes also contain some additional information appended as regular bytes,
    these vary on the individual mode used.

# PRS Specifics: Decoding a control byte example.

    As specified before, the bits in the control bytes are read from right to left order, therefore using the table
    of variable length codes above we can decode a control byte as such:
    
    Example control byte (binary) | 0101 1001
    
    1    - Direct Byte
    00XX - Short Copy
    01   - Long Copy
    0.   - Short or Long Copy (the first bit of the next control byte would decide)
    
    In the case of the last opcode, the actual next control byte would be the next byte in a valid PRS file,
    assuming the byte array/file pointer has been correctly moved after each successive short/long
    and direct byte copy.

# PRS Mode 0: Direct Byte (Opcode 1)

    A single byte is to be read from the data stream/array at the current pointer position.
    That's it.
    
    Decoding (Simplified):
        if (bit == 1)
            read and append byte to file stream
        
    Encoding (Simplified):
        Append Control Byte Variable Length Code: `1` and a byte to the file stream.
# PRS Mode 1: Short Copy (Opcode 00)

    The length of the copy is in the range of 2-5 inclusive (2, 3, 4, 5) and stored directly inside the
    opcode read from the control byte (albeit as a 0-3 value, as the minimum copy length implicitly is 2,
    thus the value stored is `actual length - 2`).
    
    The actual offset for where to copy from is appended as a singular byte and is stored as 256 - positive offset,
    i.e. assuming the offset is `-5`, it would be stored as 256 - (-5*-1) = 251.
    
    Quick ways to convert to `256 - positive offset` format include `(offset & 0xFF)` and simply adding 256 to the number 
    `offset + 256`.
    
    || || Example :
    
    Opcode  : 00XX
    Could be: 0010
    
    Which yields a size of 10 (binary) which is 2 (decimal).
    Offset the value back by 2 to get the actual length; 2 + 2 = 4.
    
    Then to obtain the offset, you would just read the next byte from
    the stream/file/byte array.

#  PRS Mode 2: Long Copy, Small Size (Opcode 01)

    An offset and size combination is written directly as 2 bytes
    to the array/file/stream with the offset taking 13 bits and size 3 bits. 
    
    The maximum offset is thus 0x1FFF and size 3-9 bytes (albeit written as 1-7, as the 
    2 offset is applied here like before).
    
    The actual 2 bytes are packed as such:
    XXXX XXXX XXXX XYYY
    
    Where the X represent the offset and Y represent the size.
    
    The two bytes are written with the rightmost 8 bits written first and the 
    leftmost 8 bits written last, thus is in Big Endian order.
    
    ----------
    
    If the size is greater than 9, then you fall back to Mode 3 for compression
    as a greater size could not fit the 3 bits. 
    
    In this mode, you cannot also encode a size of 2, as taking away 2 and writing it
    would cause 000 to be written to the size part of the short - which is reserved 
    for falling back to PRS Mode 3 (See below).
    
    || || Example :
    
    Offset of 2100 and Size of 5:
    
        2100 (decimal) = 0000 ‭1100 0001 1100‬
        3    (decimal) = 0000 0000 0000 ‭0011‬                  // 5 - 2, offset applied
    
    Packed in the XXXX XXXX XXXX XYYY format:
        0‭110 0000 1110 0000‬           (2100 left shifted by 3)
        0000 0000 0000 ‭0011‬           (3)
    
    Final result: 
        0‭110 0000 1110 0011
    
    First written byte would equal 1110 0011 and the second written byte would equal 0‭110 0000.

# PRS Mode 3: Long Copy, Large Size

    A fallback of PRS Mode 2 where the size is greater than 9.
    
    Used by the decoder when the size in the first (right to left) 3 bits of mode 2 
    reads 0 (i.e. would be 2 (decimal) after incrementing).
    
    An additional byte is appended onto the stream/file/byte array which contains
    the length of the match (size) minus 1.

# Benchmarks

A set of old/legacy benchmarks is available at [Benchmarks.md](./Benchmarks.md).

Please note that the benchmarks are outdated and no longer representative. Legacy benchmarks are provided because they contain interesting data such as performance scaling across window sizes.

Since 19-Sep-2019, there is now a new benchmark project, `csharp-prs-benchmark` :

`csharp-prs-benchmark.Tests.Model` is`s01_P1.bin` in legacy benchmark.
`csharp-prs-benchmark.Tests.ObjectLayout` is `VECTOR_LOCATOR.DFF` in legacy benchmark.

# The End

    The file is terminated with the variable length code 01 (long copy) and bytes 00 00
    (no offset, no size).
    
    Nothing more needs to be explained that the reader could not come into conclusion with
    themselves or figure out from the source code.

## Building Instructions
### Compressor/Decompressor (dlang-prs)
- Install the latest version of Visual D: https://github.com/dlang/visuald/releases with the LDC compiler  (e.g. [VisualD-v0.51.0-beta1-dmd-2.089.0-ldc2-1.18.0.exe](https://github.com/dlang/visuald/releases/download/v0.51.0-beta1/VisualD-v0.51.0-beta1-dmd-2.089.0-ldc2-1.18.0.exe))
- Add the DMD and LDC compilers to your System Environment Variables' PATH (in case they are not added).
![Adding to PATH](https://i.imgur.com/lrMoasL.png)
- Generate the Visual Studio solution by using the `dub` package manager (available in your PATH if set correctly). `dub generate visuald -b release`
- Open `dlang-prs.sln` in Visual Studio.

Note: Successful builds output in `build` directory. where the solution files reside 

### All C# Components (Wrapper, GUI Util.)
- Install the latest .NET Core SDK.
- Run the PowerShell build script for D components, `Build.ps1`; this will compile the library for x86 and x64.
- Open `csharp.sln` in Visual Studio and build.