using OpenTK.Mathematics;
using ImmediateUI.immui;
using VoxelPrototype.client.utils.math;
namespace ImmediateUI
{
    internal enum DemoPage
    {
        MenuExemple,
        AnimationDemo,
        ScrollPanel,
        TabBar,
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
            Ctx.PushArea(new(0, 0, 800, 100));
            Ctx.SetStackType(Context.StackType.Horizontal);
            //ExempleMenu button
            if (Ctx.Button( "Menu Exemple"))
            {
                CurrentPage = DemoPage.MenuExemple;
            }
            //Animation demo
            if (Ctx.Button( "Animation"))
            {
                CurrentPage = DemoPage.AnimationDemo;
            }
            //Textedit demo
            if (Ctx.Button( "Widgets"))
            {
                CurrentPage = DemoPage.Widgets;
            }
            if (Ctx.Button( "ScrollPanel"))
            {
                CurrentPage = DemoPage.ScrollPanel;
            }
            if (Ctx.Button("TabBar"))
            {
                CurrentPage = DemoPage.TabBar;
            }
            if (Ctx.Button( "Quit##0"))
            {
                Wnd.Close();
            }
            Ctx.SetStackType(Context.StackType.Vertical);
            Ctx.PopArea();
            
            if (CurrentPage == DemoPage.AnimationDemo)
            {
                AnimationDemo(Ctx, Time);
            }
            else if (CurrentPage == DemoPage.MenuExemple)
            {
                MenuExemple(Ctx, ClientSize);
            }
            else if (CurrentPage == DemoPage.Widgets)
            {
                WidgetsExemple(Ctx, ClientSize);
            }
            else if (CurrentPage == DemoPage.ScrollPanel)
            {
                ScrollPanelExemple(Ctx, ClientSize);
            }
            else if (CurrentPage == DemoPage.TabBar)
            {
                TabBarExemple(Ctx, ClientSize);
            }
        }
        public static string TextForTest = "fdfdsfsdfds";
        public static int ScrollOffset = 0;
        public static void ScrollPanelExemple(Context Ctx, Vector2i ClientSize)
        {
            Ctx.PushArea(new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 300));

            Ctx.BeginScrollPanel( "TestScrollPanel", ref ScrollOffset, 400, 0x404040FF);
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button("Singleplayer");
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button( "Multiplayer");
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button("Mods");

            Ctx.NextRow(100);
            Ctx.NextCollumn(0.5f);

           Ctx.Button("Options");
            Ctx.NextCollumn(0.5f);
           Ctx.Button("Quit");
           Ctx.EndScrollPanel();
            Ctx.PopArea();
        }
        public static void TabBarExemple(Context Ctx, Vector2i ClientSize)
        {
            Ctx.PushArea(new Rect(ClientSize.X / 2 - ClientSize.X / 4, 200, ClientSize.X / 2, 500));
            Ctx.BeginTabBar("TestTabBar");
            if(Ctx.BeginTabItem("Item1"))
            {
                Ctx.Button("ButtonItem1");
                Ctx.SliderFloat("SliderItem1", ref T, -100, 100, 0.5f);
                Ctx.EndTabItem();
            }
            if (Ctx.BeginTabItem("Item2"))
            {
                Ctx.Button("ButtonItem2");
                Ctx.ComboBox("ComboBoxItem2", ref Selected, ComboArray);
                Ctx.EndTabItem();
            }
            if (Ctx.BeginTabItem("Item3"))
            {
                Ctx.BeginTabBar("TestTabBar2");
                if (Ctx.BeginTabItem("Item1"))
                {
                    Ctx.Button("ButtonItem1");
                    Ctx.SliderFloat("SliderItem1", ref T, -100, 100, 0.5f);
                    Ctx.EndTabItem();
                }
                if (Ctx.BeginTabItem("Item2"))
                {
                    Ctx.Button("ButtonItem2");
                    Ctx.ComboBox("ComboBoxItem2", ref Selected, ComboArray);
                    Ctx.EndTabItem();
                }
                Ctx.EndTabBar();
                Ctx.EndTabItem();
            }
            Ctx.EndTabBar();
            Ctx.PopArea();
        }
        static float T = 0;
        static int Tint = 0;
        static int Selected = 0;
        static string[] ComboArray = { "Test1", "Test2", "Test3", "Test4" };
        static float PercentageValue = 0;
        static bool Reverse;
        static int intVal;
        public static void WidgetsExemple(Context Ctx, Vector2i ClientSize)
        {
            Ctx.PushArea(new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 800));
           Ctx.TextEdit( "TextEdit", ref TextForTest,100);
           Ctx.IntEdit( "IntEdit", ref intVal);
           Ctx.SliderFloat( "Slider", ref T, -100, 100, 0.5f);
           Ctx.SliderInt( "SliderInt", ref Tint, 0, 100, 10);
           Ctx.ComboBox("ComboBox", ref Selected, ComboArray);
            PercentageValue += (Reverse ? -Ctx.DT : Ctx.DT) * 50;
            if(PercentageValue < 0)
            {
                Reverse = false;
                PercentageValue = 0;
            }
            if( PercentageValue > 100)
            {
                Reverse=true;
                PercentageValue = 100;
            }
           Ctx.ProgressBar( "ProgressBar",PercentageValue);
            Ctx.PopArea();
        }
        public static void MenuExemple(Context Ctx, Vector2i ClientSize)
        {
            Ctx.PushArea(new Rect(ClientSize.X / 2 - ClientSize.X / 6, 200, ClientSize.X / 3, 800));
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button( "Singleplayer");
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button( "Multiplayer");
            Ctx.NextRow(100);
            Ctx.NextCollumn(1);
           Ctx.Button( "Mods");

            Ctx.NextRow(100);
            Ctx.NextCollumn(0.5f);

           Ctx.Button( "Options");
            Ctx.NextCollumn(0.5f);
           Ctx.Button( "Quit");

            Ctx.PopArea();
        }
        public static void AnimationDemo(Context Ctx, float Time)
        {
            Ctx.PushArea(new(10, 60, 180, 80));
            Ctx.NextRow(1);
            Ctx.NextCollumn(1);
            if (Ctx.Button("Open"))
            {
                UpdateShow = true;
                CloseShow = InterpolateFactor > 0;
            }
            Ctx.PopArea();
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
            Ctx.PushArea(new Rect(CurrentX, 150, 400, 600));
            Ctx.Next(new Vector2(0.5f, 0.5f));
           Ctx.Button( "Test1");
            Ctx.Next(new Vector2(0.5f, 0.25f));

           Ctx.Button( "Test2");
            Ctx.Next(new Vector2(0.25f, 0.5f));

           Ctx.Button( "Test3");
            Ctx.Next(new Vector2(0.75f, 0.5f));

           Ctx.Button( "Test4");
            Ctx.PopArea();

        }
    }
}
