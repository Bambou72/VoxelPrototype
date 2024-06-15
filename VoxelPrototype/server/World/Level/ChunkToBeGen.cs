using OpenTK.Mathematics;
namespace VoxelPrototype.server.World.Level
{
    internal struct ChunkToBeGen
    {
        public int PlayerID;
        public Vector2i pos;
        public int distance;
    }
}
