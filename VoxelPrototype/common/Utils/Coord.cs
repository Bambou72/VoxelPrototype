using OpenTK.Mathematics;
using VoxelPrototype.common.World;

namespace VoxelPrototype.common.Utils
{
    internal static class Coord
    {
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            Vector2i cpos;
            Vector3i bpos;
            int blockX = System.Math.Abs(x % Const.ChunkSize);
            int blockZ = System.Math.Abs(z % Const.ChunkSize);

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
        }
    }
}

