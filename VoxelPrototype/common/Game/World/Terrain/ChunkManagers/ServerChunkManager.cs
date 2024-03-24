/**
 * Chunk Manager sever side implementation
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using K4os.Compression.LZ4;
using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.API.Blocks.state;
using VoxelPrototype.common.Game.Entities.Player;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.Utils;
using VoxelPrototype.server;
namespace VoxelPrototype.common.Game.World.Terrain.ChunkManagers
{
    public partial class ServerChunkManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal Dictionary<Vector2i, Chunk> LoadedChunks = new();
        internal List<ChunkData> ChunkToBeSend = new();
        internal void Dispose()
        {
            foreach (Chunk chunk in LoadedChunks.Values)
            {
                SaveChunk(chunk);
                LoadedChunks.Remove(chunk.Position);
            }
        }
        internal void SaveChunk(Chunk Chunk)
        {
            int RegionX = Chunk.X >> 5;
            int RehionZ = Chunk.Z >> 5;
            string path = Server.TheServer.World.WorldInfo.Path + "terrain/dim0/" + RegionX + "." + RehionZ + ".vpr";
            RegionFile Region = new(path);
            int LocalChunkX = Math.Abs(Chunk.X % RegionFile.Size);
            int LocalChunkZ = Math.Abs(Chunk.Z % RegionFile.Size);
            byte[] SerializedChunk = Chunk.Serialize();
            byte[] CompressedChunk = Deflate.Compress(SerializedChunk);
            Region.WriteChunk(LocalChunkX, LocalChunkZ,CompressedChunk ,CompressionType.Deflate);
            Region.Close();
        }
        internal Chunk LoadChunk(int X,int Z)
        {
            int RegionX = X >> 5;
            int RehionZ = Z >> 5;
            string path = Server.TheServer.World.WorldInfo.Path + "terrain/dim0/" + RegionX + "." + RehionZ + ".vpr";
            RegionFile Region = new(path);
            int LocalChunkX = Math.Abs(X % RegionFile.Size);
            int LocalChunkZ = Math.Abs(Z % RegionFile.Size);
            (byte[] CompressedData,CompressionType CompressionType) =  Region.ReadChunk(LocalChunkX, LocalChunkZ);
            Region.Close();
            if (CompressedData.Length != 0)
            {
                byte[] ChunkData;
                switch (CompressionType)
                {

                    case CompressionType.Deflate:
                        ChunkData = Deflate.Decompress(CompressedData);
                        break;
                    case CompressionType.LZ4:
                        ChunkData = LZ4Pickler.Unpickle(CompressedData);
                        break;
                    default:
                        ChunkData = CompressedData;
                        break;
                }
                return new Chunk().Deserialize(ChunkData);
            }
            return null;

        }
        internal Chunk? LoadChunk(Vector2i Position)
        {
            int RegionX = Position.X >> 5;
            int RehionZ = Position.Y >> 5;
            string path  = Server.TheServer.World.WorldInfo.Path + "terrain/dim0/" +RegionX + "." + RehionZ + ".vpr";
            RegionFile Region = new(path);
            int LocalChunkX = Math.Abs(Position.X % RegionFile.Size);
            int LocalChunkZ = Math.Abs(Position.Y % RegionFile.Size );
            (byte[] CompressedData, CompressionType CompressionType) = Region.ReadChunk(LocalChunkX, LocalChunkZ);
            Region.Close();
            if(CompressedData.Length != 0)
            {
                byte[] ChunkData;
                switch (CompressionType)
                {

                    case CompressionType.Deflate:
                        ChunkData = Deflate.Decompress(CompressedData);
                        break;
                    case CompressionType.LZ4:
                        ChunkData = LZ4Pickler.Unpickle(CompressedData);
                        break;
                    default:
                        ChunkData = CompressedData;
                        break;
                }
                return new Chunk().Deserialize(ChunkData);
            }
            return null;
        }

        internal Chunk? GetChunk(Vector2i pos)
        {
            if(LoadedChunks.ContainsKey(pos))
            {
                return LoadedChunks[pos];
            }else
            {
                Chunk LoadedChunk = LoadChunk(pos);
                if(LoadedChunk != null)
                {
                    LoadedChunks.Add(pos, LoadedChunk);
                }
                return LoadedChunk;
            }
        }
        internal Chunk CreateChunk(Vector2i pos)
        {
            Chunk tempChunk = new Chunk(pos, true);
            SaveChunk(tempChunk);
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
                    ServerNetwork.SendPacket(packet, packet.peer, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        internal void CheckChunk(Player play, int x, int z, Vector3i RelatPos)
        {
            Vector2i chunkPos = new Vector2i(x, z);
            Chunk ch = GetChunk(chunkPos);
            if (ch != null)
            {
                ch.PlayerInChunk.Add(play.ClientID);
                ChunkData chunkData = new ChunkData();
                chunkData.importance = (int)Vector2.Distance(RelatPos.Xz, chunkPos);
                chunkData.Pos = chunkPos;
                chunkData.Data = Deflate.Compress(ch.Serialize());
                chunkData.peer = ServerNetwork.server.GetPeerById(play.ClientID);
                ChunkToBeSend.Add(chunkData);
            }
            else
            {
                Chunk CH = CreateChunk(chunkPos);
                ChunkData chunkData = new ChunkData();
                chunkData.importance = (int)Vector2.Distance(RelatPos.Xz, chunkPos);
                chunkData.Pos = chunkPos;
                chunkData.Data = Deflate.Compress(CH.Serialize());
                chunkData.peer = ServerNetwork.server.GetPeerById(play.ClientID);
                ChunkToBeSend.Add(chunkData);
            }
            play.inChunk.Add(chunkPos);
        }
        internal void PlayerChunk(Player play)
        {
            int minx = (int)(Math.Floor(play.Position.X / Chunk.Size) - server.Server.TheServer.World.LoadDistance);
            int minz = (int)(Math.Floor(play.Position.Z / Chunk.Size) - server.Server.TheServer.World.LoadDistance);
            int maxx = (int)(Math.Floor(play.Position.X / Chunk.Size) + server.Server.TheServer.World.LoadDistance);
            int maxz = (int)(Math.Floor(play.Position.Z / Chunk.Size) + server.Server.TheServer.World.LoadDistance);
            List<Vector2i> list = new List<Vector2i>();
            foreach (Vector2i ch in play.inChunk)
            {
                Chunk chs = GetChunk(ch);
                if (chs != null)
                {
                    if (ch.X < minx || ch.Y < minz  || ch.X > maxx || ch.Y > maxz )
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
            ServerNetwork.SendPacket(packet, ServerNetwork.server.GetPeerById(play.ClientID), DeliveryMethod.ReliableOrdered);
            foreach (Vector2i ch in list)
            {
                play.inChunk.Remove(ch);
            }
            var RelatPos = new Vector3i((int)(play.Position.X / Chunk.Size), (int)(play.Position.Y / Chunk.Size), (int)(play.Position.Z / Chunk.Size));
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
            foreach (Player play in server.Server.TheServer.World.PlayerFactory.List.Values)
            {
                PlayerChunk(play);
            }
            SendChunk();
        }
        internal BlockState GetBlock(int x, int y, int z)
        {
            (Vector2i cpos, Vector3i bpos) = Coord.GetVoxelCoord(x, y, z);
            Chunk ch = GetChunk(cpos);
            if (ch != null)
            {
                return ch.GetBlock(new Vector3i(bpos.X, bpos.Y, bpos.Z));
            }
            else
            {
                return BlockRegister.Air;
            }
        }
        internal void ChangeBlock(Vector2i cp, Vector3i bp, BlockState State)
        {
            Chunk tempChunk = GetChunk(cp);
            if(tempChunk != null)
            {
                tempChunk = CreateChunk(cp);
            }
            tempChunk.SetBlock(bp, State);
            OneBlockChange packet = new OneBlockChange
            {
                ChunkPos = cp,
                BlockPos = bp,
                BlockID = State.GetBlock().Id
            };
            foreach (ushort ClientID in tempChunk.PlayerInChunk)
            {
                ServerNetwork.SendPacket(packet, ServerNetwork.server.GetPeerById(ClientID), DeliveryMethod.ReliableOrdered);
            }
        }
        internal void HandleBlockChange(OneBlockChangeDemand data, NetPeer peer)
        {
            Chunk tempChunk = GetChunk(data.ChunkPos);
            if (tempChunk != null)
            {
                tempChunk.SetBlock(data.BlockPos, BlockRegister.GetBlock(data.BlockID).GetDefaultState());
                OneBlockChange packet = new OneBlockChange
                {
                    ChunkPos = data.ChunkPos,
                    BlockPos = data.BlockPos,
                    BlockID = data.BlockID
                };
                foreach (ushort ClientID in tempChunk.PlayerInChunk)
                {
                    ServerNetwork.SendPacket(packet, ServerNetwork.server.GetPeerById(ClientID), DeliveryMethod.ReliableOrdered);
                }
            }
            else
            {
                tempChunk = CreateChunk(data.ChunkPos);
                tempChunk.SetBlock(data.BlockPos, BlockRegister.GetBlock(data.BlockID).GetDefaultState());
            }
        }
    }
}
