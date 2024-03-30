using OpenTK.Mathematics;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.WorldGenerator;
using VoxelPrototype.common.Game.World;

namespace Voxel.Generators
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
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegister.GetBlock("Voxel@Grass").GetDefaultState());
                        }
                        else if (y < 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), BlockRegister.GetBlock("Voxel@Dirt").GetDefaultState());
                        }
                    }
                }
            }
        }
    }
}
