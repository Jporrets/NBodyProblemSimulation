namespace NBodyProblemSimulation.Utils
{
    internal class CustomVector2d
    {
        public struct Vector2d
        {
            public double X, Y;
            public Vector2d(double x, double y) { X = x; Y = y; }
            public static Vector2d Zero => new(0, 0);
            public static Vector2d operator +(Vector2d a, Vector2d b) => new(a.X + b.X, a.Y + b.Y);
            public static Vector2d operator -(Vector2d a, Vector2d b) => new(a.X - b.X, a.Y - b.Y);
            public static Vector2d operator *(Vector2d a, double s) => new(a.X * s, a.Y * s);
            public static Vector2d operator *(double s, Vector2d a) => new(a.X * s, a.Y * s);
            public double LengthSquared() => X * X + Y * Y;
        }
    }
}