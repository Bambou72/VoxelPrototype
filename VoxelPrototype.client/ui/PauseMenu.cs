using ImGuiNET;
using System.Numerics;
namespace VoxelPrototype.client.ui
{
    
    internal static class PauseMenu
    {
        internal static bool Option = false;
        internal static void Render()
        {
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X *0.2f, ImGui.GetIO().DisplaySize.Y));
            ImGui.Begin("PauseMenu",GUIManager.FrameWindow);
            ImGui.Dummy(new Vector2(0, 60));
            if (ImGui.Button("Resume", new Vector2(-1, 60)))
            {
                GUIManager.IngameMenu = false;
                Client.TheClient.SetGrab(true);
                Option = false;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Settings", new Vector2(-1, 60)))
            {
                Option = true;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Quit", new Vector2(-1, 60)))
            {
                Client.TheClient.DeInitWorld();
                Client.TheClient.NetworkManager.Deconnect();
                Client.TheClient.EmbedderServer.Stop();
                GUIManager.MainMenu = true;
                GUIManager.IngameMenu = false;
            }
            ImGui.End();
            if(Option)
            {
                SettingsMenu.Render();
            }
        }
    }
}
