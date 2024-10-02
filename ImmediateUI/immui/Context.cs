using ImmediateUI.immui.font;
using ImmediateUI.immui.math;
using ImmediateUI.immui.rendering;
using ImmediateUI.immui.utils;
using OpenTK.Mathematics;
using System.Text;
namespace ImmediateUI.immui
{
    public class Context
    {
        public  Func<int, bool> MouseDown;
        public  Func<int, bool> MouseUp;
        public  Func<int, bool> MousePressed;
        public  Func<int, bool> KeyDown;
        public  Func<int, bool> KeyUp;
        public  Func<int, bool> KeyPressed;
        internal Font font = FontLoader.LoadFromFile("Regular.otf");
        internal ulong ActiveID;
        internal ulong HotID;
        internal ulong CurrentID;
        internal Vector2i MousePosition;
        internal Vector2 ScreenSize;
        internal List<ulong> IDStack = new();
        internal ImmuiDrawList DrawList;
        public Context()
        {
            DrawList = new ImmuiDrawList();
        }
        public void ResetFrame()
        {
            DrawList.ResetForNewFrame();
            DrawList.PushClipRectFullScreen(ScreenSize);
            DrawList.PushTextureID(Window.Blank.GetHandle());
            HotID = 0;

        }
        public void OnResize(Vector2 ScreenSize)
        {
            this.ScreenSize = ScreenSize;
        }
        public Vector2 CalculateTextSize(float Size, string Text, int WrapWidth = 0, int MaxWidth = -1)
        {
            return font.CalcTextSize(Size, Text, WrapWidth, MaxWidth);
        }
        public  ImmuiDrawList GetDrawList()
        {
            return DrawList;
        }
        public Vector2 GetScreenSize()
        {
            return ScreenSize;
        }

        public bool CheckMouse(Rect Rect)
        {
            if (Rect.ContainsPoint(MousePosition))
            {
                return true;
            }
            return false;
        }
        public  string GetLabel(string Str)
        {
            return Str.Split("##")[0];
        }
        public  ulong Hash(string Str, ulong Seed = 0)
        {
            byte[] Data = Encoding.UTF8.GetBytes(Str);
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public  ulong Hash(byte[] Data, ulong Seed = 0)
        {
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public  ulong GetID(string Str)
        {
            ulong Seed = GetSeed();
            ulong ID = Hash(Str, Seed);
            CurrentID = ID;
            return ID;
        }
        public ulong GetID(int N)
        {
            ulong Seed = GetSeed();
            return Hash(BitConverter.GetBytes(N), Seed);
        }
        public ulong GetSeed()
        {
            if (IDStack.Count > 0)
            {
                return IDStack[^1];
            }
            return 0;
        }
        public void PushID(string Str)
        {
            ulong ID = GetID(Str);
            IDStack.Add(ID);
        }
        public void PushID(int N)
        {
            ulong ID = GetID(N);
            IDStack.Add(ID);
        }
        public void PushGeneratedID(ulong ID)
        {
            IDStack.Add(ID);
        }
        public void PopID()
        {
            IDStack.RemoveAt(IDStack.Count - 1);
        }
        public ulong GetCurrentStackID()
        {
            return IDStack[^1];
        }
        public ulong GetCurrentID()
        {
            return CurrentID;
        }

    }
}
