using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.worldgeneration;
using VoxelPrototype.server;
using VoxelPrototype.server.game.world.Level.Chunk;
using VoxelPrototype.utils;
namespace VoxelPrototype.voxelprototype.generator
{
    internal class ComplexChunkGenerator : IChunkGenerator
    {
        FastNoiseLite lib;
        float[] HeightMap = new float[16 * 16];

        public ComplexChunkGenerator(int Seed)
        {
            lib = new FastNoiseLite(Seed);
            lib.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            lib.SetFractalOctaves(16);
            lib.SetFrequency(0.005f);
        }

        internal int GetHeight(Vector2 pos)
        {
            return (int)(lib.GetNoise(pos.X, pos.Y) * 100 + Const.ChunkRHeight / 2);
        }

        public void GenerateHeightmap(Vector2i ChunkPos)
        {
            for(int x = 0; x < 16; x++)
            {
                for(int z = 0; z < 16; z++)
                {
                    HeightMap[x * 16 + z] = GetHeight(new Vector2(x, z) + (ChunkPos * 16));
                }
            }
        }
        public void GenerateBlock(Chunk ch)
        {
            for (int x = 0; x < Const.ChunkSize; x++)
            {
                for (int z = 0; z < Const.ChunkSize; z++)
                {
                    float Height = HeightMap[x * 16 + z];

                    for (int y = 0; y < Const.ChunkRHeight; y++)
                    {

                        if (y == Height)
                        {
                            ch.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:grass").GetDefaultState());
                        }
                        else if (y < Height && y > Height - 4)
                        {
                            ch.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:dirt").GetDefaultState());
                        }
                        else if (y <= Height - 4)
                        {
                            ch.SetBlock(new Vector3i(x, y, z), BlockRegistry.GetInstance().GetBlock("vp:stone").GetDefaultState());
                        }
                    }
                }
            }

        }
        public Chunk GenerateChunk(Vector2i Position)
        {
            GenerateHeightmap(Position);
            Chunk CH = new Chunk(Position);
            GenerateBlock(CH);
            return CH;
        }
    }
}
