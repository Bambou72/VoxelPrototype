using OpenTK.Mathematics;
using VoxelPrototype.api.block.state;
using VoxelPrototype.client.game.world.Level.Chunk.Render;
using VoxelPrototype.game;
using VoxelPrototype.game.world;
using VoxelPrototype.game.world.storage;
namespace VoxelPrototype.client.game.world.Level.Chunk
{
    public enum MeshState
    {
        Dirty,
        Generating,
        Ready
    }
    public class Section
    {
        public readonly static float SphereRadius = Const.SectionSize * MathF.Sqrt(3) / 2;
        internal SectionMesh SectionMesh;
        internal MeshState MeshState = MeshState.Dirty;
        public BlockPalette BlockPalette;
        public Chunk ParentChunk;
        public int Y;
        public bool Empty { get { return BlockPalette.Palette[0].RefCount == Const.SectionVolume; } }
        public Section(Chunk Chunk)
        {
            SectionMesh = new(this);
            ParentChunk = Chunk;
            BlockPalette = new(1);
        }
        public Vector3 Center => (Position + new Vector3(0.5f)) * Const.SectionSize;
        public Vector3i Position
        {
            get
            {
                return new Vector3i(ParentChunk.X, Y, ParentChunk.Z);
            }
        }
        public Section Deserialize(VBFCompound compound)
        {
            Y = compound.GetInt("Y").Value;
            if (compound.Contains("BP"))
            {
                BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BP"));
            }
            return this;
        }
        public VBFCompound Serialize()
        {
            VBFCompound Section = new();
            Section.AddInt("Y", Y);
            if(!Empty)
            {
                Section.Add("BP", BlockPalette.Serialize());
            }
            return Section;
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            if (pos.Y >= Const.SectionSize || pos.Y < 0)
            {
                throw new Exception("Error");
            }
            BlockPalette.Set(pos, id);
        }

        public void Dispose()
        {
            SectionMesh.Destroy();
        }
    }
}
