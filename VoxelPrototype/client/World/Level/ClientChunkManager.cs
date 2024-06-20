/* ChunkManager for client side
 * Copyrights Florian Pfeiffer
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
using OpenTK.Graphics.OpenGL;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.game.Commands;
using K4os.Compression.LZ4;
using VoxelPrototype.VBF;
using SharpFont;
using VoxelPrototype.common.Blocks;
namespace VoxelPrototype.client.World.Level
{
    public  class ClientChunkManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal ConcurrentDictionary<Vector2i, Chunk.Chunk> Clist = new(24,0);
        ConcurrentQueue<SectionToOG> Section2BeOG = new();

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
                AddChunk(new Chunk.Chunk().Deserialize((VBFCompound)VBFSerializer.Deserialize(LZ4Pickler.Unpickle(data.Data))));
                Clist[pos].State |= Chunk.ChunkSate.Changed;
                foreach (Section section in Clist[pos].Sections)
                {
                    AddSectionToBeMesh((ushort)data.importance, section);
                }
            }
        }
        internal void HandleChunkUnload(UnloadChunk data, NetPeer peer)
        {
            foreach (Vector2i ChunkPosition in data.Positions)
            {
                if (Clist.TryGetValue(ChunkPosition, out Chunk.Chunk ch))
                {
                    ch.Dispose();
                    ch = null;
                    Clist.Remove(ChunkPosition, out Chunk.Chunk _);
                }
            }
        }
        internal void AddSectionToBeMesh(ushort importance , Section sect)
        {
            Client.TheClient.World.RenderThread.AddSectionToBeMesh(new SectionToMeshing()
            {
                Importance = importance,
                Generator = new(sect),
                Pos = new Vector3i(sect.Chunk.X,sect.Y,sect.Chunk.Z)
            });
        }
        internal void AddSectionToOG( SectionToOG section)
        {

            Section2BeOG.Enqueue(section);
            
        }

        internal void HandleChunkUpdate(OneBlockChange data, NetPeer peer)
        {
            if (Clist.TryGetValue(data.ChunkPos, out Chunk.Chunk ch))
            {
                ch.SetBlock(data.BlockPos, data.State);
                int SecPos = data.BlockPos.Y / 16;
                AddSectionToBeMesh(0,ch.Sections[SecPos]);
                if(SecPos < Const.ChunkHeight)
                {
                   AddSectionToBeMesh(0,ch.Sections[SecPos+1]);
                }
                if(SecPos > 0)
                { 
                    AddSectionToBeMesh(0,ch.Sections[SecPos-1]);
                }
                //Neighboors
                foreach (Vector2i neighborpos in CubeNeighbours.XZNeighbours)
                {
                    if (Clist.TryGetValue(data.ChunkPos+ neighborpos,out var Neighbor))
                    {
                        Neighbor.State |= ChunkSate.Changed;
                        AddSectionToBeMesh(0,Neighbor.Sections[SecPos]);
                        if (SecPos < Const.ChunkHeight)
                        {
                            AddSectionToBeMesh(0,Neighbor.Sections[SecPos + 1]);
                        }
                        if (SecPos > 0)
                        {
                            AddSectionToBeMesh(0,Neighbor.Sections[SecPos - 1]);
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
            /*try
            {*/
                Chunk.Chunk CH = GetChunk(Rpos.X, Rpos.Y);
                if (CH != null)
                {
                    return CH.Sections[bpos.Y >> 4].BlockPalette.Get(new Vector3i(bpos.X,bpos.Y & 15 , bpos.Z));
                }
                return Client.TheClient.ModManager.BlockRegister.Air;
            /*}
            catch
            {
                //Logger.Error("What The fuck");
                return Client.TheClient.ModManager.BlockRegister.Air;
            }*/
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
        internal BlockState GetBlock(Vector3i BlockPos)
        {
            (Vector2i cpos, Vector3i bpos) = Coord.GetVoxelCoord(BlockPos.X, BlockPos.Y, BlockPos.Z);
            if (Clist.TryGetValue(cpos, out Chunk.Chunk ch))
            {
                return ch.GetBlock(bpos);
            }
            return Client.TheClient.ModManager.BlockRegister.Air;
        }
        internal void Update()
        {
            while (Section2BeOG.Count > 0)
            {
                if(Section2BeOG.TryDequeue(out var Sec2BeOG))
                {
                    GetChunk(Sec2BeOG.Section.X, Sec2BeOG.Section.Z).Sections[Sec2BeOG.Section.Y].SectionMesh.Upload(Sec2BeOG.OpaqueVertices, Sec2BeOG.OpaqueIndices);
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
