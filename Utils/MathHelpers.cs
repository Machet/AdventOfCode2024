namespace Utils;

public static class MathHelpers
{
	static long GetGreatestCommonDenominator(IEnumerable<long> numbers)
	{
		return numbers.Aggregate(GetGCM);
	}

	static long GetGCM(long a, long b)
	{
		return b == 0 ? a : GetGCM(b, a % b);
	}

	public static long LeastCommonMultiple(this IEnumerable<int> values)
		=> LeastCommonMultiple(values.Select(c => (long)c));

	public static long LeastCommonMultiple(this IEnumerable<long> values)
	{
		var gcm = GetGreatestCommonDenominator(values);
		return values.Aggregate((a, b) => a / gcm * b);
	}

	public static IEnumerable<long> GetDivisorsOf(long n)
	{
		if (n == 0)
		{
			yield break;
		}

		for (long i = 1; i <= Math.Sqrt(Math.Abs(n)); i++)
		{
			if (n % i == 0)
			{
				yield return i;
				yield return -i;
				var second = n / i;

				if (i != second)
				{
					yield return second;
					yield return -second;
				}
			}
		}
	}
}
