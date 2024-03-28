/**
 * Chunk implementation shared by the client and the server
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using OpenTK.Mathematics;
using VBF;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.API.Blocks.State;
using VoxelPrototype.server;
namespace VoxelPrototype.common.Game.World
{
    public class Chunk : IVBFSerializableBinary<Chunk>
    {
        //Chunk state for meshing
        internal ChunkSate State = ChunkSate.Changed;
        internal ServerChunkSate ServerState = ServerChunkSate.None;
        public const int Height = 32;
        public const int Size = 16;
        internal List<int> PlayerInChunk { get; set; }
        Section[] Sections;
        //Chunk coordinates
        public int X { get; set; }
        public int Z { get; set; }
        public Vector2i Position { get { return new Vector2i(X, Z); } set { X = value.X; Z = value.Y; } }
        public byte[] Serialize()
        {
            VBFCompound Chunk = new();
            Chunk.AddInt("PosX", X);
            Chunk.AddInt("PosZ", Z);
            Chunk.AddInt("SectionsCount", Sections.Length);
            VBFList SeSections = new();
            SeSections.ListType = VBFTag.DataType.Compound;
            for (int i = 0; i < Sections.Length; i++)
            {
                SeSections.Tags.Add(Sections[i].Serialize());
            }
            Chunk.Add("Sections", SeSections);
            return VBFSerializer.Serialize(Chunk);
        }

        public Chunk Deserialize(byte[] data)
        {
            VBFCompound compound = (VBFCompound)VBFSerializer.Deserialize(data);
            X = compound.GetInt("PosX").Value;
            Z = compound.GetInt("PosZ").Value;
            int SectionsCount = compound.GetInt("SectionsCount").Value;
            Sections = new Section[SectionsCount];
            VBFList DeSections = compound.Get<VBFList>("Sections");
            for (int i = 0; i < SectionsCount; i++)
            {
                Sections[i] = new Section().Deserialize((VBFCompound)DeSections.Tags[i]);
            }
            return this;
        }
        internal Chunk()
        {
            PlayerInChunk = new();
        }
        internal Chunk(Vector2i Pos, bool Gen)
        {
            PlayerInChunk = new List<int>();
            X = Pos.X;
            Z = Pos.Y;
            Sections = new Section[Height];

            // Boucle for ajustée pour les coordonnées y
            for (int y = 0; y < Height; y++)
            {
                var Temp = new Section();
                Temp.Y = y;
                Sections[y] = Temp;
            }
            if (Gen)
            {
                Server.TheServer.World.WorldGenerator.GenerateChunk(this);
            }
        }
        public BlockState GetBlock(Vector3i pos)
        {
            if (!(pos.Y >= Height * Section.Size || pos.Y < 0))
            {
                int sectionIndex = pos.Y / 16;
                int sectionOffset = pos.Y % 16;
                return Sections[sectionIndex].GetBlock(new Vector3i(pos.X, sectionOffset, pos.Z));
            }
            else
            {
                return BlockRegister.Air;
            }
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            int YValue = pos.Y / Section.Size;
            var section = Sections[YValue];
            int y = pos.Y - YValue * 16;
            section.SetBlock(new Vector3i(pos.X, y, pos.Z), id);
            ServerState = ServerChunkSate.Dirty;
        }
    }
}
