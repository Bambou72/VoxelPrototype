using ImmediateUI.immui.drawing;
using ImmediateUI.immui.font;
using ImmediateUI.immui.hash;
using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;
namespace ImmediateUI.immui
{
    public static class Immui
    {
        static Context CurrentContext;
        //Const
        internal const ulong InvalidID = 0;
        internal const int ARCFAST_SAMPLE_MAX = 48;
        internal const int ARCFAST_TABLE_SIZE = 48;
        internal const int CIRCLE_AUTO_SEGMENT_MIN = 4;
        internal const int CIRCLE_AUTO_SEGMENT_MAX = 512;
        internal const uint AlphaMask = 0x000000FF;

        //
        public static void SetContext(Context Ctx)
        {
            CurrentContext = Ctx;
        }
        public static void BeginFrame(Vector2i MousePos, GameWindow Window)
        {
            CurrentContext.DrawData.CmdList.Clear();
            CurrentContext.MainDrawList.ResetForNewFrame();
            CurrentContext.MainDrawList.PushClipRectFullScreen();
            CurrentContext.MainDrawList.PushTextureID(ImmediateUI.Window.Blank.GetHandle());
            for (int i = 0; i < 3; i++)
            {
                CurrentContext.MouseButtons[i] = Window.MouseState.IsButtonDown((MouseButton)i);
            }
            CurrentContext.MousePosition = MousePos;
            CurrentContext.HotID = 0;
            CurrentContext.LastHotID = CurrentContext.HotID;

        }

        public static void EndFrame()
        {
            CurrentContext.DrawData.CmdList.Add(CurrentContext.MainDrawList);
            CurrentContext.InputedChars.Clear();
        }
        //
        // Update State 
        //
        public static void OnChar(char ch)
        {
            CurrentContext.InputedChars.Add(ch);
        }
        public static void OnResize(Vector2 Size)
        {
            if(CurrentContext != null)
            {
                CurrentContext.ScreenSize = Size;
            }
        }
        //
        //Debug
        //
        public static void Demo2DRendering()
        {
            
            CurrentContext.MainDrawList.AddCircleFilled(new(75, 75), 50, 0xFF0000FF);
            CurrentContext.MainDrawList.AddCircle(new(75, 200), 50, 0xFF14ABFF);
            CurrentContext.MainDrawList.AddCircle(new(75, 325), 50, 0xFB5F45FF, Thickness: 10);
            CurrentContext.MainDrawList.AddRectFilled(new(150, 25), new(250, 125), 0x00FF00FF);
            CurrentContext.MainDrawList.AddRect(new(150, 150), new(250, 250), 0x29FF48FF);
            CurrentContext.MainDrawList.AddRect(new(150, 275), new(250, 375), 0x103699FF, Thickness: 5);
            CurrentContext.MainDrawList.AddRectFilled(new(275, 25), new(375, 125), 0x504095FF, 15, DrawFlags.RoundCornersAll);
            CurrentContext.MainDrawList.AddRectFilled(new(275, 150), new(375, 250), 0x604595FF, 15, DrawFlags.RoundCornersBottomLeft);
            CurrentContext.MainDrawList.AddRectFilled(new(275, 275), new(375, 375), 0x805095FF, 15, DrawFlags.RoundCornersBottomRight);
            CurrentContext.MainDrawList.AddRectFilled(new(275, 400), new(375, 500), 0x995595FF, 15, DrawFlags.RoundCornersTopLeft);
            CurrentContext.MainDrawList.AddRectFilled(new(275, 525), new(375, 625), 0xAA6095FF, 15, DrawFlags.RoundCornersTopRight);
            CurrentContext.MainDrawList.AddRectFilledMultiColor(new(275, 650), new(375, 750), 0xFF0000FF, 0x00FF00FF, 0x0000FFFF, 0xFF00FFFF);
            CurrentContext.MainDrawList.AddLine(new(400, 25), new(475, 125), 0x2583F8FF, Thickness: 3);
            CurrentContext.MainDrawList.AddTriangle(new(500, 25), new(600, 25), new(550, 125), 0xA6E808FF, Thickness: 4);
            CurrentContext.MainDrawList.AddTriangleFilled(new(500, 250), new(550, 150), new(600, 250), 0xB12580FF);
            CurrentContext.MainDrawList.AddBezierCurveCubic(new(625, 25), new(625, 125), new(725, 25), new(725, 125), 0x000000FF, 2);
            CurrentContext.MainDrawList.AddBezierCurveQuadratic(new(750, 25), new(850, 25), new(850, 125), 0x000000FF, 2);
            CurrentContext.MainDrawList.AddEllipse(new(900, 50), new(50, 25), 0x000000FF);
            CurrentContext.MainDrawList.AddEllipseFilled(new(1025, 50), new(50, 25), 0x000000FF);
            CurrentContext.MainDrawList.AddNGon(new(1125, 75), 50, 0xFF0000FF, 5);     
            CurrentContext.MainDrawList.AddNGon(new(1125, 175), 50, 0xFF0000FF, 6);
            /*
            for(int i = 0;i < 500 ;i++)
            {
                CurrentContext.MainDrawList.AddRect(new(200, 200),new(200+i,200+i), (uint)i *25102006);
            }*/
            CurrentContext.MainDrawList.AddText(null, 17, new(500, 400), data.LoremIpsum.LoremIpsumText, 0x00000FF,690);
        }

        //
        //
        //
        public static Font GetCurrentFont()
        {
            return CurrentContext.Style.BaseFont;
        }
        public static Vector2 GetScreenSize()
        {
            return CurrentContext.ScreenSize;
        }
        public static DrawData GetDrawData()
        {
            return CurrentContext.DrawData;
        }
        public static bool CheckMouse(Rect Rect)
        {
            if (Rect.Contains(CurrentContext.MousePosition))
            {
                return true;
            }
            return false;
        }
        public static string GetLabel(string Str)
        {
            return Str.Split("##")[0];
        }
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
        //
        //Widgets
        //
        public static bool Button(string Label, Rect Rect)
        {
            bool result = false;
            ulong ID = GetID(Label);
            if (CheckMouse(Rect))
            {
                CurrentContext.HotID = ID;
                if (CurrentContext.MouseButtons[0] && CurrentContext.ActiveID == 0)
                {
                    CurrentContext.ActiveID = ID;
                }
            }
            else if (CurrentContext.ActiveID == ID)
            {
                CurrentContext.ActiveID = 0;
            }

            if (CurrentContext.ActiveID == ID && !CurrentContext.MouseButtons[0] && CurrentContext.HotID == ID)
            {
                result = true;
                CurrentContext.ActiveID = 0;
            }
            if (CurrentContext.HotID == ID)
            {
                CurrentContext.MainDrawList.AddRectFilled((Vector2i)Rect.Min, (Vector2i)Rect.Max, 0x102030FF, 0);
            }
            else
            {
                CurrentContext.MainDrawList.AddRectFilled((Vector2i)Rect.Min, (Vector2i)Rect.Max, 0x051020FF, 0);
            }
            return result;
        }
        public static void TextInput(string Label, Rect Rect, ref string inputText)
        {
            ulong ID = GetID(Label);

            if (CheckMouse(Rect))
            {
                CurrentContext.HotID = ID;

                if (CurrentContext.MouseButtons[0] && CurrentContext.ActiveID == 0)
                {
                    CurrentContext.ActiveID = ID;
                }
            }

            if (CurrentContext.ActiveID == ID)
            {
                foreach (char ch in CurrentContext.InputedChars)
                {
                    if (ch == '\b')
                    {
                        inputText.Remove(inputText.Length - 1);
                    }
                    else
                    {
                        inputText += ch;
                    }
                }
            }
            if (CurrentContext.ActiveID == ID)
            {
                CurrentContext.MainDrawList.AddRectFilled((Vector2i)Rect.Min, (Vector2i)Rect.Max, 0x204060FF, 0);

            }
            else if (CurrentContext.HotID == ID)
            {
                CurrentContext.MainDrawList.AddRectFilled((Vector2i)Rect.Min, (Vector2i)Rect.Max, 0x102030FF, 0);
            }
            else
            {
                CurrentContext.MainDrawList.AddRectFilled((Vector2i)Rect.Min, (Vector2i)Rect.Max, 0x051020FF, 0);
            }
        }
        //
        //Utils
        //
        internal static void Normalize(ref float x, ref float y)
        {
            float d2 = x * x + y * y;
            if (d2 > 0.0f)
            {
                float inv_len = 1f / MathF.Sqrt(d2);
                x *= inv_len;
                y *= inv_len;
            }
        }

        internal static void FixNormal(ref float x, ref float y)
        {
            float d2 = x * x + y * y;
            if (d2 > 0.000001f)
            {
                float inv_len2 = 1.0f / d2; if (inv_len2 > 100.0f) inv_len2 = 100.0f;
                x *= inv_len2;
                y *= inv_len2;
            }
        }
        internal static int CircleAutoSegmentCalc(float Radian, float MaxError)
        {
            return (int)Math.Clamp(RoundupToEven((int)Math.Ceiling(MathF.PI / MathF.Acos(1 - Math.Min((MaxError), (Radian)) / (Radian)))), CIRCLE_AUTO_SEGMENT_MIN, CIRCLE_AUTO_SEGMENT_MAX);
        }
        internal static float CircleAutoSegmentCalcR(float N, float MaxError)
        {
            return ((MaxError) / (1 - MathF.Cos(MathF.PI / Math.Max((float)(N), MathF.PI))));
        }
        internal static float RoundupToEven(float V)
        {
            return ((((V) + 1) / 2) * 2);
        }
    }
}
