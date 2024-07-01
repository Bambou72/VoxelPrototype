using OpenTK.Mathematics;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.VBF;

namespace VoxelPrototype.client.World.Level.Chunk
{



    public class Chunk : IVBFSerializable<Chunk>
    {
        public readonly static float CircleRadius = (Const.ChunkSize * MathF.Sqrt(2)) / 2;

        //Chunk state for meshing
        internal ClientChunkManager Manager;
        //internal ChunkSate State = ChunkSate.Changed;
        internal Section[] Sections;
        //Chunk coordinates
        public Vector2 Center 
        { 
            get 
            {
                return (Position + new Vector2(0.5f)) * Const.ChunkSize;
            } 
        }
        public int X { get; set; }
        public int Z { get; set; }
        public Vector2i Position { get { return new Vector2i(X, Z); } set { X = value.X; Z = value.Y; } }
        public VBFCompound Serialize()
        {
            VBFCompound Chunk = new();
            Chunk.AddInt("X", X);
            Chunk.AddInt("Z", Z);
            Chunk.AddInt("SC", Sections.Length);
            VBFList SeSections = new();
            SeSections.ListType = VBFTag.DataType.Compound;
            for (int i = 0; i < Sections.Length; i++)
            {
                SeSections.Tags.Add(Sections[i].Serialize());
            }
            Chunk.Add("Sec", SeSections);
            return Chunk;
        }

        public Chunk Deserialize(VBFCompound data)
        {
            X = data.GetInt("X").Value;
            Z = data.GetInt("Z").Value;
            int SectionsCount = data.GetInt("SC").Value;
            Sections = new Section[SectionsCount];
            VBFList DeSections = data.Get<VBFList>("Sec");
            for (int i = 0; i < SectionsCount; i++)
            {
                Sections[i] = new Section(this).Deserialize((VBFCompound)DeSections.Tags[i]);
                Sections[i].Chunk = this;
            }
            return this;
        }
        public Section GetSection(int Y)
        {
            if (Y < Const.ChunkHeight * Const.SectionSize && Y >= 0)
            {
                return Sections[Y >> Const.BitShifting];
            }
            return null;
        }
        public Section GetSectionByIndex(int Y)
        {
            if (Y >= 0 && Y < Const.ChunkHeight) return Sections[Y];
            return null;
        }
        public BlockState GetBlock(Vector3i pos)
        {
            if (pos.Y < Const.ChunkHeight * Const.SectionSize && pos.Y >= 0)
            {
                int sectionIndex = pos.Y >> Const.BitShifting;
                pos.Y &= Const.And;
                return Sections[sectionIndex].BlockPalette.Get(pos);
            }
            return Client.TheClient.ModManager.BlockRegister.Air;            
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            int YValue = pos.Y >> Const.BitShifting;
            var section = Sections[YValue];
            int y = pos.Y & Const.And;
            section.SetBlock(new Vector3i(pos.X, y, pos.Z), id);
        }
        public void Dispose()
        {
            foreach(var section in Sections)
            {
                section.Dispose();
            }
        }
    }
}
