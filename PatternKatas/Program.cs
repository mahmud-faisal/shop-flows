// PatternKatas: one folder per pattern, each a standalone, framework-free demo.
// Run a kata with: dotnet run --project PatternKatas -- <PatternName>

var katas = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

if (args.Length == 0 || !katas.TryGetValue(args[0], out var run))
{
    Console.WriteLine("ShopFlows pattern katas");
    Console.WriteLine(katas.Count == 0
        ? "No katas yet. They are added day by day, each before its pattern is used in ShopFlows."
        : "Available: " + string.Join(", ", katas.Keys));
    return;
}

run();
