using OpenTK.Mathematics;
namespace VoxelPrototype.client.game.world.Level.Chunk.Render
{
    internal struct SectionVertex
    {
        internal Vector3 Position;
        internal Vector2 Uv;
        internal int AO;
        internal uint Color;
    }
}
