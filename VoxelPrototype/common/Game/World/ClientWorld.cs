using VoxelPrototype.API.Blocks.State;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Entities;
using VoxelPrototype.client.Render.World;
using VoxelPrototype.common.Game.Entities.Player.PlayerManager;
using VoxelPrototype.common.Game.World.ChunkManagers;

namespace VoxelPrototype.common.Game.World
{
    public class ClientWorld : World, IRenderSystem
    {
        public ClientChunkManager ChunkManager;
        internal ClientPlayerFactory PlayerFactory;
        internal WorldRenderer WorldRenderer;
        internal PlayersRenderer PlayerRenderer;
        internal bool Initialized = false;
        internal int RenderDistance = 12;
        public ClientWorld()
        {
            ChunkManager = new ClientChunkManager();
            PlayerFactory = new ClientPlayerFactory();
            PlayerRenderer = new PlayersRenderer();
            WorldRenderer = new();
        }
        public override void Dispose()
        {
            Initialized = false;
            ChunkManager.Dispose();
            WorldRenderer.Dispose();
            PlayerFactory.Dispose();
        }


        public void Init()
        {
            Initialized  =true;
        }
        public override void Tick()
        {
            base.Tick();
            PlayerFactory.Update();
            if (PlayerFactory.LocalPlayerExist)
            {
                ChunkManager.Update();
            }

        }

        public void UpdateRender()
        {
            WorldRenderer.Update();
        }
        public void Render()
        {
            if (PlayerFactory.LocalPlayerExist)
            {
                //SkyboxRender.RenderSkyBox(PlayerFactory.LocalPlayer._Camera.GetViewMatrix(), PlayerFactory.LocalPlayer._Camera.GetProjectionMatrix());
                WorldRenderer.Render();
                PlayerRenderer.RenderPlayers();
            }
        }
        public override BlockState GetBlock(int x, int y, int z)
        {
            ChunkManager.GetBlock(x, y, z, out var id);
            return id;
        }
        public override Chunk GetChunk(int x, int z)
        {
            return ChunkManager.GetChunk(x, z);
        }
        public override bool IsChunkExist(int x, int z)
        {
            lock (ChunkManager.Clist)
            {
                return ChunkManager.Clist.ContainsKey(new OpenTK.Mathematics.Vector2i(x,z));
            }
        }
        public int GetChunkCount()
        {
            return ChunkManager.Clist.Count;
        }
        public bool IsLocalPlayerExist()
        {
            return PlayerFactory.LocalPlayerExist;
        }
        public Camera GetLocalPlayerCamera()
        {
            return PlayerFactory.LocalPlayer._Camera;
        }
    }
}
