namespace Utils;

public static class StringExtensions
{
	public static List<int> ToIntList(this string str, char separator = ' ') => str.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToList();
	public static List<long> ToLongList(this string str) => str.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => long.Parse(i)).ToList();
}
