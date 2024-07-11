using LiteNetLib;
using OpenTK.Mathematics;
using System;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.network.packets;
using VoxelPrototype.physics;
using VoxelPrototype.server;
using VoxelPrototype.voxelprototype.command;

namespace VoxelPrototype.game.entity.player
{
    public class Player : PhysicEntity
    {
        //
        public float NormalSpeed = 4.317f;
        public float SprintSpeed = 5.612f;
        public float EntityHeight = 1.80f;
        public float EntityWidth = 0.60f;
        public float EntityDepth = 0.60f;
        public float EntityEYEHeight = 1.70f;
        public float Reach = 4.5f;
        public string Name = "Test";
        public ushort ClientID;
    }
}
