using Utils;

var map = File.ReadAllLines("input.txt").ToIntArray();

var distinctTrials = FindTrials(map, distinct: true);
var allTrials = FindTrials(map, distinct: false);

var result1 = distinctTrials.Select(t => t.ReachablePeeks.Count).Sum();
var result2 = allTrials.Count();

Console.WriteLine("Result 1: " + result1);
Console.WriteLine("Result 2: " + result2);


static IEnumerable<Trial> FindTrials(int[,] map, bool distinct)
{
	var peeks = map.FindArrayItem(9);
	var trials = peeks.Select(p => new Trial(p, [p])).ToList();

	for (int i = 8; i >= 0; i--)
	{
		trials = trials
			.SelectMany(t => map
				.GetNeighbourItems(t.Position.Point)
				.Where(tt => tt.Item == t.Position.Item - 1)
				.Select(tt => new Trial(tt, t.ReachablePeeks)))
			.ToList();

		if (distinct)
		{
			trials = trials
				.GroupBy(t => t.Position)
				.Select(g => new Trial(g.Key, g.SelectMany(t => t.ReachablePeeks).ToHashSet()))
				.ToList();
		}
	}

	return trials;
}

record Trial(ArrayItem<int> Position, HashSet<ArrayItem<int>> ReachablePeeks);
