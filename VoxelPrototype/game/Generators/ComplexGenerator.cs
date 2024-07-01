using DotnetNoise;
using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.World;
using VoxelPrototype.common.WorldGenerator;
using VoxelPrototype.common;
using VoxelPrototype.server.World.Level.Chunk;

namespace VoxelPrototype.game.Generators
{
    internal class ComplexGenerator : WorldGenerator
    {
        FastNoise lib;

        public ComplexGenerator()
        {
            Name = "Complex";
        }

        public override int GetOriginHeight()
        {
            return GetHeight(0, 0);
        }
        public override void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < Const.ChunkSize; x++)
            {
                for (int z = 0; z < Const.ChunkSize; z++)
                {
                    int GlobalX = x + (chunk.X * Const.ChunkSize);
                    int GlobalZ = z + (chunk.Z * Const.ChunkSize);
                    int Height = GetHeight(GlobalX, GlobalZ);
                    for (int y = 0; y < Const.ChunkRHeight; y++)
                    {

                        if (y == Height)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:grass").GetDefaultState());
                        }
                        else if (y < Height&&   y > (Height-4))
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:dirt").GetDefaultState());
                        }else if( y <= (Height-4))
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:stone").GetDefaultState());
                        }
                    }
                }
            }
        }
        internal int GetHeight(int x, int z)
        {
            return (int)(lib.GetNoise(x, z) * 100 + (Const.ChunkRHeight)/2);
        }
        public override void SetData(long seed)
        {
            base.SetData(seed);
            lib = new FastNoise((int)seed);
            lib.UsedNoiseType = FastNoise.NoiseType.Simplex;
            lib.Octaves = 4;
            lib.Frequency = 0.005f;
        }
    }
}
