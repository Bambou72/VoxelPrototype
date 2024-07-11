using ImGuiNET;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.server;
namespace VoxelPrototype.client.Render.GUI
{
    internal static class DebugGUI
    {
        internal static bool Opened = false;
        internal static bool DebugChunk;
        internal static bool ShowAABB;
        internal static float[] FpsHist = new float[240];
        internal static ulong C;
        /// <summary>
        /// Function who draw debug menu
        /// </summary>
        internal static void DebugMenu()
        {
            //ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            //ImGui.PushStyleColor(ImGuiCol.Border, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 500));
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
            ImGui.Begin("DebugMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            System.Numerics.Vector4 Black = new System.Numerics.Vector4(1, 0f, 0f, 1);
            ImGui.Text("Profiling:");
            C++;
            FpsHist[C % 240] = Math.Max( 0,60- ImGui.GetIO().Framerate);
            ImGui.Text($"Fps average {1000.0f / ImGui.GetIO().Framerate:0.##} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");

            ImGui.PlotHistogram("Sutter Debug", ref FpsHist[0],240,0,"",0,60,new System.Numerics.Vector2(0,100));
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
                ImGui.Text( $"Number of sended packet: {Server.TheServer.NetworkManager.NumberOfSendedPackets}");

            }
            ImGui.Text("Game Version:");
            ImGui.Text("Client engine version: " + EngineVersion.Version);
            //ImGui.PopStyleColor(2);
        }
        internal static void RenderDebug()
        {
            if (Client.TheClient.World.Initialized)
            {
                if (ShowAABB)
                {
                    //Show player aabb
                    foreach (var entity in Client.TheClient.World.PlayerFactory.PlayerList.Values)
                    {
                        DebugShapeRenderer.RenderDebugBox(new DebugBox()
                        {
                            Size = new Vector3d(entity.EntityWidth, entity.EntityHeight, entity.EntityWidth),
                            Color = new Vector4(1f, 0f, 0f, 1f),
                            Position = new Vector3((float)entity.Coll.x1, (float)entity.Coll.y1, (float)entity.Coll.z1),
                            Rotation = Quaternion.Identity,
                        });
                    }
                }
                if (DebugChunk)
                {
                    Vector3i playpos = new Vector3i((int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Const.ChunkSize), (int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Y / Const.ChunkSize), (int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Const.ChunkSize));
                    foreach (Vector2i pos in Client.TheClient.World.ChunkManager.ChunkByCoordinate.Keys)
                    {
                        DebugShapeRenderer.RenderDebugBox(new DebugBox()
                        {
                            Size = new Vector3d(Const.ChunkSize, Const.ChunkHeight * Const.SectionSize, Const.ChunkSize),
                            Color = new Vector4(1f, 0f, 0f, 1f),
                            Position = new Vector3(pos.X, 0, pos.Y) * Const.ChunkSize,
                            Rotation = Quaternion.Identity,
                        });
                    }
                }
            }
        }
    }
}
