﻿using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

namespace sorter;

public class Generator
{
    ConcurrentBag<string> _numbers;
    Random _random;
    private string _sourceFileName;
    
    char[] _chars = {'0','1','2','3','4','5','6','7','8','9'};

    private ConcurrentQueue<char[]> _arrayPool;

    public Generator(string sourceFileName)
    {
        _sourceFileName = sourceFileName;
        _random = new Random();
        _numbers = new ConcurrentBag<string>();
        _arrayPool = new ConcurrentQueue<char[]>();
        for (int i = 0; i < 1000; i++)
        {
            _arrayPool.Enqueue( new char[Constants.NUMBER_LENGTH]);
        }
    }
    public void Generate()
    {
        File.Delete(_sourceFileName);
        long iterator = 0;
        int threads = 1000;
        List<Action> functions = new List<Action>();
        
        for (int i = 0; i < threads; i++)
        {
            functions.Add(AddNumber);
        }
        using var  streamWriter =  File.AppendText(_sourceFileName);
        do
        {
            _numbers.Clear();

            Parallel.ForEach(functions, action => action());

            streamWriter.Write(string.Join('\n',  _numbers));
            streamWriter.Write('\n');
            iterator++;
        } while (iterator < 3000);
    }

    void AddNumber()
    {
        _numbers.Add( new string(GeneratePhoneNumber()));
    }
    char[] GeneratePhoneNumber()
    {
        if (_arrayPool.TryDequeue(out var array))
        {
            array[0] = '+';
            array[1] = '7';
            for (int i = 2; i < array.Length; i++)
            {
                array[i] = _chars[_random.Next(0, 9)];
            }
            _arrayPool.Enqueue(array);
            return array;
        }

        throw new InvalidOperationException("Pool is empty");

    }
    
 
}