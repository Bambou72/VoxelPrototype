using NLog;
using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Utils;
using VoxelPrototype.VBF;
namespace VoxelPrototype.common.Utils.Storage.Palette
{
    internal class BlockPalette : IVBFSerializable<BlockPalette>
    {
        public BlockState[] Palette { get; set; }
        public ushort PaletteCount { get; set; }
        public int BitCount { get; set; }
        public bool Full => PaletteCount == Palette.Length;
        public int[] RefsCount { get; set; }
        BitStorage Data;
        public BlockPalette(int bitCount)
        {
            Data = new(Const.SectionVolume, 1);
            BitCount = bitCount;
            Palette = new BlockState[(int)Math.Pow(2, BitCount)];
            RefsCount = new int[(int)Math.Pow(2, BitCount)];
            Palette[0] = Client.TheClient.ModManager.BlockRegister.Air;
            RefsCount[0] = Const.SectionVolume;
            PaletteCount = 1;
        }
        public ushort GetOrAdd(BlockState State)
        {
            for (ushort i = 0; i < Palette.Length; i++)
            {
                if (Palette[i] != null && Palette[i].Equals(State))
                {
                    RefsCount[i]++;
                    return i;
                }
            }
            if (Full)
            {
                for (ushort i = 0; i < Palette.Length; i++)
                {
                    if (Palette[i] == null || RefsCount[i] == 0 && !Palette[i].Equals(Client.TheClient.ModManager.BlockRegister.Air))
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
            ushort NewID = PaletteCount++;
            Palette[NewID] = State;
            RefsCount[NewID] = 1;
            return NewID;
        }
        public BlockState Get(Vector3i Position)
        {
            return Palette[Data.Get(Utils.TreetoOne(Position))];
        }
        public void Set(Vector3i Position, BlockState State)
        {
            ushort previousId = Data.Get(Utils.TreetoOne(Position));
            RefsCount[previousId]--;
            ushort indexid = GetOrAdd(State);
            if (BitCount > Data.BitPerEntry)
            {
                Data = Data.Grow(BitCount);
            }
            Data.Set(Utils.TreetoOne(Position), indexid);
        }

        public VBFCompound Serialize()
        {
            VBFCompound BlockPalette = new VBFCompound();
            BlockPalette.AddIntArray("Data", Data.Data);
            BlockPalette.AddInt("BPE", Data.BitPerEntry);
            BlockPalette.AddInt("PC", PaletteCount);
            BlockPalette.AddInt("BC", BitCount);
            VBFList Palette = new VBFList();
            Palette.ListType = VBFTag.DataType.Compound;
            for (int i = 0; i < this.Palette.Length; i++)
            {
                VBFCompound PaletteElement = new VBFCompound();
                if (this.Palette[i] != null)
                {
                    PaletteElement.Add("Bs", this.Palette[i].Serialize());
                    PaletteElement.AddInt("RC", RefsCount[i]);
                    Palette.Tags.Add(PaletteElement);
                }
            }
            BlockPalette.Add("Palette", Palette);
            return BlockPalette;
        }

        public BlockPalette Deserialize(VBFCompound compound)
        {
            try
            {
                BlockPalette storage = new BlockPalette(compound.GetInt("BC").Value);
                storage.Data = new BitStorage(compound.GetIntArray("Data").Value, 4096, compound.GetInt("BPE").Value);
                storage.PaletteCount = (ushort)compound.GetInt("PC").Value;
                VBFList paletteList = compound.Get<VBFList>("Palette");
                for (int i = 0; i < paletteList.Tags.Count; i++)
                {
                    VBFCompound PaletteElement = (VBFCompound)paletteList.Tags[i];
                    storage.Palette[i] = new BlockState().Deserialize(PaletteElement.Get<VBFCompound>("Bs"));
                    storage.RefsCount[i] = PaletteElement.GetInt("RC").Value;
                }
                return storage;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Failed to deserialize block storage.");
                return this;
            }

        }
    }
}
