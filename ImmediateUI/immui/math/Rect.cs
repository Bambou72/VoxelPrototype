using OpenTK.Mathematics;
namespace ImmediateUI.immui.math
{
    public struct Rect : IEquatable<Rect>
    {
        public int X,Y;
        public int W,H;
        public int XW => X + W;
        public int YH => Y + H;
        public int CenterX => (X +W)/2 ;
        public int CenterY => (Y +H)/2 ;

        public Rect(int X,int Y , int W , int H)
        {
            this.X = X; this.Y = Y;
            this.W = W; this.H = H;
        }
        public bool ContainsPoint(Vector2i Point)
        {
            if (Point.X < X || Point.X > XW|| Point.Y < Y || Point.Y > YH)
            {
                return false;
            }
            return true;
        }
        public bool ContainsRect(Rect Rect)
        {
            if (Rect.X < X || Rect.XW > XW || Rect.Y < Y || Rect.YH > YH)
            {
                return false;
            }
            return true;
        }
        public void SetMaxX(int X)
        {
            W = X -this.X;
        }
        public void SetMaxY(int Y)
        {
            H = Y - this.Y;
        }
        public bool Equals(Rect other)
        {
            if (X != other.X || Y != other.Y || W != other.W || H != other.H) return false;
            return true;
        }
    }
}
