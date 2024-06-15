using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common;
using VoxelPrototype.common.WorldGenerator;
using VoxelPrototype.server.World.Level.Chunk;

namespace VoxelPrototype.game.Generators
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
            for (int x = 0; x < Const.ChunkSize; x++)
            {
                for (int z = 0; z < Const.ChunkSize; z++)
                {
                    for (int y = 0; y < Const.ChunkHeight * Section.Size; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:grass").GetDefaultState());
                        }
                        else if ( y > -3 && y < 0)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:dirt").GetDefaultState());
                        }
                        else if (y < - 3)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:stone").GetDefaultState());
                        }
                    }
                }
            }
        }
    }
}
