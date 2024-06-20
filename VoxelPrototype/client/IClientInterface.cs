using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelPrototype.client
{
    public interface IClientInterface
    {
        public Vector2i GetWindowSize();
        public void SetWindowSize(Vector2i Size);
        public void SetFullscreen(bool Fullscreen);
        public void SetGrab(bool Grab);
        public Vector2i GetFramebufferSize();
        public void RegisterOnResize(Action<Vector2i, Vector2i> OnResizeCallback);
        public void RegisterOnChar(Action<uint> OnCharCallback);
        public bool IsKeyDown(Keys key);
        public bool IsKeyPressed(Keys key);
        public bool IsMouseButtonDown(MouseButton button);
        public bool IsMouseButtonPressed(MouseButton button);
        public Vector2d GetMousePosition();
        public Vector2d GetMouseDelta();
        public Vector2d GetMouseScroll();
        public void SwapBuffers();
        public void PollInputs();
        public bool ShouldEnd();

    }
}
