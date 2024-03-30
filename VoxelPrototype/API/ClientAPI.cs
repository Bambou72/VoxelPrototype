using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using VoxelPrototype.client;
namespace VoxelPrototype.API
{
    internal class ClientAPI
    {
        public static void SetFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                Client.TheClient.Window.WindowState = WindowState.Fullscreen;
            }
            else
            {
                Client.TheClient.Window.WindowState = WindowState.Normal;
            }
        }
        public static void SetCursorState(CursorState state)
        {
            Client.TheClient.Window.CursorState = state;
        }
        public static Vector2 GetCursorPos()
        {
            return Client.TheClient.Window.GetMouseState().Position;
        }

        public static int WindowWidth()
        {
            return Client.TheClient.Window.GetWindowSize().X;
        }
        public static int WindowHeight()
        {
            return Client.TheClient.Window.GetWindowSize().Y;
        }
        public static float AspectRatio()
        {
            return Client.TheClient.Window.GetWindowSize().X / Client.TheClient.Window.GetWindowSize().Y;
        }
    }
}
