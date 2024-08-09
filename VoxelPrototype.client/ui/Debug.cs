using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.debug;
using VoxelPrototype.server;
namespace VoxelPrototype.client.ui
{
    
    internal static class Debug
    {
        internal static bool Opened = false;
        internal static float[] FpsHist = new float[240];
        internal static ulong C;
        static bool WireFrameView = false;
        internal static void DebugMenu()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 500));
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
            ImGui.Begin("DebugMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            System.Numerics.Vector4 Black = new System.Numerics.Vector4(1, 0f, 0f, 1);
            ImGui.Text("Profiling:");
            C++;
            FpsHist[C % 240] = Math.Max(0, 60 - ImGui.GetIO().Framerate);
            ImGui.Text($"Fps average {1000.0f / ImGui.GetIO().Framerate:0.##} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");

            ImGui.PlotHistogram("Sutter Debug", ref FpsHist[0], 240, 0, "", 0, 60, new System.Numerics.Vector2(0, 100));
            if(ImGui.Button("Save World"))
            {
                Server.TheServer.World.save();
            }
            ImGui.SeparatorText("Client");
            if (Client.TheClient.World.Initialized)
            {
                if (Client.TheClient.World.IsLocalPlayerExist())
                {
                    ImGui.Text($"Player position : {Client.TheClient.World.PlayerFactory.LocalPlayer.Position:0.##}");
                    ImGui.Text($"Player  Pitch  And Yaw  :{Client.TheClient.World.PlayerFactory.LocalPlayer.Rotation.X:0}: {Client.TheClient.World.PlayerFactory.LocalPlayer.Rotation.Y:0}");
                }
                ImGui.Text("Number of section to be mesh :" + Client.TheClient.World.ChunkManager.MeshingThread.SectionToBeMeshCount);
                ImGui.Text("Number of section to be OG :" + Client.TheClient.World.ChunkManager.Section2BeOG.Count);
                ImGui.Text($"Number of client loaded chunks: {Client.TheClient.World.GetChunkCount()}");
                ImGui.Text($"Number of rendered chunks: {Client.TheClient.World.ChunkManager.RenderedChunksCount}");

            }
            if (Client.TheClient.EmbedderServer != null && Client.TheClient.EmbedderServer.IsRunning())
            {
                ImGui.SeparatorText("Embedded Server");
                ImGui.Text($"TPS: {Server.TheServer.GetTPS()}");
                ImGui.Text($"Number of server loaded chunks: {Server.TheServer.World.ChunkManager.LoadedChunkCount}");
                ImGui.Text($"Number of chunks to be send: {Server.TheServer.World.ChunkManager.ChunkToBeSend.Count}");
                ImGui.Text($"Number of sended packet: {Server.TheServer.NetworkManager.NumberOfSendedPackets}");

            }
            ImGui.Text("Game Version:");
            ImGui.Text("Client engine version: " + EngineVersion.Version);
        }
    }
}
