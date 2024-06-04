using OpenTK.Mathematics;
using VoxelPrototype.client.World;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;

namespace VoxelPrototype.client.Render.World
{
    internal class SectionMesh : IDestroyable
    {
        SubMesh OpaqueMesh;
        internal Vector3i Position;
        internal Vector3 Center;
        internal Section Section;
        internal SectionMeshGenerator Generator;

        public SectionMesh(Vector3i position, Section section)
        {
            OpaqueMesh = new();
            Position = position;
            Section = section;
            Generator = new SectionMeshGenerator(section, position);
            Center = (Position + new Vector3(0.5f)) * Section.Size;
        }
        public void Destroy()
        {
            OpaqueMesh.Destroy();
            OpaqueMesh = null;
            Generator = null;
        }
        internal void Generate()
        {
            Generator.Generate();
        }
        internal void Upload()
        {

            OpaqueMesh.SetupData(Generator.OpaqueVertices, Generator.OpaqueIndices);
            Generator.Clear();
        }
        public SubMesh GetOpaqueMesh()
        {
            return OpaqueMesh;
        }

    }
}
