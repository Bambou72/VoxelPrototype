using System.Runtime.InteropServices;
namespace VoxelPrototype.client.utils.Hebron
{
    public static unsafe class CRuntime
    {
        public static void* malloc(long size)
        {
            return Marshal.AllocHGlobal((int)size).ToPointer();
        }
        public static void memcpy(void* a, void* b, int size)
        {
            var ap = (byte*)a;
            var bp = (byte*)b;
            for (int i = 0; i < size; ++i)
            {
                *ap++ = *bp++;
            }
        }
        public static void memmove(void* a, void* b, int size)
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
            Marshal.FreeHGlobal(new nint(a));
        }

        public static void memset(void* ptr, int value, int size)
        {
            byte* bptr = (byte*)ptr;
            var bval = (byte)value;
            for (int i = 0; i < size; ++i)
            {
                *bptr++ = bval;
            }
        }
        public static void* realloc(void* a, uint newSize)
        {
            if (a == null)
            {
                return malloc(newSize);
            }
            return Marshal.ReAllocHGlobal(new nint(a), new nint(newSize)).ToPointer();
        }
    }
}