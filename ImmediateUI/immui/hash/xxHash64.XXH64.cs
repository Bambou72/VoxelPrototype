// ReSharper disable InconsistentNaming

namespace ImmediateUI.immui.hash
{
    using System.Runtime.CompilerServices;

    public static partial class xxHash64
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_round(ulong acc, ulong input)
        {
            acc += input * XXH_PRIME64_2;
            acc  = XXH_rotl64(acc, 31);
            acc *= XXH_PRIME64_1;
            return acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_avalanche(ulong hash)
        {
            hash ^= hash >> 33;
            hash *= XXH_PRIME64_2;
            hash ^= hash >> 29;
            hash *= XXH_PRIME64_3;
            hash ^= hash >> 32;
            return hash;
        }
    }
}
