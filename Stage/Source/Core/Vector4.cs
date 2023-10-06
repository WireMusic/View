using System.Numerics;

namespace Stage.Core
{
    public struct Vector4
    {
        public float X, Y, Z, W;

        public static Vector4 Zero => new Vector4(0.0f);

        public Vector4(float scalar)
        {
            X = scalar;
            Y = scalar;
            Z = scalar;
            W = scalar;
        }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 operator+(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4 operator*(Vector4 vector, float scalar)
        {
            return new Vector4(vector.X * scalar, vector.Y * scalar, vector.Z * scalar, vector.W * scalar);
        }

        public static explicit operator System.Numerics.Vector4(Vector4 vector)
        {
            return new System.Numerics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        public static explicit operator Vector4(System.Numerics.Vector4 vector)
        {
            return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}, {W}";
        }
    }
}
