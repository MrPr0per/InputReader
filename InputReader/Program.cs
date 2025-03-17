// namespace Main;
//
// static class Program
// {
//     public static void Main()
//     {
//         var queryCount = InputReader.FromConsole.Read<int>();
//         var queries = InputReader.FromConsole.Read(queryCount).Commands(ctx => ctx
//             .WithName("Help").WithParametersTypes<int, int>()
//             .WithName("Update").WithParametersTypes<int, int, string>()
//         );
//         
//
//         //     InputReader.Read(10).LinesWithPrefixConditions(ctx => ctx
//         //     .When("Help").Then<int, int>()
//         //     .When("Update").Then<int, int, string>()
//         // );
//         //
//         //
//         // "Help": int, int,
//         // "Update": int, int, string
//         //     )
//
//
//         var (n, m) = InputReader.FromConsole.Read<int, int>();
//         var values = InputReader.FromConsole.Read(1 << n).Of<uint>();
//         var requests = InputReader.FromConsole.Read(m).LinesOf<int, int>();
//
//         Console.WriteLine();
//         Console.WriteLine((n, m));
//         Console.WriteLine($"[{string.Join(", ", values)}]");
//         Console.WriteLine($"[\n    {string.Join(",\n    ", requests)}\n]");
//     }
// }