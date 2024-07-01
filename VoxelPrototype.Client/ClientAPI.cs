using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
namespace VoxelPrototype.client
{
    internal class ClientAPI
    {
        //TODO fix
        public static void SetFullscreen(bool fullscreen)
        {
            Client.TheClient.ClientInterface.SetFullscreen(fullscreen);
        }
        public static void SetCursorState(CursorState state)
        {
            Client.TheClient.ClientInterface.SetGrab(state == CursorState.Grabbed);
        }
        public static int WindowWidth()
        {
            return Client.TheClient.ClientInterface.GetFramebufferSize().X;
        }
        public static int WindowHeight()
        {
            return Client.TheClient.ClientInterface.GetFramebufferSize().Y;
        }
        public static float AspectRatio()
        {
            return WindowWidth() / (float)WindowHeight();
        }
    }
}
