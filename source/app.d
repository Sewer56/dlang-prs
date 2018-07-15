module app;

import std.stdio;
import std.conv;
import std.file;
import prs.compress;
import prs.decompress;
import std.datetime.stopwatch;

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
		byte[] prsFile = compress(original, 0x7ff); 
		std.file.write("testd.prs", prsFile);
	}

	// Benchmark
	auto compDurations = benchmark!(compbench)(1);
	writeln("Compress: " ~ compDurations[0].toString());

	// Read
	byte[] compressed = cast(byte[])read("testd.prs");

	// Decompress
	void decompbench() 
	{ 
		byte[] decompFile = decompress(compressed); 
		std.file.write("testdnew.bin", decompFile);
	}

	
	auto decompDurations = benchmark!(decompbench)(1);
	writeln("Decompress: " ~ decompDurations[0].toString());

	// Hang (testing)
	readln();
}
