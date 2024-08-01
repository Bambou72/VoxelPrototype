using NLog;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
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
                if (Palette[i].RefCount != 0 && Palette[i].State != null && Palette[i].State == State)
                {
                    Palette[i].RefCount++;
                    return i;
                }
            }
            if (PaletteCount == Palette.Length)
            {
                for (ushort i = 0; i < Palette.Length; i++)
                {
                    if (Palette[i].State == null || Palette[i].RefCount == 0 && !Palette[i].State.Equals(BlockRegistry.GetInstance().Air))
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
                Data = Data.Grow(BitCount);
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
            if (Palette[previousId].RefCount == 0 && Palette[previousId].State != BlockRegistry.GetInstance().Air)
            {
                PaletteCount--;
            }
            ushort indexid = GetOrAdd(State);
            Data.Set(TreetoOne(Position), indexid);
        }
        public VBFCompound Serialize()
        {
            VBFCompound BlockPalette = new VBFCompound();
            BlockPalette.AddLongArray("Data", Data.Data);
            BlockPalette.AddInt("BC", BitCount);
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
                VBFList paletteList = compound.Get<VBFList>("Palette");
                for (int i = 0; i < paletteList.Tags.Count; i++)
                {
                    VBFCompound PaletteElement = (VBFCompound)paletteList.Tags[i];
                    storage.Palette[i].State = new BlockState().Deserialize(PaletteElement.Get<VBFCompound>("Bs"));
                    storage.Palette[i].RefCount = (ushort)PaletteElement.GetInt("RC").Value;
                    if (storage.Palette[i].RefCount > 0)
                    {
                        storage.PaletteCount++;
                    }
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
