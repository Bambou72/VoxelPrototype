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
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(Clist[pos], data.importance);
            }
        }
        internal void HandleChunkUnload(UnloadChunk data, NetPeer peer)
        {
            foreach (Vector2i ChunkPosition in data.Positions)
            {
                if (Clist.TryGetValue(ChunkPosition, out Chunk ch))
                {
                    Client.TheClient.World.WorldRenderer.RemoveChunkMesh(ch.Position);
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
                ch.State |= ChunkSate.Changed;
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(ch, 0);
            }
            if (Clist.TryGetValue(new Vector2i(cpos.X + 1, cpos.Y), out Chunk ch1))
            {
                ch1.State |= ChunkSate.Changed;
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(ch1, 0);
            }
            if (Clist.TryGetValue(new Vector2i(cpos.X - 1, cpos.Y), out Chunk ch2))
            {
                ch2.State |= ChunkSate.Changed;
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(ch2, 0);
            }
            if (Clist.TryGetValue(new Vector2i(cpos.X, cpos.Y - 1), out Chunk ch3))
            {
                ch3.State |= ChunkSate.Changed;
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(ch3, 0);
            }
            if (Clist.TryGetValue(new Vector2i(cpos.X, cpos.Y + 1), out Chunk ch4))
            {
                ch4.State |= ChunkSate.Changed;
                Client.TheClient.World.WorldRenderer.AddChunkToBeMesh(ch4, 0);
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
