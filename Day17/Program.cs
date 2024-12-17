var lines = File.ReadAllLines("input.txt");

var registerA = ReadRegister(lines[0]);
var registerB = ReadRegister(lines[1]);
var registerC = ReadRegister(lines[2]);
var program = ReadProgram(lines[4]);

var result1 = RunProgram(registerA, registerB, registerC, program);
Console.WriteLine($"Result 1: " + string.Join(",", result1));

List<long> RunProgram(long registerA, long registerB, long registerC, List<int> program)
{
	int instructionPointer = 0;
	var output = new List<long>();

	while (instructionPointer < program.Count - 1)
	{
		var instruction = program[instructionPointer];
		var literalOperand = program[instructionPointer + 1];
		instructionPointer += 2;

		if (instruction == 0)
		{
			var comboOperand = GetComboOperand(literalOperand, registerA, registerB, registerC);
			registerA = Convert.ToInt64(Math.Truncate(registerA / Math.Pow(2.0d, comboOperand)));
		}

		if (instruction == 1)
		{
			registerB = registerB ^ literalOperand;
		}

		if (instruction == 2)
		{
			var comboOperand = GetComboOperand(literalOperand, registerA, registerB, registerC);
			registerB = comboOperand % 8;
		}

		if (instruction == 3)
		{
			instructionPointer = registerA == 0 ? instructionPointer : literalOperand;
		}

		if (instruction == 4)
		{
			registerB = registerB ^ registerC;
		}

		if (instruction == 5)
		{
			var comboOperand = GetComboOperand(literalOperand, registerA, registerB, registerC);
			output.Add(comboOperand % 8);
		}

		if (instruction == 6)
		{
			var comboOperand = GetComboOperand(literalOperand, registerA, registerB, registerC);
			registerB = Convert.ToInt64(Math.Truncate(registerA / Math.Pow(2.0d, comboOperand)));
		}

		if (instruction == 7)
		{
			var comboOperand = GetComboOperand(literalOperand, registerA, registerB, registerC);
			registerC = Convert.ToInt64(Math.Truncate(registerA / Math.Pow(2.0d, comboOperand)));
		}
	}

	return output;
}

long GetComboOperand(int operand, long registerA, long registerB, long registerC)
{
	return operand switch
	{
		0 => 0,
		1 => 1,
		2 => 2,
		3 => 3,
		4 => registerA,
		5 => registerB,
		6 => registerC,
		_ => throw new ArgumentException()
	};
}

int ReadRegister(string value)
{
	return int.Parse(value.Split(":")[1]);
}

List<int> ReadProgram(string value)
{
	var program = value.Split(":")[1];
	return program.Split(",").Select(int.Parse).ToList();
}