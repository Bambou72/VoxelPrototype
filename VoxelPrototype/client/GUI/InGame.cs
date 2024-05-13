using ImGuiNET;
using OpenTK.Windowing.Common;
using System.Numerics;
using VoxelPrototype.API;
using VoxelPrototype.common.Network.client;
namespace VoxelPrototype.client.GUI
{
    internal static class InGameGUI
    {
        internal static bool Option = false;
        internal static void Render()
        {
            ImGui.Begin("InGameMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 6, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(Vector2.Zero);
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.Dummy(new Vector2(0, 40));
            if (ImGui.Button("Resume", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                GUIVar.IngameMenu = false;
                ClientAPI.SetCursorState(CursorState.Grabbed);
                InputSystem.Grab = true;
                Option = false;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Options", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                Option = true;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Quit", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                Client.TheClient.DeInitWorld();
                ClientNetwork.Deconnect();
                Client.TheClient.EmbedderServer.Stop();              
                GUIVar.MainMenu = true;
                GUIVar.IngameMenu = false;
                Option = true;
            }
            ImGui.End();
        }
    }
}
