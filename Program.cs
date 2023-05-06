using CountWords;

public partial class Program
{
    private static void Main(string[] args)
    {
        var w = new WordCounter(false, true);
        w.CountWords();
        Console.WriteLine("Работа программы завершена");
        Console.ReadKey();
    }
}