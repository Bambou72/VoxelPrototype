/* ChunkManager for client side
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using K4os.Compression.LZ4;
using VoxelPrototype.utils;
using VoxelPrototype.network.packets;
using VoxelPrototype.client.game.world.Level.Chunk;
using VoxelPrototype.client.game.world.Level.Chunk.Render;
using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;

namespace VoxelPrototype.client.game.world.Level
{
    public class ClientChunkManager
    {
        internal World World;
        static string ChunkShaderResourceID = "voxelprototype:shaders/chunk";
        static string BlockAtlasResourceID = "voxelprototype:textures/block/atlas";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("ClientChunkManager");
        internal ReaderWriterLockSlim ChunkByCoordinateLock = new ReaderWriterLockSlim();
        internal Dictionary<Vector2i, Chunk.Chunk> ChunkByCoordinate = new();
        internal List<Chunk.Chunk> AllChunks = new();
        internal ConcurrentQueue<SectionToOG> Section2BeOG = new();
        internal Dictionary<Vector3i, int> Breaking = new();
        internal int RenderedChunksCount = 0;
        internal MeshingThread MeshingThread;
        public ClientChunkManager(World world)
        {
            InitPackets();
            World = world;
            MeshingThread = new();
            MeshingThread.Start();

        }
        public void InitPackets()
        {
            Client.TheClient.NetworkManager.RegisterHandler<ChunkData>(HandleChunk);
            Client.TheClient.NetworkManager.RegisterHandler<OneBlockChange>(HandleChunkUpdate);
            Client.TheClient.NetworkManager.RegisterHandler<UnloadChunk>(HandleChunkUnload);
        }
        public bool IsChunkExist(Vector2i Pos, bool IsLocking = false)
        {
            return ChunkByCoordinate.ContainsKey(Pos);
        }
        public bool IsChunkSurrended(Vector2i ChunkPos, bool IsLocking = false)
        {
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X, ChunkPos.Y + 1), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X, ChunkPos.Y - 1), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y - 1), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y - 1), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y + 1), IsLocking)) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y + 1), IsLocking)) return false;
            return true;
        }

        internal void Dispose()
        {
            MeshingThread.Stop();
            while (!MeshingThread.Stopped) ;
            MeshingThread.Clear();
            ChunkByCoordinate.Clear();
            AllChunks.Clear();
            Section2BeOG.Clear();
            Breaking.Clear();
        }

        internal void AddChunk(Chunk.Chunk chunk)
        {
            ChunkByCoordinate[chunk.Position] = chunk;
            AllChunks.Add(chunk);
        }
        internal void HandleChunk(NetPeer peer, ChunkData data)
        {
            Vector2i pos = data.Pos;
            var CH = new Chunk.Chunk().Deserialize((VBFCompound)VBFSerializer.Deserialize(LZ4Pickler.Unpickle(data.Data)));
            AddChunk(CH);
        }
        internal void HandleChunkUnload(NetPeer peer, UnloadChunk data)
        {
            //ChunkByCoordinateLock.EnterWriteLock();
            try
            {
                foreach (Vector2i ChunkPosition in data.Positions)
                {


                    if (ChunkByCoordinate.Remove(ChunkPosition, out var ch))
                    {
                        AllChunks.Remove(ch);
                        ch.Dispose();
                        ch = null;
                    }
                }
            }
            finally
            {
                // ChunkByCoordinateLock.ExitWriteLock();
            }
        }
        internal void AddSectionToBeMesh(ushort importance, Section sect)
        {
            sect.MeshState = MeshState.Generating;
            MeshingThread.AddSectionToBeMesh(new SectionToMeshing()
            {
                Importance = importance,
                Pos = new Vector3i(sect.ParentChunk.X, sect.Y, sect.ParentChunk.Z)
            });
        }
        internal void AddSectionToOG(SectionToOG section)
        {

            Section2BeOG.Enqueue(section);

        }
        internal void HandleChunkUpdate(NetPeer peer, OneBlockChange data)
        {
            //ChunkByCoordinateLock.EnterWriteLock();
            try
            {
                if (ChunkByCoordinate.TryGetValue(data.ChunkPos, out Chunk.Chunk ch))
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
                    if (data.BlockPos.X == Const.ChunkSize - 1 || data.BlockPos.X == 0 || data.BlockPos.Z == Const.ChunkSize - 1 || data.BlockPos.Z == 0)
                    {
                        //Neighboors
                        foreach (Vector2i neighborpos in CubeNeighbours.XZNeighbours)
                        {
                            if (ChunkByCoordinate.TryGetValue(data.ChunkPos + neighborpos, out var Neighbor))
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
            finally
            {
                //ChunkByCoordinateLock.ExitWriteLock();

            }
        }
        internal Chunk.Chunk GetChunk(Vector2i Pos)
        {
            if (ChunkByCoordinate.TryGetValue(Pos, out var chunk))
            {
                return chunk;
            }
            return null;
        }
        public void ChangeChunk(Vector3i blockp, BlockState State)
        {
            SetBlock(blockp, State);
            Vector2i cpos = Coord.GetChunkCoord(blockp.Xz);
            Vector3i bpos = Coord.GetBlockLocalCoord(blockp);
            OneBlockChangeDemand packet = new OneBlockChangeDemand
            {
                State = State,
                ChunkPos = cpos,
                BlockPos = bpos,
            };
            Client.TheClient.NetworkManager.SendPacketToServer(packet, DeliveryMethod.ReliableOrdered);
        }
        internal void SetBlock(Vector3i BlockPos, BlockState BlockState)
        {
            //ChunkByCoordinateLock.EnterWriteLock();
            if (BlockPos.Y > 0 && BlockPos.Y < Const.ChunkRHeight)
            {
                Chunk.Chunk CH = GetChunk(Coord.GetChunkCoord(BlockPos.Xz));
                if (CH != null)
                {
                    var bpos = Coord.GetBlockLocalCoord(BlockPos);
                    bpos.Y &= Const.And;
                    CH.Sections[BlockPos.Y >> Const.BitShifting].SetBlock(bpos, BlockState);
                    CH.Sections[BlockPos.Y >> Const.BitShifting].MeshState = MeshState.Dirty;
                }
            }
            //ChunkByCoordinateLock.ExitWriteLock();
        }
        internal BlockState GetBlock(Vector3i BlockPos)
        {
            {
                if (BlockPos.Y > 0 && BlockPos.Y < Const.ChunkRHeight)
                {
                    Chunk.Chunk CH = GetChunk(Coord.GetChunkCoord(BlockPos.Xz));
                    if (CH != null)
                    {
                        var bpos = Coord.GetBlockLocalCoord(BlockPos);
                        bpos.Y &= Const.And;
                        return CH.Sections[BlockPos.Y >> Const.BitShifting].BlockPalette.Get(bpos);
                    }
                }
                return BlockRegistry.GetInstance().Air;
            }
        }

        internal void Update()
        {
            while (Section2BeOG.Count > 0)
            {

                if (Section2BeOG.TryDequeue(out var Sec2BeOG))
                {
                    GetChunk(Sec2BeOG.Section.Xz).Sections[Sec2BeOG.Section.Y].SectionMesh.Upload(Sec2BeOG.OpaqueVertices, Sec2BeOG.OpaqueIndices);
                    GetChunk(Sec2BeOG.Section.Xz).Sections[Sec2BeOG.Section.Y].MeshState = MeshState.Ready;
                }
            }
        }
        internal void Render()
        {
            RenderedChunksCount = 0;
            var Shader = Client.TheClient.ShaderManager.GetShader(ChunkShaderResourceID);
            var Camera = Client.TheClient.World.GetLocalPlayerCamera();
            Camera.Update();
            Shader.SetMatrix4("view", Camera.GetViewMatrix());
            Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            Client.TheClient.TextureManager.GetTexture(BlockAtlasResourceID).Use(TextureUnit.Texture0);
            Vector3d PlayPos = Client.TheClient.World.PlayerFactory.LocalPlayer.Position;
            int minx = (int)(PlayPos.X / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int minz = (int)(PlayPos.Z / Const.ChunkSize) - Client.TheClient.World.RenderDistance;
            int maxx = (int)(PlayPos.X / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            int maxz = (int)(PlayPos.Z / Const.ChunkSize) + Client.TheClient.World.RenderDistance;
            Shader.Use();
            foreach (var chunk in AllChunks)
            {
                if (chunk.X >= minx && chunk.Z >= minz && chunk.X <= maxx && chunk.Z <= maxz)
                {
                    ushort Distance = (ushort)Vector2d.Distance(chunk.Position, PlayPos.Xz);
                    bool Surrended;
                    {
                        Surrended = IsChunkSurrended(chunk.Position, true);
                    }
                    foreach (var section in chunk.Sections)
                    {
                        bool IsSecInFrust;
                        {
                            IsSecInFrust = Camera.Frustum.IsSectionInFrustum(section);
                        }
                        if (!section.Empty && IsSecInFrust)
                        {
                            if (section.MeshState == MeshState.Dirty && Surrended)
                            {
                                AddSectionToBeMesh(Distance, section);
                            }
                            if (section.SectionMesh.GetOpaqueMesh().GetVerticesCount() != 0)
                            {
                                {
                                    RenderedChunksCount++;
                                    Matrix4 model = Matrix4.CreateTranslation(new Vector3(chunk.X * Const.ChunkSize, section.Y * Const.SectionSize, chunk.Z * Const.ChunkSize));
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
}
