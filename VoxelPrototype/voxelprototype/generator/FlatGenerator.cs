using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.server.game.world.Level.Chunk;

namespace VoxelPrototype.voxelprototype.generator
{
    internal class FlatGenerator : WorldGenerator
    {
        public FlatGenerator()
        {
            Name = "Flat";

        }

        public int GetOriginHeigh()
        {
            throw new NotImplementedException();
        }
        public override int GetOriginHeight()
        {
            return 0;
        }
        public override void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < Const.ChunkSize; x++)
            {
                for (int z = 0; z < Const.ChunkSize; z++)
                {
                    for (int y = 0; y < Const.ChunkRHeight; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("voxelprototype:grass").GetDefaultState());
                        }
                        else if (y > -3 && y < 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("voxelprototype:dirt").GetDefaultState());
                        }
                        else if (y < -3)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("voxelprototype:stone").GetDefaultState());
                        }
                    }
                }
            }
        }
    }
}
