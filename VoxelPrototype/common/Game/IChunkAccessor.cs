using VoxelPrototype.common.Game.World.Terrain;

namespace VoxelPrototype.common.Game
{
    internal interface IChunkAccessor
    {
        public bool IsChunkExist(int x, int z);
        public Chunk GetChunk(int x, int z);
    }
}
