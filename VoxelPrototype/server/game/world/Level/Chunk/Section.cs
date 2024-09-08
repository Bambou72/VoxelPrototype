using OpenTK.Mathematics;
using VoxelPrototype.api.block.state;
using VoxelPrototype.game;
using VoxelPrototype.game.world.storage;
using VoxelPrototype.game.world;
namespace VoxelPrototype.server.game.world.Level.Chunk
{
    public class Section 
    {
        public BlockPalette BlockPalette;
        public Chunk ParentChunk;
        public int Y;
        public bool Empty { get { return BlockPalette.Palette[0].RefCount == Const.SectionVolume; } }
        public Section(Chunk Chunk)
        {
            ParentChunk = Chunk;
            BlockPalette = new(1);
        }
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
            if(compound.Contains("BP"))
            {
                BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BP"));
            }
            return this;
        }
        public VBFCompound Serialize()
        {
            VBFCompound Section = new();
            Section.AddInt("Y", Y);
            if (!Empty)
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

    }
}
