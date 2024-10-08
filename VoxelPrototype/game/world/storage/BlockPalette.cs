﻿using NLog;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;
using VoxelPrototype.utils.collections;
namespace VoxelPrototype.game.world.storage
{
    public struct BlockPaletteEntry
    {
        public ushort RefCount;
        public BlockState State;
    }
    public class BlockPalette : IVBFSerializable<BlockPalette>
    {
        public BlockPaletteEntry[] Palette { get; set; }
        LongBitStorage Data;
        public ushort PaletteCount { get; set; }
        public int BitCount { get; set; }
        public bool Full => PaletteCount == Palette.Length;
        public BlockPalette(int bitCount)
        {
            Data = new(Const.SectionVolume);
            BitCount = bitCount;
            Palette = new BlockPaletteEntry[(int)Math.Pow(2, BitCount)];
            Palette[0] = new()
            {
                RefCount = Const.SectionVolume,
                State = BlockRegistry.GetInstance().Air
            };
            PaletteCount = 1;
        }
        public ushort GetOrAdd(BlockState State)
        {
            for (ushort i = 0; i < Palette.Length; i++)
            {
                if (Palette[i].State != null && Palette[i].State == State)
                {
                    Palette[i].RefCount++;
                    return i;
                }
            }
            if (Full)
            {
                for (ushort i = 0; i < Palette.Length; i++)
                {
                    if (Palette[i].State == null || Palette[i].RefCount == 0 && !(Palette[i].State  == BlockRegistry.GetInstance().Air))
                    {
                        Palette[i].State = State;
                        Palette[i].RefCount = 1;
                        return i;
                    }
                }
                BitCount *= 2;
                BlockPaletteEntry[] newpa = new BlockPaletteEntry[(int)Math.Pow(2, BitCount)];
                Array.Copy(Palette, newpa, Palette.Length);
                Palette = newpa;
            }
            ushort NewID = PaletteCount++;
            Palette[NewID].State = State;
            Palette[NewID].RefCount = 1;
            return NewID;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockState Get(Vector3i Position)
        {
            return Palette[Data.Get(TreetoOne(Position))].State;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public void Set(Vector3i Position, BlockState State)
        {
            ulong previousId = Data.Get(TreetoOne(Position));
            Palette[previousId].RefCount--;
            ushort indexid = GetOrAdd(State);
            if (BitCount > Data.BitPerEntry)
            {
                Data = Data.Grow(BitCount);
            }
            Data.Set(TreetoOne(Position), indexid);
        }
        public VBFCompound Serialize()
        {
            VBFCompound BlockPalette = new VBFCompound();
            BlockPalette.AddLongArray("Data", Data.Data);
            BlockPalette.AddInt("BC", BitCount);
            BlockPalette.AddInt("PC", PaletteCount);
            VBFList Palette = new VBFList();
            Palette.ListType = VBFTag.DataType.Compound;
            for (int i = 0; i < this.Palette.Length; i++)
            {
                VBFCompound PaletteElement = new VBFCompound();
                if (this.Palette[i].State != null)
                {
                    PaletteElement.Add("Bs", this.Palette[i].State.Serialize());
                    PaletteElement.AddInt("RC", this.Palette[i].RefCount);
                    Palette.Tags.Add(PaletteElement);
                }
            }
            BlockPalette.Add("Palette", Palette);
            return BlockPalette;
        }

        public  BlockPalette Deserialize(VBFCompound compound)
        {
            try
            {
                int BitCount = compound.GetInt("BC").Value;
                BlockPalette storage = new BlockPalette(BitCount);
                storage.Data = new LongBitStorage(compound.GetLongArray("Data").Value, Const.SectionVolume, BitCount);
                storage.PaletteCount = (ushort)compound.GetInt("PC").Value;
                VBFList paletteList = compound.Get<VBFList>("Palette");
                for (int i = 0; i < paletteList.Tags.Count; i++)
                {
                    VBFCompound PaletteElement = (VBFCompound)paletteList.Tags[i];
                    storage.Palette[i].State = new BlockState().Deserialize(PaletteElement.Get<VBFCompound>("Bs"));
                    storage.Palette[i].RefCount = (ushort)PaletteElement.GetInt("RC").Value;
                }
                return storage;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Failed to deserialize block storage.");
                return this;
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int TreetoOne(Vector3i pos)
        {
            return pos.Z << 2 * Const.SectionSizePowerOf2 | pos.Y << Const.SectionSizePowerOf2 | pos.X;
        }
    }
}
