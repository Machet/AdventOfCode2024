using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

var toAnalyze = new Queue<ArrayItem<char>>([map.GetItem(0, 0)]);
var inspected = new HashSet<ArrayItem<char>>();
var regionsFound = new List<Region>();

while (toAnalyze.Count > 0)
{
	var current = toAnalyze.Dequeue();
	if (inspected.Contains(current))
	{
		continue;
	}

	var (region, touching) = FindRegion(current, map);

	regionsFound.Add(region);
	inspected.AddRange(region.Fields);
	toAnalyze.EnqueueRange(touching);
}

Console.WriteLine("Result 1:" + regionsFound.Select(r => r.Perimeter * r.Fields.Count).Sum());
Console.WriteLine("Result 2:" + regionsFound.Select(r => CalculateFenceCost(r, map)).Sum());

(Region region, HashSet<ArrayItem<char>> touching) FindRegion(ArrayItem<char> regionStart, char[,] map)
{
	var touching = new HashSet<ArrayItem<char>>();
	var regionFields = new HashSet<ArrayItem<char>>();
	var perimeter = 0;

	var toAnalyze = new Queue<ArrayItem<char>>([regionStart]);
	while (toAnalyze.Count != 0)
	{
		var current = toAnalyze.Dequeue();
		if (!regionFields.Add(current))
		{
			continue;
		}

		var neighbors = map.GetNeighbourItems(current.Point).ToList();
		var matching = neighbors.Where(n => n.Item == regionStart.Item).ToList();
		var other = neighbors.Where(n => n.Item != regionStart.Item).ToList();

		toAnalyze.EnqueueRange(matching);
		touching.AddRange(other);
		perimeter += (4 - matching.Count);
	}

	return (new Region(regionFields.ToList(), perimeter), touching);
}

int CalculateFenceCost(Region region, char[,] map)
{
	var points = region.Fields.Select(f => f.Point).ToList();
	var fencesR = points.GroupBy(p => p.X).SelectMany(SelectFencesRow).ToList();
	var fencesCostR = fencesR.GroupBy(f => (f.Direction, f.Point.Y)).Select(g => GetCostRow(g.ToList()));
	var fencesC = points.GroupBy(p => p.Y).SelectMany(SelectFencesColumn).ToList();
	var fencesCostC = fencesC.GroupBy(f => (f.Direction, f.Point.X)).Select(g => GetCostColumn(g.ToList()));
	var discountedPerimeter = fencesCostR.Sum() + fencesCostC.Sum();
	return discountedPerimeter * region.Fields.Count;
}

IEnumerable<FencePostition> SelectFencesRow(IEnumerable<Point> fences)
{
	var row = fences.OrderBy(row1 => row1.Y).ToList();

	for (int i = 0; i < row.Count; i++)
	{
		if (i == 0 || row[i].Y - 1 != row[i - 1].Y)
		{
			yield return new FencePostition(row[i], MapDirection.West);
		}

		if (i == row.Count - 1 || row[i].Y + 1 != row[i + 1].Y)
		{
			yield return new FencePostition(row[i], MapDirection.East);
		}
	}
}

IEnumerable<FencePostition> SelectFencesColumn(IEnumerable<Point> fences)
{
	var column = fences.OrderBy(row1 => row1.X).ToList();

	for (int i = 0; i < column.Count; i++)
	{
		if (i == 0 || column[i].X - 1 != column[i - 1].X)
		{
			yield return new FencePostition(column[i], MapDirection.North);
		}

		if (i == column.Count - 1 || column[i].X + 1 != column[i + 1].X)
		{
			yield return new FencePostition(column[i], MapDirection.South);
		}
	}
}

int GetCostRow(List<FencePostition> fences)
{
	Point? lastPos = null;
	int cost = 0;
	foreach (var fence in fences.OrderBy(f => f.Point.X))
	{
		if (fence.Point.X - 1 != lastPos?.X)
		{
			cost++;
		}

		lastPos = fence.Point;
	}

	return cost;
}

int GetCostColumn(List<FencePostition> fences)
{
	Point? lastPos = null;
	int cost = 0;
	foreach (var fence in fences.OrderBy(f => f.Point.Y))
	{
		if (fence.Point.Y - 1 != lastPos?.Y)
		{
			cost++;
		}

		lastPos = fence.Point;
	}

	return cost;
}

record Region(List<ArrayItem<char>> Fields, int Perimeter);
record FencePostition(Point Point, MapDirection Direction);