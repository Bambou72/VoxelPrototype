using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
namespace VoxelPrototype.client
{
    internal class ClientAPI
    {
        //TODO fix
        public static void SetFullscreen(bool fullscreen)
        {
            Client.TheClient.WindowState = fullscreen == true ? WindowState.Fullscreen : WindowState.Normal;
        }
        public static void SetCursorState(CursorState state)
        {
            Client.TheClient.CursorState= state;
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
            return WindowWidth() / (float)WindowHeight();
        }
    }
}
