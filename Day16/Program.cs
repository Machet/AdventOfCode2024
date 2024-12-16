using System.Collections.Immutable;
using Utils;

var map = File.ReadAllLines("input.txt").ToCharArray();
var paths = map.FindArrayItem('.').Select(i => i.Point).ToHashSet();
var start = map.FindArrayItem('S').Single().Point;
var end = map.FindArrayItem('E').Single().Point;
paths.Add(end);

var initialState = new SearchState(new MapPosition(start, MapDirection.East), end, paths, 0, ImmutableHashSet<Point>.Empty);
var shortestPaths = StateSearch.FindBestPath<SearchState, MapPosition>(initialState);

Console.WriteLine("Result 1:" + shortestPaths.First().Score);
Console.WriteLine("Result 2:" + shortestPaths.SelectMany(p => p.Visited).Distinct().Count() + 1);

record MapPosition(Point Position, MapDirection Direction);

record SearchState(MapPosition MapPosition, Point FinalPosition, HashSet<Point> AvailablePaths, int Score, ImmutableHashSet<Point> Visited)
	: StateSearch.SearchState<MapPosition>
{
	public MapPosition Key => MapPosition;

	public bool IsFinal => MapPosition.Position == FinalPosition;

	public int ScoreHeuristic => Score + MapPosition.Position.ManhattanDistance(FinalPosition);

	public IEnumerable<StateSearch.SearchState<MapPosition>> GetNextStates()
	{
		var direction = MapPosition.Direction;
		var states = new List<SearchState>()
		{
			NextInDirection(direction, 1),
			NextInDirection(direction.Turn90L(), 1000 + 1),
			NextInDirection(direction.Turn90R(), 1000 + 1),
			NextInDirection(direction.Reverse(), 2000 + 1),
		};

		return states.Where(s => AvailablePaths.Contains(s.MapPosition.Position) && !Visited.Contains(s.MapPosition.Position));
	}

	private SearchState NextInDirection(MapDirection dir, int moveScore)
	{
		var nextPos = MapPosition.Position.GetInDirection(dir);
		return new SearchState(new MapPosition(nextPos, dir), FinalPosition, AvailablePaths, Score + moveScore, Visited.Add(nextPos));
	}
}