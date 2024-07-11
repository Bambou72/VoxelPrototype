using OpenTK.Mathematics;
using VoxelPrototype.game.entity.player;
namespace VoxelPrototype.client.game.entity
{
    public class SimulationState
    {
        public Vector3d position;
        public ulong currentTick;
    }
    public class InputState
    {
        public PlayerControls controls;
        public double dt;
        public ulong currentTick;
    }
}
