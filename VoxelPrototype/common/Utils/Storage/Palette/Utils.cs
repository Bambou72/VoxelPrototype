using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace VoxelPrototype.common.Utils.Storage.Palette
{
    internal static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int TreetoOne(Vector3i pos)
        {
            return pos.Z << (2*Const.SectionSizePowerOf2) | pos.Y << Const.SectionSizePowerOf2 | pos.X;
        }
    }
}
