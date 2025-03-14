public static class InputReader
{
    public static T Read<T>() => Convert<T>(Console.ReadLine());
    public static (T1, T2) Read<T1, T2>() => ReadTuple<T1, T2>(Console.ReadLine().Split());
    public static (T1, T2, T3) Read<T1, T2, T3>() => ReadTuple<T1, T2, T3>(Console.ReadLine().Split());
    public static (T1, T2, T3, T4) Read<T1, T2, T3, T4>() => ReadTuple<T1, T2, T3, T4>(Console.ReadLine().Split());

    public static InputReaderContext Read(int count) => new(count);

    public class InputReaderContext
    {
        private readonly int count;

        public InputReaderContext(int count)
        {
            this.count = count;
        }

        public T[] Of<T>() => Console.ReadLine()
            .Split()
            .Take(count)
            .Select(Convert<T>)
            .ToArray();


        public T1[] LinesOf<T1>() => LazyLinesOf<T1>().ToArray();
        public (T1, T2)[] LinesOf<T1, T2>() => LazyLinesOf<T1, T2>().ToArray();
        public (T1, T2, T3)[] LinesOf<T1, T2, T3>() => LazyLinesOf<T1, T2, T3>().ToArray();
        public (T1, T2, T3, T4)[] LinesOf<T1, T2, T3, T4>() => LazyLinesOf<T1, T2, T3, T4>().ToArray();

        public IEnumerable<T1> LazyLinesOf<T1>() => Enumerable.Range(0, count)
            .Select(_ => InputReader.Read<T1>());

        public IEnumerable<(T1, T2)> LazyLinesOf<T1, T2>() => Enumerable.Range(0, count)
            .Select(_ => InputReader.Read<T1, T2>());

        public IEnumerable<(T1, T2, T3)> LazyLinesOf<T1, T2, T3>() => Enumerable.Range(0, count)
            .Select(_ => InputReader.Read<T1, T2, T3>());

        public IEnumerable<(T1, T2, T3, T4)> LazyLinesOf<T1, T2, T3, T4>() => Enumerable.Range(0, count)
            .Select(_ => InputReader.Read<T1, T2, T3, T4>());
    }

    private static (T1, T2) ReadTuple<T1, T2>(string[] values)
        => (Convert<T1>(values[0]), Convert<T2>(values[1]));

    private static (T1, T2, T3) ReadTuple<T1, T2, T3>(string[] values)
        => (Convert<T1>(values[0]), Convert<T2>(values[1]), Convert<T3>(values[2]));

    private static (T1, T2, T3, T4) ReadTuple<T1, T2, T3, T4>(string[] values)
        => (Convert<T1>(values[0]), Convert<T2>(values[1]), Convert<T3>(values[2]), Convert<T4>(values[3]));

    private static T Convert<T>(string value) => (T)System.Convert.ChangeType(value, typeof(T));
}