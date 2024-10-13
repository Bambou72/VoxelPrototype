using ImmediateUI.immui.font;
using ImmediateUI.immui.math;
using ImmediateUI.immui.rendering;
using ImmediateUI.immui.utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;
namespace ImmediateUI.immui
{
    public class Context
    {
        public Func<int, bool> MouseDown;
        public Func<int, bool> MouseUp;
        public Func<int, bool> MousePressed;
        public Func<int, bool> KeyDown;
        public Func<int, bool> KeyUp;
        public Func<int, bool> KeyPressed;
        internal Font font = FontLoader.LoadFromFile("Regular.otf");
        private ulong ActiveID;
        private ulong HotID;
        private ulong KeyFocusID;
        internal ulong CurrentID;
        internal Vector2i MousePosition;
        internal Vector2i LastMousePosition;
        internal Vector2i ScrollDelta;
        internal Vector2 ScreenSize;
        internal bool Dragging = false;
        internal bool NotInteractable = false;
        internal Vector2i DragDistance;
        internal List<ulong> IDStack = new();
        private Renderer DrawList;
        internal char InputedChar = (char)0;
        internal Stack<Rect> ClipStack = new();
        public Context()
        {
            DrawList = new Renderer();
        }
        public ulong GetHotID()
        {
            return HotID;
        }
        public void SetHotID(ulong ID)
        {
            HotID = ID;
        }
        public ulong GetActiveID()
        {
            return ActiveID;
        }
        public void SetActiveID(ulong ID)
        {
            ActiveID = ID;
        }
        public ulong GetKeyFocusID()
        {
            return KeyFocusID;
        }
        public void SetKeyFocusID(ulong ID)
        {
            KeyFocusID = ID;
        }
        public void ResetFrame()
        {
            if (HotID == 0 && MouseDown(0))
            {
                NotInteractable = true;
            }
            if (NotInteractable && !MouseDown(0))
            {
                NotInteractable = false;
            }
            if(!MouseDown(0))
            {
                Dragging =false;
            }else
            {
                if(Dragging)
                {
                    DragDistance = MousePosition - LastMousePosition;

                }else
                {
                    Dragging = true;
                }
                LastMousePosition   = MousePosition;
            }
            ClipStack.Clear();
            if ((ActiveID != 0 && ActiveID != KeyFocusID) || KeyPressed((int)Keys.Escape))
            {
                KeyFocusID = 0;
            }
            if (HotID == 0 && MousePressed(0))
            {
                KeyFocusID = 0;
            }
            DrawList.ResetForNewFrame();
            DrawList.PushClipRect(new(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y));
            DrawList.PushTextureID(Window.Blank.GetHandle());
            HotID = 0;

        }

        public void OnResize(Vector2 ScreenSize)
        {
            this.ScreenSize = ScreenSize;
        }
        public void OnTextInput(char Char)
        {
            this.InputedChar = Char;
        }
        public Vector2 CalculateTextSize(float Size, string Text, int WrapWidth = 0, int MaxWidth = -1)
        {
            return font.CalcTextSize(Size, Text, WrapWidth, MaxWidth);
        }
        public Renderer GetDrawList()
        {
            return DrawList;
        }
        public Vector2 GetScreenSize()
        {
            return ScreenSize;
        }
        public (bool ,bool) BaseWiget(Rect Rect, ulong ID)
        {
            if (IsHover(Rect))
            {
                if (GetHotID() == 0)
                {
                    SetHotID(ID);
                    if (MouseDown(0) && GetActiveID() == 0 && !NotInteractable)
                    {
                        SetActiveID(ID);
                    }
                }
            }
            return (ID== HotID, ActiveID ==ID);
        }
        public bool IsHover(Rect Rect)
        {
            if (ClipStack.Count > 0)
            {
                Rect ClipRect = ClipStack.Peek();
                if (!MouseInRect(ClipRect))
                {
                    return false;
                }
            }
                return MouseInRect(Rect);
        }
        private bool MouseInRect(Rect Rect)
        {
            if (Rect.ContainsPoint(MousePosition))
            {
                return true;
            }
            return false;
        }
        public string GetLabel(string Str)
        {
            return Str.Split("##")[0];
        }
        public ulong Hash(string Str, ulong Seed = 0)
        {
            byte[] Data = Encoding.UTF8.GetBytes(Str);
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public ulong Hash(byte[] Data, ulong Seed = 0)
        {
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public ulong GetID(string Str)
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
        public void PushClipRect(Rect Rect)
        {
            ClipStack.Push(Rect);
        }
        public void PopClipRect()
        {
            ClipStack.Pop();
        }
    }
}
