using OpenTK.Mathematics;
using VoxelPrototype.common.Blocks;

namespace VoxelPrototype.common.Utils
{
    internal static class Coord
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
        internal static Vector2i GetVoxelChunkCoord(int x, int y, int z)
        {
            int chunkX = x / Const.ChunkSize;
            int chunkZ = z / Const.ChunkSize;
            int chunkModX = x % Const.ChunkSize;
            int chunkModZ = z % Const.ChunkSize;
            return new Vector2i(
                x < 0 ? chunkX - (chunkModX == 0 ? 0 : 1) : chunkX,
                z < 0 ? chunkZ - (chunkModZ == 0 ? 0 : 1) : chunkZ
            );
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
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            int chunkX = x / Const.ChunkSize;
            int chunkZ = z / Const.ChunkSize;
            int chunkModX = x % Const.ChunkSize;
            int chunkModZ = z % Const.ChunkSize;

            // Calculate cpos (chunk position)
            int cposX = x < 0 ? chunkX - ((chunkModX != 0) ? 1 : 0) : chunkX;
            int cposZ = z < 0 ? chunkZ - ((chunkModZ != 0) ? 1 : 0) : chunkZ;
            Vector2i cpos = new Vector2i(cposX, cposZ);

            // Calculate bpos (block position)
            int blockPosX = x < 0 ? ((chunkModX != 0) ? Const.ChunkSize - Math.Abs(chunkModX) : 0) : Math.Abs(chunkModX);
            int blockPosZ = z < 0 ? ((chunkModZ != 0) ? Const.ChunkSize - Math.Abs(chunkModZ) : 0) : Math.Abs(chunkModZ);
            Vector3i bpos = new Vector3i(blockPosX, y, blockPosZ);

            return (cpos, bpos);
        }

    }
}

