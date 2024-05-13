using OpenTK.Mathematics;
namespace VoxelPrototype.common.Game.Entities.Player
{
    public class SimulationState
    {
        public Vector3d position;
        public ulong currentTick;
    }
    public class InputState
    {
        internal PlayerControls controls;
        public double dt;
        public ulong currentTick;
    }
}
