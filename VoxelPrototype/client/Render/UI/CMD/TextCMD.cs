using Crecerelle.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Text;

namespace VoxelPrototype.client.Render.UI.CMD
{
    internal struct TextCMD
    {
        internal string Text;
        internal Vector3 Position;
        internal Vector3 Color;
        internal float Scale;
        internal Font Font;
    }
}
