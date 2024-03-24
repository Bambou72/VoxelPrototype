using OpenTK.Mathematics;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.Game.World;
using VoxelPrototype.common.Game.World.Terrain;

namespace Voxel.Generators
{
    internal class FlatGenerator : VoxelPrototype.common.API.WorldGenerator.WorldGenerator
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
