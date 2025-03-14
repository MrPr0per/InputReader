namespace Main;

static class Program
{
    public static void Main()
    {
        var (n, m) = InputReader.Read<int, int>();
        var values = InputReader.Read(1 << n).Of<uint>();
        var requests = InputReader.Read(m).LinesOf<int, int>();

        Console.WriteLine();
        Console.WriteLine((n, m));
        Console.WriteLine($"[{string.Join(", ", values)}]");
        Console.WriteLine($"[\n    {string.Join(",\n    ", requests)}\n]");
    }
}