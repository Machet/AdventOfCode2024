using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var antenas = input
	.SelectItems()
	.Where(v => v.Item != '.');

var antinodes1 = antenas.GroupBy(v => v.Item)
	.SelectMany(group => group.GenerateAllCombinations())
	.SelectMany(pair => GenerateAntinodes1(pair.first.Point, pair.second.Point))
	.Where(p => p.IsWithin(input))
	.ToHashSet();

Console.WriteLine("Result 1: " + antinodes1.Count);

var antinodes2 = antenas.GroupBy(v => v.Item)
	.SelectMany(group => group.GenerateAllCombinations())
	.SelectMany(pair => GenerateAntinodes2(pair.first.Point, pair.second.Point, input))
	.ToHashSet();

Console.WriteLine("Result 2: " + antinodes2.Count);

static IEnumerable<Point> GenerateAntinodes1(Point first, Point second)
{
	var diff = second - first;
	yield return second + diff;
	yield return first - diff;
}

static IEnumerable<Point> GenerateAntinodes2(Point first, Point second, char[,] map)
{
	var diff = second - first;

	var iterator = first;
	while (iterator.IsWithin(map))
	{
		yield return iterator;
		iterator = iterator - diff;
	}

	iterator = second;
	while (iterator.IsWithin(map))
	{
		yield return iterator;
		iterator = iterator + diff;
	}
}
