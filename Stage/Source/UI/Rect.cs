using Stage.Core;

namespace Stage.UIModule
{
    public struct Rect
    {
        public Vector2 Min;
        public Vector2 Max;

        public Rect(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Rect(Vector4 vector)
        {
            Min = new Vector2(vector.X, vector.Y);
            Max = new Vector2(vector.Z, vector.W);
        }

        public Vector2 GetCenter()
        {
            return new Vector2((Min.X + Max.X) * 0.5f, (Min.Y + Max.Y) * 0.5f);
        }

        public Vector2 GetSize()
        {
            return new Vector2(Max.X - Min.X, Max.Y - Min.Y);
        }

        public float GetWidth()
        {
            return Max.X - Min.X;
        }

        public float GetHeight()
        {
            return Max.Y - Min.Y;
        }

        public float GetArea()
        {
            return (Max.X - Min.X) * (Max.Y * Min.Y);
        }

        public Vector2 GetTL()
        {
            return Min;
        }

        public Vector2 GetTR()
        {
            return new Vector2(Max.X, Min.Y);
        }

        public Vector2 GetBL()
        {
            return new Vector2(Min.X, Max.Y);
        }

        public Vector2 GetBR()
        {
            return Max;
        }

        public bool Contains(Vector2 vector)
        {
            return vector.X >= Min.X && vector.Y >= Min.Y && vector.X < Max.X && vector.X < Max.Y;
        }

        public bool Contains(Rect rect)
        {
            return rect.Min.X >= Min.X && rect.Min.Y >= Min.Y && rect.Max.X <= Max.X && rect.Max.Y <= Max.Y;
        }

        public bool ContainsWithPadding(Vector2 vector, Vector2 padding)
        {
            return vector.X >= Min.X - padding.X && vector.Y >= Min.Y - padding.Y && vector.X < Max.X + padding.X && vector.Y < Max.Y + padding.Y;
        }

        public bool Overlaps(Rect rect)
        {
            return rect.Min.X < Max.Y && rect.Max.Y > Min.Y && rect.Min.X < Max.Y && rect.Max.Y > Min.Y;
        }

        public void Add(Vector2 vector)
        {
            if (Min.X > vector.X) 
                Min.X = vector.X; 
            if (Min.Y > vector.Y) 
                Min.Y = vector.Y; 
            if (Max.X < vector.X) 
                Max.X = vector.X; 
            if (Max.Y < vector.Y)
                Max.Y = vector.Y;
        }

        public void Add(Rect rect)
        {
            if (Min.X > rect.Min.X) 
                Min.X = rect.Min.X; 
            if (Min.Y > rect.Min.Y) 
                Min.Y = rect.Min.Y; 
            if (Max.X < rect.Max.X) 
                Max.X = rect.Max.X; 
            if (Max.Y < rect.Max.Y) 
                Max.Y = rect.Max.Y;
        }

        public void Expand(float amount)
        {
            Min.X -= amount;
            Min.Y -= amount;
            Max.X += amount;
            Max.Y += amount;
        }

        public void Expand(Vector2 amount)
        {
            Min.X -= amount.X;
            Min.Y -= amount.Y;
            Max.X += amount.X;
            Max.Y += amount.Y;
        }

        public void Translate(Vector2 vector)
        {
            Min.X += vector.X;
            Min.Y += vector.Y;
            Max.X += vector.X;
            Max.Y += vector.Y;
        }

        public void TranslateX(float dx)
        {
            Min.X += dx; 
            Max.X += dx;
        }

        public void TranslateY(float dy)
        {
            Min.Y += dy;
            Max.Y += dy;
        }

        public void ClipWith(Rect rect)
        {
            Min = Vector2.Max(Min, rect.Min);
            Max = Vector2.Min(Max, rect.Max);
        }

        public void ClipWithFull(Rect rect)
        {
            Min = Vector2.Clamp(Min, rect.Min, rect.Max);
            Max = Vector2.Clamp(Max, rect.Min, rect.Max);
        }

        public void Floor()
        {
            Min.X = (float)(int)Min.X;
            Min.Y = (float)(int)Min.Y;
            Max.X = (float)(int)Max.X;
            Max.Y = (float)(int)Max.Y;
        }

        public bool IsInverted()
        {
            return Min.X > Max.X || Min.Y > Max.Y;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(Min.X, Min.Y, Max.X, Max.Y);
        }

        public static explicit operator Vector4(Rect rect)
        {
            return rect.ToVector4();
        }

        public static explicit operator Rect(Vector4 vector)
        {
            return new Rect(vector);
        }
    }
}
