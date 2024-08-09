using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
namespace VoxelPrototype.utils
{
    public static class Coord
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i GetChunkCoord(Vector2i Pos)
        {
            Pos.X = Pos.X >> Const.BitShifting;
            Pos.Y = Pos.Y >> Const.BitShifting;
            return Pos;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3i GetBlockLocalCoord(Vector3i Pos)
        {
            Pos.X &= Const.And;
            Pos.Z &= Const.And;
            return Pos;
        }
    }
}

