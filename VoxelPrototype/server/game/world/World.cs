using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.game;
using VoxelPrototype.server.game.entity;
using VoxelPrototype.server.game.world.Level;
using VoxelPrototype.server.game.world.Level.Chunk;
using VoxelPrototype.VBF;

namespace VoxelPrototype.server.game.world
{
    public class World : IWorld
    {
        public Random RNG;
        //param
        public string Name;
        public long Seed;
        //Distance
        public int LoadDistance = 12;
        //Tick
        public ulong CurrentTick;
        public string path;
        //Voxel
        public WorldGenerator WorldGenerator;
        public ServerChunkManager ChunkManager;
        //Players
        public PlayerManager PlayerFactory;

        public World(string path, WorldSettings Settings = null)
        {
            this.path = path;
            if (path == null)
            {
                path = "";
            }
            if (Settings != null)
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + "/terrain");
                Directory.CreateDirectory(path + "/terrain/dim0");
                Seed = Settings.GetSeed();
                WorldGenerator = WorldGeneratorRegistry.GetInstance().CreateWorldGenerator(Settings.GetGenerator());
                File.WriteAllBytes(path + "world.vpw", SerializeWorldData());
            }
            else
            {
                DeserializeWorldData(File.ReadAllBytes(path + "world.vpw"));
            }
            WorldGenerator.SetData(Seed);
            ChunkManager = new ServerChunkManager();
            PlayerFactory = new PlayerManager();

        }
        public byte[] SerializeWorldData()
        {
            VBFCompound root = new VBFCompound();
            root.AddLong("Seed", Seed);
            root.AddString("Generator", WorldGenerator.Name);
            return VBFSerializer.Serialize(root);
        }
        public void DeserializeWorldData(byte[] data)
        {
            VBFCompound root = (VBFCompound)VBFSerializer.Deserialize(data);
            Seed = root.GetLong("Seed").Value;
            WorldGenerator = WorldGeneratorRegistry.GetInstance().CreateWorldGenerator(root.GetString("Generator").Value);
        }
        public void GetParam()
        {

        }
        public void Dispose()
        {
            ChunkManager.Dispose();
            PlayerFactory.Dispose();
            File.WriteAllBytes(path + "world.vpw", SerializeWorldData());
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
            return ChunkManager.GetBlock(x, y, z) == BlockRegistry.GetInstance().Air;
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

        public bool IsClient()
        {
            return false;
        }
    }

}
