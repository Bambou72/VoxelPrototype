using ImGuiNET;
using Newtonsoft.Json;
using System.Numerics;
using VoxelPrototype.network;

namespace VoxelPrototype.client.Render.GUI
{
    [Serializable]
    internal struct ServerInfo
    {
        public string Ip;
        public int Port;
        public string Name;
        public string Description;
    }
    internal static class MultiplayerGUI
    {
        static List<ServerInfo> Servers = new List<ServerInfo>();
        static int SelectedServer = 0;
        static string Ip = "";
        static string Name = "";
        static int Port = 23482;
        internal static void Render()
        {
            ImGui.Begin("MultiplayerMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            ImGui.SetWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X / 6 * 5, ImGui.GetIO().DisplaySize.Y));
            ImGui.SetWindowPos(new Vector2(ImGui.GetIO().DisplaySize.X / 6, 0));
            ImGui.Dummy(new Vector2(0, 20));
            ImGui.SetWindowFontScale(3f);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize("Multiplayer").X) * 0.5f);
            ImGui.TextColored(new Vector4(246f / 255f, 188f / 255f, 30f / 255f, 1), "Multiplayer");
            ImGui.SetWindowFontScale(1);
            ImGui.Dummy(new Vector2(0, 50));
            ImGui.BeginChild("Multi", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 150));
            List<ServerInfo> toremove = new List<ServerInfo>();
            for (int i = 0; i < Servers.Count; i++)
            {
                ImGui.SetWindowFontScale(1.75f);
                if (ImGui.Button("Connect"))
                {
                    Client.TheClient.NetworkManager.Connect(Servers[i].Ip, Servers[i].Port);
                    Client.TheClient.World.Init();
                    //GUIVar.MainMenu = false;
                }
                ImGui.SameLine();
                if (ImGui.Button("Delete"))
                {
                    toremove.Add(Servers[i]);
                }
                else
                {
                    ImGui.SameLine();
                    ImGui.SetWindowFontScale(2f);
                    if (ImGui.Selectable(Servers[i].Name, SelectedServer == i))
                    {
                        // La sélection a changé, mettre à jour l'indice de l'élément sélectionné
                        SelectedServer = i;
                    }
                    // Vérifier si l'élément est sélectionné et afficher les détails
                    if (SelectedServer == i)
                    {
                        RenderServerInfo(i);
                    }
                }
            }
            foreach (ServerInfo server in toremove)
            {
                Servers.Remove(server);
            }
            ImGui.SetWindowFontScale(1f);
            ImGui.EndChild();
            ImGui.InputText("Name", ref Name, 20);
            ImGui.InputText("Ip", ref Ip, 15);
            ImGui.InputInt("Port", ref Port);
            if (ImGui.Button("Add Server", new Vector2(200, 50)))
            {
                Servers.Add(new ServerInfo { Name = Name, Ip = Ip, Port = Port, Description = "" });
                SaveSevers();
            }
            ImGui.End();
        }
        private static void RenderServerInfo(int i)
        {
            ImGui.Indent(); // Indenter les détails pour les distinguer visuellement
            var info = Servers[i];
            ImGui.TextWrapped("Name: " + info.Name);
            ImGui.TextWrapped("Description: " + info.Description);
            ImGui.TextWrapped("Ip: " + info.Ip);
            ImGui.TextWrapped("Port: " + info.Port);
            ImGui.Unindent();
        }
        internal static void LoadSevers()
        {
            if (File.Exists("config/servers.json"))
            {
                string json = File.ReadAllText("config/servers.json");
                Servers = JsonConvert.DeserializeObject<List<ServerInfo>>(json);
            }
        }
        internal static void SaveSevers()
        {
            string json = JsonConvert.SerializeObject(Servers, Formatting.Indented);
            File.WriteAllText("config/servers.json", json);
        }
    }
}
