using ImGuiNET;
using System.Numerics;
namespace VoxelPrototype.client.ui
{
    internal static class TitleScreen
    {
        internal enum Menu
        {
            None,
            Singleplayer,
            Multiplayer,
            Mods,
            Settings,
            Credits
        }
        internal static Menu ActiveMenu = Menu.None;
        public static void Render()
        {
            if(Client.TheClient.KeyboardState.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                ActiveMenu = Menu.None;
            }
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 5, ImGui.GetIO().DisplaySize.Y));
            ImGui.Begin("Main", GUIManager.FrameWindow);
            ImGui.Dummy(new Vector2(0, 30));
            ImGui.Image(Client.TheClient.TextureManager.GetTexture("voxelprototype:textures/gui/title").GetHandle(), new(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().X / 3.5857f),new Vector2(0,1),new(1,0));
            ImGui.Dummy(new Vector2(0, 30));
            if (ImGui.Button("Singleplayer", new Vector2(-1, 50)))
            {
                ActiveMenu = Menu.Singleplayer;
            }
            //ImGui.Dummy(new Vector2(0, 10));
            if (ImGui.Button("Multiplayer", new Vector2(-1, 50)))
            {
                ActiveMenu = Menu.Multiplayer;
            }
            if (ImGui.Button("Mods", new Vector2(-1, 50)))
            {
                ActiveMenu = Menu.Mods;
            }
            if (ImGui.Button("Settings", new Vector2(-1, 50)))
            {
                ActiveMenu = Menu.Settings;
            }
            if (ImGui.Button("Credits", new Vector2(-1, 50)))
            {
                ActiveMenu = Menu.Credits;
            }
            if (ImGui.Button("Quit", new Vector2(-1, 50)))
            {
                Client.TheClient.Close();
            }
            ImGui.SetCursorPosX(ImGui.GetWindowSize().X /2 - ImGui.CalcTextSize(Versions.Version).X/2);
            ImGui.SetCursorPosY(ImGui.GetWindowSize().Y  - ImGui.CalcTextSize(Versions.Version).Y -10);
            ImGui.Text(Versions.Version);
            ImGui.End();
            if (ActiveMenu == Menu.Singleplayer)
            {
                SingleplayerMenu.Render();
            }
            else if (ActiveMenu == Menu.Multiplayer)
            {
                MultiplayerMenu.Render();
            }
            else if (ActiveMenu == Menu.Mods)
            {
            }
            else if (ActiveMenu == Menu.Settings)
            {
                SettingsMenu.Render();
            }
            else if (ActiveMenu == Menu.Credits)
            {
                CreditsMenu.Render();
            }
        }
    }
}
