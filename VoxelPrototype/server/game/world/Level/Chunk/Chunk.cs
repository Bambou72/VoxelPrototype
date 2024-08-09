using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;

namespace VoxelPrototype.server.game.world.Level.Chunk
{
    [Flags]
    internal enum ChunkSate
    {
        None = 0,
        Ready = 1,
        Changed = 2,
        Unsaved = 4,
    }
    internal enum ServerChunkSate
    {
        None,
        Dirty,
    }

    public class Chunk :  IVBFSerializable<Chunk>
    {
        //Chunk state for meshing
        internal ChunkSate State = ChunkSate.Changed;
        internal ServerChunkSate ServerState = ServerChunkSate.None;
        internal List<int> PlayerInChunk { get; set; }

        internal Section[] Sections;
        //Chunk coordinates
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
            try
            {
                X = data.GetInt("X").Value;
                Z = data.GetInt("Z").Value;
                int SectionsCount = data.GetInt("SC").Value;
                Sections = new Section[SectionsCount];
                VBFList DeSections = data.Get<VBFList>("Sec");
                for (int i = 0; i < SectionsCount; i++)
                {
                    Sections[i] = new Section(this).Deserialize((VBFCompound)DeSections.Tags[i]);
                }
                return this;
            }
            catch
            {
                return null;
            }

        }
        internal Chunk()
        {
            PlayerInChunk = new();
        }
        internal Chunk(Vector2i Pos)
        {
            PlayerInChunk = new List<int>();
            X = Pos.X;
            Z = Pos.Y;
            Sections = new Section[Const.ChunkHeight];

            // Boucle for ajustée pour les coordonnées y
            for (int y = 0; y < Const.ChunkHeight; y++)
            {
                var Temp = new Section(this);
                Temp.Y = y;
                Sections[y] = Temp;
            }
        }
        public Section GetSection(int Y)
        {
            if (Y < Const.ChunkRHeight && Y >= 0)
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
            if (pos.Y < Const.ChunkRHeight && pos.Y >= 0)
            {
                //int sectionIndex = pos.Y / 16;
                int sectionIndex = pos.Y >> Const.BitShifting;
                //int sectionOffset = pos.Y % 16; 
                pos.Y &= Const.And;
                return Sections[sectionIndex].BlockPalette.Get(pos);
            }
            else
            {
                return BlockRegistry.GetInstance().Air;
            }
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            int YValue = pos.Y >> Const.BitShifting;
            var section = Sections[YValue];
            pos.Y &= Const.And;
            section.SetBlock(pos, id);
            ServerState = ServerChunkSate.Dirty;
        }
    }

}
