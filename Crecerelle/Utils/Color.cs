using OpenTK.Mathematics;

namespace Crecerelle.Utils
{
    public partial struct Color
    {
        public static Color White = new Color(1, 1, 1, 1);
        public static Color Black = new Color(0, 0, 0, 1);
        public float X, Y, Z, A;
        public Vector3 RGB { get { return new Vector3(X, Y, Z); } }
        public Color(float x, float y, float z, float a)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
        }
        public static implicit operator Color(System.Numerics.Vector4 v)
        {
            return new Color(v.X, v.Y, v.Z, v.W);
        }
    }

}
