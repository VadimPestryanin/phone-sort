using System.Collections.Concurrent;
using System.Text;

namespace sorter;

public class Sorter
{
    private string _sourceFileName;
    private string _resultFileName;

    private Dictionary<string,StreamWriter> _streamWriters;
    public Sorter(string sourceFileName,string resultFileName)
    {
        this._sourceFileName = sourceFileName;
        _resultFileName = resultFileName;
        _streamWriters = new Dictionary<string,StreamWriter>();
    }

    public void SplitSort()
    {
        string? line;
        using (StreamReader reader = new StreamReader(_sourceFileName))
        {
            string filePrefix;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    filePrefix = line.Substring(0, 6);
                    _streamWriters.TryGetValue(filePrefix, out var sw);
                    if (sw == null)
                    {
                        _streamWriters[filePrefix] = sw= new StreamWriter(GenerateName(filePrefix));
                    }
                 
                    sw.WriteLine(line);
                }
            }
            while(line != null);

            foreach (var sw in _streamWriters.Values)
            {
                sw.Dispose();
            }
        }
        
        //sort + merge
        var files = Directory.GetFiles(".", "+7*.txt").Select(x=>x.Substring(2)).OrderBy(x=>x).ToArray();

        using (var outputStream = File.Create(_resultFileName))
        {
            foreach (var file in files)
            {
                using var inputStream = File.OpenRead(file);
                inputStream.CopyTo(outputStream);
            }
        }
    }

    string GenerateName(string prefix) => $"{prefix}.txt";

}