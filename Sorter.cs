using System.Collections.Concurrent;
using System.Text;

namespace sorter;

public class Sorter
{
    private string _sourceFileName;
    private string _resultFileName;

    private Dictionary<string,StreamWriter> _streamWriters;
    private Dictionary<string,FileStream> _fileStreams;
    public Sorter(string sourceFileName,string resultFileName)
    {
        this._sourceFileName = sourceFileName;
        _resultFileName = resultFileName;
        _streamWriters = new Dictionary<string,StreamWriter>();
        _fileStreams = new Dictionary<string,FileStream>();
    }


    private void Split_SR_SW()
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
    }

    private void Split_FS()
    {
        Span<byte> buffer = new byte[Constants.NUMBER_LENGTH + 1];//number length + LF
        int bytesRead = 1;
        string filePrefix = string.Empty;
     
        
        using (FileStream stream = new FileStream(_sourceFileName,FileMode.Open, FileAccess.Read))
        {
            do
            {
                bytesRead = stream.Read(buffer);
                if (bytesRead > 0 )
                {
                    filePrefix = Encoding.UTF8.GetString(buffer.Slice(0,6));
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
                sw.Close();
            }
        }
    }
    public void SplitSort()
    {
        Split_FS();
        
        //sort + merge
        var files = Directory.GetFiles(".", "+7*.txt").Select(x=>x.Substring(2)).OrderBy(x=>x);

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            Array.Sort(lines, StringComparer.OrdinalIgnoreCase);
            File.AppendAllLines(_resultFileName, lines);
        }

    }

    string GenerateName(string prefix) => $"{prefix}.txt";

}