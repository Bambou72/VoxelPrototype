﻿using VoxelPrototype.common.World;

namespace VoxelPrototype.api.WorldGenerator
{
    public class WorldGenerator
    {
        long Seed;
        public virtual void SetData(long seed)
        {
            Seed = seed;
        }
        public virtual int GetOriginHeight()
        {
            return 0;
        }
        public virtual void GenerateChunk(Chunk chunk)
        {

        }
    }
}
