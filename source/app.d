module app;

import std.stdio;
import std.conv;
import std.file;
import prs.compress;
import prs.decompress;
import prs.estimate;
import std.datetime.stopwatch;
import std.container.array;
import core.thread;

/*
	[Visual D]
	To run me, right click the dlang-prs project in the solution explorer and hit properties, then change output type to Executable.
	Under Linker category/tab, change the output's extension to .exe.

    In addition; remove the "-shared" class from additional commandline arguments.
*/
void main()
{
	// Whoohoo!
	writeln("Benchmark Start!");
    
    Thread.sleep(dur!("msecs")( 1000 ));

	// Read
	byte[] original = cast(byte[])read("compressme.bin");
    byte[] prsFile;

	// Compress
	void compbench() 
	{ 
		// 1/4 search buffer size, use 1FFF for benchmarking against other implementations.
		prsFile = compress(original, 0x1FFF); 
	}

	// Benchmark
	auto compDurations = benchmark!(compbench)(1);
	writeln("Compress: " ~ compDurations[0].toString());
    std.file.write("compressed.prs", (&prsFile[0])[0 .. prsFile.length]);

	// Read
	byte[] compressed = cast(byte[])read("decompressme.prs");
    byte[] decompressed;

	// Decompress
	void decompbench() 
	{ 
		decompressed = decompress(compressed); 
	}

    // Benchmark
    auto decompDurations = benchmark!(decompbench)(1);
	writeln("Decompress: " ~ decompDurations[0].toString());
    std.file.write("decompressed.bin", decompressed);
    
    // Estimate
	int estimatebench() 
	{ 
		return estimate(compressed);
	}

    // Benchmark
    auto estimateDurations = benchmark!(estimatebench)(1);
	writeln("Estimate: " ~ estimateDurations[0].toString());

	// Hang (testing)
	readln();
}
