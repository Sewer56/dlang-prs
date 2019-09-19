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

------

## X64 Compression

### Windows Executable, Mixed Compression

TSonic_win.exe: 5,132,288 bytes. (Sonic Heroes' executable)

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |  2998   | 62.83               | 3,224,642        |
| dlang-prs  | 0x1FFF Window |  9652   | 57.11               | 2,931,333        |
| Prs.NET    | 0x1FF0 Window |  14483  | 55.30               | 2,838,437        |
| prs-util   | 0x1FFF Window |  16535  | 58.21               | 2,987,736        |

### Medium Size, Highly Compressible

s01_P1.bin: 135,168 bytes
Sonic Heroes object layout - huge lot of 0s.

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |   28    | 14.61               | 19,748           |
| dlang-prs  | 0x1FFF Window |   83    | 13.68               | 18,497           |
| Prs.NET    | 0x1FF0 Window |   109   | 13.59               | 18,368           |
| prs-util   | 0x1FFF Window |   472   | 13.90               | 18,788           |

### Medium Size, Realistic Use Case

VECTOR_LOCATOR.DFF: 169,637 bytes
Medium, RenderWare Clump Model/Stream

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |   82    | 50.15               | 85,079           |
| dlang-prs  | 0x1FFF Window |   254   | 44.15               | 74,890           |
| Prs.NET    | 0x1FF0 Window |   367   | 43.56               | 73,900           |
| prs-util   | 0x1FFF Window |   419   | 45.53               | 77,237           |

## X86 Compression

### Windows Executable, Mixed Compression

TSonic_win.exe: 5,132,288 bytes. (Sonic Heroes' executable)

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |  4093   | 62.83               | 3,224,642        |
| dlang-prs  | 0x1FFF Window |  13339  | 57.11               | 2,931,333        |
| Prs.NET    | 0x1FF0 Window |  28091  | 55.30               | 2,838,437        |
| prs-util   | 0x1FFF Window |  26815  | 58.21               | 2,987,736        |

### Medium Size, Highly Compressible

s01_P1.bin: 135,168 bytes
Sonic Heroes object layout - huge lot of 0s.

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |   39    | 14.61               | 19,748           |
| dlang-prs  | 0x1FFF Window |   110   | 13.68               | 18,497           |
| Prs.NET    | 0x1FF0 Window |   179   | 13.59               | 18,368           |
| prs-util   | 0x1FFF Window |   533   | 13.90               | 18,788           |

### Medium Size, Realistic Use Case

VECTOR_LOCATOR.DFF: 169,637 bytes
Medium, RenderWare Clump Model/Stream

| Compressor | Notes         | Time/ms | Compression Ratio/% | Final Size/bytes |
| ---------- | ------------- | :-----: | ------------------- | ---------------- |
| dlang-prs  | 0x7FF Window  |   116   | 50.15               | 85,079           |
| dlang-prs  | 0x1FFF Window |   386   | 44.15               | 74,890           |
| Prs.NET    | 0x1FF0 Window |   714   | 43.56               | 73,900           |
| prs-util   | 0x1FFF Window |   665   | 45.53               | 77,237           |

## X64 Decompression

Reminder: The PRS compressed files used for benchmarking decompression are the output of Prs.NET.

TSonic_win.exe

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  28.70  |
| Prs.NET    |  69.56  |
| prs-util   |  43.78  |

s01_P1.bin:

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  0.28   |
| Prs.NET    |  1.79   |
| prs-util   |  0.73   |

VECTOR_LOCATOR.DFF: 

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  0.636  |
| Prs.NET    |  2.213  |
| prs-util   |  1.411  |

## X86 Decompression

TSonic_win.exe:

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  31.97  |
| Prs.NET    | 101.79  |
| prs-util   |  55.63  |

s01_P1.bin:

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  0.29   |
| Prs.NET    |  2.44   |
| prs-util   |  0.86   |

VECTOR_LOCATOR.DFF: 

| Compressor | Time/ms |
| ---------- | :-----: |
| dlang-prs  |  0.654  |
| Prs.NET    |  3.014  |
| prs-util   |  1.626  |

## Search Buffer Size Scaling (dlang-prs - X64)

A feature unique to dlang-prs is that it allows you to set the size of the search buffer used for compression; letting the user adjust between compression ratio and speed. Here is a data set of individual tests across each file; mapping the final file size and time taken together.

### TSonic_win; 

| Compressor   | Time/ms | File Size/bytes |
| ------------ | :-----: | --------------- |
| 0x1FFF       |  9680   | 2,931,333       |
| 0x1C00       |  8599   | 2,955,088       |
| 0x1800       |  7543   | 2,983,479       |
| 0x1400       |  6418   | 3,020,668       |
| 0x1000       |  5295   | 3,064,558       |
| 0xC00        |  4212   | 3,128,102       |
| 0x800        |  2973   | 3,223,750       |
| 0x400        |  1676   | 3,386,723       |
| 0xFF         |   563   | 3,717,609       |
| 0x64         |   295   | 4,048,881       |
| 0x10         |   108   | 4,900,127       |
| 0x0A         |   84    | 5,083,734       |
| Uncompressed |   N/A   | 5,132,288       |
| 0x0          |   40    | 5,773,827       |

A Windows executable is ideal for this kind of test as it contains all sorts of random data and the file contents are very mixed. The result set can be treated as a set of very realistic expectations. 

### s01_P1.bin (Medium Size, Highly Compressible):

| Compressor   | Time/ms | File Size/bytes |
| ------------ | :-----: | --------------- |
| 0x1FFF       |  77.96  | 18,497          |
| 0x1C00       |  68.99  | 18,612          |
| 0x1800       |  60.77  | 18,783          |
| 0x1400       |  52.98  | 18,936          |
| 0x1000       |  43.67  | 19,151          |
| 0xC00        |  35.54  | 19,391          |
| 0x800        |  27.42  | 19,746          |
| 0x400        |  16.84  | 20,433          |
| 0xFF         |  8.20   | 21,980          |
| 0x64         |  5.92   | 23,554          |
| 0x10         |  4.64   | 37,768          |
| 0x0A         |  4.52   | 37,915          |
| Uncompressed |   N/A   | 135,168         |
| 0x00         |  1.03   | 152,067         |

### VECTOR_LOCATOR.DFF (Medium Size, RW Clump/3D Model):

| Compressor   | Time/ms | File Size/bytes |
| ------------ | :-----: | --------------- |
| 0x1FFF       | 242.34  | 74,890          |
| 0x1C00       | 219.17  | 75,346          |
| 0x1800       | 190.16  | 76,165          |
| 0x1400       | 171.28  | 77,068          |
| 0x1000       | 142.81  | 81,219          |
| 0xC00        | 111.26  | 82,975          |
| 0x800        |  79.53  | 85,066          |
| 0x400        |  44.03  | 88,529          |
| 0xFF         |  14.85  | 94,096          |
| 0x64         |  7.14   | 102,682         |
| 0x10         |  3.99   | 123,146         |
| 0x0A         |  2.37   | 144,389         |
| Uncompressed |   N/A   | 169,637         |
| 0x00         |  1.24   | 190,844         |

![Graph A](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling.png)
![Graph B](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling2.png)
![Graph C](https://raw.githubusercontent.com/sewer56lol/dlang-prs/master/images/SlidingWindowScaling3.png)