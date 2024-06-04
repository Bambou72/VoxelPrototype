using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Entities;
using VoxelPrototype.client.Render.World;
using VoxelPrototype.client.World.Level;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Entities.Player.PlayerManager;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.client.World
{

    public class World :   ITickable
    {
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;

        public ClientChunkManager ChunkManager;
        internal ClientPlayerFactory PlayerFactory;
        internal PlayersRenderer PlayerRenderer;
        internal bool Initialized = false;
        internal int RenderDistance = 12;
        public World()
        {
            ChunkManager = new ClientChunkManager();
            PlayerFactory = new ClientPlayerFactory();
            PlayerRenderer = new PlayersRenderer();
        }
        public  void Dispose()
        {
            Initialized = false;
            ChunkManager.Dispose();
            PlayerFactory.Dispose();
        }


        public void Init()
        {
            Initialized = true;
        }
        public  void Tick(float DT)
        {
            CurrentTick++;
            PlayerFactory.Update(DT);
            if (PlayerFactory.LocalPlayerExist)
            {
                ChunkManager.Update();
            }

        }

        public void UpdateRender()
        {
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
        public  BlockState GetBlock(int x, int y, int z)
        {
            ChunkManager.GetBlock(x, y, z, out var id);
            return id;
        }
        public  Chunk GetChunk(int x, int z)
        {
            return ChunkManager.GetChunk(x, z);
        }
        public  bool IsChunkExist(int x, int z)
        {
            lock (ChunkManager.Clist)
            {
                return ChunkManager.Clist.ContainsKey(new OpenTK.Mathematics.Vector2i(x, z));
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
        public FrustumCamera GetLocalPlayerCamera()
        {
            return PlayerFactory.LocalPlayer._Camera;
        }
    }
    
}
