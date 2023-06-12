using System.Collections.Concurrent;
using System.Text;

namespace sorter;

public class Sorter
{
    private string _sourceFileName;
    private string _resultFileName;

    //private Dictionary<string,StreamWriter> _streamWriters;
    private Dictionary<string,FileStream> _fileStreams;
    public Sorter(string sourceFileName,string resultFileName)
    {
        this._sourceFileName = sourceFileName;
        _resultFileName = resultFileName;
        //_streamWriters = new Dictionary<string,StreamWriter>();
        _fileStreams = new Dictionary<string,FileStream>();
    }

    private void SplitFiles()
    {
        Span<byte> buffer = new byte[Constants.NUMBER_LENGTH + 1];//number length + LF
        int bytesRead;
        string filePrefix = string.Empty;


        using FileStream stream = new FileStream(_sourceFileName,FileMode.Open, FileAccess.Read);
        do
        {
            bytesRead = stream.Read(buffer);
            if (bytesRead > 0 )
            {
                filePrefix = Encoding.UTF8.GetString(buffer.Slice(0,5));
                _fileStreams.TryGetValue(filePrefix, out var fs);
                if (fs == null)
                {
                    _fileStreams[filePrefix] = fs = new FileStream(GenerateName(filePrefix),FileMode.Append, FileAccess.Write);
                }
                 
                fs.Write(buffer);
            }
        }
        while(bytesRead  > 0);

        foreach (var sw in _fileStreams.Values)
        {
            sw.Dispose();
        }
    }
    public void SplitSort()
    {
        SplitFiles();

        //sort + merge
        var files = Directory.GetFiles(".", "+7*.txt").OrderBy(x=>x).ToList();

        int batchSize = 5;
        var batch = files.Take(batchSize).ToList();
        while (batch.Any())
        {
            Parallel.ForEach(batch, s =>
            {
                var lines = ReadAllLines(s);
                Array.Sort(lines, StringComparer.OrdinalIgnoreCase);
                File.WriteAllLines(s, lines);
            });
            
            files.RemoveRange(0,files.Count < batchSize ? files.Count : batchSize);
            batch = files.Take(batchSize).ToList();
        }
        
        using var outputStream = File.Create(_resultFileName);
        foreach (var file in files)
        {
            using var inputStream = File.OpenRead(file);
            inputStream.CopyTo(outputStream);
        }
    }

    string GenerateName(string prefix) => $"{prefix}.txt";
    
    public static string[] ReadAllLines(string path)
    {
        string? line;
        List<string> lines = new List<string>(1000);

        using StreamReader sr = new StreamReader(path, Encoding.UTF8);
        while ((line = sr.ReadLine()) != null)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }

   
}