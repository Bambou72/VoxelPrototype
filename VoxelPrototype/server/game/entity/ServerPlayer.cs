using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.game;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.network.packets;
using VoxelPrototype.physics;
using VoxelPrototype.voxelprototype.command;
namespace VoxelPrototype.server.game.entity
{
    public class ServerPlayer : Player
    {
        internal List<Vector2i> inChunk = new();
        internal Queue<PlayerControlsServer> Controls = new();
        internal ulong LastClientTick;
        public ServerPlayer(Vector3d _Position, ushort _ClientID)
        {
            //ViewRay = new Ray(Vector3d.Zero, Vector3.Zero, Reach);

            Position = _Position;
            ClientID = _ClientID;
        }
        public  void UpdateServer(PlayerControlsServer Control, IWorld World)
        {
            LastClientTick = Control.ClientTick;
            base.Update(World, Control.Dt);
            float Speed = NormalSpeed;
            if (Control.control)
            {
                Speed = SprintSpeed;
            }
            if (Control.forward)
            {
                Acceleration += Control.Front * Speed;
                Control.forward = false;
            }
            if (Control.backward)
            {
                Acceleration -= Control.Front * Speed;
                Control.backward = false;
            }
            if (Control.left)
            {
                Acceleration -= Control.Right * Speed;
                Control.left = false;
            }
            if (Control.right)
            {
                Acceleration += Control.Right * Speed;
                Control.right = false;
            }
            if (Control.space)
            {
                if (!Fly && !NoClip)
                {
                    Jump();
                }
                else
                {
                    Acceleration.Y = Speed; // Up
                }
                Control.space = false;
            }
            if (Control.shift)
            {
                if (!Fly && !NoClip)
                {
                }
                else
                {
                    Acceleration.Y = -Speed; // Down
                }
                Control.shift = false;
            }
        }
        internal void SendData()
        {
            PlayerData packet = new PlayerData { ClientID = ClientID, Position = Position, Rotation = Rotation, Name = Name, ClientTick = LastClientTick, ServerTick = Server.TheServer.World.CurrentTick, Fly = Fly, Ghost = NoClip };
            Server.TheServer.NetworkManager.SendPacketToAll(packet, DeliveryMethod.Unreliable);
        }
    }
}
