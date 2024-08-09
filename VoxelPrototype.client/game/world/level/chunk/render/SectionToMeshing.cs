using OpenTK.Mathematics;

namespace VoxelPrototype.client.game.world.Level.Chunk.Render
{
    internal struct SectionToMeshing
    {
        internal Vector3i Pos;
        internal int Importance;
        internal int NumberOfIt;
    }
    internal struct SectionToOG
    {
        internal Vector3i Section;
        internal SectionVertex[] OpaqueVertices;
        internal uint[] OpaqueIndices;
    }
}
