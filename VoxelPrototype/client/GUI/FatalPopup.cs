using ImGuiNET;
namespace VoxelPrototype.client.GUI
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
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.
                ImGui.TextWrapped(ex.TargetSite.ToString());
#pragma warning restore CS8602 // Déréférencement d'une éventuelle référence null.
                if (ImGui.Button("OK"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }
    }
}
