using ImmediateUI.immui.math;
using ImmediateUI.immui;
using OpenTK.Mathematics;
using ImmediateUI.immui.layout;

namespace ImmediateUI
{
    internal enum DemoPage
    {
        MenuExemple,
        AnimationDemo
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
        public static void ShowDemo(Context Ctx, Vector2i ClientSize, float Time)
        {
            Composer DemoChoice = new Composer(new(0,0,500,50));
            Rect rect;
            //ExempleMenu button
            Vector4i Padding1 = new(10);
            DemoChoice.NextRow(1f);
            rect = DemoChoice.NextCollumn(0.4f, Padding1);
            if(Immui.Button(Ctx,"Menu Exemple",rect))
            {
                CurrentPage = DemoPage.MenuExemple;
            }
            //Animation demo
            rect = DemoChoice.NextCollumn(0.3f, Padding1);
            if (Immui.Button(Ctx, "Animation", rect))
            {
                CurrentPage = DemoPage.AnimationDemo;
            }

            if (CurrentPage == DemoPage.AnimationDemo)
            {
                AnimationDemo(Ctx,Time);
            }else if(CurrentPage == DemoPage.MenuExemple)
            {
                MenuExemple(Ctx, ClientSize);
            }


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
