using OpenTK.Mathematics;

namespace VoxelPrototype.common.Utils.Storage.Palette
{
    internal static class Utils
    {
        internal static int TreetoOne(Vector3i pos)
        {
            return pos.Z << 8 | pos.Y << 4 | pos.X;
        }
    }
}
