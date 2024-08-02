using ImGuiNET;
using System.Numerics;
namespace VoxelPrototype.client.ui
{
    
    internal static class MainGUI
    {
        internal static bool SinglePlayer = false;
        internal static bool MultiPlayer = false;
        internal static bool Option = false;
        internal static bool ModelEditor = false;
        public static void Render()
        {
            ImGui.Begin("Menu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDocking);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 6, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(Vector2.Zero);
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.Dummy(new Vector2(0, 40));
            if (ImGui.Button("Singleplayer", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                SinglePlayer = true;
                MultiPlayer = false;
                Option = false;
                ModelEditor = false;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Multiplayer", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                SinglePlayer = false;
                MultiPlayer = true;
                Option = false;
                ModelEditor = false;
            }
            ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Options", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                SinglePlayer = false;
                MultiPlayer = false;
                Option = true;
                ModelEditor = false;
            }
            ImGui.Dummy(new Vector2(0, 10));

            if (ImGui.Button("Quit", new Vector2(ImGui.GetIO().DisplaySize.X / 6 - 20, 60)))
            {
                Client.TheClient.Close();
            }
            ImGui.End();
        }
    }
}
