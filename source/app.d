module app;

import std.stdio;
import std.conv;
import std.file;
import prs.compress;
import prs.decompress;
import prs.estimate;
import std.datetime.stopwatch;

/*
	[Visual D]
	To run me, right click the dlang-prs project in the solution explorer and hit properties, then change output type to Executable.
	Under Linker category/tab, change the output's extension to .exe
*/
void main()
{
	// Whoohoo!
	writeln("Benchmark Start!");

	// Read
	byte[] original = cast(byte[])read("test.bin");

	// Compress
	void compbench() 
	{ 
		// 1/4 search buffer size, use 1FFF for benchmarking against other implementations.
		auto prsFile = compress(original, 0x7ff); 
		std.file.write("testd.prs", (&prsFile[0])[0 .. prsFile.length]);
	}

	// Benchmark
	auto compDurations = benchmark!(compbench)(10);
	writeln("Compress: " ~ compDurations[0].toString());

	// Read
	byte[] compressed = cast(byte[])read("testd.prs");

	// Decompress
	void decompbench() 
	{ 
		auto decompFile = decompress(compressed); 
		std.file.write("testdnew.bin", (&decompFile[0])[0 .. decompFile.length]);
	}

    // Estimate
	int estimatebench() 
	{ 
		return estimate(compressed);
	}

	auto decompDurations = benchmark!(decompbench)(10);
	writeln("Decompress: " ~ decompDurations[0].toString());

    auto estimateDurations = benchmark!(estimatebench)(10);
	writeln("Estimate: " ~ estimateDurations[0].toString());

	// Hang (testing)
	readln();
}
