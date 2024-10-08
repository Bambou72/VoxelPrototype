﻿using ImGuiNET;
using ModelEditor;
using ModelEditor.ModelEditor;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
public static class Program
{
    public static Window? window;
    public static NativeWindowSettings nativeWindowSettings;
    private static void Main()
    {
        nativeWindowSettings = new()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Voxel Prototype Model Editor",
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
}
public class Window : GameWindow
{
    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }
    ImGuiController Controller;
    protected override void OnLoad()
    {
        base.OnLoad();
        Controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        // DarkModern style from ImThemes
        var style = ImGui.GetStyle();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename.ToString());
        style.Alpha = 1.0f;
        style.DisabledAlpha = 0.6000000238418579f;
        style.WindowPadding = new System.Numerics.Vector2(8.0f, 8.0f);
        style.WindowRounding = 0.0f;
        style.WindowBorderSize = 1.0f;
        style.WindowMinSize = new System.Numerics.Vector2(32.0f, 32.0f);
        style.WindowTitleAlign = new System.Numerics.Vector2(0.5f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.None;
        style.ChildRounding = 0.0f;
        style.ChildBorderSize = 1.0f;
        style.PopupRounding = 10.0f;
        style.PopupBorderSize = 1.0f;
        style.FramePadding = new System.Numerics.Vector2(10.0f, 5.0f);
        style.FrameRounding = 0.0f;
        style.FrameBorderSize = 0.0f;
        style.ItemSpacing = new System.Numerics.Vector2(4.0f, 4.0f);
        style.ItemInnerSpacing = new System.Numerics.Vector2(4.0f, 4.0f);
        style.CellPadding = new System.Numerics.Vector2(4.0f, 2.0f);
        style.IndentSpacing = 21.0f;
        style.ColumnsMinSpacing = 5.0f;
        style.ScrollbarSize = 15.0f;
        style.ScrollbarRounding = 0.0f;
        style.GrabMinSize = 10.0f;
        style.GrabRounding = 0.0f;
        style.TabRounding = 0.0f;
        style.TabBorderSize = 0.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new System.Numerics.Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new System.Numerics.Vector2(0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.4980392158031464f, 0.4980392158031464f, 0.4980392158031464f, 1.0f);
        style.Colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.05882352963089943f, 0.05882352963089943f, 0.05882352963089943f, 1f);
        style.Colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.0784313753247261f, 0.0784313753247261f, 0.0784313753247261f, 0.9399999976158142f);
        style.Colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.4274509847164154f, 0.4274509847164154f, 0.4980392158031464f, 0.5f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.2644366025924683f, 0.2858321070671082f, 0.3175965547561646f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 0.6705882549285889f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 0.6705882549285889f);
        style.Colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.03921568766236305f, 0.03921568766236305f, 0.03921568766236305f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.2746412754058838f, 0.2850725948810577f, 0.3004291653633118f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.5099999904632568f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.1372549086809158f, 0.1372549086809158f, 0.1372549086809158f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.01960784383118153f, 0.01960784383118153f, 0.01960784383118153f, 0.5299999713897705f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.3098039329051971f, 0.3098039329051971f, 0.3098039329051971f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.407843142747879f, 0.407843142747879f, 0.407843142747879f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.5098039507865906f, 0.5098039507865906f, 0.5098039507865906f, 1.0f);
        style.Colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.9999899864196777f, 0.9999945759773254f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 1.0f);
        style.Colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.4070806205272675f, 0.4172268807888031f, 0.4291845560073853f, 0.4000000059604645f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.2505663931369781f, 0.2557240426540375f, 0.2618025541305542f, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.1931883096694946f, 0.1953609734773636f, 0.1974248886108398f, 1.0f);
        style.Colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.407843142747879f, 0.4156862795352936f, 0.4274509847164154f, 0.4000000059604645f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 0.6705882549285889f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 0.6705882549285889f);
        style.Colors[(int)ImGuiCol.Separator] = new System.Numerics.Vector4(0.4274509847164154f, 0.4274509847164154f, 0.4980392158031464f, 0.5f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.407843142747879f, 0.4156862795352936f, 0.4274509847164154f, 0.7803921699523926f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.407843142747879f, 0.4156862795352936f, 0.4274509847164154f, 1.0f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.407843142747879f, 0.4156862795352936f, 0.4274509847164154f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 0.6705882549285889f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 0.9490196108818054f);
        style.Colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.407843142747879f, 0.4156862795352936f, 0.4274509847164154f, 0.8627451062202454f);
        style.Colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 0.800000011920929f);
        style.Colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 1.0f);
        style.Colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.06666667014360428f, 0.1019607856869698f, 0.1450980454683304f, 0.9724000096321106f);
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.1921568661928177f, 0.196078434586525f, 0.196078434586525f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.6078431606292725f, 0.6078431606292725f, 0.6078431606292725f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(1.0f, 0.4274509847164154f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(0.7843137383460999f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new System.Numerics.Vector4(0.1882352977991104f, 0.1882352977991104f, 0.2000000029802322f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new System.Numerics.Vector4(0.3098039329051971f, 0.3098039329051971f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new System.Numerics.Vector4(0.2274509817361832f, 0.2274509817361832f, 0.2470588237047195f, 1.0f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 0.05999999865889549f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.250980406999588f, 0.2549019753932953f, 0.2627451121807098f, 0.3490196168422699f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(1.0f, 1.0f, 0.0f, 0.8999999761581421f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new System.Numerics.Vector4(0.5882353186607361f, 0.5882353186607361f, 0.5882353186607361f, 1.0f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 0.699999988079071f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new System.Numerics.Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.3499999940395355f);
        ModelRenderer.Initialize();
       // TextureManager.LoadTextures();
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
        Controller.Update( (float)e.Time);
        ModelEditorGUI.Render();
        Controller.Render();
        //
        //UI
        SwapBuffers();
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (MouseState.IsButtonDown(MouseButton.Middle))
        {
            ModelRenderer.Camera.Rotate(MouseState.Delta * (float)e.Time);
        }
        else if (MouseState.IsButtonDown(MouseButton.Right))
        {
            ModelRenderer.Camera.Translate(MouseState.Delta * (float)e.Time);
        }
        ModelRenderer.Camera.Zoom(MouseState.ScrollDelta.Y);
        base.OnUpdateFrame(e);
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        Controller.WindowResized(ClientSize.X, ClientSize.Y); ;
    }
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        Controller.PressChar((char)e.Unicode);
    }
}