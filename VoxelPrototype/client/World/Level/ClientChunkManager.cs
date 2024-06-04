/*ChunkManager for client side
 *Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.client.World.Level.Chunk;
namespace VoxelPrototype.client.World.Level
{
    public  class ClientChunkManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal ConcurrentDictionary<Vector2i, Chunk.Chunk> Clist = new();
        internal ConcurrentQueue<Section> SectionToBeMesh = new();
        internal Dictionary<Vector3i, int> Breaking = new();
        internal int RenderedChunksCount = 0;

        internal void Dispose()
        {
            Clist.Clear();
            Breaking.Clear();
        }

        internal void AddChunk(Chunk.Chunk chunk)
        {
            bool suc = false;
            while (!suc)
            {
                lock (Clist)
                {
                    Clist[chunk.Position] = chunk;
                    suc = true;
                }
            }
        }
        internal void HandleChunk(ChunkData data, NetPeer peer)
        {
            Vector2i pos = data.Pos;
            if (!Clist.TryGetValue(pos, out Chunk.Chunk _))
            {
                AddChunk(new Chunk.Chunk().Deserialize(Deflate.Decompress(data.Data)));
                Clist[pos].State |= Chunk.ChunkSate.Changed;
                foreach (Chunk.Section section in Clist[pos].Sections)
                {
                    SectionToBeMesh.Enqueue(section);
                }
            }
        }
        internal void HandleChunkUnload(UnloadChunk data, NetPeer peer)
        {
            foreach (Vector2i ChunkPosition in data.Positions)
            {
                if (Clist.TryGetValue(ChunkPosition, out Chunk.Chunk ch))
                {
                    ch = null;
                    Clist.Remove(ChunkPosition, out Chunk.Chunk _);
                }
            }
        }
        internal void HandleChunkUpdate(OneBlockChange data, NetPeer peer)
        {
            Vector2i cpos = data.ChunkPos;
            Vector3i bpos = data.BlockPos;
            if (Clist.TryGetValue(cpos, out Chunk.Chunk ch))
            {
                ch.SetBlock(bpos, data.State);
                //Neighboors
                int SecPos = bpos.Y / 16;
                foreach (Vector2i neighborpos in CubeNeighbours.XZNeighbours)
                {
                    if (Clist.TryGetValue(cpos+ neighborpos,out var Neighbor))
                    {
                        Neighbor.State |= ChunkSate.Changed;
                        for (int i = -1; i <= 1; i++)
                        {
                            Section sec = ch.Sections[SecPos + i];
                            if (sec != null && !SectionToBeMesh.Contains(sec))
                            {
                                SectionToBeMesh.Enqueue(sec);
                            }
                        }
                    }
                }
                
            }
        }
        internal BlockState GetBlockForMesh(Vector3i bpos, Vector2i cpos)
        {
            Vector2i Rpos = cpos;
            if (bpos.X > Const.ChunkSize - 1)
            {
                Rpos.X += 1;
                bpos.X = 0;
            }
            else if (bpos.X < 0)
            {
                Rpos.X -= 1;
                bpos.X = Const.ChunkSize - 1;
            }
            if (bpos.Z > Const.ChunkSize - 1)
            {
                Rpos.Y += 1;
                bpos.Z = 0;
            }
            else if (bpos.Z < 0)
            {
                Rpos.Y -= 1;
                bpos.Z = Const.ChunkSize - 1;
            }
            try
            {
                Chunk.Chunk CH = GetChunk(Rpos.X, Rpos.Y);
                if (CH != null)
                {
                    return CH.GetBlock(bpos);
                }
                return Client.TheClient.ModManager.BlockRegister.Air;
            }
            catch
            {
                return Client.TheClient.ModManager.BlockRegister.Air;
            }
        }

        internal Chunk.Chunk GetChunk(int x, int z)
        {
            Vector2i Pos = new Vector2i(x, z);
            if (Clist.ContainsKey(Pos))
            {
                return Clist[Pos];
            }
            return null;
        }
        public void ChangeChunk(Vector3i blockp, BlockState State)
        {
            (Vector2i cpos, Vector3i bpos) = Coord.GetVoxelCoord(blockp.X, blockp.Y, blockp.Z);
            OneBlockChangeDemand packet = new OneBlockChangeDemand
            {
                State = State,
                ChunkPos = cpos,
                BlockPos = bpos,
            };
            ClientNetwork.SendPacket(packet, DeliveryMethod.ReliableOrdered);
        }
        internal bool GetBlock(int x, int y, int z, out BlockState id)
        {
            bool suc = false;
            Chunk.Chunk chunk = null;
            while (!suc)
            {
                lock (Clist)
                {
                    (Vector2i cpos, Vector3i bpos) = Coord.GetVoxelCoord(x, y, z);
                    if (Clist.TryGetValue(cpos, out Chunk.Chunk ch))
                    {
                        id = ch.GetBlock(new Vector3i(bpos.X, bpos.Y, bpos.Z));
                        suc = true;
                        return true;
                    }
                    else
                    {
                        id = Client.TheClient.ModManager.BlockRegister.Air;
                        return false;
                    }
                }
            }
            id = Client.TheClient.ModManager.BlockRegister.Air;
            return false;
        }
        internal void Update()
        {
            for (int i = 0; i < 100; i++)
            {
                if(SectionToBeMesh.Count == 0)
                {
                    break;
                }
                if(SectionToBeMesh.TryDequeue(out Section sec))
                {
                    if (sec.Chunk.IsSurrendedClient())
                    {
                        sec.SectionMesh.Generate();
                        sec.SectionMesh.Upload();

                    }else
                    {
                        SectionToBeMesh.Enqueue(sec);
                    }

                }
            }
        }
        internal void Render()
        {
            RenderedChunksCount = 0;
            var Shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/chunk"));
            var Camera = Client.TheClient.World.GetLocalPlayerCamera();
            Camera.Update();
            Shader.SetMatrix4("view", Camera.GetViewMatrix());
            Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            Client.TheClient.TextureManager.GetTexture(new Resources.ResourceID("textures/block/atlas")).Use(TextureUnit.Texture0);
            int minx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int minz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int maxx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            int maxz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            Shader.Use();
            foreach (var pos in Clist.Keys)
            {
                if (pos.X >= minx && pos.Y >= minz && pos.X <= maxx && pos.Y <= maxz)
                {
                    foreach (var section in Clist[pos].Sections)
                    {
                        if (section.SectionMesh.GetOpaqueMesh().GetVerticesCount() != 0 && Camera.Frustum.IsSectionInFrustum(section.SectionMesh))
                        {
                            RenderedChunksCount++;
                            Matrix4 model = Matrix4.CreateTranslation(new Vector3(pos.X * Const.ChunkSize, section.Y * Chunk.Section.Size, pos.Y * Const.ChunkSize));
                            Shader.SetMatrix4("model", model);
                            GL.BindVertexArray(section.SectionMesh.GetOpaqueMesh().GetVAO());
                            GL.DrawElements(PrimitiveType.Triangles, section.SectionMesh.GetOpaqueMesh().GetIndicesCount(), DrawElementsType.UnsignedInt, 0);
                            GL.BindVertexArray(0);
                        }
                    }
                }
            }
        }
    }
}
