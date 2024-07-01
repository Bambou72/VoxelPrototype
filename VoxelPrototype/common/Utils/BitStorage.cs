using System.Collections;
using System.Runtime.CompilerServices;

namespace VoxelPrototype.common.Utils
{

    internal sealed class BitStorage 
    {
        internal int[] Data;
        private int Size;
        internal int BitPerEntry;
        internal int BitPerEntryMask;
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
            BitPerEntryMask = CalculateShiftAmount(bitPerEntry);
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
            BitPerEntryMask = CalculateShiftAmount(bitPerEntry);
            Data = new int[(Size * BitPerEntry + 31) / 32];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CalculateShiftAmount(int bitPerEntry)
        {
            return 5 - (int)Math.Log2(bitPerEntry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Get(int index)
        {
            return (ushort)((Data[index  >> BitPerEntryMask] >> ((index * BitPerEntry) & 31)) & EntryMask);
        }

        public void Set(int index, ushort value)
        {
            if (value > EntryMask)
            {
                throw new ArgumentException($"Value should be in the range (0, {EntryMask}].", nameof(value));
            }
            int intIndex = index >> BitPerEntryMask;
            int offsetWithinInt = (index * BitPerEntry) & 31;
            ushort oldValue = Get(index); // Get the existing entry value
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
                    int mask2 = (1 << (BitPerEntry - bitsRemaining)) - 1;
                    Data[intIndex] = Data[intIndex] & ~mask1 | value << offsetWithinInt;
                    Data[intIndex + 1] = Data[intIndex + 1] & ~mask2 | value >> bitsRemaining;
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
                Ne.Set(i, Get(i));
            }
            return Ne;
        }
    }
}
