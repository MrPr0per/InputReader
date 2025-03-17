using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        Assert.That(10, Is.EqualTo(value));
    }

    [Test]
    public void Read_MultipleValues()
    {
        var reader = CreateReader("asdfasdf 5 3 2.5");
        var (s, n, m, k) = reader.Read<string, int, int, double>();
        Assert.That("asdfasdf", Is.EqualTo(s));
        Assert.That(5, Is.EqualTo(n));
        Assert.That(3, Is.EqualTo(m));
        Assert.That(2.5, Is.EqualTo(k));
    }

    [Test]
    public void ReadArrayOf_ValidInput()
    {
        var reader = CreateReader("0 1 2 1000000000");
        var values = reader.Read(4).Of<ulong>();
        CollectionAssert.AreEqual(new ulong[] { 0, 1, 2, 1000000000 }, values);
    }

    [Test]
    public void ReadArrayOf_InvalidCount()
    {
        var reader = CreateReader("1 2 3 4");
        Assert.Throws<ArgumentException>(() => reader.Read(5).Of<int>());
    }

    [Test]
    public void Read_LinesOf()
    {
        var reader = CreateReader("asdf 1", "asdd 2", "aswe 3");
        var lines = reader.Read(3).LinesOf<string, int>();
        CollectionAssert.AreEqual(new (string, int)[]
        {
            ("asdf", 1),
            ("asdd", 2),
            ("aswe", 3)
        }, lines);
    }

    [Test]
    public void Read_LazyLinesOf()
    {
        var reader = CreateReader("line1", "line2", "line3");
        var lazyLines = reader.Read(3).LazyLinesOf<string>();
        CollectionAssert.AreEqual(new[] { "line1", "line2", "line3" }, lazyLines.ToArray());
    }

    [Test]
    public void Read_Commands()
    {
        var reader = CreateReader("Help 0 10", "Update 0 5 asdfasdf", "Help -1 0", "Help 20 0");
        var commands = reader.Read(4).Commands(ctx => ctx
            .WithName("Help").WithParametersTypes<int, int>()
            .WithName("Update").WithParametersTypes<int, int, string>());

        CollectionAssert.AreEqual(new (string, object)[]
        {
            ("Help", (0, 10)),
            ("Update", (0, 5, "asdfasdf")),
            ("Help", (-1, 0)),
            ("Help", (20, 0))
        }, commands);
    }
}