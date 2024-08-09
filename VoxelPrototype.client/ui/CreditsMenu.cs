using ImGuiNET;
using System.Numerics;

namespace VoxelPrototype.client.ui
{
    internal static class CreditsMenu
    {
        internal static void Render()
        {
            ImGui.Begin("CreditsMenu", GUIManager.FrameWindow);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 5 * 4, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 5, 0));
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.SetWindowFontScale(3f);
            ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2 - ImGui.CalcTextSize("Credits").X / 2);
            ImGui.TextColored(new Vector4(246f / 255f, 188f / 255f, 30f / 255f, 1), "Credits");
            ImGui.SetWindowFontScale(1);
            ImGui.Dummy(new Vector2(0, 50));
            ImGui.SetWindowFontScale(2);
            ImGui.Text(
            @"
Dev:
    -Florian Pfeiffer : Lead | Engine | Graphic | Gameplay
Art:
    -Florian Pfeiffer : Textures | Models | Fonts


Libs:
    - OpenTK MIT
    - MicroUI MIT
    - DearImgui MIT
    - NLog BSD-3-Clause
    - LiteNetLib MIT
    - FastNoiseLite MIT
    - K4os.Compression.LZ4 MIT
            "
            );
            ImGui.SetWindowFontScale(1);

            ImGui.End();
        }
    }
}
