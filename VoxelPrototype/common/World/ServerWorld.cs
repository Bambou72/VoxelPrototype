using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.client;
using VoxelPrototype.common.Entities.Player.PlayerManager;
using VoxelPrototype.common.World.ChunkManagers;

namespace VoxelPrototype.common.World
{
    public class ServerWorld : World
    {
        string Path;
        //Voxel
        internal WorldGenerator WorldGenerator;
        public ServerChunkManager ChunkManager;
        //Players
        public ServerPlayerFactory PlayerFactory;

        public ServerWorld(WorldSettings Settings, string Path)
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
        public override void Dispose()
        {
            ChunkManager.Dispose();
            PlayerFactory.Dispose();
            using var fs = new FileStream(Path + "world.vpw", FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(WorldInfo.Serialize());
        }
        public override void Tick(float DT)
        {
            base.Tick(DT);
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
        public override BlockState GetBlock(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z);
        }
        public override bool IsAir(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z) == Client.TheClient.ModManager.BlockRegister.Air;
        }
        public override bool IsTransparent(int x, int y, int z)
        {
            return ChunkManager.GetBlock(x, y, z).Block.Transparency;
        }
        public override bool IsChunkExist(int x, int z)
        {
            return ChunkManager.GetChunk(new Vector2i(x, z)) != null;
        }
        public override Chunk GetChunk(int x, int z)
        {
            return ChunkManager.GetChunk(new Vector2i(x, z));
        }
        public override void SetBlock(int x, int y, int z, BlockState State)
        {
            ChunkManager.SetBlock(x, y, z, State);
        }
    }
}
