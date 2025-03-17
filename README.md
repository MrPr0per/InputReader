# InputReader

`InputReader` — это С# проект, предоставляющий Fluent API для считывания ввода из задачек по программированию

## Установка

- Скопировать файл `InputReader.cs` себе в проект 
  или его содержимое себе в файл с кодом, перенеся using-и в начало файла
- Профит

## Использование

Для считывания - вызывать у `InputReader.FromConsole`
или у `new InputReader(<функцияСоздающаяСтроки>)` разные методы

Примечание: пока в качестве разделителя захардкожен пробел
Т.е. чтобы, например, считать 3 инта нужно ввести "1 2 3"

## Пример использования

Формат ввода:

```markdown
В первой строке записаны два числа n и m (1≤n≤17,1≤m≤105).
Во второй строке записаны 2^n целых чисел a1,a2,…,a^2n (0≤ai<2^30).
В следующих m строках записаны запросы.
В i-ой строке записаны целые числа pi, bi (1≤pi≤2^n, 0≤bi<2^30) — это i-ый запрос.
```

Как считать:

```csharp
var (n, m) = InputReader.FromConsole.Read<int, int>();
var values = InputReader.FromConsole.Read(1 << n).Of<uint>();
var requests = InputReader.FromConsole.Read(m).LazyLinesOf<int, int>();
```

## Полное* описание доступных команд

\* - возможно уже не полное
примеры использования так же можно посмотреть в `InputReaderTests.cs`

```csharp
// считывание константного числа значений разных типов из одной строки, например:
// >>> asdfasdf 5 3 2.5
var (s, n, m, k) = InputReader.FromConsole.Read<string, int, int, double>(); 
// (допускается и одно значение)
// >>> 10
var value = InputReader.FromConsole.Read<int>();

// считывание любого числа значений одного типа из одной строки 
// >>> 0 1 2 1000000000
var values1 = InputReader.FromConsole.ReadArrayOf<ulong>();

// считывание определенного числа значений одного типа из одной строки (если их там больше или меньше - ошибка)
// например 5 интов:
// >>> 1 2 3 4 5 6 - ошибка
// >>> 1 2 3 4 5   - ок
// >>> 1 2 3 4     - ошибка
var values2 = InputReader.FromConsole.Read(5).Of<int>();

// считывание переменного числа строк 
// >>> asdf 1
// ... asdd 2
// ... aswe 3
var lines = InputReader.FromConsole.Read(m).LinesOf<string, int>();
// ленивое считывание строк (то же самое, но IEnumerable вместо массива)
var lazyLines = InputReader.FromConsole.Read(m).LazyLinesOf<string>();

// считывание запросов разного формата
var queries = InputReader.FromConsole.Read(5).Commands(ctx => ctx // или LazyCommands вместо Commands
    .WithName("Help").WithParametersTypes<int, int>()
    .WithName("Update").WithParametersTypes<int, int, string>()
    .WithName("Exit").WithoutParameters()
);
// >>> Help 0 10
// ... Update 0 5 asdfasdf
// ... Help -1 0
// ... Help 20 0
// ... Exit
```

