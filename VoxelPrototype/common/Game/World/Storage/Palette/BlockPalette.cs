﻿using NLog;
using OpenTK.Mathematics;
using System.Runtime.Serialization.Formatters.Binary;
using VBF;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.API.Blocks.state;
using VoxelPrototype.common.Utils;
namespace VoxelPrototype.common.Game.World.Storage.Palette
{
    internal class BlockPalette : IVBFSerializable<BlockPalette>
    {
        public BlockState[] Palette { get; set; }
        public int PaletteCount { get; set; }
        public int BitCount { get; set; }
        public bool Full => PaletteCount == Palette.Length;
        public int[] RefsCount { get; set; }
        BitStorage Data;
        public BlockPalette(int bitCount)
        {
            Data = new(Section.Size * Section.Size * Section.Size, 1);
            BitCount = bitCount;
            Palette = new BlockState[(int)Math.Pow(2, BitCount)];
            RefsCount = new int[(int)Math.Pow(2, BitCount)];
            Palette[0] = BlockRegister.Air;
            RefsCount[0] = Section.Size * Section.Size * Section.Size;
            PaletteCount = 1;
        }
        private BlockState Get(int index)
        {
            return Palette[index];
        }
        public int GetOrAdd(BlockState State)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                if (Palette[i] != null && Palette[i].Equals(State))
                {
                    RefsCount[i]++;
                    return i;
                }
            }
            if (Full)
            {
                for (int i = 0; i < Palette.Length; i++)
                {
                    if (Palette[i] == null || RefsCount[i] == 0 && !Palette[i].Equals(BlockRegister.Air))
                    {
                        Palette[i] = State;
                        RefsCount[i] = 1;
                        return i;
                    }
                }
                BitCount *= 2;
                BlockState[] newpa = new BlockState[(int)Math.Pow(2, BitCount)];
                int[] newrc = new int[(int)Math.Pow(2, BitCount)];
                Array.Copy(Palette, newpa, Palette.Length);
                Array.Copy(RefsCount, newrc, RefsCount.Length);
                Palette = newpa;
                RefsCount = newrc;
            }
            int NewID = PaletteCount++;
            Palette[NewID] = State;
            RefsCount[NewID] = 1;
            return NewID;
        }
        public BlockState Get(Vector3i Position)
        {

            return Get(Data[Utils.TreetoOne(Position.X, Position.Y, Position.Z)]);
        }
        public void Set(Vector3i Position, BlockState State)
        {
            int previousId = Data[Utils.TreetoOne(Position.X, Position.Y, Position.Z)];
            RefsCount[previousId]--;
            int indexid = GetOrAdd(State);
            if (BitCount > Data.BitPerEntry)
            {
                Data = Data.Grow(BitCount);
            }
            Data[Utils.TreetoOne(Position.X, Position.Y, Position.Z)] = indexid;
        }

        public VBFCompound Serialize()
        {
            VBFCompound BlockPalette = new VBFCompound();
            BlockPalette.AddIntArray("Data", Data.Data);
            BlockPalette.AddInt("BitPerEntry", Data.BitPerEntry);
            BlockPalette.AddInt("PaletteCount", PaletteCount);
            BlockPalette.AddInt("BitCount", BitCount);
            VBFList Palette = new VBFList();
            Palette.ListType = VBFTag.DataType.Compound;
            for (int i = 0; i < this.Palette.Length; i++)
            {
                VBFCompound PaletteElement = new VBFCompound();
                if (this.Palette[i] != null)
                {
                    PaletteElement.AddString("ID", this.Palette[i].GetBlock().Id);
                }
                else
                {
                    PaletteElement.AddString("ID", "null");
                }
                PaletteElement.AddInt("RefCount", RefsCount[i]);
                Palette.Tags.Add(PaletteElement);
            }
            BlockPalette.Add("Palette", Palette);
            return BlockPalette;

        }

        public BlockPalette Deserialize(VBFCompound compound)
        {
            try
            {
                BlockPalette storage = new BlockPalette(compound.GetInt("BitCount").Value);
                storage.Data = new BitStorage(compound.GetIntArray("Data").Value, 4096, compound.GetInt("BitPerEntry").Value);
                storage.PaletteCount = compound.GetInt("PaletteCount").Value;
                VBFList paletteList = compound.Get<VBFList>("Palette");
                for (int i = 0; i < paletteList.Tags.Count; i++)
                {
                    VBFCompound PaletteElement = (VBFCompound)paletteList.Tags[i];
                    string ID = PaletteElement.GetString("ID").Value;
                    BinaryFormatter formatter = new BinaryFormatter();
                    if (ID == "null")
                    {
                        storage.Palette[i] = null;
                    }
                    else
                    {
                        storage.Palette[i] = BlockRegister.GetBlock(ID).GetDefaultState();
                    }
                    storage.RefsCount[i] = PaletteElement.GetInt("RefCount").Value;
                }
                return storage;
            }
            catch (Exception ex)
            {
                // Gérer les erreurs de désérialisation ici (par exemple, enregistrer un avertissement ou renvoyer une valeur par défaut)
                LogManager.GetCurrentClassLogger().Error(ex, "Failed to deserialize block storage.");
                return this;
            }

        }
    }
}