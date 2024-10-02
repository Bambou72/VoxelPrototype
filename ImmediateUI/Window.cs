using ImmediateUI.immui;
using ImmediateUI.immui.math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;
using VoxelPrototype.client.rendering;
using VoxelPrototype.client.rendering.texture;
namespace ImmediateUI
{
    public class Window : GameWindow
    {
        ImmuiController UIController;
        string TestString = "";
        internal static string Vert = @"#version 330
layout(location = 0) in vec2 Pos;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in uint Colors;
uniform mat4 projection;
out vec2 TextureCoords;
out vec4 Color;
vec4 intToColor(uint color) {
    float r = float((color >> 24) & 0xFFu) / 255.0;
    float g = float((color >> 16) & 0xFFu) / 255.0;
    float b = float((color >> 8) & 0xFFu) / 255.0;
    float a = float(color & 0xFFu) / 255.0;
    return vec4(r, g, b, a);
}
void main(){
	gl_Position = vec4(Pos,0, 1.0) * projection;
	TextureCoords = TexCoords;
	Color = intToColor(Colors);
}";
        internal static string Frag = @"#version 330
uniform sampler2D Texture;
in vec2 TextureCoords;
in vec4 Color;
out vec4 OutColor;
void main()
{
    OutColor = Color* texture(Texture, TextureCoords);
    if(OutColor.a == 0)
    {
        discard;
    }
}";
        internal static Shader UiShader = new(Vert, Frag, true);
        static byte[] BlankData = [255, 255, 255, 255];
        public static Texture Blank = TextureLoader.LoadFromDataByte(BlankData, 1, 1);
        public Context UIContext;
        public Window(GameWindowSettings GS, NativeWindowSettings NS) : base(GS, NS)
        {
            VSync = VSyncMode.Off;
            Title = "Test UI";
            ClientSize = new(1200, 800);
            UIContext = new();
            UIContext.MouseDown = MouseDown;
            UIContext.MousePressed = MousePressed;
            UIContext.MouseUp = MouseUp;
            UIContext.KeyDown = KeyDown;
            UIContext.KeyUp = KeyUp;
            UIContext.KeyPressed = KeyPressed;
        }

        public bool  MouseDown(int Code)
        {
            return MouseState.IsButtonDown((MouseButton)Code);
        }
        public bool MouseUp(int Code)
        {
            return MouseState.IsButtonReleased((MouseButton)Code);
        }
        public bool MousePressed(int Code)
        {
            return MouseState.IsButtonPressed((MouseButton)Code);
        }
        public bool KeyDown(int Code)
        {
            return KeyboardState.IsKeyDown((Keys)Code);
        }
        public bool KeyUp(int Code)
        {
            return KeyboardState.IsKeyReleased((Keys)Code);
        }
        public bool KeyPressed(int Code)
        {
            return KeyboardState.IsKeyPressed((Keys)Code);
        }
        protected override void OnLoad()
        {
            UIController = new();
            GL.ClearColor(0.24f, 0.58f, 0.91f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = ClientSize.X +":"+ClientSize.Y + ":FPS:" + (1) / e.Time;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            UIController.Update(UIContext, this);

            UIContext.ResetFrame();
            Demo.ShowDemo(UIContext,ClientSize, (float)e.Time);
            UIController.Render(UIContext);
            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            if(UIContext != null)
            {
                UIContext.OnResize(e.Size);
            }
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            //UIContext.OnChar((char)e.Unicode);
        }
    }
}
