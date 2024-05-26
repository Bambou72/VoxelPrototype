using OpenTK.Mathematics;
namespace VoxelPrototype.common.Entities.Player
{
    internal struct PlayerControls
    {
        internal bool forward, backward, left, right, space, shift, control = false;
        internal Vector3 Front = Vector3.Zero;
        internal Vector3 Right = Vector3.Zero;
        internal Vector3 Rotation = Vector3.Zero;
        public PlayerControls() { }
    }
    internal struct PlayerControlsServer
    {
        internal bool forward, backward, left, right, space, shift, control = false;
        internal Vector3 Front = Vector3.Zero;
        internal Vector3 Right = Vector3.Zero;
        internal Vector3 Rotation = Vector3.Zero;
        internal ulong ClientTick;
        internal double Dt;
        public PlayerControlsServer() { }
    }
}
