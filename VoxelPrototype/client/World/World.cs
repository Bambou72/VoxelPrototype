using OpenTK.Mathematics;
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
        internal RenderThread RenderThread;
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
            RenderThread.Stop();
            Initialized = false;
            ChunkManager.Dispose();
            PlayerFactory.Dispose();
        }


        public void Init()
        {
            Initialized = true;
            RenderThread = new();
            RenderThread.Start();
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
       
        public  Chunk GetChunk(int x, int z)
        {
            return ChunkManager.GetChunk(x, z);
        }
        public  bool IsChunkExist(Vector2i Pos)
        {
            lock (ChunkManager.Clist)
            {
                return ChunkManager.Clist.ContainsKey(Pos);
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
        public bool IsChunkSurrended(Vector2i ChunkPos)
        {
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X, ChunkPos.Y + 1))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X, ChunkPos.Y - 1))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y - 1))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y - 1))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X - 1, ChunkPos.Y + 1))) return false;
            if (!IsChunkExist(new Vector2i(ChunkPos.X + 1, ChunkPos.Y + 1))) return false;
            return true;
        }
        public BlockState GetBlock(Vector3i BlockPos)
        {
            return ChunkManager.GetBlock(BlockPos);
        }
    }

}
