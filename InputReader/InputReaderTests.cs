using NUnit.Framework;

[TestFixture]
public class InputReaderTests
{
    private static InputReader CreateReader(params string[] inputs)
    {
        var queue = new Queue<string>(inputs);
        return new InputReader(new ReaderSettings(() => queue.Dequeue()));
    }

    [Test]
    public void Read_SingleValue()
    {
        var reader = CreateReader("10");
        var value = reader.Read<int>();
        Assert.That(value, Is.EqualTo(10));
    }

    [Test]
    public void Read_MultipleValues()
    {
        var reader = CreateReader("asdfasdf 5 3 2.5");
        var (s, n, m, k) = reader.Read<string, int, int, double>();
        Assert.That(s, Is.EqualTo("asdfasdf"));
        Assert.That(n, Is.EqualTo(5));
        Assert.That(m, Is.EqualTo(3));
        Assert.That(k, Is.EqualTo(2.5));
    }

    [Test]
    public void Read_VariableValues()
    {
        var reader = CreateReader("0 1 2 1000000000");
        var values = reader.Read(4).Of<ulong>();
        Assert.That(values, Is.EqualTo(new ulong[] { 0, 1, 2, 1000000000 }));
    }

    [Test]
    public void ReadArrayOf_ValidInput()
    {
        var reader = CreateReader("0 1 2 1000000000 0");
        var values = reader.ReadArrayOf<long>();
        Assert.That(values, Is.EqualTo(new ulong[] { 0, 1, 2, 1000000000, 0 }));
    }


    [Test]
    public void ReadArrayOf_InvalidCount()
    {
        var reader = CreateReader("1 2 3 4");
        Assert.Throws<ArgumentException>(() => reader.Read(5).Of<int>());
    }

    [Test]
    public void ReadArrayOf_InvalidFormat()
    {
        var reader = CreateReader("1 a 3 4");
        Assert.Throws<FormatException>(() => reader.Read(4).Of<int>());
    }

    [Test]
    public void Read_LinesOf()
    {
        var reader = CreateReader("asdf 1", "asdd 2", "aswe 3");
        var lines = reader.Read(3).LinesOf<string, int>();
        Assert.That(lines, Is.EqualTo(new[]
        {
            ("asdf", 1),
            ("asdd", 2),
            ("aswe", 3)
        }));
    }

    [Test]
    public void Read_LazyLinesOf()
    {
        var reader = CreateReader("line1", "line2", "line3");
        var lazyLines = reader.Read(3).LazyLinesOf<string>();
        Assert.That(lazyLines.ToArray(), Is.EqualTo(new[] { "line1", "line2", "line3" }));
    }

    [Test]
    public void Read_Commands()
    {
        var reader = CreateReader(
            "Help 0 10",
            "Update 0 5 asdfasdf",
            "Help -1 0",
            "Help 20 0",
            "Exit");
        var commands = reader.Read(5).Commands(ctx => ctx
            .WithName("Help").WithParametersTypes<int, int>()
            .WithName("Update").WithParametersTypes<int, int, string>()
            .WithName("Exit").WithoutParameters()
        );

        Assert.That(commands, Is.EqualTo(new (string, object?)[]
        {
            ("Help", (0, 10)),
            ("Update", (0, 5, "asdfasdf")),
            ("Help", (-1, 0)),
            ("Help", (20, 0)),
            ("Exit", null)
        }));
    }

    [Test]
    public void Read_Commands_InvalidCommand()
    {
        var reader = CreateReader("Unknown 1 2");
        Assert.Throws<InvalidOperationException>(() => reader.Read(1).Commands(ctx => ctx
            .WithName("Help").WithParametersTypes<int, int>()
            .WithName("Update").WithParametersTypes<int, int, string>()));
    }

    [Test]
    public void Read_Commands_InvalidParameters()
    {
        var reader = CreateReader("Help 1 a");
        Assert.Throws<FormatException>(() => reader.Read(1).Commands(ctx => ctx
            .WithName("Help").WithParametersTypes<int, int>()));
    }

    [Test]
    public void Read_Commands_InvalidCountOfElements()
    {
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(-1).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(0).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(1).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(2).Of<string>());
        Assert.DoesNotThrow(() => CreateReader("a b c").Read(3).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(4).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(5).Of<string>());
        Assert.Throws<ArgumentException>(() => CreateReader("a b c").Read(6).Of<string>());

        // строк не может быть слишком много, в отличие от значений в строке
        // да и сликом мало как будто бы тоже быть не может
        // // reader = CreateReader("1 2", "3 4", "5 6");
        // // Assert.Throws<ArgumentException>(() => reader.Read(2).LinesOf<int, ushort>());
        // reader = CreateReader("1 2", "3 4", "5 6");
        // Assert.Throws<ArgumentException>(() => reader.Read(4).LinesOf<int, ushort>());
        // // reader = CreateReader("1 2", "3 4", "5 6");
        // // Assert.Throws<ArgumentException>(() => reader.Read(2).LazyLinesOf<int, int>().ToArray());
        // reader = CreateReader("1 2", "3 4", "5 6");
        // Assert.Throws<ArgumentException>(() => reader.Read(4).LazyLinesOf<int, int>().ToArray());
        //
        // // reader = CreateReader("1 2", "3 4", "5 6");
        // // Assert.Throws<ArgumentException>(() => reader.Read(2).Commands(ctx => ctx
        // //     .WithName("1").WithParameterType<int>()));
        // reader = CreateReader("1 2", "3 4", "5 6");
        // Assert.Throws<ArgumentException>(() => reader.Read(4).Commands(ctx => ctx
        //     .WithName("1").WithParameterType<int>()));
    }
}