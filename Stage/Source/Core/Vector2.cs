global using vec2 = Stage.Core.Vector2;

using System;

namespace Stage.Core
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X, Y;

        public static Vector2 Zero => new Vector2(0.0f);

        public Vector2(float scalar)
        {
            X = scalar;
            Y = scalar;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X < rhs.X ? lhs.X : rhs.X, lhs.Y < rhs.Y ? lhs.Y : rhs.Y);
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X >= rhs.X ? lhs.X : rhs.X, lhs.Y >= rhs.Y ? lhs.Y : rhs.Y);
        }

        public static Vector2 Clamp(Vector2 v, Vector2 mn, Vector2 mx)
        {
            return new Vector2((v.X < mn.X) ? mn.X : (v.X > mx.X) ? mx.X : v.X, (v.Y < mn.Y) ? mn.Y : (v.Y > mx.Y) ? mx.Y : v.Y);
        }

        public static Vector2 operator+(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator-(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator+(Vector2 a, float b)
        {
            return new Vector2(a.X + b, a.Y + b);
        }

        public static Vector2 operator-(Vector2 a, float b)
        {
            return new Vector2(a.X - b, a.Y - b);
        }

        public static Vector2 operator*(Vector2 vector, float scalar)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector2 operator%(Vector2 a, Vector2 b)
        {
            if (b.X == 0 || b.Y == 0)
                return new Vector2(0.0f);

            return new Vector2(a.X % b.X, a.Y % b.Y);
        }

        public static bool operator==(Vector2 a, Vector2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator!=(Vector2 a, Vector2 b)
        {
            return !(a.X == b.X) && (a.Y == b.Y);
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public override bool Equals(object? other) => Equals(other as Vector2?);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
