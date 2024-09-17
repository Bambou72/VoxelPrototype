using OpenTK.Mathematics;
namespace ImmediateUI.immui.math
{
    public struct Rect : IEquatable<Rect>
    {
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Max 
        { 
            get 
            { 
                return Position + Size; 
            }
            set 
            { 
                Size = value - Position; 
            } 
        }
        public Vector2 Center => Position + Size/2;
        public Rect(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }
        public bool Contains(Vector2i Point)
        {
            if (Point.X < Position.X || Point.X > Max.X || Point.Y < Position.Y || Point.Y > Max.Y)
            {
                return false;
            }
            return true;
        }
        public Rect Pad(Rect Pad)
        {
            return new(Position - Pad.Position, Size - Pad.Size);
        }
        public Rect UnPad(Rect Pad)
        {
            return this.Pad(Pad.Scaled(-1));
        }
        public Rect Scaled(float Scale)
        {
            this = new(
                Position * Scale, 
                Max  * Scale
            );
            return this;
        }

        public bool Equals(Rect other)
        {
            if (Position != other.Position || Max != other.Max) return false;
            return true;
        }

        public static Rect operator *(Rect Rect, float Float) => new Rect(Rect.Position * Float, Rect.Size * Float);
        public static Rect operator *(float Float, Rect Rect) => new Rect(Rect.Position * Float, Rect.Size * Float);
    }
}
