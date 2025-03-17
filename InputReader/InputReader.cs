using System.Globalization;

public class ReaderSettings
{
    public Func<string> ReadLine { get; }
    // public TSeparator Separator { get; } // сделаю, когда впервые понадобится

    public ReaderSettings(Func<string> readLine)
    {
        ReadLine = readLine;
    }
}

public class InputReader
{
    public static InputReader FromConsole { get; } = new(new ReaderSettings(Console.ReadLine));

    private readonly ReaderSettings settings;

    public InputReader(ReaderSettings settings)
    {
        this.settings = settings;
    }

    # region Read<T1..Tn>

    public T Read<T>() => Convert<T>(settings.ReadLine());
    public (T1, T2) Read<T1, T2>() => ConvertToTuple<T1, T2>(settings.ReadLine().Split());
    public (T1, T2, T3) Read<T1, T2, T3>() => ConvertToTuple<T1, T2, T3>(settings.ReadLine().Split());
    public (T1, T2, T3, T4) Read<T1, T2, T3, T4>() => ConvertToTuple<T1, T2, T3, T4>(settings.ReadLine().Split());

    # endregion

    public T[] ReadArrayOf<T>()
    {
        return settings.ReadLine()
            .Split()
            .Select(Convert<T>)
            .ToArray();
    }

    public InputReaderContext Read(int count) => new(this, settings, count);

    public class InputReaderContext
    {
        private readonly InputReader inputReader;
        private readonly ReaderSettings settings;
        private readonly int count;

        public InputReaderContext(InputReader inputReader, ReaderSettings settings, int count)
        {
            this.inputReader = inputReader;
            this.count = count;
            this.settings = settings;
        }

        public T[] Of<T>()
        {
            var line = settings.ReadLine();
            var values = line.Split();
            if (values.Length != count)
                throw new ArgumentException($"Ожидалось {count} значений, а получено {values.Length}: {line}");
            return values
                .Select(Convert<T>)
                .ToArray();
        }

        # region LinesOf

        public T1[] LinesOf<T1>() => LazyLinesOf<T1>().ToArray();
        public (T1, T2)[] LinesOf<T1, T2>() => LazyLinesOf<T1, T2>().ToArray();
        public (T1, T2, T3)[] LinesOf<T1, T2, T3>() => LazyLinesOf<T1, T2, T3>().ToArray();
        public (T1, T2, T3, T4)[] LinesOf<T1, T2, T3, T4>() => LazyLinesOf<T1, T2, T3, T4>().ToArray();

        # endregion

        # region LazyLinesOf

        public IEnumerable<T1> LazyLinesOf<T1>() => Enumerable.Range(0, count)
            .Select(_ => inputReader.Read<T1>());

        public IEnumerable<(T1, T2)> LazyLinesOf<T1, T2>() => Enumerable.Range(0, count)
            .Select(_ => inputReader.Read<T1, T2>());

        public IEnumerable<(T1, T2, T3)> LazyLinesOf<T1, T2, T3>() => Enumerable.Range(0, count)
            .Select(_ => inputReader.Read<T1, T2, T3>());

        public IEnumerable<(T1, T2, T3, T4)> LazyLinesOf<T1, T2, T3, T4>() => Enumerable.Range(0, count)
            .Select(_ => inputReader.Read<T1, T2, T3, T4>());

        #endregion

        # region Commands

        public class ParametersParsers : Dictionary<string, Func<string[], object?>>
        // {имя_команды: параметры[] => кортеж_с_значениями либо null, если команда без параметров}
        {
        }

        /// <summary>
        /// Считывает массив команд разного формата в виде ("имяКоманды", (object)(кортежАргументов)))
        /// </summary>
        /// <param name="configure">Просто напишите ctx => ctx.</param>
        public (string, object?)[] Commands(Func<CommandBuilder, CommandBuilder> configure)
            => LazyCommands(configure).ToArray();

        /// <summary>
        /// Считывает ленивую коллекцию команд разного формата в виде ("имяКоманды", (object)(кортежАргументов)))
        /// </summary>
        /// <param name="configure">Просто напишите ctx => ctx.</param>
        public IEnumerable<(string, object?)> LazyCommands(Func<CommandBuilder, CommandBuilder> configure)
        {
            var parametersParsers = new ParametersParsers();
            var builder = new CommandBuilder(parametersParsers);
            configure(builder);

            for (var i = 0; i < count; i++)
            {
                var input = settings.ReadLine().Split();
                var commandName = input[0];

                if (parametersParsers.TryGetValue(commandName, out var parser))
                    yield return (commandName, parser(input.Skip(1).ToArray()));
                else
                    throw new InvalidOperationException($"Unknown command name: {commandName}");
            }
        }


        public class CommandBuilder
        {
            private readonly ParametersParsers parametersParsers;

            public CommandBuilder(ParametersParsers parametersParsers)
            {
                this.parametersParsers = parametersParsers;
            }

            public ParametersBuilder WithName(string name)
            {
                return new ParametersBuilder(
                    this,
                    parser => parametersParsers.Add(name, parser));
            }
        }

        public class ParametersBuilder
        {
            private readonly CommandBuilder parent;
            private readonly Action<Func<string[], object?>> addParser;

            public ParametersBuilder(CommandBuilder parent, Action<Func<string[], object?>> addParser)
            {
                this.addParser = addParser;
                this.parent = parent;
            }

            public CommandBuilder WithoutParameters()
            {
                addParser(_ => null);
                return parent;
            }

            public CommandBuilder WithParameterType<T1>()
            {
                addParser(parameters => Convert<T1>(parameters[0]));
                return parent;
            }

            public CommandBuilder WithParametersTypes<T1, T2>()
            {
                addParser(parameters => ConvertToTuple<T1, T2>(parameters));
                return parent;
            }

            public CommandBuilder WithParametersTypes<T1, T2, T3>()
            {
                addParser(parameters => ConvertToTuple<T1, T2, T3>(parameters));
                return parent;
            }
        }

        # endregion
    }

    # region Convert[ToTuple]

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

    private static T Convert<T>(string value) =>
        (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);

    #endregion
}