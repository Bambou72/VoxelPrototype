using ImmediateUI.immui.hash;
using System.Text;
namespace ImmediateUI.immui
{
    public static partial class Immui
    {
        //
        //Hash and ID
        //
        public static ulong Hash(string Str, ulong Seed = 0)
        {
            byte[] Data = Encoding.UTF8.GetBytes(Str);
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public static ulong Hash(byte[] Data, ulong Seed = 0)
        {
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public static ulong GetID(string Str)
        {
            ulong Seed = GetSeed();
            ulong ID = Hash(Str, Seed);
            CurrentContext.CurrentID = ID;
            return ID;
        }
        public static ulong GetID(int N)
        {
            ulong Seed = GetSeed();
            return Hash(BitConverter.GetBytes(N), Seed);
        }
        public static ulong GetSeed()
        {
            if (CurrentContext.IDStack.Count > 0)
            {
                return CurrentContext.IDStack[^1];
            }
            return 0;
        }
        public static void PushID(string Str)
        {
            ulong ID = GetID(Str);
            CurrentContext.IDStack.Add(ID);
        }
        public static void PushID(int N)
        {
            ulong ID = GetID(N);
            CurrentContext.IDStack.Add(ID);
        }
        public static void PushGeneratedID(ulong ID)
        {
            CurrentContext.IDStack.Add(ID);
        }
        public static void PopID()
        {
            CurrentContext.IDStack.RemoveAt(CurrentContext.IDStack.Count - 1);
        }
        public static ulong GetCurrentStackID()
        {
            return CurrentContext.IDStack[^1];
        }
        public static ulong GetCurrentID()
        {
            return CurrentContext.CurrentID;
        }
    }
}
