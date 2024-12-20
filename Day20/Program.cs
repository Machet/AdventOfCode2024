using Utils;

var map = File.ReadAllLines("input.txt").ToCharArray();
var start = map.FindArrayItem('S').First().Point;
var end = map.FindArrayItem('E').First().Point;
var availablePlaces = map.FindArrayItem('.').Select(i => i.Point).Concat([start, end]).ToHashSet();

var costsFromStart = GetCosts(start, availablePlaces);
var costsFromEnd = GetCosts(end, availablePlaces);
var shortest = costsFromStart[end];

var bestSchortcuts2 = costsFromStart.Keys
	.SelectMany(s => GetCostWhenUsingShortcut2(s, 2, costsFromStart, costsFromEnd))
	.Select(s => shortest - s.Cost)
	.Where(i => i >= 100)
	.ToList();

Console.WriteLine("Result 1: " + bestSchortcuts2.Count);

var bestSchortcuts20 = costsFromStart.Keys
	.SelectMany(s => GetCostWhenUsingShortcut2(s, 20, costsFromStart, costsFromEnd))
	.Select(s => shortest - s.Cost)
	.Where(i => i >= 100)
	.ToList();

Console.WriteLine("Result 2: " + bestSchortcuts20.Count);


static List<Shortcut> GetCostWhenUsingShortcut2(Point start, int shortcutReach, Dictionary<Point, int> costsFromStart, Dictionary<Point, int> costsFromEnd)
{
	return start.GetArea(shortcutReach)
		.Where(costsFromEnd.ContainsKey)
		.Select(r => new Shortcut(start, r, costsFromStart[start] + costsFromEnd[r] + r.ManhattanDistance(start)))
		.ToList();
}


static Dictionary<Point, int> GetCosts(Point start, HashSet<Point> availablePlaces)
{
	var visited = new HashSet<Point>();
	var costs = new Dictionary<Point, int> { { start, 0 } };
	var queue = new Queue<Point>();
	queue.Enqueue(start);

	while (queue.Count > 0)
	{
		var current = queue.Dequeue();
		visited.Add(current);

		foreach (var next in current.GetNeighbours())
		{
			if (!availablePlaces.Contains(next) || visited.Contains(next))
			{
				continue;
			}

			if (!costs.ContainsKey(next))
			{
				costs[next] = costs[current] + 1;
				queue.Enqueue(next);
			}
		}
	}

	return costs;
}

record Shortcut(Point Start, Point End, int Cost);
