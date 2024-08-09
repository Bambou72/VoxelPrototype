using OpenTK.Mathematics;
namespace VoxelPrototype.game.entity.player
{
    public struct PlayerControls
    {
        public bool forward, backward, left, right, space, shift, control = false;
        public Vector3 Front = Vector3.Zero;
        public Vector3 Right = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public PlayerControls() { }
    }
    public struct PlayerControlsServer
    {
        public bool forward, backward, left, right, space, shift, control = false;
        public Vector3 Front = Vector3.Zero;
        public Vector3 Right = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public ulong ClientTick;
        public double Dt;
        public PlayerControlsServer() { }
    }
}
