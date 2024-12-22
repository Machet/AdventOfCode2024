
using System.Collections.Immutable;

var numbers = File.ReadAllLines("input.txt").Select(long.Parse).ToList();
var result = numbers.Select(n => CalculateAfter(n, 2000)).Sum();
Console.WriteLine("Result 1: " + result);

var boughtBananas = new Dictionary<WatchedPrices, int>();
foreach (var number in numbers)
{
	WatchStock(number, 2000, boughtBananas);
}

Console.WriteLine("Result 2: " + boughtBananas.Values.Max());

static long CalculateAfter(long number, int iterations)
{
	for (int i = 0; i < iterations; i++)
	{
		number = CalculateNextNumber(number);
	}

	return number;
}

static long WatchStock(long number, int iterations, Dictionary<WatchedPrices, int> findings)
{
	var price = GetBananaPrice(number);
	var watchedPriceChanges = new WatchedPrices(ImmutableList<long>.Empty);
	var found = new HashSet<WatchedPrices>();
	for (int i = 0; i < iterations; i++)
	{
		number = CalculateNextNumber(number);
		var newPrice = GetBananaPrice(number);
		watchedPriceChanges = watchedPriceChanges.Next(newPrice - price);
		price = newPrice;

		if (watchedPriceChanges.IsFull && found.Add(watchedPriceChanges))
		{
			var bought = findings.GetValueOrDefault(watchedPriceChanges, 0);
			findings[watchedPriceChanges] = bought + (int)newPrice;
		}
	}

	return number;
}

static long CalculateNextNumber(long number)
{
	number = Prune(Mix(number, number * 64));
	number = Prune(Mix(number, number / 32));
	number = Prune(Mix(number, number * 2048));

	return number;
}

static long Mix(long number, long another) => number ^ another;
static long Prune(long number) => number % 16777216;
static long GetBananaPrice(long number) => number % 10;

class WatchedPrices
{
	public ImmutableList<long> PriceChanges { get; }
	public string StringValue { get; }
	public bool IsFull => PriceChanges.Count == 4;

	public WatchedPrices(ImmutableList<long> priceChanges)
	{
		PriceChanges = priceChanges;
		StringValue = string.Join(",", priceChanges);
	}

	public WatchedPrices Next(long newPrice)
	{
		return IsFull
			? new WatchedPrices(PriceChanges.RemoveAt(0).Add(newPrice))
			: new WatchedPrices(PriceChanges.Add(newPrice));
	}

	public override int GetHashCode() => StringValue.GetHashCode();
	public override string ToString() => StringValue;
	public override bool Equals(object? obj) => obj is WatchedPrices other && StringValue.Equals(other.StringValue);
}
