using OpenTK.Mathematics;
using VoxelPrototype.VBF;
using VoxelPrototype.common.Utils.Storage.Palette;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.client.World.Level.Chunk.Render;

namespace VoxelPrototype.client.World.Level.Chunk
{
    public class Section : IVBFSerializable<Section>
    {
        public const int Size = 16;
        public readonly static float SphereRadius = Size * MathF.Sqrt(3) / 2;
        internal BlockPalette BlockPalette;
        internal SectionMesh SectionMesh;
        internal int Y;
        internal Chunk Chunk;
        public Section(Chunk chunk)
        {
            this.Chunk = chunk;
            BlockPalette = new(1);
        }

        public bool Empty { get { return BlockPalette.RefsCount[0] == Math.Pow(Size, 3); } }

        public Section Deserialize(VBFCompound compound)
        {
            Y = compound.GetInt("Y").Value;
            BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BP"));
            SectionMesh = new(new Vector3i(Chunk.X, Y, Chunk.Z), this);
            return this;
        }
        public int GetLinearIndex(int x, int y, int z)
        {
            const int sectionSize = 16;
            const int extendedSize = sectionSize + 2; // 18 (16 for the section plus 1 layer on each side)

            // Transform coordinates to the 0-based extended space
            int transformedX = x + 1;
            int transformedY = y + 1;
            int transformedZ = z + 1;

            // Calculate the linear index
            return transformedX * extendedSize * extendedSize + transformedY * extendedSize + transformedZ;
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
            if (pos.Y > 15 || pos.Y < 0)
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
