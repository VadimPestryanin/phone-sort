// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using sorter;


void CleanupDirectory(string pattern)
{
    var files = Directory.GetFiles(".", pattern).ToArray();
    foreach (var file in files)
    {
        File.Delete(file);
    }
}



int ATTEMPTS = 15;
string testFile = "testfile.txt";
string resultfileName = "result.txt";
var generator = new Generator(testFile);

var results = new long[ATTEMPTS];

CleanupDirectory("*.txt");
Thread.Sleep(300);
Console.WriteLine($"Cleanup Complete");
var sw = new Stopwatch();
Console.WriteLine($"Generation started");
sw.Start();
generator.Generate();
Console.WriteLine($"Generation finished, elapsed time: {sw.ElapsedMilliseconds} ms");
sw.Stop();

for(int i =0;i< ATTEMPTS;i++){
    var sorter = new Sorter(testFile,resultfileName);
    CleanupDirectory("+7*.txt");
    sw.Restart();
    sorter.SplitSort();
    Console.WriteLine($"Run {i+1} finished, elapsed time: {sw.ElapsedMilliseconds} ms");
    results[i] = sw.ElapsedMilliseconds;
}
Console.WriteLine($"SplitSort AVG time : {results.Average()}");
sw.Stop();
