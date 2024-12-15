using System.Diagnostics;
using Utils;

var input = File.ReadAllLines("input.txt");
var map = input.TakeWhile(line => !string.IsNullOrWhiteSpace(line)).ToArray().ToCharArray();
var moves = input.SkipWhile(line => !string.IsNullOrWhiteSpace(line)).Skip(1).SelectMany(c => c).ToArray();

var boxes1 = map.FindArrayItem('O').Select(i => new Box([i.Point])).ToList();
var walls1 = map.FindArrayItem('#').Select(i => i.Point).ToHashSet();
var robot1 = map.FindArrayItem('@').Single().Point;
var mapSize1 = new Vector(map.GetLength(0), map.GetLength(1));

var boxes2 = boxes1.SelectMany(r => r.Points).Select(i => new Box([new Point(i.X, i.Y * 2), new Point(i.X, i.Y * 2 + 1)])).ToList();
var walls2 = walls1.SelectMany(i => new List<Point> { new Point(i.X, i.Y * 2), new Point(i.X, i.Y * 2 + 1) }).ToHashSet();
var robot2 = new Point(robot1.X, robot1.Y * 2);
var mapSize2 = new Vector(map.GetLength(0), map.GetLength(1) * 2);

var warehouseState1 = PerformMoves(robot1, boxes1, walls1, moves);
Console.WriteLine("Result 1: " + warehouseState1.Select(r => r.GetGps()).Sum());

var warehouseState2 = PerformMoves(robot2, boxes2, walls2, moves);
Console.WriteLine("Result 2: " + warehouseState2.Select(r => r.GetGps()).Sum());

List<Box> PerformMoves(Point robot, List<Box> boxes, HashSet<Point> walls, char[] moves)
{
	foreach (var move in moves)
	{
		var direction = MapDirection.FromArrowSign(move);
		var (newRobot, newBoxes) = FindPushedBoxes(boxes, robot, direction);
		if (!walls.Contains(newRobot) && !newBoxes.Any(r => r.Collide(walls)))
		{
			boxes = newBoxes;
			robot = newRobot;
		}
	}

	return boxes;
}

(Point newRobot, List<Box> newRocks) FindPushedBoxes(List<Box> boxes, Point robotPosition, MapDirection direction)
{
	var newPosition = robotPosition.GetInDirection(direction);
	var staticBoxes = boxes.ToList();
	var boxesToMove = boxes.Where(r => r.Collide(newPosition)).ToHashSet();
	var movedBoxes = new List<Box>();

	while (boxesToMove.Count > 0)
	{
		var boxToMove = boxesToMove.First();
		var newBoxes = boxesToMove.Select(b => b.Move(direction)).ToList();

		staticBoxes = staticBoxes.Except(boxesToMove).ToList();
		movedBoxes.AddRange(newBoxes);
		boxesToMove = staticBoxes.Where(b => newBoxes.Any(nb => b.Collide(nb))).ToHashSet();
	}

	return (newPosition, staticBoxes.Concat(movedBoxes).ToList());
}

record Box(HashSet<Point> Points)
{
	public bool Collide(Point point) => Collide([point]);
	public bool Collide(Box other) => Collide(other.Points);
	public bool Collide(IEnumerable<Point> points) => Points.Any(p => points.Contains(p));
	public bool Collide(HashSet<Point> points) => Points.Any(points.Contains);
	public Box Move(MapDirection direction) => new Box(Points.Select(p => p.GetInDirection(direction)).ToHashSet());
	public int GetGps() => Points.First().X * 100 + Points.First().Y;
}