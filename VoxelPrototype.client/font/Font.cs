using OpenTK.Mathematics;
using VoxelPrototype.client.rendering.texture;
namespace VoxelPrototype.client.font
{
    //
    public class Font
    {
        public Dictionary<char, Character> Characters = new();
        public ITexture Atlas;
        public int Height;
        public int Ascent;
        public int Descent;
        public int LineGap;
        public int LineHeight;
    }
    public class Character
    {
        public Vector2i Size;
        public Vector2i Bearing;
        public uint Advance;
        public Vector2 UV0;
        public Vector2 UV1;
        public bool Rendarable = true;
    }
}
