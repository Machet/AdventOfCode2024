namespace Utils;

public static class EnumerableExtensions
{
	public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> source, int count)
	{
		return source
			.Select((x, i) => new { Index = i, Value = x })
			.GroupBy(x => x.Index / count)
			.Select(x => x.Select(v => v.Value).ToList());
	}

	public static IEnumerable<T> IterateInCircles<T>(this IEnumerable<T> items)
	{
		while (true)
		{
			foreach (var item in items)
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<(T first, T second)> GenerateAllCombinations<T>(this IEnumerable<T> items)
	{
		var toIterate = items.ToList();
		for (int i = 0; i < toIterate.Count; i++)
		{
			for (var j = i + 1; j < toIterate.Count; j++)
			{
				yield return (toIterate[i], toIterate[j]);
			}
		}
	}

	public static void Print<T>(this IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			Console.WriteLine(item);
		}
	}
}
