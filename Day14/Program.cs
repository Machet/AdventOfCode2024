using System.Text.RegularExpressions;
using Utils;

var mapSize = new Vector(101, 103);

var robots = File.ReadAllLines("input.txt")
	.Select(ParseRobot)
	.ToList();

var robots100s = robots
	.Select(r => Simulate(r, 100, mapSize));

var hiddenImageAt = Enumerable.Range(1, 101 * 103)
	.Select(iteration => (Iteration: iteration, BiggestArea: EvaluateIterationContainsImage(iteration, robots, mapSize)))
	.OrderByDescending(x => x.BiggestArea)
	.FirstOrDefault();


Console.WriteLine("Result 1: " + CountSafetyLevel(robots100s, mapSize));
Console.WriteLine("Result 2: " + hiddenImageAt.Iteration);

static Robot ParseRobot(string line)
{
	var match = new Regex(@"p=(?<px>-?[0-9]*),(?<py>-?[0-9]*) v=(?<vx>-?[0-9]*),(?<vy>-?[0-9]*)").Match(line);

	return new Robot(
		new Point(int.Parse(match.Groups["px"].Value), int.Parse(match.Groups["py"].Value)),
		new Vector(int.Parse(match.Groups["vx"].Value), int.Parse(match.Groups["vy"].Value))
	);
}

static Robot Simulate(Robot robot, int seconds, Vector mapSize)
{
	var potentialPosition = robot.Position + robot.Velocity * seconds;
	var actualPosition = new Point(
		potentialPosition.X % mapSize.X >= 0 ? potentialPosition.X % mapSize.X : mapSize.X + (potentialPosition.X % mapSize.X),
		potentialPosition.Y % mapSize.Y >= 0 ? potentialPosition.Y % mapSize.Y : mapSize.Y + (potentialPosition.Y % mapSize.Y)
	);

	return new Robot(actualPosition, robot.Velocity);
}

static int CountSafetyLevel(IEnumerable<Robot> robots, Vector mapSize)
{
	var middleX = mapSize.X / 2;
	var middleY = mapSize.Y / 2;

	return robots
		.Select(r => r.Position)
		.Where(p => p.X != middleX && p.Y != middleY)
		.Select(p => (p.X < middleX, p.Y < middleY, p))
		.GroupBy(p => (p.Item1, p.Item2))
		.Select(g => g.Count())
		.Aggregate(1, (i, v) => i * v);
}

static int EvaluateIterationContainsImage(int iteration, List<Robot> robots, Vector mapSize)
{
	var positions = robots
		.Select(r => Simulate(r, iteration, mapSize))
		.Select(r => r.Position)
		.ToHashSet();

	var maxArea = 0;
	while (positions.Count > 0)
	{
		var regionFields = new HashSet<Point>();
		var toAnalyze = new Queue<Point>([positions.First()]);

		while (toAnalyze.Count != 0)
		{
			var current = toAnalyze.Dequeue();
			positions.Remove(current);
			if (!regionFields.Add(current))
			{
				continue;
			}

			var neighbors = current.GetNeighbours().Where(positions.Contains).ToList();
			toAnalyze.EnqueueRange(neighbors);
		}

		maxArea = Math.Max(maxArea, regionFields.Count);
	}

	return maxArea;
}

record Robot(Point Position, Vector Velocity);