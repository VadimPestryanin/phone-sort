// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using sorter;


void CleanupDirectory()
{
    var files = Directory.GetFiles(".", "*.txt").ToArray();
    foreach (var file in files)
    {
        File.Delete(file);
    }
}



string testFile = "testfile.txt";
string resultfileName = "result.txt";
var generator = new Generator(testFile);
var sorter = new Sorter(testFile,resultfileName);

Console.WriteLine($"Initializing...");

CleanupDirectory();
Thread.Sleep(1000);
Console.WriteLine($"Initializing Complete");

var sw = new Stopwatch();

Console.WriteLine($"Generation started");
sw.Start();
generator.Generate();
Console.WriteLine($"Generation finished, elapsed time: {sw.Elapsed}");
sw.Stop();

Console.WriteLine($"SplitSort started");
sw.Start();
sorter.SplitSort();
Console.WriteLine($"SplitSort finished, elapsed time: {sw.Elapsed}");
sw.Stop();
