using NLog.LayoutRenderers.Wrappers;
using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.client.game.entity;
using VoxelPrototype.client.game.world.Level;
using VoxelPrototype.client.game.world.Level.Chunk;
using VoxelPrototype.client.rendering.camera;
using VoxelPrototype.game;

namespace VoxelPrototype.client.game.world
{

    public class World : IWorld
    {
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;

        public ClientChunkManager ChunkManager;
        internal PlayerManager PlayerFactory;
        internal PlayersRenderer PlayerRenderer;
        internal bool Initialized = false;
        internal int RenderDistance = 6;
        internal ClientChat Chat;
        public World()
        {
        }

        public void Dispose()
        {
            Initialized = false;
            Chat.Dispose();
            Chat = null;
            ChunkManager.Dispose();
            ChunkManager = null;
            PlayerFactory.Dispose();
            PlayerFactory = null;
        }


        public void Init()
        {
            ChunkManager = new ClientChunkManager(this);
            PlayerFactory = new PlayerManager();
            PlayerRenderer = new PlayersRenderer();
            Chat = new();
            Initialized = true;
        }
        public void Tick(float DT)
        {
            CurrentTick++;
            {
                PlayerFactory.Update(DT);
            }
            if (PlayerFactory.LocalPlayerExist)
            {
                {
                    ChunkManager.Update();
                }
            }

        }


        public void Render()
        {
            if (PlayerFactory.LocalPlayerExist)
            {
                //SkyboxRender.RenderSkyBox(PlayerFactory.LocalPlayer._Camera.GetViewMatrix(), PlayerFactory.LocalPlayer._Camera.GetProjectionMatrix());
                ChunkManager.Render();
                PlayerRenderer.RenderPlayers();
            }
        }

        public Chunk GetChunk(Vector2i Pos)
        {
            return ChunkManager.GetChunk(Pos);
        }

        public int GetChunkCount()
        {
            return ChunkManager.AllChunks.Count;
        }
        public bool IsLocalPlayerExist()
        {
            if (Initialized == false)
            {
                return false;
            }
            return PlayerFactory.LocalPlayerExist;
        }
        public FrustumCamera GetLocalPlayerCamera()
        {
            return PlayerFactory.LocalPlayer._Camera;
        }
        public bool IsChunkExist(Vector2i Position)
        {
            return ChunkManager.IsChunkExist(Position);
        }
        public bool IsChunkSurrended(Vector2i ChunkPos)
        {
            return ChunkManager.IsChunkSurrended(ChunkPos);
        }
        public BlockState GetBlock(Vector3i BlockPos)
        {
            if (BlockPos.Y >= 0 && BlockPos.Y < Const.ChunkRHeight)
            {
                return ChunkManager.GetBlock(BlockPos);
            }
            return BlockRegistry.GetInstance().Air;
        }

        public BlockState GetBlock(int x, int y, int z)
        {
            if (y >= 0 && y < Const.ChunkRHeight)
            {
                return ChunkManager.GetBlock(new Vector3i(x, y, z));
            }
            return BlockRegistry.GetInstance().Air;
        }
        public void SetBlock(int x, int y, int z, BlockState State)
        {
            ChunkManager.SetBlock(new Vector3i(x, y, z), State);
        }

        public bool IsTransparent(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public bool IsAir(int x, int y, int z)
        {
            if (GetBlock(x, y, z) == BlockRegistry.GetInstance().Air) { return true; }
            return false;
        }

        public bool IsClient()
        {
            return true;
        }
    }

}
