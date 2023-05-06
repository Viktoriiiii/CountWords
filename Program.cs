using BenchmarkDotNet.Attributes;
using CountWords;

public partial class Program
{
    [Benchmark]
    private static void Main(string[] args)
    {
        var w = new WordCounter(false, true);
        w.CountWords();
        Console.WriteLine("Завершено");
        Console.ReadKey();
    }
}