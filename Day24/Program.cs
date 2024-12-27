using System.Collections.Immutable;
using Utils;

var lines = File.ReadAllLines("input.txt");
var values = lines.TakeWhile(l => l != "").Select(ParseValue).ToDictionary(v => v.input, v => v.value);
var connections = lines.SkipWhile(l => l != "").Skip(1).Select(ParseConnection).ToDictionary(c => c.output, c => c.connection);
var outputKeys = connections.Keys.Where(v => v.StartsWith("z")).OrderByDescending(GetPosition).ToList();

var outputs = outputKeys.Select(z => GetValue(z, connections, values)).ToList();
var result = ConvertToInt(outputs);

Console.WriteLine("Result 1: " + result);

// output(n) = x(n) xor y(n) xor carryOver(n-1)
// carryOver(n) = (x(n) and y(n)) or (x(n) xor y(n) and carryOver(n-1))
var connectionsToFix = connections.ToImmutableDictionary(c => c.Key, c => c.Value);
var validConnections = new HashSet<string>();
var validationResults = outputKeys.Select(o => ValidateOutput(o, connections)).ToList();
var fixes = new HashSet<string>();

foreach (var key in outputKeys.OrderBy(GetPosition))
{
	var validationResult = ValidateOutput(key, connectionsToFix);
	if (validationResult.IsValid)
	{
		continue;
	}

	var matching = connections.Where(c => c.Value.Operation == validationResult.Expecting);
	foreach(var match in matching)
	{
		var potential = SwapConnections(validationResult.InvalidConnection, match.Key, connectionsToFix);
		var potentialResult = ValidateOutput(key, potential);

		if (potentialResult.IsValid)
		{
			connectionsToFix = potential;
			fixes.Add(match.Key);
			fixes.Add(validationResult.InvalidConnection);
			break;
		}
	}
}
Console.WriteLine("Result 2: " + string.Join(",", fixes.OrderBy(f => f)));

static ValidationResult ValidateOutput(string connectionName, IDictionary<string, Connection> connections)
{
	var mainConnection = connections[connectionName];
	var inputLevel = GetPosition(connectionName);
	var xInput = InputName("x", inputLevel);
	var yInput = InputName("y", inputLevel);

	if (inputLevel == 0)
	{
		return mainConnection.IsXorOf("x00", "y00") ? ValidationResult.Valid : ValidationResult.Invalid(connectionName, "XOR", 0);
	}

	if (mainConnection.Operation != "XOR" || mainConnection.HasXYParameters)
	{
		return ValidationResult.Invalid(connectionName, "XOR", 0);
	}

	var left = connections[mainConnection.Left];
	var right = connections[mainConnection.Right];

	if (left.IsXorOf(xInput, yInput))
	{
		return ValidateCarryOver(inputLevel - 1, mainConnection.Right, connections).AddScore(1);
	}

	if (right.IsXorOf(xInput, yInput))
	{
		return ValidateCarryOver(inputLevel - 1, mainConnection.Left, connections).AddScore(1);
	}

	return ValidateCarryOver(inputLevel - 1, mainConnection.Left, connections).IsValid
		? ValidationResult.Invalid(mainConnection.Right, $"XOR", 1)
		: ValidateCarryOver(inputLevel - 1, mainConnection.Right, connections).IsValid
			? ValidationResult.Invalid(mainConnection.Left, $"XOR", 1)
			: ValidationResult.Invalid(connectionName, $"XOR", 0);
}

static ValidationResult ValidateCarryOver(int n, string connectionName, IDictionary<string, Connection> connections)
{
	var connection = connections[connectionName];

	if (n == 0)
	{
		return connection.IsAndOf("x00", "y00") ? ValidationResult.Valid : ValidationResult.Invalid(connectionName, "AND", 0);
	}

	if (connection.Operation != "OR")
	{
		return ValidationResult.Invalid(connectionName, "OR", 0);
	}

	var xInput = InputName("x", n);
	var yInput = InputName("y", n);
	var left = connections[connection.Left];
	var right = connections[connection.Right];

	if (left.IsAndOf(xInput, yInput))
	{
		return ValidateCarryOverLvl2(n, connection.Right, connections).AddScore(n);
	}

	if (right.IsAndOf(xInput, yInput))
	{
		return ValidateCarryOverLvl2(n, connection.Left, connections).AddScore(n);
	}

	return ValidateCarryOverLvl2(n, connection.Left, connections).IsValid
	? ValidationResult.Invalid(connection.Right, $"AND", n)
	: ValidateCarryOverLvl2(n, connection.Right, connections).IsValid
		? ValidationResult.Invalid(connection.Left, $"AND", n)
		: ValidationResult.Invalid(connectionName, $"OR", 0);
}

static ValidationResult ValidateCarryOverLvl2(int n, string connectionName, IDictionary<string, Connection> connections)
{
	var connection = connections[connectionName];

	if (connection.Operation != "AND" || connection.HasXYParameters)
	{
		return ValidationResult.Invalid(connectionName, "AND", 0);
	}

	var xInput = InputName("x", n);
	var yInput = InputName("y", n);
	var left = connections[connection.Left];
	var right = connections[connection.Right];

	if (left.IsXorOf(xInput, yInput))
	{
		return ValidateCarryOver(n - 1, connection.Right, connections).AddScore(n * 2);
	}

	if (right.IsXorOf(xInput, yInput))
	{
		return ValidateCarryOver(n - 1, connection.Left, connections).AddScore(n * 2);
	}

	return ValidateCarryOver(n - 1, connection.Left, connections).IsValid
		? ValidationResult.Invalid(connection.Right, $"XOR", n * 2)
		: ValidateCarryOver(n - 1, connection.Right, connections).IsValid
			? ValidationResult.Invalid(connection.Left, $"XOR", n * 2)
			: ValidationResult.Invalid(connectionName, $"AND", 0);
}

ImmutableDictionary<string, Connection> SwapConnections(string firstName, string secondName, ImmutableDictionary<string, Connection> connections)
{
	var first = connections[firstName];
	var second = connections[secondName];
	return connections.SetItem(secondName, first).SetItem(firstName, second);
}

static string InputName(string input, int n)
{
	return input.ToString() + n.ToString("D2");
}

static bool GetValue(string output, Dictionary<string, Connection> connections, Dictionary<string, bool> outputValues)
{
	if (outputValues.TryGetValue(output, out var value))
	{
		return value;
	}

	var connection = connections[output];
	var arg1 = GetValue(connection.Left, connections, outputValues);
	var arg2 = GetValue(connection.Right, connections, outputValues);
	return Calculate(arg1, arg2, connection.Operation);
}

static bool Calculate(bool val1, bool val2, string operation)
{
	return operation switch
	{
		"AND" => val1 & val2,
		"OR" => val1 | val2,
		"XOR" => val1 ^ val2,
		_ => throw new InvalidOperationException()
	};
}

static ulong ConvertToInt(List<bool> boolList)
{
	ulong result = 0;
	for (int i = 0; i < boolList.Count; i++)
	{
		if (boolList[i])
		{
			result |= 1UL << (boolList.Count - 1 - i);
		}
	}
	return result;
}

static int GetPosition(string inputOutputName)
{
	return int.Parse(inputOutputName.Substring(1));
}

static (string input, bool value) ParseValue(string line)
{
	var parts = line.Split(":", StringSplitOptions.TrimEntries);
	return (parts[0], parts[1] == "1");
}

static (string output, Connection connection) ParseConnection(string line)
{
	var parts = line.Split("->", StringSplitOptions.TrimEntries);
	var output = parts[1];
	var partsConnection = parts[0].Split(" ", StringSplitOptions.TrimEntries);
	return (output, new Connection(partsConnection[0], partsConnection[2], partsConnection[1]));
}

record Connection(string Left, string Right, string Operation)
{
	public bool IsAndOf(string val1, string val2) => Operation == "AND" && HasParameters(val1, val2);
	public bool IsOrOf(string val1, string val2) => Operation == "OR" && HasParameters(val1, val2);
	public bool IsXorOf(string val1, string val2) => Operation == "XOR" && HasParameters(val1, val2);

	public bool HasParameters(string val1, string val2) => (Left == val1 && Right == val2) || (Left == val2 && Right == val1);
	public bool HasXYParameters => (Left.StartsWith("x") && Right.StartsWith("y")) || (Left.StartsWith("y") && Right.StartsWith("x"));
}

record ValidationResult(bool IsValid, string InvalidConnection, string Expecting, int ValiditiyScore)
{
	public ValidationResult AddScore(int score) => new ValidationResult(IsValid, InvalidConnection, Expecting, ValiditiyScore + score);

	public static ValidationResult Invalid(string connection, string expecting, int score) => new(false, connection, expecting, score);
	public static ValidationResult Valid => new(true, "", "", 1000000);
}