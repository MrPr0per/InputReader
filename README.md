﻿# InputReader

`InputReader` — это С# проект, предоставляющий Fluent API для считывания ввода из задачек по программированию

## Установка

- Скопировать файл `InputReader.cs` себе в проект
- Профит

## Использование

Для считывания - вызывать у статического класса `InputReader` разные методы

Примечание: так как в большинстве задач используется чтение из консоли и пробел в качестве разделителя, то пока эти
параметры захардкожены и изменить их нельзя

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
var (n, m) = InputReader.Read<int, int>();
var values = InputReader.Read(1 << n).Of<uint>();
var requests = InputReader.Read(m).LazyLinesOf<int, int>();
```

## Полное* описание доступных команд

\* - возможно уже не полное

```csharp
// считывание константного числа значений разных типов из одной строки, например:
// >>> asdfasdf 5 3 2.5
var (s, n, m, k) = InputReader.Read<string, int, int, double>(); 
// (допускается и одно значение)
// >>> 10
var value = InputReader.Read<int>();

// считывание любого числа значений одного типа из одной строки 
// >>> 0 1 2 1000000000
var values1 = InputReader.ReadArrayOf<ulong>();
// считывание определенного числа значений одного типа из одной строки (если их там больше или меньше - ошибка)
// >>> 1 2 3 4 5 - ок
// >>> 1 2 3 4   - ошибка
var values2 = InputReader.Read(n).Of<ulong>();

// считывание переменного числа строк 
// >>> asdf 1
// ... asdd 2
// ... aswe 3
var lines = InputReader.Read(m).LinesOf<string, int>();
// ленивое считывание строк (то же самое, но IEnumerable вместо массива)
var lazyLines = InputReader.Read(m).LazyLinesOf<string>();
```

