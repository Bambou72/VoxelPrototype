using OpenTK.Mathematics;
namespace VoxelPrototype.common.World.ChunkManagers
{
    internal struct ChunkToBeGen
    {
        public int PlayerID;
        public Vector2i pos;
        public int distance;
    }
}
