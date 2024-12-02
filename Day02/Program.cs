
using System.Collections.Immutable;

var result1 = File.ReadAllLines("input.txt")
	.Select(GetInput)
	.Where(IsSafe)
	.Count();

Console.WriteLine("Result 1: " + result1);

var result2 = File.ReadAllLines("input.txt")
	.Select(GetInput)
	.Where(IsSafeWithToleration)
	.Count();

Console.WriteLine("Result 2: " + result2);

static ImmutableList<int> GetInput(string line)
{
	return line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select(int.Parse)
		.ToImmutableList();
}

static bool IsSafe(ImmutableList<int> list)
{
	var toCompare = list[0];
	var dir = list[1] > toCompare ? 'A' : 'D';

	for (int i = 1; i < list.Count; i++)
	{
		var diff = list[i] - toCompare;

		if (!CheckLevelValidity(dir, diff))
		{
			return false;
		}

		toCompare = list[i];
	}

	return true;
}

static bool IsSafeWithToleration(ImmutableList<int> list)
{
	for (int i = 0; i < list.Count; i++) 
	{
		if (IsSafe(list.RemoveAt(i)))
		{
			return true;
		}
	}

	return false;
}

static bool CheckLevelValidity(char dir, int diff)
{
	return Math.Abs(diff) >= 1 && Math.Abs(diff) <= 3
		&& ((dir == 'A' && diff > 0) || (dir == 'D' && diff < 0));
}

