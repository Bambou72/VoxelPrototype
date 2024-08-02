using ImGuiNET;

namespace VoxelPrototype.client.ui
{
    
    internal static class FatalPopup
    {
        internal static void ShowFatal(Exception ex)
        {
            ImGui.OpenPopup("FatalPopup");
            // Begin the modal popup
            if (ImGui.BeginPopupModal("FatalPopup"))
            {
                ImGui.TextWrapped("Oh no a fatal error occurred, we're sorry for that, please send us the following message afterwards and the context in which the error occurred in a github issue.");
                ImGui.Separator();
                ImGui.Text("Error message");
                ImGui.TextWrapped(ex.Message);
                ImGui.Text("Error source");
                ImGui.TextWrapped(ex.TargetSite.ToString());
                if (ImGui.Button("OK"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }
    }
}
