var input = File.ReadAllLines("input.txt");
var patterns = input[0].Split(",", StringSplitOptions.TrimEntries).ToLookup(c => c[0]);
var designs = input.Skip(2).ToList();

var cache = new Dictionary<string, long>();
var arrangementCounts = designs.Select(d => GetArragementCount(d, patterns, cache)).ToList();

Console.WriteLine("Result 1: " + arrangementCounts.Where(c => c != 0).Count());
Console.WriteLine("Result 2: " + arrangementCounts.Sum());

static long GetArragementCount(string design, ILookup<char, string> patterns, Dictionary<string, long> cache)
{
	if (design.Length == 0)
	{
		return 1;
	}

	if (cache.TryGetValue(design, out var result))
	{
		return result;
	}

	var matches = patterns[design[0]];
	long count = 0;
	foreach (var match in matches)
	{
		if (design.StartsWith(match) && design.Length >= match.Length)
		{
			var subCount = GetArragementCount(design[match.Length..], patterns, cache);
			count += subCount;
		}
	}

	cache[design] = count;
	return count;
}