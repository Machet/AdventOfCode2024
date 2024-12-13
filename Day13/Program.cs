using System.Text.RegularExpressions;
using Utils;

var games = File.ReadAllLines("input.txt")
	.Split(4)
	.Select(ParseGame);

var gamesBig = games.Select(g => new Game(g.ButtonA, g.ButtonB, g.Prize + 10000000000000));

var results = games.Select(PlayGame).ToList();
var resultsBig = gamesBig.Select(PlayGame).ToList();

Console.WriteLine("Result 1: " + results.Sum());
Console.WriteLine("Result 2: " + resultsBig.Sum());

static long PlayGame(Game game)
{
	var a1 = (decimal)game.ButtonA.Y / game.ButtonA.X;
	var a2 = (decimal)game.ButtonB.Y / game.ButtonB.X;
	var b1 = game.Prize.Y - a1 * game.Prize.X;
	var crossX = b1 / (a2 - a1);
	var crossY = a2 * crossX;

	var timesBPress = (long)decimal.Round(crossX / game.ButtonB.X);
	var timesAPress = (long)decimal.Round((game.Prize.X - crossX) / game.ButtonA.X);

	if (game.ButtonA * timesAPress + game.ButtonB * timesBPress == game.Prize)
	{
		return 3 * timesAPress + timesBPress;
	}

	return 0;
}

static Game ParseGame(List<string> lines)
{
	var aMatch = new Regex(@"Button A: X\+(?<x>-?[0-9]*), Y\+(?<y>-?[0-9]*)").Match(lines[0]);
	var bMatch = new Regex(@"Button B: X\+(?<x>-?[0-9]*), Y\+(?<y>-?[0-9]*)").Match(lines[1]);
	var prizeMatch = new Regex("Prize: X=(?<x>-?[0-9]*), Y=(?<y>-?[0-9]*)").Match(lines[2]);

	var a = new Position(long.Parse(aMatch.Groups["x"].Value), long.Parse(aMatch.Groups["y"].Value));
	var b = new Position(long.Parse(bMatch.Groups["x"].Value), long.Parse(bMatch.Groups["y"].Value));
	var prize = new Position(long.Parse(prizeMatch.Groups["x"].Value), long.Parse(prizeMatch.Groups["y"].Value));

	return new Game(a, b, prize);
}

record Game(Position ButtonA, Position ButtonB, Position Prize);

record Position(long X, long Y)
{
	public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);
	public static Position operator +(Position a, long i) => new Position(a.X + i, a.Y + i);
	public static Position operator *(Position a, long x) => new Position(a.X * x, a.Y * x);
}