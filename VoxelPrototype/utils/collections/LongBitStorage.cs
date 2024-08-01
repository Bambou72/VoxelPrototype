using System.Runtime.CompilerServices;
namespace VoxelPrototype.utils.collections
{
    internal sealed class LongBitStorage
    {
        internal long[] Data;
        private int Size;
        internal int BitPerEntry;
        internal int BitPerEntryMask;
        private long EntryMask;

        public LongBitStorage(long[] data, int size, int bitPerEntry)
        {
            if (bitPerEntry <= 0 || bitPerEntry > 64)
            {
                throw new ArgumentException("BitPerEntry should be in the range (0, 64].", nameof(bitPerEntry));
            }
            Size = size;
            BitPerEntry = bitPerEntry;
            EntryMask = (1L << bitPerEntry) - 1;
            BitPerEntryMask = CalculateShiftAmount(bitPerEntry);
            Data = data;
        }

        public LongBitStorage(int size, int bitPerEntry = 1)
        {
            if (bitPerEntry <= 0 || bitPerEntry > 64)
            {
                throw new ArgumentException("BitPerEntry should be in the range (0, 64].", nameof(bitPerEntry));
            }
            Size = size;
            BitPerEntry = bitPerEntry;
            EntryMask = (1L << bitPerEntry) - 1;
            BitPerEntryMask = CalculateShiftAmount(bitPerEntry);
            Data = new long[(Size * BitPerEntry + 63) / 64];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CalculateShiftAmount(int bitPerEntry)
        {
            return 6 - (int)Math.Log2(bitPerEntry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Get(int index)
        {
            return (ulong)(Data[index >> BitPerEntryMask] >> (index * BitPerEntry & 63) & EntryMask);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public void Set(int index, ulong value)
        {
            if (value > (ulong)EntryMask)
            {
                throw new ArgumentException($"Value should be in the range (0, {EntryMask}].", nameof(value));
            }
            int longIndex = index >> BitPerEntryMask;
            int offsetWithinLong = index * BitPerEntry & 63;
            ulong oldValue = Get(index); // Get the existing entry value
            if (oldValue != value)
            {
                if (offsetWithinLong + BitPerEntry <= 64)
                {
                    long mask = (long)EntryMask << offsetWithinLong;
                    Data[longIndex] = Data[longIndex] & ~mask | (long)value << offsetWithinLong;
                }
                else
                {
                    int bitsRemaining = 64 - offsetWithinLong;
                    long mask1 = (long)EntryMask << offsetWithinLong;
                    long mask2 = (1L << BitPerEntry - bitsRemaining) - 1;
                    Data[longIndex] = Data[longIndex] & ~mask1 | (long)value << offsetWithinLong;
                    Data[longIndex + 1] = Data[longIndex + 1] & ~mask2 | (long)value >> bitsRemaining;
                }
            }
        }

        public LongBitStorage Grow(int NewBitPerEntry)
        {
            if (NewBitPerEntry <= BitPerEntry)
            {
                return this;
            }
            LongBitStorage Ne = new LongBitStorage(Size, NewBitPerEntry);
            for (int i = 0; i < Size; i++)
            {
                Ne.Set(i, Get(i));
            }
            return Ne;
        }
    }
}
