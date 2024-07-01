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
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Entities.Player.PlayerManager;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.client.World
{

    public class World : IWor
    {
        internal int LoadDistance = 12;
        //Tick
        internal ulong CurrentTick;

        public ClientChunkManager ChunkManager;
        internal ClientPlayerFactory PlayerFactory;
        internal PlayersRenderer PlayerRenderer;
        internal bool Initialized = false;
        internal int RenderDistance = 6;
        public World()
        {
        }
        public  void Dispose()
        {
            Initialized = false;
            
            ChunkManager.Dispose();
            ChunkManager = null;
            PlayerFactory.Dispose();
            PlayerFactory = null ;
        }


        public void Init()
        {
            ChunkManager = new ClientChunkManager(this);
            PlayerFactory = new ClientPlayerFactory();
            PlayerRenderer = new PlayersRenderer();
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
       
        public  Chunk GetChunk(Vector2i Pos)
        {
            return ChunkManager.GetChunk(Pos);
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
            if(Initialized == false)
            {
                return false;
            }
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
            if(BlockPos.Y >=0 && BlockPos.Y < Const.ChunkRHeight)
            {
                return ChunkManager.GetBlock(BlockPos);
            }
            return Client.TheClient.ModManager.BlockRegister.Air;
        }
        public BlockState GetBlock(Vector2i CPosition ,Vector3i BlockPosition)
        {
            return ChunkManager.GetBlock(CPosition, BlockPosition);
        }
    }

}
