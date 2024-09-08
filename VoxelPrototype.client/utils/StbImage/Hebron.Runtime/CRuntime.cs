using System.Runtime.InteropServices;

namespace VoxelPrototype.client.utils.StbImage.Hebron.Runtime
{
    internal static unsafe class CRuntime
    {

        public static void* malloc(ulong size)
        {
            return Marshal.AllocHGlobal((int)(long)size).ToPointer();
        }
        public static void memcpy(void* a, void* b, ulong size)
        {
            var ap = (byte*)a;
            var bp = (byte*)b;
            for (long i = 0; i < (long)size; ++i)
            {
                *ap++ = *bp++;
            }
        }
        public static void memmove(void* a, void* b, ulong size)
        {
            void* temp = null;

            try
            {
                temp = malloc(size);
                memcpy(temp, b, size);
                memcpy(a, temp, size);
            }

            finally
            {
                if (temp != null)
                {
                    free(temp);
                }
            }
        }


        public static void free(void* a)
        {
            var ptr = new nint(a);
            Marshal.FreeHGlobal(ptr);
        }

        public static void memset(void* ptr, int value, ulong size)
        {
            byte* bptr = (byte*)ptr;
            var bval = (byte)value;
            for (long i = 0; i < (long)size; ++i)
            {
                *bptr++ = bval;
            }
        }

        public static void* realloc(void* a, ulong newSize)
        {
            if (a == null)
            {
                return malloc(newSize);
            }

            var ptr = new nint(a);
            var result = Marshal.ReAllocHGlobal(ptr, new nint((long)newSize));

            return result.ToPointer();

        }
        public static double frexp(double number, int* exponent)
        {
            const long DBL_EXP_MASK = 0x7ff0000000000000L;
            const int DBL_MANT_BITS = 52;
            const long DBL_SGN_MASK = -1 - 0x7fffffffffffffffL;
            const long DBL_MANT_MASK = 0x000fffffffffffffL;
            const long DBL_EXP_CLR_MASK = DBL_SGN_MASK | DBL_MANT_MASK;
            var bits = BitConverter.DoubleToInt64Bits(number);
            var exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
            *exponent = 0;

            if (exp == 0x7ff || number == 0D)
                number += number;
            else
            {
                // Not zero and finite.
                *exponent = exp - 1022;
                if (exp == 0)
                {
                    // Subnormal, scale number so that it is in [1, 2).
                    number *= BitConverter.Int64BitsToDouble(0x4350000000000000L); // 2^54
                    bits = BitConverter.DoubleToInt64Bits(number);
                    exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
                    *exponent = exp - 1022 - 54;
                }

                // Set exponent to -1 so that number is in [0.5, 1).
                number = BitConverter.Int64BitsToDouble(bits & DBL_EXP_CLR_MASK | 0x3fe0000000000000L);
            }
            return number;
        }
        public static ulong strlen(sbyte* str)
        {
            var ptr = str;

            while (*ptr != '\0')
                ptr++;

            return (ulong)ptr - (ulong)str - 1;
        }
    }
}