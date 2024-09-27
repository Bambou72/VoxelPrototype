using ImmediateUI.immui;
using ImmediateUI.immui.math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
        public static float timecount = 0;
        static int NumberOfButton = 0;
        public Window(GameWindowSettings GS, NativeWindowSettings NS) : base(GS, NS)
        {
            VSync = VSyncMode.Off;
            Title = "Test UI";
            ClientSize = new(1200, 800);
            Immui.SetContext(new Context());
            Immui.MouseDown = MouseDown;
            Immui.MousePressed = MousePressed;
            Immui.MouseUp = MouseUp;
            Immui.KeyDown = KeyDown;
            Immui.KeyUp = KeyUp;
            Immui.KeyPressed = KeyPressed;
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
            GL.ClearColor(0.99f, 0.81f, 0.71f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = ClientSize.X +":"+ClientSize.Y + ":FPS:" + (1) / e.Time;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            UIController.Update(this);
            Immui.BeginFrame();
            //Immui.BeginWindow("Test");
            //Immui.EndWindow();
            /*
            timecount += (float)e.Time;
            if(timecount >0.5f )
            {
                timecount = 0;
                NumberOfButton++;
                NumberOfButton = NumberOfButton % 3;
            }
            int realNumberOfButton = (int)Math.Pow(NumberOfButton, 2);
            Layout LT = new(NumberOfButton, NumberOfButton, new Rect(10, 10, 700, 700));
            for(int i = 0;i< realNumberOfButton; i++)
            {
                Rect Rect =  LT.GetNext();
                if (Immui.Button("Test "+ i, Rect))
                {
                    Console.WriteLine($"Test {i} is clicked");
                }

            }*/
            /*
            int i = 0;
            Rect CurP;
            Layout LT = new(new Rect(10, 10, 400, 400));
            LT.NextRow(200);
             CurP = LT.NextCollumn(200);
            Immui.Button("Test " + i, CurP);
            i++;
             CurP = LT.NextCollumn(100);
            Immui.Button("Test " + i, CurP);
            i++;
            CurP = LT.NextCollumn(100);
            Immui.Button("Test " + i, CurP);
            i++;
            LT.NextRow(100);
            CurP = LT.NextCollumn(400);
            Immui.Button("Test " + i, CurP);
            i++;
            LT.NextRow(100);
            CurP = LT.NextCollumn(150);
            Immui.Button("Test " + i, CurP);
            i++;
            CurP = LT.NextCollumn(250);
            Immui.Button("Test " + i, CurP);
            i++;/*
            for (int i = 0; i < realNumberOfButton; i++)
            {
                Rect Rect = LT.GetNext();
                if (Immui.Button("Test " + i, Rect))
                {
                    Console.WriteLine($"Test {i} is clicked");
                }

            }
            */
            //Immui.Demo2DRendering();
            //Test1
            Rect CurP;
            Layout LT = new(new Rect(ClientSize.X / 2 - ClientSize.X / 6,200, ClientSize.X / 3, 800));
            LT.NextRow(100);
            CurP = LT.NextCollumn(1);
            Immui.Button("Singleplayer", CurP);
            LT.NextRow(100);

            CurP = LT.NextCollumn(1);
            Immui.Button("Multiplayer", CurP);
            LT.NextRow(100);

            CurP = LT.NextCollumn(1);
            Immui.Button("Mods", CurP);
            LT.NextRow(100);

            CurP = LT.NextCollumn(0.5f);
            Immui.Button("Options", CurP);
            CurP = LT.NextCollumn(0.5f);
            Immui.Button("Quit", CurP);
            //Test2
            Rect CurP2;
            Layout LT2 = new(new Rect(10,10, ClientSize.X / 5, ClientSize.X / 5));
            CurP2 = LT2.Next(0.5f,0.5f);
            Immui.Button("Test1", CurP2);
            CurP2 = LT2.Next(0.5f, 0.25f);
            Immui.Button("Test2", CurP2);
            CurP2 = LT2.Next(0.25f, 0.5f);
            Immui.Button("Test3", CurP2);
            CurP2 = LT2.Next(0.75f, 0.5f);
            Immui.Button("Test4", CurP2);

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
