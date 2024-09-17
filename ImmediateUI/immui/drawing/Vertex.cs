using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace ImmediateUI.immui.drawing
{
    //[StructLayout(LayoutKind.Sequential)]
    public   struct Vertex
    {
        public  Vector2 Position;
        public  Vector2 UV;
        public  uint Color;

        public Vertex(Vector2 position, Vector2 uV, uint color)
        {
            Position = position;
            UV = uV;
            Color = color;
        }
    }
}
