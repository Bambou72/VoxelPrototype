using OpenTK.Mathematics;
using System.Diagnostics.Contracts;
namespace ImmediateUI.immui.math
{
    public struct Rect : IEquatable<Rect>
    {
        public Vector2 Min, Max;
        public Vector2 Size => Max - Min;

        public Rect(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
        public bool Contains(Vector2i Point)
        {
            if (Point.X < Min.X || Point.X > Max.X || Point.Y < Min.Y || Point.Y > Max.Y)
            {
                return false;
            }
            return true;
        }
        public Rect Pad(Rect Pad)
        {
            return new(Min - Pad.Min, Max - Pad.Max);
        }

        public bool Equals(Rect other)
        {
            if (Min != other.Min || Max != other.Max) return false;
            return true;
        }

        [Pure]
        public static Rect operator *(Rect Rect, float Float) => new Rect(Rect.Min * Float, Rect.Max * Float);
        [Pure]
        public static Rect operator *(float Float, Rect Rect) => new Rect(Rect.Min * Float, Rect.Max * Float);
    }
}
