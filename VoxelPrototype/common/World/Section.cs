using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.common.World.Storage.Palette;
namespace VoxelPrototype.common.World
{
    public class Section : IVBFSerializable<Section>
    {
        public const int Size = 16;
        internal BlockPalette BlockPalette;
        internal int Y;
        internal Chunk Chunk;
        public Section()
        {
            BlockPalette = new(1);
        }

        public bool Empty { get { return BlockPalette.RefsCount[0] == Math.Pow(Size, 3); } }

        public Section Deserialize(VBFCompound compound)
        {
            Y = compound.GetInt("YPos").Value;
            BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BlockPalette"));
            return this;
        }
        public VBFCompound Serialize()
        {
            VBFCompound Section = new();
            Section.AddInt("YPos", Y);
            Section.Add("BlockPalette", BlockPalette.Serialize());
            return Section;
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            if (pos.Y > 15 || pos.Y < 0)
            {
                throw new Exception("Error");
            }
            BlockPalette.Set(new Vector3i(pos.X, pos.Y, pos.Z), id);
        }
    }
}
