using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace VoxelPrototype.client.ui
{
    
    internal static class GUIManager
    {
        internal static bool MainMenu = true;
        internal static bool IngameMenu = false;
        internal static bool ConsoleMenu = false;
        internal static bool DebugMenu = false;
        internal static bool InInputText = false;
        static ImGuiController Controller;
        public const  ImGuiWindowFlags FrameWindow = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDocking;
        internal static void Init(Vector2i ClientSize)
        {
            Controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        }
        internal static void Update()
        {
            if (Client.TheClient.KeyboardState.IsKeyPressed(Keys.T) && !InInputText)
            {
                if (!ConsoleMenu)
                {
                    Client.TheClient.NoInput = true;
                    ConsoleMenu = true;
                    Client.TheClient.SetGrab(false);
                }
                else
                {
                    ConsoleMenu = false;
                    Client.TheClient.NoInput = false;
                    Client.TheClient.SetGrab(true);
                }
            }
            if (ConsoleMenu == true && Client.TheClient.KeyboardState.IsKeyPressed(Keys.Escape))
            {
                ConsoleMenu = false;
                Client.TheClient.NoInput = false;
                Client.TheClient.SetGrab(true);
            }
            if (Client.TheClient.KeyboardState.IsKeyPressed(Keys.Escape) && !ConsoleMenu && Client.TheClient.World.Initialized)
            {
                if (IngameMenu)
                {
                    IngameMenu = false;
                    Client.TheClient.SetGrab(true);
                }
                else
                {
                    IngameMenu = true;
                    Client.TheClient.SetGrab(false);
                }
            }
            
            if (Client.TheClient.KeyboardState.IsKeyPressed(Keys.F1))
            {
                if (DebugMenu == false)
                {
                    DebugMenu = true;
                }
                else
                {
                    DebugMenu = false;
                }
            }
            if (Client.TheClient.KeyboardState.IsKeyPressed(Keys.F4))
            {
                Client.TheClient.ResourceManager.Reload();
            }
            if (DebugMenu)
            {
                Debug.DebugMenu();
            }

            if (MainMenu)
            {
                TitleScreen.Render();
            }
            else if (IngameMenu)
            {
                PauseMenu.Render();
            }
            if (Client.TheClient.World.Initialized)
            {

                if (ConsoleMenu)
                {
                    Console.ConsoleDraw();
                }
                if (!ConsoleMenu)
                {
                    InInputText = false;
                }
            }
        }
        internal static void Begin(GameWindow window, double deltaSecond)
        {
            Controller.Update(window, deltaSecond);
        }
        internal static void End()
        {
            Controller.Render();
        }
        internal static void ResizeCallback(Vector2i ClientSize)
        {
            Controller.WindowResized(ClientSize.X, ClientSize.Y); ;
        }
        internal static void CharCallback(char Char)
        {
            Controller.PressChar(Char);
        }
    }
}
