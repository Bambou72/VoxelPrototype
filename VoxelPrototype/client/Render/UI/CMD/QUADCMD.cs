using Crecerelle.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.client.Render.UI.CMD
{
    internal struct QUADCMD
    {
        internal Vector3 Position;
        internal Vector2i Size;
        internal Color Color;
        internal Vector2 TextBegin;
        internal Vector2 TextEnd;
        internal Texture Texture;
    }
}
