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
using System.Linq;
using System.Collections.Generic;
using VoxelPrototype.client.World.Level.Chunk.Render;
using VoxelPrototype.common.Threading;
namespace VoxelPrototype.client.World.Level
{
    public class ClientChunkManager
    {
        internal World World;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal ConcurrentDictionary<Vector2i, Chunk.Chunk> Clist = new();
        ConcurrentQueue<SectionToOG> Section2BeOG = new();
        internal Dictionary<Vector3i, int> Breaking = new();
        internal int RenderedChunksCount = 0;
        internal MeshingThread MeshingThread;

        public ClientChunkManager(World world)
        {

            World = world;
            MeshingThread = new();
            MeshingThread.Start();

        }

        internal void Dispose()
        {
            MeshingThread.Stop();
            while (!MeshingThread.Stopped) ;
            MeshingThread.Clear();
            Clist.Clear();
            Section2BeOG.Clear();
            Breaking.Clear();
        }

        internal void AddChunk(Chunk.Chunk chunk)
        {
            Clist[chunk.Position] = chunk;
        }
        internal void HandleChunk(ChunkData data, NetPeer peer)
        {
            Vector2i pos = data.Pos;
            if (!Clist.TryGetValue(pos, out Chunk.Chunk _))
            {
                AddChunk(new Chunk.Chunk().Deserialize((VBFCompound)VBFSerializer.Deserialize(LZ4Pickler.Unpickle(data.Data))));
                Clist[pos].Manager = this;
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
            sect.MeshState = MeshState.Generating;
            MeshingThread.AddSectionToBeMesh(new SectionToMeshing()
            {
                Importance = importance,
                Data = new(sect),
                Pos = new Vector3i(sect.Chunk.X, sect.Y, sect.Chunk.Z)
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
                int SecPos = data.BlockPos.Y / Const.SectionSize;
                ch.Sections[SecPos].MeshState = MeshState.Dirty;
                
                if (SecPos < Const.ChunkHeight)
                {
                    ch.Sections[SecPos + 1].MeshState = MeshState.Dirty;
                }
                if (SecPos > 0)
                {
                    ch.Sections[SecPos - 1].MeshState = MeshState.Dirty;
                }
                if (data.BlockPos.X == Const.ChunkSize-1  || data.BlockPos.X ==0 || data.BlockPos.Z == Const.ChunkSize-1 ||data.BlockPos.Z ==0)
                {
                    //Neighboors
                    foreach (Vector2i neighborpos in CubeNeighbours.XZNeighbours)
                    {
                        if (Clist.TryGetValue(data.ChunkPos + neighborpos, out var Neighbor))
                        {
                            Neighbor.Sections[SecPos].MeshState = MeshState.Dirty;
                            if (SecPos < Const.ChunkHeight)
                            {
                                Neighbor.Sections[SecPos + 1].MeshState = MeshState.Dirty;
                            }
                            if (SecPos > 0)
                            {
                                Neighbor.Sections[SecPos - 1].MeshState = MeshState.Dirty;
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
            /*try
            {*/
                Chunk.Chunk CH = GetChunk(Rpos);
                if (CH != null)
                {
                    return CH.Sections[bpos.Y >> Const.BitShifting].BlockPalette.Get(new Vector3i(bpos.X,bpos.Y & Const.And , bpos.Z));
                }
                return Client.TheClient.ModManager.BlockRegister.Air;
            /*}
            catch
            {
                //Logger.Error("What The fuck");
                return Client.TheClient.ModManager.BlockRegister.Air;
            }*/
        }

        internal Chunk.Chunk GetChunk(Vector2i Pos)
        {
            if (Clist.ContainsKey(Pos))
            {
                return Clist[Pos];
            }
            return null;
        }
        public void ChangeChunk(Vector3i blockp, BlockState State)
        {
            Vector2i cpos = Coord.GetChunkCoord(blockp.X, blockp.Y, blockp.Z);
            Vector3i bpos = Coord.GetBlockLocalCoord(blockp.X, blockp.Y, blockp.Z);
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
            if(BlockPos.Y >0 && BlockPos.Y < Const.ChunkRHeight)
            {
                Chunk.Chunk CH = GetChunk(Coord.GetChunkCoord(BlockPos.X, BlockPos.Y, BlockPos.Z));
                if (CH != null)
                {
                    var bpos = Coord.GetBlockLocalCoord(BlockPos.X, BlockPos.Y, BlockPos.Z);
                    bpos.Y &= Const.And;
                    return CH.Sections[BlockPos.Y >> Const.BitShifting].BlockPalette.Get(bpos);
                }
            }
            return Client.TheClient.ModManager.BlockRegister.Air;            
        }
        internal BlockState GetBlock(Vector2i CPosition, Vector3i BlockPos)
        {
            Chunk.Chunk CH = GetChunk(CPosition);
            if (CH != null)
            {
                return CH.GetBlock(BlockPos);
            }
            return Client.TheClient.ModManager.BlockRegister.Air;
        }
        internal void Update()
        {
            while (Section2BeOG.Count > 0)
            {
                if(Section2BeOG.TryDequeue(out var Sec2BeOG))
                {
                    GetChunk(Sec2BeOG.Section.Xz).Sections[Sec2BeOG.Section.Y].SectionMesh.Upload(Sec2BeOG.OpaqueVertices, Sec2BeOG.OpaqueIndices);
                    GetChunk(Sec2BeOG.Section.Xz).Sections[Sec2BeOG.Section.Y].MeshState = MeshState.Ready;
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
            Vector3d PlayPos = Client.TheClient.World.PlayerFactory.LocalPlayer.Position;
            int minx = (int)(PlayPos.X / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int minz = (int)(PlayPos.Z / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int maxx = (int)(PlayPos.X / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            int maxz = (int)(PlayPos.Z / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            Shader.Use();
            foreach (var pos in Clist.Keys)
            {
                if (pos.X >= minx && pos.Y >= minz && pos.X <= maxx && pos.Y <= maxz)
                {
                    Chunk.Chunk ch = Clist[pos];
                    ushort Distance = (ushort)Vector2d.Distance(ch.Position, PlayPos.Xz);
                    bool Surrended =  World.IsChunkSurrended(ch.Position);
                    foreach (var section in ch.Sections)
                    {
                        if (!section.Empty && Camera.Frustum.IsSectionInFrustum(section.SectionMesh))
                        {
                            if (section.MeshState == MeshState.Dirty && Surrended)
                            {
                                AddSectionToBeMesh(Distance, section);
                            }
                            if (section.SectionMesh.GetOpaqueMesh().GetVerticesCount() != 0)
                            {
                                RenderedChunksCount++;
                                Matrix4 model = Matrix4.CreateTranslation(new Vector3(pos.X * Const.ChunkSize, section.Y * Const.SectionSize, pos.Y * Const.ChunkSize));
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
}
