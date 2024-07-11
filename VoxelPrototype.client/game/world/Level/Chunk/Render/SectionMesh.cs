using OpenTK.Mathematics;
namespace VoxelPrototype.client.game.world.Level.Chunk.Render
{
    internal class SectionMesh
    {
        SubMesh OpaqueMesh;
        internal Vector3i Position;
        internal Vector3 Center;
        public SectionMesh(Vector3i position, Section section)
        {
            OpaqueMesh = new();
            Position = position;
            Center = (Position + new Vector3(0.5f)) * Const.SectionSize;
        }
        public void Destroy()
        {
            OpaqueMesh.Destroy();
            OpaqueMesh = null;
        }
        internal void Upload(SectionVertex[] OpaqueVertices, uint[] OpaqueIndices)
        {
#if PROFILE
            using (Profiler.BeginEvent("Section Mesh Upload",Profiler.ColorType.Red))
#endif
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
