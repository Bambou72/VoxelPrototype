using VoxelPrototype.api.Blocks.State;

namespace VoxelPrototype.common.World
{
    public class World : IBlockAcessor, IChunkAccessor, ITickable
    {
        //RNG
        internal Random RNG;
        //WorldInfo
        internal WorldInfo WorldInfo;
        //Distance
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;
        public virtual void Dispose() { }
        public virtual void Tick(float DT)
        {
            CurrentTick++;
        }

        public virtual BlockState GetBlock(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAir(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsTransparent(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsChunkExist(int x, int z)
        {
            throw new NotImplementedException();
        }

        public virtual Chunk GetChunk(int x, int z)
        {
            throw new NotImplementedException();
        }

        public virtual void SetBlock(int x, int y, int z, BlockState State)
        {
            throw new NotImplementedException();
        }
    }
}
