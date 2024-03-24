using ImGuiNET;
using System.Numerics;
using VoxelPrototype.common.API.WorldGenerator;
using VoxelPrototype.common.Game;
using VoxelPrototype.common.Network.client;
namespace VoxelPrototype.client.GUI
{
    internal static class SingleplayerGUI
    {
        static string WorldName = "";
        static string WorldSeed = "";
        static int CurrentWorldGenerator = 0;
        internal static int SelectedWorld = -1;

        internal static void Render()
        {
            ImGui.Begin("SingleplayerMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 6 * 5, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 6, 0));
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.SetWindowFontScale(3f);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize("Singleplayer").X) * 0.5f);
            ImGui.TextColored(new Vector4(246f / 255f, 188f / 255f, 30f / 255f, 1), "Singleplayer");
            ImGui.SetWindowFontScale(1);
            ImGui.Dummy(new Vector2(0, 50));
            ImGui.BeginChild("Single", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y));
            if (ImGui.BeginTabBar("Tabs"))
            {
                ImGui.SetNextItemWidth(200);
                if (ImGui.BeginTabItem("Worlds list"))
                {
                    RenderWorldSelector();
                    ImGui.EndTabItem();
                }
                ImGui.SetNextItemWidth(200);
                if (ImGui.BeginTabItem("World Creation"))
                {
                    RenderWorldCreator();
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndChild();
            ImGui.End();
        }
        private static void RenderWorldCreator()
        {
            ImGui.Text("World Name: "); ImGui.SameLine(); ImGui.InputText("## wc1", ref WorldName, 100);
            ImGui.Text("World Seed: "); ImGui.SameLine(); ImGui.InputText("## wc3", ref WorldSeed, 10);
            ImGui.Combo("World Generators",ref CurrentWorldGenerator,WorldGeneratorRegistry.AllGeneratorsName(), WorldGeneratorRegistry.AllGeneratorsName().Length);
            if (ImGui.Button("Create", new Vector2(200, 75)))
            {
                var param = new WorldSettings(WorldSeed == "" ? new Random().Next() : int.Parse(WorldSeed), WorldGeneratorRegistry.AllGeneratorsName()[CurrentWorldGenerator], WorldName);
                Client.TheClient.EmbedderServer = new(param, "worlds/" + WorldName + "/");
                Client.TheClient.EmbedderServer.Run();
                ClientNetwork.Connect("localhost", 23482);
                GUIVar.MainMenu = false;
            }
        }
        private static void RenderWorldSelector()
        {
            for (int i = 0; i < Client.TheClient.Worlds.Count; i++)
            {
                ImGui.SetWindowFontScale(1.75f);
                if (ImGui.Button("Play##" + Client.TheClient.Worlds[i]))
                {
                    Client.TheClient.EmbedderServer = new(new WorldSettings(Client.TheClient.WorldsInfo[i].Seed, Client.TheClient.WorldsInfo[i].GetWorldGenerator(), Client.TheClient.WorldsInfo[i].Name), Client.TheClient.WorldsInfo[i].Path + "/");
                    Client.TheClient.EmbedderServer.Run();
                    ClientNetwork.Connect("localhost", 23482);
                    GUIVar.MainMenu = false;
                }
                ImGui.SameLine();
                ImGui.SetWindowFontScale(2f);
                if (ImGui.Selectable(Client.TheClient.Worlds[i], SelectedWorld == i))
                {
                    // La sélection a changé, mettre à jour l'indice de l'élément sélectionné
                    SelectedWorld = i;
                }
                // Vérifier si l'élément est sélectionné et afficher les détails
                if (SelectedWorld == i)
                {
                    RenderWorldInfo(i);
                }
            }
            ImGui.SetWindowFontScale(1f);
        }
        private static void RenderWorldInfo(int i)
        {
            ImGui.SetWindowFontScale(1.75f);
            ImGui.Indent(); // Indenter les détails pour les distinguer visuellement
            var info = Client.TheClient.WorldsInfo[i];
            ImGui.TextColored(new Vector4(0.18f, 0.55f, 0.97f, 1), "Name: ");
            ImGui.SameLine(0, 0);
            ImGui.TextColored(new Vector4(0.06f, 0.74f, 0.16f, 1), info.Name);
            ImGui.TextColored(new Vector4(0.18f, 0.55f, 0.97f, 1), "Seed: ");
            ImGui.Unindent();
        }
    }
}
