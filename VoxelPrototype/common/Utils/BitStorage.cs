namespace VoxelPrototype.common.Utils
{
    internal sealed class BitStorage
    {
        internal int[] Data;
        private int Size;
        internal int BitPerEntry;
        private int EntryMask;
        public BitStorage(int[] data, int size, int bitPerEntry)
        {
            if (bitPerEntry <= 0 || bitPerEntry > 32)
            {
                throw new ArgumentException("BitPerEntry should be in the range (0, 32].", nameof(bitPerEntry));
            }
            Size = size;
            BitPerEntry = bitPerEntry;
            EntryMask = (1 << bitPerEntry) - 1;
            Data = data;
        }
        public BitStorage(int size, int bitPerEntry)
        {
            if (bitPerEntry <= 0 || bitPerEntry > 32)
            {
                throw new ArgumentException("BitPerEntry should be in the range (0, 32].", nameof(bitPerEntry));
            }
            Size = size;
            BitPerEntry = bitPerEntry;
            EntryMask = (1 << bitPerEntry) - 1;
            Data = new int[(Size * BitPerEntry + 31) / 32];
        }
        public int this[int index]
        {
            get
            {
                int intIndex = index * BitPerEntry / 32;
                int offsetWithinInt = index * BitPerEntry % 32;
                if (offsetWithinInt + BitPerEntry <= 32)
                {
                    return Data[intIndex] >> offsetWithinInt & EntryMask;
                }
                else
                {
                    int remainingBits = 32 - offsetWithinInt;
                    int firstPart = Data[intIndex] >> offsetWithinInt;
                    int secondPart = Data[intIndex + 1] & (1 << BitPerEntry - remainingBits) - 1;
                    return (secondPart << remainingBits | firstPart) & EntryMask;
                }
            }
            set
            {
                if (value < 0 || value > EntryMask)
                {
                    throw new ArgumentException($"Value should be in the range (0, {EntryMask}].", nameof(value));
                }
                int intIndex = index * BitPerEntry / 32;
                int offsetWithinInt = index * BitPerEntry % 32;
                int oldValue = this[index]; // Get the existing entry value
                if (oldValue != value)
                {
                    if (offsetWithinInt + BitPerEntry <= 32)
                    {
                        int mask = EntryMask << offsetWithinInt;
                        Data[intIndex] = Data[intIndex] & ~mask | value << offsetWithinInt;
                    }
                    else
                    {
                        int bitsRemaining = 32 - offsetWithinInt;
                        int mask1 = EntryMask << offsetWithinInt;
                        int mask2 = (1 << BitPerEntry - bitsRemaining) - 1;
                        Data[intIndex] = Data[intIndex] & ~mask1 | value << offsetWithinInt;
                        Data[intIndex + 1] = Data[intIndex + 1] & ~mask2 | value >> bitsRemaining;
                    }
                }
            }
        }
        public BitStorage Grow(int NewBitPerEntry)
        {
            if (NewBitPerEntry <= BitPerEntry)
            {
                return this;
            }
            BitStorage Ne = new BitStorage(Size, NewBitPerEntry);
            for (int i = 0; i < Size; i++)
            {
                Ne[i] = this[i];
            }
            return Ne;
        }
    }
}
