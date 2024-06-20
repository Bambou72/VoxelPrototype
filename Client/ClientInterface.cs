using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client;

namespace Client
{
    internal class ClientInterface : IClientInterface
    {
        internal ClientWindow window;

        public ClientInterface(ClientWindow _window)
        {
            window = _window;
        }

        public Vector2i GetFramebufferSize()
        {
            return window.FramebufferSize;
        }

        public Vector2i GetWindowSize()
        {
            return window.Size;
        }
        public void SetWindowSize(Vector2i Size)
        {
            window.Size = Size;
        }
        public bool IsKeyDown(Keys key)
        {
            return window.KeyboardListener.IsKeyDown(key);
        }
        public bool IsKeyPressed(Keys key)
        {
            return window.KeyboardListener.IsKeyPressed(key);
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            return window.MouseListener.IsMouseButtonDown(button);
        }
        public bool IsMouseButtonPressed(MouseButton button)
        {
            return window.MouseListener.IsMouseButtonPressed(button);
        }

        public Vector2d GetMousePosition()
        {
            return window.MouseListener.GetMousePosition();
        }

        public Vector2d GetMouseDelta()
        {
            return window.MouseListener.GetMouseDelta();
        }

        public Vector2d GetMouseScroll()
        {
            return window.MouseListener.GetScroll();
        }

        public void SwapBuffers()
        {
            window.SwapBuffer();
        }

        public void PollInputs()
        {
            window.PollEvent();
        }
        public void RegisterOnResize(Action<Vector2i, Vector2i> OnResizeCallback)
        {
            window.RegisterResizeCallback(OnResizeCallback);
        }

        public void RegisterOnChar(Action<uint> OnCharCallback)
        {
            window.RegisterCharCallback(OnCharCallback);
        }

        public void SetFullscreen(bool Fullscreen)
        {
            window.Fullscreen = Fullscreen;
        }
        public void SetGrab(bool Grab)
        {
            window.Grab = Grab;
        }
        public bool ShouldEnd()
        {
            return window.ShouldClose();
        }
    }
}
