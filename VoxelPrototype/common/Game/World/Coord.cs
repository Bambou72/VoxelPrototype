using OpenTK.Mathematics;
namespace VoxelPrototype.common.Game.World
{
    internal static class Coord
    {
        internal static (Vector2i, Vector3i) GetVoxelCoord(int x, int y, int z)
        {
            Vector2i cpos;
            Vector3i bpos;
            int blockX = Math.Abs(x % Chunk.Size);
            int blockZ = Math.Abs(z % Chunk.Size);

            if (x < 0 && z >= 0)
            {
                cpos = new Vector2i(x / Chunk.Size - (x % Chunk.Size == 0 ? 0 : 1), z / Chunk.Size);
                bpos = new Vector3i(blockX == 0 ? 0 : Chunk.Size - blockX, y, blockZ);
            }
            else if (x >= 0 && z < 0)
            {
                cpos = new Vector2i(x / Chunk.Size, z / Chunk.Size - (z % Chunk.Size == 0 ? 0 : 1));
                bpos = new Vector3i(blockX, y, blockZ == 0 ? 0 : Chunk.Size - blockZ);
            }
            else if (x < 0 && z < 0)
            {
                cpos = new Vector2i(x / Chunk.Size - (x % Chunk.Size == 0 ? 0 : 1), z / Chunk.Size - (z % Chunk.Size == 0 ? 0 : 1));
                bpos = new Vector3i(blockX == 0 ? 0 : Chunk.Size - blockX, y, blockZ == 0 ? 0 : Chunk.Size - blockZ);
            }
            else
            {
                cpos = new Vector2i(x / Chunk.Size, z / Chunk.Size);
                bpos = new Vector3i(blockX, y, blockZ);
            }
            return (cpos, bpos);
        }
    }
}

