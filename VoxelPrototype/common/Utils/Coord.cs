using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using VoxelPrototype.common.Blocks;

namespace VoxelPrototype.common.Utils
{
    public static class Coord
    {
        /*
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            Vector2i cpos;
            Vector3i bpos;

            int blockX = Math.Abs(x % Const.ChunkSize);
            int blockZ = Math.Abs(z % Const.ChunkSize);
            if (x < 0 && z >= 0)
            {
                cpos = new Vector2i(x / Const.ChunkSize - (x % Const.ChunkSize == 0 ? 0 : 1), z / Const.ChunkSize);
                bpos = new Vector3i(blockX == 0 ? 0 : Const.ChunkSize - blockX, y, blockZ);
            }
            else if (x >= 0 && z < 0)
            {
                cpos = new Vector2i(x / Const.ChunkSize, z / Const.ChunkSize - (z % Const.ChunkSize == 0 ? 0 : 1));
                bpos = new Vector3i(blockX, y, blockZ == 0 ? 0 : Const.ChunkSize - blockZ);
            }
            else if (x < 0 && z < 0)
            {
                cpos = new Vector2i(x / Const.ChunkSize - (x % Const.ChunkSize == 0 ? 0 : 1), z / Const.ChunkSize - (z % Const.ChunkSize == 0 ? 0 : 1));
                bpos = new Vector3i(blockX == 0 ? 0 : Const.ChunkSize - blockX, y, blockZ == 0 ? 0 : Const.ChunkSize - blockZ);
            }   
            else
            {
                cpos = new Vector2i(x / Const.ChunkSize, z / Const.ChunkSize);
                bpos = new Vector3i(blockX, y, blockZ);
            }
            return (cpos, bpos);
        }*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2i GetChunkCoord(int x, int y, int z)
        {
            return new Vector2i(x >> Const.BitShifting , z >> Const.BitShifting);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3i GetBlockLocalCoord(int x, int y, int z)
        {
            return new Vector3i(x & Const.And,y, z & Const.And);
        }
        /*
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            int chunkX = x / Const.ChunkSize;
            int chunkZ = z / Const.ChunkSize;
            int chunkModX = x % Const.ChunkSize;
            int chunkModZ = z % Const.ChunkSize;

            // Calculate cpos (chunk position)
            Vector2i cpos = new Vector2i(
                x < 0 ? chunkX - (chunkModX == 0 ? 0 : 1) : chunkX,
                z < 0 ? chunkZ - (chunkModZ == 0 ? 0 : 1) : chunkZ
            );

            // Calculate bpos (block position)
            Vector3i bpos = new Vector3i(
                x < 0 ? (chunkModX == 0 ? 0 : Const.ChunkSize - Math.Abs(chunkModX)) : Math.Abs(chunkModX),
                y,
                z < 0 ? (chunkModZ == 0 ? 0 : Const.ChunkSize - Math.Abs(chunkModZ)) : Math.Abs(chunkModZ)
            );

            return (cpos, bpos);
        }*/
        /*
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            int chunkX = x>=0 ?  x / Const.ChunkSize : (x - Const.ChunkSize + 1) / Const.ChunkSize;
            int chunkZ = x >= 0 ? z / Const.ChunkSize : (z - Const.ChunkSize + 1) / Const.ChunkSize;
            Vector2i cpos = new Vector2i(chunkX, chunkZ);
            int bx = (x % Const.ChunkSize + Const.ChunkSize) % Const.ChunkSize;
            int bz = (z % Const.ChunkSize + Const.ChunkSize) % Const.ChunkSize;
            if(x <0)
            {
                bx = 16 - bx;
            }
            if (z < 0)
            {
                bz = 16 - bz;
            }
            Vector3i bpos = new Vector3i(bx, y, bz);

            return (cpos, bpos);
        }*/

    }
}

