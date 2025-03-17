public static class InputReader
{
    public static T Read<T>() => Convert<T>(Console.ReadLine());
    public static (T1, T2) Read<T1, T2>() => ConvertToTuple<T1, T2>(Console.ReadLine().Split());
    public static (T1, T2, T3) Read<T1, T2, T3>() => ConvertToTuple<T1, T2, T3>(Console.ReadLine().Split());
    public static (T1, T2, T3, T4) Read<T1, T2, T3, T4>() => ConvertToTuple<T1, T2, T3, T4>(Console.ReadLine().Split());

    public static InputReaderContext Read(int count) => new(count);

    public class InputReaderContext
    {
        private readonly int count;

        public InputReaderContext(int count)
        {
            this.count = count;
        }

        public T[] Of<T>()
        {
            var line = Console.ReadLine();
            var values = line
                .Split()
                .Take(count)
                .Select(Convert<T>)
                .ToArray();
            if (values.Length != count)
                throw new ArgumentException($"Ожидалось {count} значений, а получено {values.Length}: {line}");
            return values;
        }


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


        /// <summary>
        /// Считывает массив команд разного формата в виде ("имяКоманды", (object)(кортежАргументов)))
        /// </summary>
        /// <param name="configure">Просто напишите ctx => ctx.</param>
        public (string, object)[] Commands(Func<CommandBuilder, CommandBuilder> configure)
            => LazyCommands(configure).ToArray();

        /// <summary>
        /// Считывает ленивую коллекцию команд разного формата в виде ("имяКоманды", (object)(кортежАргументов)))
        /// </summary>
        /// <param name="configure">Просто напишите ctx => ctx.</param>
        public IEnumerable<(string, object)> LazyCommands(Func<CommandBuilder, CommandBuilder> configure)
        {
            var builder = new CommandBuilder();
            configure(builder);

            for (var i = 0; i < count; i++)
            {
                var input = Console.ReadLine().Split();
                var commandName = input[0];

                if (builder.ParametersParsers.TryGetValue(commandName, out var parser))
                    yield return (commandName, parser(input.Skip(1).ToArray()));
                else
                    throw new InvalidOperationException($"Unknown command name: {commandName}");
            }
        }
    }

    public class CommandBuilder
    {
        public Dictionary<string, Func<string[], object>> ParametersParsers { get; } = new();

        public ParametersBuilder WithName(string name)
        {
            return new ParametersBuilder(name, this);
        }
    }

    public class ParametersBuilder
    {
        private readonly string commandName;
        private readonly CommandBuilder parent;

        public ParametersBuilder(string commandName, CommandBuilder parent)
        {
            this.commandName = commandName;
            this.parent = parent;
        }

        public CommandBuilder WithParameterType<T1>()
        {
            parent.ParametersParsers[commandName] = parameters => Convert<T1>(parameters[0]);
            return parent;
        }

        public CommandBuilder WithParametersTypes<T1, T2>()
        {
            parent.ParametersParsers[commandName] = parameters => ConvertToTuple<T1, T2>(parameters);
            return parent;
        }

        public CommandBuilder WithParametersTypes<T1, T2, T3>()
        {
            parent.ParametersParsers[commandName] = parameters => ConvertToTuple<T1, T2, T3>(parameters);
            return parent;
        }
    }

    private static (T1, T2) ConvertToTuple<T1, T2>(string[] values)
        => values.Length == 2
            ? (Convert<T1>(values[0]), Convert<T2>(values[1]))
            : throw new ArgumentException($"Неверное число значений: ожидалось 2, получено {values.Length}");

    private static (T1, T2, T3) ConvertToTuple<T1, T2, T3>(string[] values)
        => values.Length == 3
            ? (Convert<T1>(values[0]), Convert<T2>(values[1]), Convert<T3>(values[2]))
            : throw new ArgumentException($"Неверное число значений: ожидалось 3, получено {values.Length}");

    private static (T1, T2, T3, T4) ConvertToTuple<T1, T2, T3, T4>(string[] values)
        => values.Length == 4
            ? (Convert<T1>(values[0]), Convert<T2>(values[1]), Convert<T3>(values[2]), Convert<T4>(values[3]))
            : throw new ArgumentException($"Неверное число значений: ожидалось 4, получено {values.Length}");


    private static T Convert<T>(string value) => (T)System.Convert.ChangeType(value, typeof(T));
}