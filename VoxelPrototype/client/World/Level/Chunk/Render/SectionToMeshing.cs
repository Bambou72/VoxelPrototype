using OpenTK.Mathematics;
using VoxelPrototype.client.World.Level.Chunk;

namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal struct SectionToMeshing
    {
        internal Vector3i Pos;
        internal SectionMeshDataCache Data;
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
