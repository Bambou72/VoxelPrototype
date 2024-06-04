using OpenTK.Mathematics;
namespace VoxelPrototype.server.World
{
    internal struct ChunkToBeGen
    {
        public int PlayerID;
        public Vector2i pos;
        public int distance;
    }
}
