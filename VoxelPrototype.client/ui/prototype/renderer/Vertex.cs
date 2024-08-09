using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace VoxelPrototype.client.ui.prototype.renderer
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vertex
    {
        public Vector2 Position;
        public Vector2 TexCoords;
        public uint Color;
    }
}
