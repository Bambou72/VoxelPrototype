using OpenTK.Mathematics;
namespace VoxelPrototype.client.game.world.Level.Chunk.Render
{
    internal class SectionMesh
    {
        SubMesh OpaqueMesh;
        public SectionMesh(Section section)
        {
            OpaqueMesh = new();
        }
        public void Destroy()
        {
            OpaqueMesh.Destroy();
            OpaqueMesh = null;
        }
        internal void Upload(SectionVertex[] OpaqueVertices, uint[] OpaqueIndices)
        {
            {
                OpaqueMesh.SetupData(OpaqueVertices, OpaqueIndices);
            }
        }
        public SubMesh GetOpaqueMesh()
        {
            return OpaqueMesh;
        }

    }
}
