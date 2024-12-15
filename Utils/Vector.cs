namespace Utils;

public record Vector(int X, int Y)
{
	public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
	public static Vector operator *(Vector a, int x) => new Vector(a.X * x, a.Y * x);
}
