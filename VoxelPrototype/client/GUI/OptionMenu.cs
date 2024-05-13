using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;
using VoxelPrototype.API;
using VoxelPrototype.server;
namespace VoxelPrototype.client.GUI
{
    internal static class OptionMenu
    {
        static float Fov;
        internal static float FrustumFov=70;
        static bool FollowCameraFov = true;
        static bool WireFrameView = false;
        static bool Fullscreen = false;
        internal static void Render()
        {
            ImGui.Begin("OptionMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 6 * 5, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 6, 0));
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.SetWindowFontScale(3f);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize("Option").X) * 0.5f);
            ImGui.TextColored(new Vector4(246f / 255f, 188f / 255f, 30f / 255f, 1), "Option");
            ImGui.SetWindowFontScale(1);
            ImGui.Dummy(new Vector2(0, 50));
            ImGui.BeginChild("Option", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y));
            ImGui.SetNextItemWidth(200);
            if (ImGui.BeginTabBar("OptionTabs"))
            {
                if (ImGui.BeginTabItem("Graphic"))
                {
                    GraphicOption();
                    ImGui.EndTabItem();
                }
                ImGui.SetNextItemWidth(200);
                if (ImGui.BeginTabItem("Voxel"))
                {
                    VoxelOption();
                    ImGui.EndTabItem();
                }
                ImGui.SetNextItemWidth(200);
                if (ImGui.BeginTabItem("Debug"))
                {
                    DebugOption();
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndChild();
            ImGui.End();
        }
        private static void GraphicOption()
        {
            if (ImGui.Checkbox("Fullscreen", ref Fullscreen)) { ClientAPI.SetFullscreen(Fullscreen); };
            if (ImGui.DragFloat("FOV", ref Fov,0.5f, 1, 110))
            {
                Client.TheClient.World.GetLocalPlayerCamera().Fov = Fov;
            }
            /*
            ImGui.Checkbox("Frustum FOV follow camera's fov", ref FollowCameraFov);
            if (FollowCameraFov)
            {
                FrustumFov = Client.TheClient.World.GetLocalPlayerCamera().Fov;
            }else
            {
                ImGui.DragFloat("Frustum FOV", ref FrustumFov,0.5f, 1, 110);
            }*/
        }
        private static void VoxelOption()
        {
            if (Client.TheClient.World.Initialized)
            {
                if (ImGui.SliderInt("Render Distance:", ref Client.TheClient.World.RenderDistance, 2, 32))
                {
                    if (Client.TheClient.EmbedderServer != null && Client.TheClient.EmbedderServer.IsRunning())
                    {
                        Server.TheServer.World.LoadDistance = Client.TheClient.World.RenderDistance+1;
                    }
                }
            }
        }
        private static void DebugOption()
        {
            ImGui.Checkbox("ChunkDebug", ref DebugGUI.DebugChunk);
            ImGui.Checkbox("AABB view", ref DebugGUI.ShowAABB);
            if (ImGui.Button("Debug Mesh"))
            {
                if (WireFrameView == true)
                {
                    WireFrameView = false;
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
                else if (WireFrameView == false)
                {
                    WireFrameView = true;
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                }
            }
        }
    }
}
