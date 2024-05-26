using OpenTK.Mathematics;
using VoxelPrototype.common.World;

namespace VoxelPrototype.common.World.Storage.Palette
{
    internal static class Utils
    {
        internal static readonly int SectionSizeSquared = Section.Size * Section.Size;
        /*
        internal static int TreetoOne(Vector3i pos)
        {
            return pos.X + Section.Size * pos.Y + SectionSizeSquared * pos.Z;
        }*/
        internal static int TreetoOne(Vector3i pos)
        {
            return pos.Z << 8 | pos.Y << 4 | pos.X;
        }
    }
}
