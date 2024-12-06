using Utils;

var input = File.ReadAllLines("input.txt").ToCharArray();
var visitedByGuard = SimulateGuardRoute(input).Visited;

Console.WriteLine("result 1: " + visitedByGuard.Count);

var blockingPositionsCount = visitedByGuard
	.Where(p => input.GetItem(p).Item != '^')
	.Where(IsBlockedByObstacleAt)
	.Count();

Console.WriteLine("result 2: " + blockingPositionsCount);

bool IsBlockedByObstacleAt(Point obstaclePosition)
{
	var newMap = (char[,])input.Clone();
	newMap[obstaclePosition.X, obstaclePosition.Y] = '#';

	return !SimulateGuardRoute(newMap).Escaped;
}

(bool Escaped, List<Point> Visited) SimulateGuardRoute(char[,] map)
{
	var visited = new HashSet<GuardState>();
	var currentState = new GuardState(map.FindArrayItem('^').Single().Point, MapDirection.North);

	while (currentState.Position.IsWithin(map) && !visited.Contains(currentState))
	{
		visited.Add(currentState);

		var nextPos = currentState.Position.GetInDirection(currentState.Direction);
		var nextValue = map.FindItem(nextPos);
		if (nextValue == null || nextValue.Item == '.' || nextValue.Item == '^')
		{
			currentState = new GuardState(nextPos, currentState.Direction);
		}
		else
		{
			currentState = new GuardState(currentState.Position, currentState.Direction.Turn90R());
		}
	}

	return (!visited.Contains(currentState), visited.Select(v => v.Position).Distinct().ToList());
}

record GuardState(Point Position, MapDirection Direction);