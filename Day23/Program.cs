
using System.Collections.Immutable;
using System.Linq;
using Utils;

var input = File.ReadAllLines("input.txt").Select(Parse);
var connections = input.Select(i => i.from).Concat(input.Select(i => i.to)).Distinct().ToDictionary(i => i, i => new HashSet<string>());

foreach (var connection in input)
{
	connections[connection.from].Add(connection.to);
	connections[connection.to].Add(connection.from);
}

var triplets = Distinct(input.SelectMany(i => FindConnected([i.from, i.to], connections)));
Console.WriteLine("Result 1: " + triplets.Where(t => t.Any(i => i.StartsWith("t"))).Count());

var current = triplets;
while(true)
{
	var next = current.SelectMany(t => FindConnected(t, connections).ToImmutableList()).ToList();
	if (next.Count == 0)
	{
		break;
	}

	current = Distinct(next).ToList();
}

var biggest = string.Join(",", current.First().OrderBy(i => i));
Console.WriteLine("Result 2: " + biggest);

(string from, string to) Parse(string line)
{
	var parts = line.Split("-");
	return (parts[0], parts[1]);
}

IEnumerable<ImmutableList<string>> FindConnected(ImmutableList<string> group, Dictionary<string, HashSet<string>> connections)
{
	var candidates = connections[group.First()].Except(group).ToList();
	foreach (var candidate in candidates)
	{
		if (group.All(i => connections[candidate].Contains(i)))
		{
			yield return group.Add(candidate);
		}
	}
}

IEnumerable<ImmutableList<string>> Distinct(IEnumerable<ImmutableList<string>> list)
{
	return list.DistinctBy(l => string.Join(",", l.OrderBy(i => i)));
}

