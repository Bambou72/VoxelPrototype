﻿using OpenTK.Mathematics;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.VBF;

namespace VoxelPrototype.client.World.Level.Chunk
{
    [Flags]
    internal enum ChunkSate
    {
        None = 0,
        Ready = 1,
        Changed = 2,
        Unsaved = 4,
    }
    public class Chunk : IVBFSerializableBinary<Chunk>
    {
        //Chunk state for meshing
        internal ChunkSate State = ChunkSate.Changed;
        internal Section[] Sections;
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
        public bool IsSurrendedClient()
        {
            if (Client.TheClient.World.ChunkManager.GetChunk(X + 1, Z) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X - 1, Z) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X, Z + 1) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X, Z - 1) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X - 1, Z - 1) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X + 1, Z - 1) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X - 1, Z + 1) == null) return false;
            if (Client.TheClient.World.ChunkManager.GetChunk(X + 1, Z + 1) == null) return false;
            return true;
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
                Sections[i] = new Section(this).Deserialize((VBFCompound)DeSections.Tags[i]);
                Sections[i].Chunk = this;
            }
            return this;
        }
        public Section GetSection(int Y)
        {
            if (Y < Const.ChunkHeight * Section.Size && Y >= 0)
            {
                return Sections[Y >> 4];
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
            if (pos.Y < Const.ChunkHeight * Section.Size && pos.Y >= 0)
            {
                //int sectionIndex = pos.Y / 16;
                int sectionIndex = pos.Y >> 4;
                //int sectionOffset = pos.Y % 16; 
                pos.Y = pos.Y & 15;
                return Sections[sectionIndex].BlockPalette.Get(pos);
            }
            else
            {
                return Client.TheClient.ModManager.BlockRegister.Air;
            }
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            int YValue = pos.Y / Section.Size;
            var section = Sections[YValue];
            int y = pos.Y - YValue * 16;
            section.SetBlock(new Vector3i(pos.X, y, pos.Z), id);
        }
    }

}