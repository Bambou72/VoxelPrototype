using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
public class BenchmarkArrayVsDictionary
{
    private const int size = 4096;
    private int[] array;
    private Dictionary<int, int> dictionary;

    [GlobalSetup]
    public void Setup()
    {
        array = new int[size];
        dictionary = new Dictionary<int, int>(size);

        for (int i = 0; i < size; i++)
        {
            array[i] = i;
            dictionary[i] = i;
        }
    }

    [Benchmark]
    public long IterateArray()
    {
        long sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        return sum;
    }

    [Benchmark]
    public long IterateDictionary()
    {
        long sum = 0;
        foreach (var kvp in dictionary)
        {
            sum += kvp.Value;
        }
        return sum;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BenchmarkArrayVsDictionary>();
    }
}