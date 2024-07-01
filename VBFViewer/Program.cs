using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;
using VBFViewer;

public  static class Program 
{
    internal static ImGuiController Controller;
    internal static NativeWindow window;
    internal static Texture Logo;
    public static ImFontPtr GoodFont;

    private static void Main()
    {

        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "VBF Viewer",
            Flags = ContextFlags.ForwardCompatible,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            NumberOfSamples = 0,
            MaximumClientSize = new Vector2i(800, 600),
            MinimumClientSize = new Vector2i(800, 600),
        };
        using (window = new NativeWindow(nativeWindowSettings))
        {
            window.Resize += (ResizeEventArgs) => {
                GL.Viewport(0, 0, ResizeEventArgs.Width, ResizeEventArgs.Height);
                if (Controller != null)
                {
                    Controller.WindowResized(ResizeEventArgs.Width, ResizeEventArgs.Height);

                }
            };
            //Logo = TextureLoader.LoadFromFile("logo.png",fliped:false);
            ResizeAndCenterWindow(window, 800, 600);
            Controller = new ImGuiController(window.ClientSize.X, window.ClientSize.Y);
            GoodFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("font.ttf", 20);
            Controller.RecreateFontDeviceTexture();
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename.ToString());
            ImGui.StyleColorsDark();
            while (!window.IsExiting)
            {
                window.ProcessEvents(0);
                GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                Controller.Update(window, 0.16f);
                Viewer.RenderGUI();
                Controller.Render();
                window.Context.SwapBuffers();
            }
        }
    }
    public static void ResizeAndCenterWindow(NativeWindow window, int width, int height)
    {
        int x, y;
        MonitorInfo currentMonitor = Monitors.GetMonitorFromWindow(window);
        Box2i monitorRectangle = currentMonitor.ClientArea;
        x = (monitorRectangle.Max.X + monitorRectangle.Min.X - width) / 2;
        y = (monitorRectangle.Max.Y + monitorRectangle.Min.Y - height) / 2;
        // Avoid putting it offscreen.
        if (x < monitorRectangle.Min.X) x = monitorRectangle.Min.X;
        if (y < monitorRectangle.Min.Y) y = monitorRectangle.Min.Y;
        // Actually move the window.
        window.ClientRectangle = new Box2i(x, y, x + width, y + height);
    }

}