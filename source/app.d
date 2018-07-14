module app;

import std.stdio;
import std.conv;
import std.file;
import prs.compress;
import std.datetime.stopwatch;

void main()
{
	// Read
	byte[] original = cast(byte[])read("test.bin");

	// Compress
	void f0() 
	{ 
		byte[] prsFile = compress(original, 0x512); 
		std.file.write("testd.prs", prsFile);
	}

	// Benchmark
	auto durations = benchmark!(f0)(1);
	writeln(durations[0]);

	// Hang (testing)
	readln();
}
