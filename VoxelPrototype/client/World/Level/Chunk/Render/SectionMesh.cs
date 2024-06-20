using OpenTK.Mathematics;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;

namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal class SectionMesh : IDestroyable
    {
        SubMesh OpaqueMesh;
        internal Vector3i Position;
        internal Vector3 Center;
        internal Section Section;
        public SectionMesh(Vector3i position, Section section)
        {
            OpaqueMesh = new();
            Position = position;
            Section = section;
            Center = (Position + new Vector3(0.5f)) * Section.Size;
        }
        public void Destroy()
        {
            OpaqueMesh.Destroy();
            OpaqueMesh = null;
        }
        internal void Upload(SectionVertex[] OpaqueVertices, uint[] OpaqueIndices)
        {
            OpaqueMesh.SetupData(OpaqueVertices, OpaqueIndices);
        }
        public SubMesh GetOpaqueMesh()
        {
            return OpaqueMesh;
        }

    }
}
