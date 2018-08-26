
# Changelog
	25-Aug-2018: Testing methodology changed to not include file write times in compression/decompression. All benchmarks have been retaken.
	24-Aug-2018: Decompression speed of dlang-prs improved by up to 33%.
	24-Aug-2018: prs-util [X64] added to benchmarks.
	

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

- CPU: i7 4790k @ 4.5GHz
- RAM: 24GB CL9 1866MHz 

Each benchmark is an average of 5 runs.
Benchmark consists of compressing/decompressing a file in memory.

Decompression benchmarks are performed on files output by FraGag.Compression.Prs.

## Contestants:

- **FraGag.Compression.Prs**: Compiled under newest .NET Framework at the time of testing (4.7.2).
- **dlang-prs**: Compiled under LDC2 (LDC 1.11.0-beta2).
- **prs-util** (OnVar/Rust): Compiled with `opt-level = 3` and `RUSTFLAGS="-C target-cpu=native" on the nightly toolchains.

### Not benchmarking:
- **prsutil** (Essen/Erlang): Not familliar with Erlang *enough*.
- **fuzziqer's utility**: Failed to compile on Windows. Using Linux results would not be representative. Algorithmically identical to FraGag.Compression.Prs but in C++.
						
### Extra Notes
**FraGag.Compression.Prs**: The highest performing overloads of the Compress/Decompress method have been used. To be precise, the overloads using MemoryStream under the hood (accepting byte arrays). The FileStream overloads are very slow.

**dlang-prs** is the only compressor which has an option of manually setting the sliding window size for compression, allowing for a speed-compression ratio adjustment.

C# implementations were measured using *System.Diagnostics.Stopwatch*.
D implementations were measured using *benchmark()*
Rust implementations were measured using https://github.com/ellisonch/rust-stopwatch

--------------------------------
## X64 Compression

### Windows Executable, Mixed Compression
TSonic_win.exe: 5,132,288 bytes. (Sonic Heroes' executable)

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 2998    | 62.83               | 3,224,642        |
| dlang-prs  | 0x1FFF Window | 9652    | 57.11               | 2,931,333        |
| Prs.NET    | 0x1FF0 Window | 14483   | 55.30               | 2,838,437        |
| prs-util   | 0x1FFF Window | 16535   | 58.21               | 2,987,736        |

### Medium Size, Highly Compressible
s01_P1.bin: 135,168 bytes
Sonic Heroes object layout - huge lot of 0s.

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 28      | 14.61               | 19,748           |
| dlang-prs  | 0x1FFF Window | 83      | 13.68               | 18,497           |
| Prs.NET    | 0x1FF0 Window | 109     | 13.59               | 18,368           |
| prs-util   | 0x1FFF Window | 472     | 13.90               | 18,788           |

### Medium Size, Realistic Use Case
VECTOR_LOCATOR.DFF: 169,637 bytes
Medium, RenderWare Clump Model/Stream

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 82      | 50.15               | 85,079           |
| dlang-prs  | 0x1FFF Window | 254     | 44.15               | 74,890           |
| Prs.NET    | 0x1FF0 Window | 367     | 43.56               | 73,900           |
| prs-util   | 0x1FFF Window | 419     | 45.53               | 77,237           |

## X86 Compression

### Windows Executable, Mixed Compression
TSonic_win.exe: 5,132,288 bytes. (Sonic Heroes' executable)

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 4093    | 62.83               | 3,224,642        |
| dlang-prs  | 0x1FFF Window | 13339   | 57.11               | 2,931,333        |
| Prs.NET    | 0x1FF0 Window | 28091   | 55.30               | 2,838,437        |
| prs-util   | 0x1FFF Window | 26815   | 58.21               | 2,987,736        |

### Medium Size, Highly Compressible
s01_P1.bin: 135,168 bytes
Sonic Heroes object layout - huge lot of 0s.

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 39      | 14.61               | 19,748           |
| dlang-prs  | 0x1FFF Window | 110     | 13.68               | 18,497           |
| Prs.NET    | 0x1FF0 Window | 179     | 13.59               | 18,368           |
| prs-util   | 0x1FFF Window | 533     | 13.90               | 18,788           |

### Medium Size, Realistic Use Case
VECTOR_LOCATOR.DFF: 169,637 bytes
Medium, RenderWare Clump Model/Stream

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
|------------|---------------|:-------:|---------------------|------------------|
| dlang-prs  | 0x7FF Window  | 116     | 50.15               | 85,079           |
| dlang-prs  | 0x1FFF Window | 386     | 44.15               | 74,890           |
| Prs.NET    | 0x1FF0 Window | 714     | 43.56               | 73,900           |
| prs-util   | 0x1FFF Window | 665     | 45.53               | 77,237           |

## X64 Decompression
Reminder: The PRS compressed files used for benchmarking decompression are the output of Prs.NET.

TSonic_win.exe

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 28.70   |
| Prs.NET    | 69.56   |
| prs-util   | 43.78   |

s01_P1.bin:

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 0.28    |
| Prs.NET    | 1.79    |
| prs-util   | 0.73    |

VECTOR_LOCATOR.DFF: 

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 0.636   |
| Prs.NET    | 2.213   |
| prs-util   | 1.411   |

## X86 Decompression
TSonic_win.exe:

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 31.97   |
| Prs.NET    | 101.79  |
| prs-util   | 55.63   |

s01_P1.bin:

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 0.29    |
| Prs.NET    | 2.44    |
| prs-util   | 0.86    |

VECTOR_LOCATOR.DFF: 

| Compressor | Time/ms |
|------------|:-------:|
| dlang-prs  | 0.654   |
| Prs.NET    | 3.014   |
| prs-util   | 1.626   |

## Search Buffer Size Scaling (dlang-prs - X64)

A feature unique to dlang-prs is that it allows you to set the size of the search buffer used for compression; letting the user adjust between compression ratio and speed. Here is a data set of individual tests across each file; mapping the final file size and time taken together.

### TSonic_win; 

| Compressor   | Time/ms | File Size/bytes |
|--------------|:-------:|-----------------|
| 0x1FFF       | 9680    | 2,931,333       |
| 0x1C00       | 8599    | 2,955,088       |
| 0x1800       | 7543    | 2,983,479       |
| 0x1400       | 6418    | 3,020,668       |
| 0x1000       | 5295    | 3,064,558       |
| 0xC00        | 4212    | 3,128,102       |
| 0x800        | 2973    | 3,223,750       |
| 0x400        | 1676    | 3,386,723       |
| 0xFF         | 563     | 3,717,609       |
| 0x64         | 295     | 4,048,881       |
| 0x10         | 108     | 4,900,127       |
| 0x0A         | 84      | 5,083,734       |
| Uncompressed | N/A     | 5,132,288       |
| 0x0          | 40      | 5,773,827       |

A Windows executable is ideal for this kind of test as it contains all sorts of random data and the file contents are very mixed. The result set can be treated as a set of very realistic expectations. 

### s01_P1.bin (Medium Size, Highly Compressible):

| Compressor   | Time/ms | File Size/bytes |
|--------------|:-------:|-----------------|
| 0x1FFF       | 77.96   | 18,497          |
| 0x1C00       | 68.99   | 18,612          |
| 0x1800       | 60.77   | 18,783          |
| 0x1400       | 52.98   | 18,936          |
| 0x1000       | 43.67   | 19,151          |
| 0xC00        | 35.54   | 19,391          |
| 0x800        | 27.42   | 19,746          |
| 0x400        | 16.84   | 20,433          |
| 0xFF         | 8.20    | 21,980          |
| 0x64         | 5.92    | 23,554          |
| 0x10         | 4.64    | 37,768          |
| 0x0A         | 4.52    | 37,915          |
| Uncompressed | N/A     | 135,168         |
| 0x00         | 1.03    | 152,067         |

### VECTOR_LOCATOR.DFF (Medium Size, RW Clump/3D Model):

| Compressor   | Time/ms | File Size/bytes |
|--------------|:-------:|-----------------|
| 0x1FFF       | 242.34  | 74,890          |
| 0x1C00       | 219.17  | 75,346          |
| 0x1800       | 190.16  | 76,165          |
| 0x1400       | 171.28  | 77,068          |
| 0x1000       | 142.81  | 81,219          |
| 0xC00        | 111.26  | 82,975          |
| 0x800        | 79.53   | 85,066          |
| 0x400        | 44.03   | 88,529          |
| 0xFF         | 14.85   | 94,096          |
| 0x64         | 7.14    | 102,682         |
| 0x10         | 3.99    | 123,146         |
| 0x0A         | 2.37    | 144,389         |
| Uncompressed | N/A     | 169,637         |
| 0x00         | 1.24    | 190,844         |

![Graph A](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling.png)
![Graph B](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling2.png)
![Graph C](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling3.png)

# The End

    The file is terminated with the variable length code 01 (long copy) and bytes 00 00
    (no offset, no size).

    Nothing more needs to be explained that the reader could not come into conclusion with
    themselves or figure out from the source code.

