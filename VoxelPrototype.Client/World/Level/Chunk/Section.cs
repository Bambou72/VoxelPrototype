using OpenTK.Mathematics;
using VoxelPrototype.VBF;
using VoxelPrototype.common.Utils.Storage.Palette;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.client.World.Level.Chunk.Render;
using VoxelPrototype.common;

namespace VoxelPrototype.client.World.Level.Chunk
{
    public enum MeshState
    {
        Dirty,
        Generating,
        Ready
    }
    public class Section : IVBFSerializable<Section>
    {
        public readonly static float SphereRadius = Const.SectionSize * MathF.Sqrt(3) / 2;
        internal BlockPalette BlockPalette;
        internal SectionMesh SectionMesh;
        internal int Y;
        internal ReaderWriterLockSlim Lock = new();
        internal MeshState MeshState = MeshState.Dirty;
        internal Vector3i Position
        {
            get
            {
                return new Vector3i(Chunk.X, Y, Chunk.Z);
            }
        }

        internal Chunk Chunk;
        public Section(Chunk chunk)
        {
            this.Chunk = chunk;
            BlockPalette = new(1);
        }

        public bool Empty { get { return BlockPalette.RefsCount[0] == Const.SectionVolume; } }

        public Section Deserialize(VBFCompound compound)
        {
            Y = compound.GetInt("Y").Value;
            BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BP"));
            SectionMesh = new(new Vector3i(Chunk.X, Y, Chunk.Z), this);
            return this;
        }
        public VBFCompound Serialize()
        {
            VBFCompound Section = new();
            Section.AddInt("Y", Y);
            Section.Add("BP", BlockPalette.Serialize());
            return Section;
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            if (pos.Y > Const.ChunkSizeM1 || pos.Y < 0)
            {
                throw new Exception("Error");
            }
            BlockPalette.Set(new Vector3i(pos.X, pos.Y, pos.Z), id);
        }
        public void Dispose()
        {
            SectionMesh.Destroy();
        }
    }
}
