﻿using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.Properties;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.client;

namespace VoxelPrototype.common.Game.Blocks
{
    internal class Lamp : Block
    {
        BooleanProperty Lit = new("Lit");
        public Lamp()
        {
        }
        public override void RegisterProperties(BlockStateBuilder Builder)
        {
            base.RegisterProperties(Builder);
            Builder.Register(Lit);
        }
        public override void OnInteract(Vector3i Pos, BlockState State, bool ServerSide)
        {
            base.OnInteract(Pos, State, ServerSide);
            if (State.Get(new BooleanProperty("Lit")) == true)
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(Pos, State.With(new BooleanProperty("Lit"), false));
            }
            else
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(Pos, State.With(new BooleanProperty("Lit"), true));
            }
        }
    }
}
