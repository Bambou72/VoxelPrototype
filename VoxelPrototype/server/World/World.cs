using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Entities.Player.PlayerManager;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;
using VoxelPrototype.common.WorldGenerator;

namespace VoxelPrototype.server.World
{
    public class World : IBlockAcessor, ITickable
    {
        internal Random RNG;
        //WorldInfo
        internal WorldInfo WorldInfo;
        //Distance
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;

        string Path;
        //Voxel
        internal WorldGenerator WorldGenerator;
        public ServerChunkManager ChunkManager;
        //Players
        public ServerPlayerFactory PlayerFactory;

        public World(WorldSettings Settings, string Path)
        {
            this.Path = Path;
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory("worlds/" + Settings.GetName());
                Directory.CreateDirectory("worlds/" + Settings.GetName() + "/terrain");
                Directory.CreateDirectory("worlds/" + Settings.GetName() + "/terrain/dim0");
                WorldInfo = new WorldInfo();
                WorldInfo.Path = Path;
                WorldInfo.SetSeed(Settings.GetSeed());
                WorldInfo.SetWorldGenerator(Settings.GetGenerator());
                using var fs = new FileStream(Path + "world.vpw", FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(WorldInfo.Serialize());
                fs.Close();
            }
            else
            {
                WorldInfo = new WorldInfo().Deserialize(File.ReadAllBytes(Path + "world.vpw"));
                WorldInfo.Path = Path;
            }
            WorldGenerator = Client.TheClient.ModManager.WorldGeneratorRegistry.CreateWorldGenerator(WorldInfo.GetWorldGenerator());
            WorldGenerator.SetData(WorldInfo.GetSeed());
            ChunkManager = new ServerChunkManager();
            PlayerFactory = new ServerPlayerFactory();

        }
        public void Dispose()
        {
            ChunkManager.Dispose();
            PlayerFactory.Dispose();
            using var fs = new FileStream(Path + "world.vpw", FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(WorldInfo.Serialize());
        }
        public void Tick(float DT)
        {
            CurrentTick++;
            ChunkManager.Update();
            PlayerFactory.Update();
            PlayerFactory.SendData();
            if (CurrentTick % 600 == 0)
            {
                foreach (Chunk chunk in ChunkManager.LoadedChunks.Values)
                {
                    if (chunk.ServerState == ServerChunkSate.Dirty)
                    {
                        ChunkManager.SaveChunk(chunk);
                    }
                }
            }
        }
        public BlockState GetBlock(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z);
        }
        public bool IsAir(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z) == Client.TheClient.ModManager.BlockRegister.Air;
        }
        public bool IsTransparent(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z).Block.Transparent;
        }
        public bool IsChunkExist(int x, int z)
        {
            return ChunkManager.GetChunk(new Vector2i(x, z)) != null;
        }
        public Chunk GetChunk(int x, int z)
        {
            return ChunkManager.GetChunk(new Vector2i(x, z));
        }
        public void SetBlock(int x, int y, int z, BlockState State)
        {
            ChunkManager.SetBlock(x, y, z, State);
        }
    }

}
