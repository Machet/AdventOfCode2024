using System.Collections.Immutable;
using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var xmasesFound = SearchXmases(input);
Console.WriteLine("Result 1: " + xmasesFound);

var x_masesFound = SearchX_Mases(input);
Console.WriteLine("Result 2: " + x_masesFound);

static int SearchXmases(char[,] input)
{
	var starts = input.SelectItems()
		.Where(x => x.Item == 'X');

	var toCheck = ImmutableQueue.Create('M', 'A', 'S');
	var directions = MapDirection.All;

	var tasks = new Queue<SearchXmas>(starts.SelectMany(s => directions.Select(d => new SearchXmas(d, s.Point, toCheck))));
	var found = 0;

	while (tasks.Count > 0)
	{
		var task = tasks.Dequeue();
		var pointToCheck = task.NextPoint();
		var charToCheck = task.ToCheck.Peek();
		var remainingChars = task.ToCheck.Dequeue();
		var itemToCheck = input.FindItem(pointToCheck);

		if (itemToCheck?.Item != charToCheck)
		{
			continue;
		}

		if (remainingChars.IsEmpty)
		{
			found++;
		}
		else
		{
			tasks.Enqueue(new SearchXmas(task.Direction, pointToCheck, remainingChars));
		}
	}

	return found;
}

static int SearchX_Mases(char[,] input)
{
	return input.SelectItems()
		.Where(x => x.Item == 'A')
		.Where(x => HasMasAt(input, x.Point))
		.Count();
}

static bool HasMasAt(char[,] input, Point point)
{
	var diagonal1 = input.SelectItems([point.GetNorthWestOf(), point.GetSouthEastOf()]);
	var diagonal2 = input.SelectItems([point.GetNorthEastOf(), point.GetSouthWestOf()]);
	return diagonal1.Any(i => i.Item == 'M')
		&& diagonal1.Any(i => i.Item == 'S')
		&& diagonal2.Any(i => i.Item == 'M')
		&& diagonal2.Any(i => i.Item == 'S');
}

record SearchXmas(MapDirection Direction, Point Place, ImmutableQueue<char> ToCheck)
{
	public Point NextPoint() => Place.GetInDirection(Direction);
}
