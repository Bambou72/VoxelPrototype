/*
MIT License
Copyright (c) 2018 Melnik Alexander
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Runtime.CompilerServices;
namespace ImmediateUI.immui.utils
{
    public static class xxHash64
    {
        private static readonly ulong XXH_PRIME64_1 = 11400714785074694791UL;
        private static readonly ulong XXH_PRIME64_2 = 14029467366897019727UL;
        private static readonly ulong XXH_PRIME64_3 = 1609587929392839161UL;
        private static readonly ulong XXH_PRIME64_4 = 9650029242287828579UL;
        private static readonly ulong XXH_PRIME64_5 = 2870177450012600261UL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH_rotl64(ulong x, int r)
        {
            return x << r | x >> 64 - r;
        }
        /// <summary>
        /// Compute xxHash for the data byte array
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(byte[] data, int length, ulong seed = 0)
        {
            fixed (byte* pData = &data[0])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        /// <summary>
        /// Compute xxHash for the data byte array
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(byte[] data, int offset, int length, ulong seed = 0)
        {
            fixed (byte* pData = &data[0 + offset])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeComputeHash(byte* ptr, int length, ulong seed)
        {
            return __inline__XXH64(ptr, length, seed);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_round(ulong acc, ulong input)
        {
            acc += input * XXH_PRIME64_2;
            acc = XXH_rotl64(acc, 31);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong __inline__XXH64(byte* input, int len, ulong seed)
        {
            ulong h64;

            if (len >= 32)
            {
                byte* end = input + len;
                byte* limit = end - 31;
                ulong v1 = seed + XXH_PRIME64_1 + XXH_PRIME64_2;
                ulong v2 = seed + XXH_PRIME64_2;
                ulong v3 = seed + 0;
                ulong v4 = seed - XXH_PRIME64_1;
                do
                {
                    var reg1 = *(ulong*)(input + 0);
                    var reg2 = *(ulong*)(input + 8);
                    var reg3 = *(ulong*)(input + 16);
                    var reg4 = *(ulong*)(input + 24);

                    // XXH64_round
                    v1 += reg1 * XXH_PRIME64_2;
                    v1 = v1 << 31 | v1 >> 64 - 31;
                    v1 *= XXH_PRIME64_1;

                    // XXH64_round
                    v2 += reg2 * XXH_PRIME64_2;
                    v2 = v2 << 31 | v2 >> 64 - 31;
                    v2 *= XXH_PRIME64_1;

                    // XXH64_round
                    v3 += reg3 * XXH_PRIME64_2;
                    v3 = v3 << 31 | v3 >> 64 - 31;
                    v3 *= XXH_PRIME64_1;

                    // XXH64_round
                    v4 += reg4 * XXH_PRIME64_2;
                    v4 = v4 << 31 | v4 >> 64 - 31;
                    v4 *= XXH_PRIME64_1;
                    input += 32;
                } while (input < limit);

                h64 = (v1 << 1 | v1 >> 64 - 1) +
                      (v2 << 7 | v2 >> 64 - 7) +
                      (v3 << 12 | v3 >> 64 - 12) +
                      (v4 << 18 | v4 >> 64 - 18);

                // XXH64_mergeRound
                v1 *= XXH_PRIME64_2;
                v1 = v1 << 31 | v1 >> 64 - 31;
                v1 *= XXH_PRIME64_1;
                h64 ^= v1;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v2 *= XXH_PRIME64_2;
                v2 = v2 << 31 | v2 >> 64 - 31;
                v2 *= XXH_PRIME64_1;
                h64 ^= v2;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v3 *= XXH_PRIME64_2;
                v3 = v3 << 31 | v3 >> 64 - 31;
                v3 *= XXH_PRIME64_1;
                h64 ^= v3;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v4 *= XXH_PRIME64_2;
                v4 = v4 << 31 | v4 >> 64 - 31;
                v4 *= XXH_PRIME64_1;
                h64 ^= v4;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;
            }
            else
            {
                h64 = seed + XXH_PRIME64_5;
            }

            h64 += (ulong)len;

            // XXH64_finalize
            len &= 31;
            while (len >= 8)
            {
                ulong k1 = XXH64_round(0, *(ulong*)input);
                input += 8;
                h64 ^= k1;
                h64 = XXH_rotl64(h64, 27) * XXH_PRIME64_1 + XXH_PRIME64_4;
                len -= 8;
            }
            if (len >= 4)
            {
                h64 ^= *(uint*)input * XXH_PRIME64_1;
                input += 4;
                h64 = XXH_rotl64(h64, 23) * XXH_PRIME64_2 + XXH_PRIME64_3;
                len -= 4;
            }
            while (len > 0)
            {
                h64 ^= *input++ * XXH_PRIME64_5;
                h64 = XXH_rotl64(h64, 11) * XXH_PRIME64_1;
                --len;
            }
            // XXH64_avalanche
            h64 ^= h64 >> 33;
            h64 *= XXH_PRIME64_2;
            h64 ^= h64 >> 29;
            h64 *= XXH_PRIME64_3;
            h64 ^= h64 >> 32;
            return h64;
        }
    }
}

