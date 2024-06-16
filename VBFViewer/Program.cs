using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VBFViewer;

public static class Program
{
    public static Window? window;
    public static NativeWindowSettings nativeWindowSettings;
    private static void Main()
    {

        nativeWindowSettings = new()
        {
            ClientSize = new Vector2i(1000, 800),
            Title = "VBF Viewer",
            Flags = ContextFlags.ForwardCompatible,
        };
        Game();


    }

    private static void Game()
    {
        using (window = new Window(GameWindowSettings.Default, nativeWindowSettings))
        {
            window.VSync = VSyncMode.On;
            window.Run();
        }
    }

    public class Window : GameWindow
    {
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }
        ImGuiController Controller;
        protected override void OnLoad()
        {
            base.OnLoad();
            Controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename.ToString());
            Style.SetupImGuiStyle();
        }
        protected override void OnUnload()
        {
            base.OnUnload();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //Clear buffer
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            Controller.Render();
            //
            //UI
            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Controller.Update(this, (float)e.Time);

            Viewer.RenderGUI();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            if (Controller != null)
            {
                Controller.WindowResized(ClientSize.X, ClientSize.Y); ;

            }
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            Controller.PressChar((char)e.Unicode);
        }
    }
}