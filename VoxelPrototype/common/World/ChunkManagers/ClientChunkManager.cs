/**
 * ChunkManager for client side
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.client;
using VoxelPrototype.client.Render.World;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;
namespace VoxelPrototype.common.World.ChunkManagers
{
    public partial class ClientChunkManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal ConcurrentDictionary<Vector2i, Chunk> Clist = new();
        internal Dictionary<Vector3i, int> Breaking = new();
        internal void Dispose()
        {
            Clist.Clear();
            Breaking.Clear();
        }

        internal void Update()
        {
        }
        internal void AddChunk(Chunk chunk)
        {
            bool suc = false;
            while (!suc)
            {
                lock (Clist)
                {
                    Clist[chunk.Position]=  chunk;
                    suc = true;
                }
            }
        }
        internal void HandleChunk(ChunkData data, NetPeer peer)
        {
            Vector2i pos = data.Pos;
            if (!Clist.TryGetValue(pos, out Chunk _))
            {
                AddChunk(new Chunk().Deserialize(Deflate.Decompress(data.Data)));
                Clist[pos].State |= ChunkSate.Changed;
                foreach (Section section in Clist[pos].Sections)
                {
                    Client.TheClient.World.WorldRenderer.GenerateSection(section);
                }
            }
        }
        internal void HandleChunkUnload(UnloadChunk data, NetPeer peer)
        {
            foreach (Vector2i ChunkPosition in data.Positions)
            {
                if (Clist.TryGetValue(ChunkPosition, out Chunk ch))
                {
                    foreach(Section section in ch.Sections)
                    {
                        Client.TheClient.World.WorldRenderer.DestroySection(section);
                    }
                    ch = null;
                    Clist.Remove(ChunkPosition, out Chunk _);
                }
            }
        }
        internal void HandleChunkUpdate(OneBlockChange data, NetPeer peer)
        {
            Vector2i cpos = data.ChunkPos;
            Vector3i bpos = data.BlockPos;
            if (Clist.TryGetValue(cpos, out Chunk ch))
            {
                ch.SetBlock(bpos, data.State);
            }
            //Neighboors
            int SecPos = bpos.Y / 16 ;
            foreach (Vector2i neighborpos in CubeNeighbours.XZNeighbours)
            {
                if(Clist.ContainsKey(neighborpos + cpos))
                {
                    Chunk Neighbor = Clist[neighborpos+ cpos];
                    Neighbor.State |= ChunkSate.Changed;
                    for (int i = -1; i <= 1; i++)
                    {
                        Section sec = ch.GetSectionByIndex(SecPos + i);
                        if(sec != null) 
                        {
                            Client.TheClient.World.WorldRenderer.GenerateSection(sec);
                        }
                    }
                }
            }
        }
        internal BlockState GetBlockForMesh(Vector3i bpos, Vector2i cpos)
        {
            Vector2i Rpos = cpos;
            if (bpos.X > Chunk.Size - 1)
            {
                Rpos.X += 1;
                bpos.X = 0;
            }
            else if (bpos.X < 0)
            {
                Rpos.X -= 1;
                bpos.X = Chunk.Size - 1;
            }
            if (bpos.Z > Chunk.Size - 1)
            {
                Rpos.Y += 1;
                bpos.Z = 0;
            }
            else if (bpos.Z < 0)
            {
                Rpos.Y -= 1;
                bpos.Z = Chunk.Size - 1;
            }
            try
            {
                Chunk CH = GetChunk(Rpos.X, Rpos.Y);
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

        internal Chunk GetChunk(int x, int z)
        {
            //bool suc = false;
            //while (!suc)
            //{
            //lock (Clist)
            //{
            Vector2i Pos = new Vector2i(x, z);
            if (Clist.ContainsKey(Pos))
            {
                return Clist[Pos];
            }
            return null;
                    
                    //suc = true;
                //}
            //}
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
            Chunk chunk = null;
            while (!suc)
            {
                lock (Clist)
                {
                    (Vector2i cpos, Vector3i bpos) = Coord.GetVoxelCoord(x, y, z);
                    if (Clist.TryGetValue(cpos, out Chunk ch))
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
    }
}
