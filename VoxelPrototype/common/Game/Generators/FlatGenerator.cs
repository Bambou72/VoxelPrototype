using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.client;
using VoxelPrototype.common.World;

namespace VoxelPrototype.common.Game.Generators
{
    internal class FlatGenerator : WorldGenerator
    {
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
            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    for (int y = 0; y < Chunk.Height * Section.Size; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:grass").GetDefaultState());
                        }
                        else if (y < 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:dirt").GetDefaultState());
                        }
                    }
                }
            }
        }
    }
}
