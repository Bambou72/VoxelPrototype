using ImmediateUI.immui.font;
using OpenTK.Mathematics;
namespace ImmediateUI.immui
{
    public class Style
    {
        public Font BaseFont;
        public float FontSize = 25;
        public Vector4i Padding = new(10);
        public Dictionary<string, uint> Colors;
    }
}
