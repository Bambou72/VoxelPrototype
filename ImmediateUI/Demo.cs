using ImmediateUI.immui.math;
using ImmediateUI.immui;
using OpenTK.Mathematics;
using ImmediateUI.immui.layout;
using System.Runtime.Intrinsics.X86;
namespace ImmediateUI
{
    internal enum DemoPage
    {
        MenuExemple,
        AnimationDemo,
        ScrollPanel,
        Widgets
    }
    internal static class Demo
    {
        public static float timecount = 0;
        static int NumberOfButton = 0;
        static float InterpolateFactor;
        static float TimeToOpen = 0.1f;
        static bool UpdateShow = false;
        static bool CloseShow = false;
        static DemoPage CurrentPage;
        public static void ShowDemo(Window Wnd, Context Ctx, Vector2i ClientSize, float Time)
        {
            Composer DemoChoice = new Composer(new(0,0,800,50));
            Rect rect;
            //ExempleMenu button
            Vector4i Padding1 = new(10);
            DemoChoice.NextRow(1f);
            rect = DemoChoice.NextCollumn(0.25f, Padding1);
            if(Immui.Button(Ctx,"Menu Exemple",rect))
            {
                CurrentPage = DemoPage.MenuExemple;
            }
            //Animation demo
            rect = DemoChoice.NextCollumn(0.2f, Padding1);
            if (Immui.Button(Ctx, "Animation", rect))
            {
                CurrentPage = DemoPage.AnimationDemo;
            }
            //Textedit demo
            rect = DemoChoice.NextCollumn(0.2f, Padding1);
            if (Immui.Button(Ctx, "Widgets", rect))
            {
                CurrentPage = DemoPage.Widgets;
            }
            rect = DemoChoice.NextCollumn(0.25f, Padding1);
            if (Immui.Button(Ctx, "ScrollPanel", rect))
            {
                CurrentPage = DemoPage.ScrollPanel;
            }
            rect = DemoChoice.NextCollumn(0.1f, Padding1);
            if (Immui.Button(Ctx, "Quit##0", rect))
            {
                Wnd.Close();    
            }
            if (CurrentPage == DemoPage.AnimationDemo)
            {
                AnimationDemo(Ctx,Time);
            }else if(CurrentPage == DemoPage.MenuExemple)
            {
                MenuExemple(Ctx, ClientSize);
            }else if(CurrentPage==DemoPage.Widgets)
            {
                WidgetsExemple(Ctx,ClientSize);
            }
            else if (CurrentPage == DemoPage.ScrollPanel)
            {
                ScrollPanelExemple(Ctx, ClientSize);
            }
        }
        public static string TextForTest = "fdfdsfsdfds";
        public static int ScrollOffset = 0;
        public static void ScrollPanelExemple(Context Ctx, Vector2i ClientSize)
        {
            Vector4i Padding1 = new(10);
            Rect ScrollRecct = Immui.BeginScrollPanel(Ctx, new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 300), ref ScrollOffset, 400,0x404040FF);
            Composer LT2 = new(ScrollRecct);
            Rect CurP;
            LT2.NextRow(100);
            CurP = LT2.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Singleplayer", CurP);
            LT2.NextRow(100);
            CurP = LT2.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Multiplayer", CurP);
            LT2.NextRow(100);
            CurP = LT2.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Mods", CurP);
            LT2.NextRow(100);
            CurP = LT2.NextCollumn(1 / 2f, Padding1);
            Immui.Button(Ctx, "Options", CurP);
            CurP = LT2.NextCollumn(1 / 2f, Padding1);
            Immui.Button(Ctx, "Quit", CurP);
            Immui.EndScrollPanel(Ctx);
        }
        static float T = 0;

        public static void WidgetsExemple(Context Ctx, Vector2i ClientSize)
        {
            Vector4i Padding1 = new(10);
            Rect CurP;
            Composer LT = new(new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 800));
            LT.NextRow(50);
            CurP = LT.NextCollumn(1, Padding1);
            Immui.TextEdit(Ctx, "TextEdit", ref TextForTest, CurP);
            LT.NextRow(50);
            CurP = LT.NextCollumn(1, Padding1);
            Immui.Slider(Ctx, "Slider", CurP, ref T,-100,100,0.5f);
        }
        public static void MenuExemple(Context Ctx,Vector2i ClientSize)
        {
            Vector4i Padding1 = new(10);
            Rect CurP;
            Composer LT = new(new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 800));
            LT.NextRow(100);
            CurP = LT.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Singleplayer", CurP);
            LT.NextRow(100);
            CurP = LT.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Multiplayer", CurP);
            LT.NextRow(100);
            CurP = LT.NextCollumn(1, Padding1);
            Immui.Button(Ctx, "Mods", CurP);
            LT.NextRow(100);
            CurP = LT.NextCollumn(1 / 2f, Padding1);
            Immui.Button(Ctx, "Options", CurP);
            CurP = LT.NextCollumn(1 / 2f, Padding1);
            Immui.Button(Ctx, "Quit", CurP);
        }
        public static void AnimationDemo(Context Ctx,float Time)
        {
            Rect CurP = new(10,60,180,80);
            if (Immui.Button(Ctx, "Open", CurP))
            {
                UpdateShow = true;
                CloseShow = InterpolateFactor > 0;
            }
            if (UpdateShow)
            {
                if (CloseShow)
                {
                    InterpolateFactor -= (float)Time / TimeToOpen;
                    if (InterpolateFactor < 0.0)
                    {
                        InterpolateFactor = 0.0f;
                        UpdateShow = false;
                    }
                }
                else
                {
                    InterpolateFactor += (float)Time / TimeToOpen;
                    if (InterpolateFactor >= 1.0)
                    {
                        InterpolateFactor = 1.0f;
                        UpdateShow = false; 
                    }
                }
            }
            //Test2
            int HideX = -450;
            int ShowX = 0;
            int CurrentX = (int)(HideX + InterpolateFactor * (ShowX - HideX));
            Rect CurP2;
            Vector4i Padding1 = new(10);
            Composer LT2 = new(new Rect(CurrentX, 150, 400, 600));
            CurP2 = LT2.Next(0.5f, 0.5f, Padding1);
            Immui.Button(Ctx, "Test1", CurP2);
            CurP2 = LT2.Next(0.5f, 0.25f, Padding1);
            Immui.Button(Ctx, "Test2", CurP2);
            CurP2 = LT2.Next(0.25f, 0.5f, Padding1);
            Immui.Button(Ctx, "Test3", CurP2);
            CurP2 = LT2.Next(0.75f, 0.5f, Padding1);
            Immui.Button(Ctx, "Test4", CurP2);
        }
    }
}
