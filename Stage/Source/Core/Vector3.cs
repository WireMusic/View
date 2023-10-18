using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stage.Core
{
    public struct Vector3
    {
        public float X, Y, Z;

        public static Vector4 Zero => new Vector4(0.0f);

        public Vector3(float scalar)
        {
            X = scalar;
            Y = scalar;
            Z = scalar;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator+(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator*(Vector3 vector, float scalar)
        {
            return new Vector3(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
        }

        public static explicit operator System.Numerics.Vector3(Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
        }

        public static explicit operator Vector3(System.Numerics.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }
}
