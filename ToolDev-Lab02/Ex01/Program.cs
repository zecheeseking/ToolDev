using System;
using System.Text;

namespace Ex01
{
    internal class Program
    {
        private struct Vector2D
        {
            private double X;
            private double Y;

            public Vector2D(double x, double y)
            {
                X = x;
                Y = y;
            }

            public Vector2D(Vector2D other)
            {
                X = other.X;
                Y = other.Y;
            }

            public double Length
            {
                private set { }
                get { return Math.Sqrt((X * X) + (Y * Y)); }
            }

            public double Angle
            {
                private set { }
                get { return Math.Atan2(Y, X); }
            }

            public static Vector2D Zero
            {
                private set { }
                get { return new Vector2D(0, 0); }
            }

            public double DotProduct(Vector2D other)
            {
                return (X * other.X) + (Y * other.Y);
            }

            public static Vector2D FromPolar(double length, double angle)
            {
                return new Vector2D(length * Math.Cos(angle), length * Math.Sin(angle));
            }

            public static Vector2D operator +(Vector2D lhs, Vector2D rhs)
            {
                return new Vector2D(lhs.X + rhs.X, lhs.Y + rhs.Y);
            }

            public static Vector2D operator -(Vector2D lhs, Vector2D rhs)
            {
                return new Vector2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
            }

            public void AddTo(Vector2D other)
            {
                X += other.X;
                Y += other.Y;
            }

            public static bool Normalize(Vector2D target)
            {
                if (target.Length == double.NaN)
                    return false;

                double len = target.Length;

                target.X = target.X / len;
                target.Y = target.Y / len;

                return true;
            }
        }

        private static void Main(string[] args)
        {
        }
    }
}