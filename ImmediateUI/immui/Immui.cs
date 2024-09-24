using ImmediateUI.immui.drawing;
using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace ImmediateUI.immui
{
    public  static partial class Immui
    {
        static Context CurrentContext;
        static IO IO =new();
        public static void SetContext(Context Ctx)
        {
            CurrentContext = Ctx;
        }
        public static void BeginFrame()
        {
            CurrentContext.MouseCaptured = false;
            CurrentContext.DrawData.CmdList.Clear();
            CurrentContext.MainDrawList.ResetForNewFrame();
            CurrentContext.MainDrawList.PushClipRectFullScreen();
            CurrentContext.MainDrawList.PushTextureID(ImmediateUI.Window.Blank.GetHandle());
            foreach(var Wind in CurrentContext.Windows.Values)
            {
                Wind.DrawList.ResetForNewFrame();
                Wind.DrawList.PushClipRectFullScreen();
                Wind.DrawList.PushTextureID(ImmediateUI.Window.Blank.GetHandle());
            }
            CurrentContext.HotID = 0;

        }
        public static IO GetIO()
        {
            return IO;
        }
        public static void EndFrame()
        {
            CurrentContext.DrawData.CmdList.Add(CurrentContext.MainDrawList);
            foreach (var Wind in CurrentContext.Windows.Values)
            {
                CurrentContext.DrawData.CmdList.Add(Wind.DrawList);
            }
            IO.InputedChars.Clear();
        }
        //
        // Update State 
        //
        public static void OnChar(char ch)
        {
            IO.InputedChars.Add(ch);
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
            CurrentContext.MainDrawList.AddText(new(500, 400), data.LoremIpsum.LoremIpsumText, 17, 0x00000FF,WrapWidth: 690);
        }
        
        //
        //
        //
        //TODO : Add support for window
        public static Vector2 CalculateTextSize(float Size,string Text,int WrapWidth=0, int MaxWidth = -1)
        {
            return CurrentContext.Style.BaseFont.CalcTextSize(Size, Text, WrapWidth, MaxWidth);
        }
        public static ImmuiDrawList GetCurrentDrawList()
        {
            if(CurrentContext.CurrentWindow != null)
            {
                return CurrentContext.CurrentWindow.DrawList;
            }
            return CurrentContext.MainDrawList;
        }
        public static Style GetCurrentStyle()
        {
            return CurrentContext.Style;
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
            if (!CurrentContext.MouseCaptured && Rect.ContainsPoint(CurrentContext.MousePosition))
            {
                CurrentContext.MouseCaptured = true;
                return true;
            }
            return false;
        }
        public static string GetLabel(string Str)
        {
            return Str.Split("##")[0];
        }
    }
}
