using OpenTK.Mathematics;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.client.World.Level.Chunk.Render;

namespace VoxelPrototype.client.World.Level
{
    internal struct SectionToMeshing
    {
        internal Vector3i Pos;
        internal SectionMeshGenerator Generator;
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
