namespace Utils;

public record IntRange(int start, int end)
{
	public bool Contains(int number) => start <= number && number <= end;
}
