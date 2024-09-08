using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.worldgeneration;
using VoxelPrototype.server.game.world.Level.Chunk;

namespace VoxelPrototype.voxelprototype.generator
{
    internal class FlatGenerator : IChunkGenerator
    {
        public Chunk GenerateChunk(Vector2i Position)
        {
            Chunk chunk = new Chunk(Position);
            for (int x = 0; x < Const.ChunkSize; x++)
            {
                for (int z = 0; z < Const.ChunkSize; z++)
                {
                    for (int y = 0; y < Const.ChunkRHeight; y++)
                    {

                        if (y == 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:grass").GetDefaultState());
                        }
                        else if (y > -3 && y < 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:dirt").GetDefaultState());
                        }
                        else if (y < -3)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:stone").GetDefaultState());
                        }
                    }
                }
            }
            return chunk;
        }
    }
}
