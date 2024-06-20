using ImGuiNET;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.common;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.World;
using VoxelPrototype.server;
namespace VoxelPrototype.client.Render.GUI
{
    internal static class DebugGUI
    {
        internal static bool Opened = false;
        internal static bool DebugChunk;
        internal static bool ShowAABB;
        /// <summary>
        /// Function who draw debug menu
        /// </summary>
        internal static void DebugMenu()
        {
            ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            ImGui.PushStyleColor(ImGuiCol.Border, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            ImGui.Begin("DebugMenu", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            ImGui.SetWindowPos(new System.Numerics.Vector2(0, 0));
            System.Numerics.Vector4 Black = new System.Numerics.Vector4(1, 0f, 0f, 1);
            ImGui.TextColored(Black, "Profiling:");
            ImGui.TextColored(Black, $"Fps average {1000.0f / ImGui.GetIO().Framerate:0.##} ms/frame ({ImGui.GetIO().Framerate:0.#} FPS)");
            ImGui.SeparatorText("Client");
            if (Client.TheClient.World.Initialized)
            {
                ImGui.TextColored(Black, "Player position :" + Client.TheClient.World.PlayerFactory.LocalPlayer.Position);
                ImGui.TextColored(Black, "Number of section to be mesh :" + Client.TheClient.World.RenderThread.SectionToBeMeshCount);
            }
            ImGui.TextColored(Black, $"Number of client loaded chunks: {Client.TheClient.World.GetChunkCount()}");
            ImGui.TextColored(Black, $"Number of rendered chunks: {Client.TheClient.World.ChunkManager.RenderedChunksCount}");
            if (Client.TheClient.EmbedderServer != null && Client.TheClient.EmbedderServer.IsRunning())
            {
                ImGui.SeparatorText("Embedded Server");
                ImGui.TextColored(Black, $"TPS: {Server.TheServer.ServerTimer.GetTPS()}");
                ImGui.TextColored(Black, $"Number of server loaded chunks: {Server.TheServer.World.ChunkManager.LoadedChunkCount}");
                ImGui.TextColored(Black, $"Number of chunks to be send: {Server.TheServer.World.ChunkManager.ChunkToBeSend.Count}");
                ImGui.TextColored(Black, $"Number of sended packet: {ServerNetwork.NumberOfSendedPacket}");

            }
            ImGui.TextColored(Black, "Game Version:");
            ImGui.TextColored(Black, "Client engine version: " + EngineVersion.Version);
            ImGui.TextColored(Black, "Server engine version: " + ClientNetwork.ServerEngineVersion);
            ImGui.PopStyleColor(2);
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
                    foreach (Vector2i pos in Client.TheClient.World.ChunkManager.Clist.Keys)
                    {
                        DebugShapeRenderer.RenderDebugBox(new DebugBox()
                        {
                            Size = new Vector3d(Const.ChunkSize, Const.ChunkHeight * Section.Size, Const.ChunkSize),
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
