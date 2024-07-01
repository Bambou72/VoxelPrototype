using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Entities.Player.PlayerManager;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;
using VoxelPrototype.common.WorldGenerator;
using VoxelPrototype.server.World.Level;
using VoxelPrototype.server.World.Level.Chunk;
using VoxelPrototype.VBF;

namespace VoxelPrototype.server.World
{
    public class World : IWorld
    {
        internal Random RNG;
        //param
        internal string Name;
        internal long Seed;
        //Distance
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;
        internal string path;
        //Voxel
        internal WorldGenerator WorldGenerator;
        public ServerChunkManager ChunkManager;
        //Players
        public ServerPlayerFactory PlayerFactory;
        public ModManager ModManager;

        public World(ModManager ModManager, string path, WorldSettings Settings =null)
        {
            this.ModManager = ModManager;
            this.path = path;
            if(path == null )
            {
                path = "";
            }
            if(Settings != null)
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + "/terrain");
                Directory.CreateDirectory(path + "/terrain/dim0");
                Seed= Settings.GetSeed();
                WorldGenerator = ModManager.WorldGeneratorRegistry.CreateWorldGenerator(Settings.GetGenerator());
                File.WriteAllBytes(path + "world.vpw", SerializeWorldData());
            }
            else
            {
                DeserializeWorldData(File.ReadAllBytes(path + "world.vpw"));
            }
            WorldGenerator.SetData(Seed);
            ChunkManager = new ServerChunkManager();
            PlayerFactory = new ServerPlayerFactory();

        }
        public byte[] SerializeWorldData()
        {
            VBFCompound root  = new VBFCompound();
            root.AddLong("Seed", Seed);
            root.AddString("Generator", WorldGenerator.Name);
            return VBFSerializer.Serialize(root);
        }
        public void DeserializeWorldData(byte[] data)
        {
            VBFCompound root = (VBFCompound)VBFSerializer.Deserialize(data);
            Seed =  root.GetLong("Seed").Value;
            WorldGenerator =  client.Client.TheClient.ModManager.WorldGeneratorRegistry.CreateWorldGenerator(root.GetString("Generator").Value);
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
            return ChunkManager.GetBlock(x, y, z) == ModManager.BlockRegister.Air;
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
