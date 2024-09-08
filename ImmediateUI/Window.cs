using ImmediateUI.immui;
using ImmediateUI.immui.math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelPrototype.client.rendering;
using VoxelPrototype.client.rendering.texture;
namespace ImmediateUI
{
    public class Window : GameWindow
    {
        public static Window Instance;
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

        public Window(GameWindowSettings GS, NativeWindowSettings NS) : base(GS, NS)
        {
            Title = "Test UI";
            Instance = this;
            ClientSize = new(1200, 800);
            Immui.SetContext(new Context());
        }
        protected override void OnLoad()
        {
            UIController = new();
            GL.ClearColor(0.99f, 0.81f, 0.71f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = ClientSize.X +":"+ClientSize.Y;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            Immui.BeginFrame((Vector2i)MousePosition, this);
            /*
            if (Immui.Button("Test", new Rect(new(400, 200), new(600, 275))))
            {
                Console.WriteLine(Immui.GetCurrentID());
            }*/
            Immui.Demo2DRendering();
            Immui.EndFrame();
            UIController.Render();
            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            Immui.OnResize(e.Size);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            Immui.OnChar((char)e.Unicode);
        }
    }
}
