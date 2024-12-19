
using System.Collections.Immutable;
using Utils;

var fallingBytes = File.ReadAllLines("input.txt").Select(Parse).ToList();

var shortestPath = FindPathAtNanosecond(1024, fallingBytes);
Console.WriteLine("Result 1: " + shortestPath!.Length);

var pathBrokenAt = FindFirstThatBreaks(1024, fallingBytes.Count, fallingBytes);

var byteWhichBroken = fallingBytes[pathBrokenAt - 1];
Console.WriteLine($"Result 2: {byteWhichBroken.X},{byteWhichBroken.Y}");

static int FindFirstThatBreaks(int rangeStart, int rangeEnd, List<Point> fallingBytes)
{
	if (rangeStart == rangeEnd)
	{
		return rangeStart;
	}

	var middle = (rangeStart + rangeEnd) / 2;
	return FindPathAtNanosecond(middle, fallingBytes) == null
		? FindFirstThatBreaks(rangeStart, middle, fallingBytes)
		: FindFirstThatBreaks(middle + 1, rangeEnd, fallingBytes);
}

static SearchState? FindPathAtNanosecond(int nanosecond, List<Point> fallingBytes)
{
	var mapSize = 70;
	var corruptedBytes = fallingBytes.Take(nanosecond).ToHashSet();
	var availablePaths = Enumerable.Range(0, mapSize + 1)
		.SelectMany(x => Enumerable.Range(0, mapSize + 1)
		.Select(y => new Point(x, y)))
		.Where(p => !corruptedBytes.Contains(p))
		.ToHashSet();

	var initialState = new SearchState(new Point(0, 0), new Point(mapSize, mapSize), availablePaths, 0, ImmutableHashSet<Point>.Empty);
	return StateSearch.FindBestPaths<SearchState, Point>(initialState).FirstOrDefault();
}

Point Parse(string line)
{
	var parts = line.Split(',');
	return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
}

record SearchState(Point Position, Point FinalPosition, HashSet<Point> AvailablePaths, int Length, ImmutableHashSet<Point> Visited)
	: StateSearch.SearchState<Point>
{
	public Point Key => Position;

	public bool IsFinal => Position == FinalPosition;

	public int ScoreHeuristic => Length + Position.ManhattanDistance(FinalPosition);

	public IEnumerable<StateSearch.SearchState<Point>> GetNextStates()
	{
		var states = Position.GetNeighbours()
			.Select(p => new SearchState(p, FinalPosition, AvailablePaths, Length + 1, Visited.Add(p)));

		return states.Where(s => AvailablePaths.Contains(s.Position) && !Visited.Contains(s.Position)).ToList();
	}
}