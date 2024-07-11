using VoxelPrototype.server.game.world.Level.Chunk;

namespace VoxelPrototype.api.WorldGenerator
{
    public class WorldGenerator
    {
        long Seed;
        public string Name;
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
