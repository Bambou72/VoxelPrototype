using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace VoxelPrototype.client
{
    public struct MouseDownEvent
    {
        public MouseButton Button;
        public Vector2i MousePosition;
    }
    public struct MouseUpEvent
    {
        public MouseButton Button;
        public Vector2i MousePosition;
    }
    public struct MouseClickedEvent
    {
        public MouseButton Button;
        public Vector2i MousePosition;
    }
    public struct MouseMovedEvent
    {
        public Vector2i MousePosition;
        public Vector2i Delta;
    }
    public struct MouseWheelEvent
    {
        public Vector2i Delta;
    }
    public struct KeyDownEvent
    {
        public Keys Key;
        public KeyModifiers Modifiers;
    }
    public struct KeyUpEvent
    {
        public Keys Key;
        public KeyModifiers Modifiers;
    }
    public struct KeyPressedEvent
    {
        public Keys Key;
        public KeyModifiers Modifiers;
    }
}
