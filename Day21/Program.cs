using System.Collections.Immutable;
using Utils;

var numbersToFind = File.ReadAllLines("input.txt");
char[,] numeric = { { '7', '8', '9' }, { '4', '5', '6' }, { '1', '2', '3' }, { ' ', '0', 'A' } };
char[,] directional = { { ' ', '^', 'A' }, { '<', 'v', '>' } };
var numericPossiblePaths = GeneratePossiblePaths(numeric);
var directionalPossiblePaths = GeneratePossiblePaths(directional);
var directionalCostCache = directionalPossiblePaths.ToDictionary(x => (x.Key, 1), x => (long)x.Value.Select(y => y.Count).Min());

var complexities1 = numbersToFind.Select(n => GetComplexityForNumeric(n, numeric, 2)).ToList();
Console.WriteLine("Result 1: " + complexities1.Sum());

var complexities2 = numbersToFind.Select(n => GetComplexityForNumeric(n, numeric, 25)).ToList();
Console.WriteLine("Result 2: " + complexities2.Sum());

long GetComplexityForNumeric(IEnumerable<char> code, char[,] keypad, int robotCount)
{
	var currentPos = 'A';
	long sum = 0;
	foreach (var c in code)
	{
		var nupadPaths = numericPossiblePaths[new Path(currentPos, c)];
		var minComplexity = nupadPaths.Select(p => GetComplexityForDirectional(p, robotCount)).Min();
		currentPos = c;
		sum += minComplexity;
	}

	return sum * int.Parse(new string(code.Where(c => c != 'A').ToArray()));
}

long GetComplexityForDirectional(IEnumerable<char> code, int level)
{
	var currentPos = 'A';
	long sum = 0;
	foreach (var c in code)
	{
		var path = new Path(currentPos, c);
		var actions = directionalPossiblePaths[path];
		var buttonCost = directionalCostCache.TryGetValue((path, level), out var cachedCost)
			? cachedCost
			: actions.Select(a => GetComplexityForDirectional(a, level - 1)).Min();

		directionalCostCache[(path, level)] = buttonCost;
		currentPos = c;
		sum += buttonCost;
	}

	return sum;
}

static Dictionary<Path, List<ImmutableList<char>>> GeneratePossiblePaths(char[,] keypad)
{
	return keypad.SelectItems()
		.Select(i => i.Item)
		.Where(v => v != ' ')
		.GenerateAllPossibleCombinations()
		.Select(pair => new Path(pair.first, pair.second))
		.ToDictionary(p => p, p => GetNumpadPossiblePaths(p.From, p.To, keypad).ToList());
}

static List<ImmutableList<char>> GetNumpadPossiblePaths(char from, char to, char[,] keypad)
{
	var start = keypad.FindArrayItem(from).Single();
	var end = keypad.FindArrayItem(to).Single();
	var baseState = new SearchState(start.Point, end.Point, keypad, 0, ImmutableList<char>.Empty);
	var best = StateSearch.FindBestPaths<SearchState, int>(baseState);
	return best.Select(b => b.Path.Add('A')).ToList();
}

record SearchState(Point Position, Point Destination, char[,] keypad, int Length, ImmutableList<char> Path)
	: StateSearch.SearchState<int>
{
	public int Key => GetHashCode();

	public int ScoreHeuristic => Position.ManhattanDistance(Destination) + Path.Count * 2;

	public bool IsFinal => Position == Destination;

	public IEnumerable<StateSearch.SearchState<int>> GetNextStates()
	{
		return MapDirection.Main.Select(d => (val: d.ToArrowSign(), pos: Position.GetInDirection(d)))
			.Where(n => n.pos.IsWithin(keypad) && keypad[n.pos.X, n.pos.Y] != ' ')
			.Where(n => n.pos.ManhattanDistance(Destination) < Position.ManhattanDistance(Destination))
			.Select(n => new SearchState(n.pos, Destination, keypad, Length + 1, Path.Add(n.val)))
			.ToList();
	}
}

record Path(char From, char To);