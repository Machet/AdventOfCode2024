using System.Data;
using System.Text.RegularExpressions;

var input = File.ReadAllText("inputt.txt");

var result1 = SumMuls(input);
Console.WriteLine("Result 1: " + result1);

var result2 = SumMuls(ReplaceDonts(input));
Console.WriteLine("Result 2: " + result2);

int SumMuls(string input)
{
	var template = new Regex("mul\\((?<x>-?[0-9]*),(?<y>-?[0-9]*)\\)");
	return template.Matches(input)
		.Select(m => int.Parse(m.Groups["x"].Value) * int.Parse(m.Groups["y"].Value))
		.Sum();
}

string ReplaceDonts(string input)
{
	var template = new Regex("don't\\(\\)([\\s\\S]*?)do\\(\\)");
	var result = template.Matches(input);
	return template.Replace(input, string.Empty);
}