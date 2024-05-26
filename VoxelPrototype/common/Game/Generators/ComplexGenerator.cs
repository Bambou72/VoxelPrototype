using DotnetNoise;
using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.client;
using VoxelPrototype.common.World;

namespace VoxelPrototype.common.Game.Generators
{
    internal class ComplexGenerator : WorldGenerator
    {
        FastNoise lib;
        public override int GetOriginHeight()
        {
            return GetHeight(0, 0);
        }
        public override void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    int GlobalX = x + chunk.X * Chunk.Size;
                    int GlobalZ = z + chunk.Z * Chunk.Size;
                    int Height = GetHeight(GlobalX, GlobalZ);
                    for (int y = 0; y < Chunk.Height * Section.Size; y++)
                    {

                        if (y == Height)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:grass").GetDefaultState());
                        }
                        else if (y < Height)
                        {
                            chunk.SetBlock(new Vector3i(x, y, z), Client.TheClient.ModManager.BlockRegister.GetBlock("voxelprototype:dirt").GetDefaultState());
                        }
                    }
                }
            }
        }
        internal int GetHeight(int x, int z)
        {
            return (int)(lib.GetNoise(x, z) * 200 + 256);
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
