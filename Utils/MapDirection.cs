namespace Utils;
public record class MapDirection(string Value)
{
	public static readonly MapDirection North = new MapDirection("N");
	public static readonly MapDirection East = new MapDirection("E");
	public static readonly MapDirection South = new MapDirection("S");
	public static readonly MapDirection West = new MapDirection("W");
	public static readonly MapDirection NorthEast = new MapDirection("NE");
	public static readonly MapDirection SouthEast = new MapDirection("SE");
	public static readonly MapDirection SouthWest = new MapDirection("SW");
	public static readonly MapDirection NorthWest = new MapDirection("NW");

	public static List<MapDirection> Main => [North, East, South, West];
	public static List<MapDirection> All => [North, East, South, West, NorthEast, SouthEast, SouthWest, NorthWest];

	public MapDirection Turn90R()
	{
		return Value switch
		{
			"N" => new MapDirection("E"),
			"E" => new MapDirection("S"),
			"S" => new MapDirection("W"),
			"W" => new MapDirection("N"),
			_ => throw new Exception()
		};
	}

	public MapDirection Turn90L()
	{
		return Value switch
		{
			"N" => new MapDirection("W"),
			"E" => new MapDirection("N"),
			"S" => new MapDirection("E"),
			"W" => new MapDirection("S"),
			_ => throw new Exception()
		};
	}

	public MapDirection Reverse()
	{
		return Value switch
		{
			"N" => new MapDirection("S"),
			"E" => new MapDirection("W"),
			"S" => new MapDirection("N"),
			"W" => new MapDirection("E"),
			_ => throw new Exception()
		};
	}

	public char ToArrowSign()
	{
		return Value switch
		{
			"N" => '^',
			"E" => '>',
			"S" => 'v',
			"W" => '<',
			_ => throw new Exception()
		};
	}

	public static MapDirection FromArrowSign(char move)
	{
		return move switch
		{
			'^' => North,
			'>' => East,
			'v' => South,
			'<' => West,
			_ => throw new Exception()
		};
	}
}
