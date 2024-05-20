using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using VoxelPrototype.client;
namespace VoxelPrototype.api
{
    internal class ClientAPI
    {
        public static void SetFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                Client.TheClient.WindowState = WindowState.Fullscreen;
            }
            else
            {
                Client.TheClient.WindowState = WindowState.Normal;
            }
        }
        public static void SetCursorState(CursorState state)
        {
            Client.TheClient.CursorState = state;
        }
        public static Vector2 GetCursorPos()
        {
            return Client.TheClient.MouseState.Position;
        }

        public static int WindowWidth()
        {
            return Client.TheClient.ClientSize.X;
        }
        public static int WindowHeight()
        {
            return Client.TheClient.ClientSize.Y;
        }
        public static float AspectRatio()
        {
            return Client.TheClient.ClientSize.X / Client.TheClient.ClientSize.Y;
        }
    }
}
