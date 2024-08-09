using OpenTK.Mathematics;
using VoxelPrototype.server.game.world.Level.Chunk;

namespace VoxelPrototype.api.worldgeneration
{
    public interface IChunkGenerator
    {
        public Chunk GenerateChunk(Vector2i Position);
    }
}
