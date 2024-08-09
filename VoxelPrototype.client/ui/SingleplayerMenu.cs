using ImGuiNET;
using NLog;
using System.Numerics;
using VoxelPrototype.api.worldgeneration;
using VoxelPrototype.game;
namespace VoxelPrototype.client.ui
{

    internal static class SingleplayerMenu
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static string WorldName = "";
        static string WorldSeed = "";
        static int CurrentWorldGenerator = 0;
        internal static int SelectedWorld = -1;

        internal static void Render()
        {
            ImGui.Begin("SingleplayerMenu", GUIManager.FrameWindow);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 5 * 4, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 5, 0));
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
            ImGui.Combo("World Generators", ref CurrentWorldGenerator, WorldGeneratorRegistry.GetInstance().AllGeneratorsName(), WorldGeneratorRegistry.GetInstance().AllGeneratorsName().Length);
            if (ImGui.Button("Create", new Vector2(200, 75)))
            {
                var param = new WorldSettings(WorldSeed == "" ? new Random().Next() : int.Parse(WorldSeed), WorldGeneratorRegistry.GetInstance().AllGeneratorsName()[CurrentWorldGenerator], WorldName);
                Client.TheClient.EmbedderServer = new("worlds/" + WorldName + "/", param);
                Client.TheClient.EmbedderServer.Start();
                Client.TheClient.NetworkManager.Connect("localhost", 23482);
                Client.TheClient.World.Init();
                GUIManager.MainMenu = false;
            }
        }
        private static void RenderWorldSelector()
        {
            var Worlds = LoadWorld();
            for (int i = 0; i < Worlds.Count; i++)
            {
                ImGui.SetWindowFontScale(1.75f);
                if (ImGui.Button("Play##" + Worlds[i].Name))
                {
                    Client.TheClient.EmbedderServer = new(Worlds[i].Path + "/");
                    Client.TheClient.EmbedderServer.Start();
                    Client.TheClient.NetworkManager.Connect("localhost", 23482);
                    Client.TheClient.World.Init();
                    GUIManager.MainMenu = false;
                }
                ImGui.SameLine();
                ImGui.SetWindowFontScale(2f);
                if (ImGui.Selectable(Worlds[i].Name, SelectedWorld == i))
                {
                    // La sélection a changé, mettre à jour l'indice de l'élément sélectionné
                    SelectedWorld = i;
                }
                // Vérifier si l'élément est sélectionné et afficher les détails
                if (SelectedWorld == i)
                {
                    RenderWorldInfo(Worlds, i);
                }
            }
            ImGui.SetWindowFontScale(1f);
        }
        private static void RenderWorldInfo(List<WorldInfo> Worlds, int i)
        {
            ImGui.SetWindowFontScale(1.75f);
            ImGui.Indent(); // Indenter les détails pour les distinguer visuellement
            var info = Worlds[i];
            ImGui.TextColored(new Vector4(0.18f, 0.55f, 0.97f, 1), "Name: ");
            ImGui.SameLine(0, 0);
            ImGui.TextColored(new Vector4(0.06f, 0.74f, 0.16f, 1), info.Name);
            ImGui.TextColored(new Vector4(0.18f, 0.55f, 0.97f, 1), "Seed: ");
            ImGui.Unindent();
        }


        public static List<WorldInfo> LoadWorld()
        {
            List<WorldInfo> Worlds = new();
            string[] WorldFolders = Directory.GetDirectories("worlds");
            foreach (string world in WorldFolders)
            {
                if (File.Exists(world + "/world.vpw"))
                {
                    try
                    {
                        WorldInfo worldInfo = new WorldInfo().Deserialize(File.ReadAllBytes(world + "/world.vpw"));
                        worldInfo.Path = world;
                        worldInfo.Name = world.Split("\\").Last();
                        Worlds.Add(worldInfo);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Worldinfo can't be load , possibly due to corrupted data.");
                    }
                }
                else
                {
                }
            }
            return Worlds;
        }

    }
}
