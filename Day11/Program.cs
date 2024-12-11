var input = File.ReadAllText("input.txt")
	.Split(" ", StringSplitOptions.RemoveEmptyEntries)
	.Select(long.Parse)
	.ToList();

var cache = new Dictionary<State, long>();
var result1 = input.Select(value => GetLengthAfter(value, 25, cache)).Sum();
var result2 = input.Select(value => GetLengthAfter(value, 75, cache)).Sum();

Console.WriteLine("Result 1: " + result1);
Console.WriteLine("Result 2: " + result2);

long GetLengthAfter(long value, int iterations, Dictionary<State, long> cache)
{
	if (cache.ContainsKey(new State(value, iterations)))
	{
		return cache[new State(value, iterations)];
	}

	var newValues = Blink(value);
	var length = iterations == 1
		? newValues.Count()
		: newValues.Sum(v => GetLengthAfter(v, iterations - 1, cache));

	cache[new State(value, iterations)] = length;
	return length;
}

long[] Blink(long value)
{
	if (value == 0)
	{
		return [1];
	}

	var written = value.ToString();
	if (written.Length % 2 == 0)
	{
		var left = written.Substring(0, written.Length / 2);
		var right = written.Substring(written.Length / 2);
		return [long.Parse(left), long.Parse(right)];
	}

	return [value * 2024];
}

record State(long Value, int CountOfBlinks);