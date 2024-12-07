using System.Collections.Immutable;

var input = File.ReadAllLines("input.txt").Select(Parse).ToList();
var result1 = input.Where(s => CanBeProduced(s, "+", "*")).Select(x => x.TestNumber).Sum();
var result2 = input.Where(s => CanBeProduced(s, "+", "*", "||")).Select(x => x.TestNumber).Sum();

Console.WriteLine("Result 1: " + result1);
Console.WriteLine("Result 2: " + result2);

static bool CanBeProduced(State startState, params string[] operators)
{
	var toProcess = new Stack<State>([startState]);
	var operations = GetOperationsToApply(operators).ToList();

	while (toProcess.Count > 0)
	{
		var state = toProcess.Pop();
		if (state.ToProcess.IsEmpty)
		{
			if (state.TestNumber == state.Calculated)
			{
				return true;
			}
			else
			{
				continue;
			}
		}

		if (state.TestNumber < state.Calculated)
		{
			continue;
		}

		var newToProces = state.ToProcess.Dequeue(out var number);

		foreach (var operation in operations)
		{
			toProcess.Push(new State(state.TestNumber, operation(state.Calculated, number), newToProces));
		}
	}

	return false;
}

static IEnumerable<Func<long, long, long>> GetOperationsToApply(string[] operators)
{
	if (operators.Contains("+"))
	{
		yield return (x, y) => x + y;
	}

	if (operators.Contains("*"))
	{
		yield return (x, y) => x * y;
	}

	if (operators.Contains("||"))
	{
		yield return (x, y) => long.Parse(x.ToString() + y);
	}
}

static State Parse(string line)
{
	var parts = line.Split(':');
	var testNumber = long.Parse(parts[0]);
	var toProcess = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
	return new State(testNumber, 0, ImmutableQueue.Create(toProcess));
}

record State(long TestNumber, long Calculated, ImmutableQueue<long> ToProcess);