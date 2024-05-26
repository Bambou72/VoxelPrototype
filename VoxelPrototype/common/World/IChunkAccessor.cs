namespace VoxelPrototype.common.World
{
    internal interface IChunkAccessor
    {
        public bool IsChunkExist(int x, int z);
        public Chunk GetChunk(int x, int z);
    }
}
