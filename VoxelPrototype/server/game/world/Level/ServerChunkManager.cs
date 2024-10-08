﻿/**
 * Chunk Manager sever side implementation
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using K4os.Compression.LZ4;
using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.network.packets;
using VoxelPrototype.server.game.entity;
using VoxelPrototype.server.game.world.Level.Chunk;
using VoxelPrototype.utils;

namespace VoxelPrototype.server.game.world.Level
{
    public class ServerChunkManager
    { 
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("ServerChunkManager");
        public Dictionary<Vector2i, Chunk.Chunk> LoadedChunks = new();
        public int LoadedChunkCount { get { return LoadedChunks.Count; } }
        public Dictionary<Vector2i, RegionFile> TempRegions = new();
        public List<ChunkData> ChunkToBeSend = new();

        public ServerChunkManager()
        {
            InitPackets();
        }
        public void InitPackets()
        {
            Server.TheServer.NetworkManager.RegisterHandler<OneBlockChangeDemand>(HandleBlockChange);
        }
        internal void Dispose()
        {
            foreach (Chunk.Chunk chunk in LoadedChunks.Values)
            {
                if (chunk.ServerState == ServerChunkSate.Dirty)
                {
                    SaveChunk(chunk);
                }
                LoadedChunks.Remove(chunk.Position);
            }
            LoadedChunks.Clear();
            foreach (RegionFile region in TempRegions.Values)
            {
                region.Save();
                region.Dispose();
            }
            TempRegions.Clear();
            ChunkToBeSend.Clear();
        }

        internal void SaveChunk(Chunk.Chunk Chunk)
        {
            Chunk.ServerState = ServerChunkSate.None;
            int RegionX = Chunk.X >> 5;
            int RehionZ = Chunk.Z >> 5;
            RegionFile Region;
            if (TempRegions.ContainsKey(new Vector2i(RegionX, RehionZ)))
            {
                Region = TempRegions[new Vector2i(RegionX, RehionZ)];
            }
            else
            {
                string path = Server.TheServer.World.path + "terrain/dim0/" + RegionX + "." + RehionZ + ".vpr";
                Region = new(path);
                TempRegions.Add(new Vector2i(RegionX, RehionZ), Region);
            }
            Region.SetChunk(Chunk.Position, Chunk.Serialize());
        }
        internal Chunk.Chunk LoadChunk(int X, int Z)
        {
            int RegionX = X >> 5;
            int RegionZ = Z >> 5;
            RegionFile Region;
            if (TempRegions.ContainsKey(new Vector2i(RegionX, RegionZ)))
            {
                Region = TempRegions[new Vector2i(RegionX, RegionZ)];
            }
            else
            {
                string path = Server.TheServer.World.path + "terrain/dim0/" + RegionX + "." + RegionZ + ".vpr";
                Region = new(path);
                TempRegions.Add(new Vector2i(RegionX, RegionZ), Region);
            }
            var d = Region.GetChunk(new Vector2i(X, Z));
            if (d != default(VBFCompound))
            {
                return new Chunk.Chunk().Deserialize(d);
            }
            return null;
        }
        internal Chunk.Chunk? LoadChunk(Vector2i Position)
        {
            int RegionX = Position.X >> 5;
            int RehionZ = Position.Y >> 5;
            RegionFile Region;
            if (TempRegions.ContainsKey(new Vector2i(RegionX, RehionZ)))
            {
                Region = TempRegions[new Vector2i(RegionX, RehionZ)];
            }
            else
            {
                string path = Server.TheServer.World.path + "terrain/dim0/" + RegionX + "." + RehionZ + ".vpr";
                Region = new(path);
                TempRegions.Add(new Vector2i(RegionX, RehionZ), Region);
            }
            VBFCompound d = Region.GetChunk(Position);
            if (d != null)
            {
                return new Chunk.Chunk().Deserialize(d);
            }
            return null;
        }

        internal Chunk.Chunk? GetChunk(Vector2i pos)
        {
            if (LoadedChunks.ContainsKey(pos))
            {
                return LoadedChunks[pos];
            }
            else
            {
                Chunk.Chunk LoadedChunk = LoadChunk(pos);
                if (LoadedChunk != null)
                {
                    LoadedChunks.Add(pos, LoadedChunk);
                }
                return LoadedChunk;
            }
        }
        internal Chunk.Chunk CreateChunk(Vector2i pos)
        {
            Chunk.Chunk tempChunk = Server.TheServer.World.WorldGenerator.GenerateChunk(pos);
            SaveChunk(tempChunk);
            LoadedChunks[pos] = tempChunk;
            return tempChunk;
        }
        internal void SendChunk()
        {
            ChunkToBeSend = ChunkToBeSend.OrderBy(o => o.importance).ToList();
            for (int i = 0; i < 100; i++)
            {
                if (ChunkToBeSend.Count > 0)
                {
                    var packet = ChunkToBeSend[0];
                    ChunkToBeSend.RemoveAt(0);
                    Server.TheServer.NetworkManager.SendPacket(packet.peer, packet, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        internal void CheckChunk(ServerPlayer play, int x, int z, Vector3i RelatPos)
        {
            Vector2i chunkPos = new Vector2i(x, z);
            Chunk.Chunk ch = GetChunk(chunkPos);
            if (ch != null)
            {
                ch.PlayerInChunk.Add(play.ClientID);
                ChunkData chunkData = new ChunkData();
                chunkData.importance = (int)Vector2.Distance(RelatPos.Xz, chunkPos);
                chunkData.Pos = chunkPos;
                chunkData.Data = LZ4Pickler.Pickle(VBFSerializer.Serialize(ch.Serialize()));
                chunkData.peer = Server.TheServer.NetworkManager.NetManager.GetPeerById(play.ClientID);
                ChunkToBeSend.Add(chunkData);
            }
            else
            {
                Chunk.Chunk CH = CreateChunk(chunkPos);
                CH.PlayerInChunk.Add(play.ClientID);
                ChunkData chunkData = new ChunkData();
                chunkData.importance = (int)Vector2.Distance(RelatPos.Xz, chunkPos);
                chunkData.Pos = chunkPos;
                chunkData.Data = LZ4Pickler.Pickle(VBFSerializer.Serialize(CH.Serialize()));
                chunkData.peer = Server.TheServer.NetworkManager.NetManager.GetPeerById(play.ClientID);
                ChunkToBeSend.Add(chunkData);
            }
            play.inChunk.Add(chunkPos);
        }
        internal void PlayerChunk(ServerPlayer play)
        {
            int minx = (int)(Math.Floor(play.Position.X / Const.ChunkSize) - Server.TheServer.World.LoadDistance);
            int minz = (int)(Math.Floor(play.Position.Z / Const.ChunkSize) - Server.TheServer.World.LoadDistance);
            int maxx = (int)(Math.Floor(play.Position.X / Const.ChunkSize) + Server.TheServer.World.LoadDistance);
            int maxz = (int)(Math.Floor(play.Position.Z / Const.ChunkSize) + Server.TheServer.World.LoadDistance);
            List<Vector2i> list = new List<Vector2i>();
            foreach (Vector2i ch in play.inChunk)
            {
                Chunk.Chunk chs = GetChunk(ch);
                if (chs != null)
                {
                    if (ch.X < minx || ch.Y < minz || ch.X > maxx || ch.Y > maxz)
                    {
                        chs.PlayerInChunk.Remove(play.ClientID);
                        list.Add(ch);
                    }
                }
                else
                {
                    list.Add(ch);
                }
            }
            UnloadChunk packet = new() { Positions = list.ToArray() };
            Server.TheServer.NetworkManager.SendPacket(Server.TheServer.NetworkManager.NetManager.GetPeerById(play.ClientID), packet, DeliveryMethod.ReliableOrdered);
            foreach (Vector2i ch in list)
            {
                play.inChunk.Remove(ch);
            }
            var RelatPos = new Vector3i((int)(play.Position.X / Const.ChunkSize), (int)(play.Position.Y / Const.ChunkSize), (int)(play.Position.Z / Const.ChunkSize));
            for (int x = minx; x <= maxx; x++)
            {
                for (int z = minz; z <= maxz; z++)
                {
                    if (!play.inChunk.Contains(new Vector2i(x, z)))
                    {
                        CheckChunk(play, x, z, RelatPos);
                    }
                }

            }
        }
        //[Time]
        internal void Update()
        {
            foreach (RegionFile Region in TempRegions.Values)
            {
                Region.Save();
                Region.Dispose();
            }
            TempRegions.Clear();
            foreach (Chunk.Chunk chunk in LoadedChunks.Values)
            {
                if (chunk.PlayerInChunk.Count == 0)
                {
                    if (chunk.ServerState == ServerChunkSate.Dirty)
                    {
                        SaveChunk(chunk);
                    }
                    LoadedChunks.Remove(chunk.Position);
                }
            }
            foreach (ServerPlayer play in Server.TheServer.World.PlayerFactory.List.Values)
            {
                PlayerChunk(play);
            }
            SendChunk();
        }
        internal BlockState GetBlock(int x, int y, int z)
        {
            Chunk.Chunk ch = GetChunk(Coord.GetChunkCoord(new Vector2i(x, z)));
            if (ch != null)
            {
                return ch.GetBlock(Coord.GetBlockLocalCoord(new Vector3i(x, y, z)));
            }
            else
            {
                return BlockRegistry.GetInstance().Air;
            }
        }
        internal void SetBlock(int x, int y, int z, BlockState State)
        {
            Chunk.Chunk ch = GetChunk(Coord.GetChunkCoord(new Vector2i(x, z)));
            if (ch != null)
            {
                ch.SetBlock(Coord.GetBlockLocalCoord(new Vector3i(x, y, z)), State);
            }
        }
        internal void ChangeBlock(Vector2i cp, Vector3i bp, BlockState State)
        {
            Chunk.Chunk tempChunk = GetChunk(cp);
            if (tempChunk != null)
            {
                tempChunk = CreateChunk(cp);
            }
            tempChunk.SetBlock(bp, State);
            OneBlockChange packet = new OneBlockChange
            {
                ChunkPos = cp,
                BlockPos = bp,
                State = State
            };
            foreach (ushort ClientID in tempChunk.PlayerInChunk)
            {
                Server.TheServer.NetworkManager.SendPacket(Server.TheServer.NetworkManager.NetManager.GetPeerById(ClientID), packet, DeliveryMethod.ReliableOrdered);
            }
        }
        internal void HandleBlockChange(NetPeer peer, OneBlockChangeDemand data)
        {
            Chunk.Chunk tempChunk = GetChunk(data.ChunkPos);
            if (tempChunk != null)
            {
                tempChunk.SetBlock(data.BlockPos, data.State);
                OneBlockChange packet = new OneBlockChange
                {
                    ChunkPos = data.ChunkPos,
                    BlockPos = data.BlockPos,
                    State = data.State
                };
                foreach (ushort ClientID in tempChunk.PlayerInChunk)
                {
                    Server.TheServer.NetworkManager.SendPacket(Server.TheServer.NetworkManager.NetManager.GetPeerById(ClientID), packet, DeliveryMethod.ReliableOrdered);
                }
            }
            else
            {
                tempChunk = CreateChunk(data.ChunkPos);
                tempChunk.SetBlock(data.BlockPos, data.State);
            }
        }
    }
}
