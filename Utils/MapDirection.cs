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

	public static List<MapDirection> All => [North, East, South, West, NorthEast, SouthEast, SouthWest, NorthWest];
}
