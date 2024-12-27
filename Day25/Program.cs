using Utils;

var schematics = GetLockSchematics(File.ReadAllLines("input.txt")).ToList();
var keys = schematics.Where(i => i.IsKey).ToList();
var locks = schematics.Where(i => i.IsLock).ToList();

var matching = keys.SelectMany(k => locks.Select(l => IsMatching(k, l))).Where(r => r).ToList();

static bool IsMatching(Schematic key, Schematic @lock)
{
	for (int i = 0; i < key.PinHeights.Length; i++)
	{
		if (@lock.PinHeights[i] < key.PinHeights[i])
		{
			return false;
		}
	}

	return true;
}

Console.WriteLine("Result: " + matching.Count());

static IEnumerable<Schematic> GetLockSchematics(string[] lines)
{
	var temp = new List<string>();
	foreach (var line in lines)
	{
		if (line == "")
		{
			yield return ParseSchematic(temp.ToArray().ToCharArray());
			temp.Clear();
		}
		else
		{
			temp.Add(line);
		}
	}

	yield return ParseSchematic(temp.ToArray().ToCharArray());
}

static Schematic ParseSchematic(char[,] chars)
{
	var isLock = chars[0, 0] == '#';
	var toCount = isLock ? '.' : '#';
	var z = chars.SelectItems()
		.Where(x => x.Item == toCount)
		.GroupBy(x => x.Point.Y).ToList();

	var pinHeights = chars.SelectItems()
		.Where(x => x.Item == toCount)
		.GroupBy(x => x.Point.Y)
		.OrderBy(x => x.Key)
		.Select(g => g.Count() - 1)
		.ToArray();

	return new Schematic(isLock, pinHeights);
}

record Schematic(bool IsLock, int[] PinHeights)
{
	public bool IsKey => !IsLock;
	override public string ToString() => string.Join(",", PinHeights.Select(h => IsKey ? h : 6 - h));
};