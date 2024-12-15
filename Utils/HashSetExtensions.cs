namespace Utils;

public static class HashSetExtensions
{
	public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			set.Add(item);
		}
	}

	public static void RemoveRange<T>(this HashSet<T> set, IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			set.Remove(item);
		}
	}
}
