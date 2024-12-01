
var values = File.ReadAllLines("input.txt").Select(GetValues);
var values1 = values.Select(x => x.Val1).Order();
var values2 = values.Select(x => x.Val2).Order();

var result1 = values1.Zip(values2, (v1, v2) => Math.Abs(v1 - v2)).Sum();
Console.WriteLine(result1);

var result2 = values1.Select(v1 => v1 * values2.Count(v2 => v2 == v1)).Sum();
Console.WriteLine(result2);

Input GetValues(string line)
{
	var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
	return new Input(int.Parse(values[0]), int.Parse(values[1]));
}

record Input(int Val1, int Val2);