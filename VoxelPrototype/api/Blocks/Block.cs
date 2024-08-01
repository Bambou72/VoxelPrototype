using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.game;
using VoxelPrototype.physics;
namespace VoxelPrototype.api.Blocks
{
    public class Block
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BlockStateHolder StateHolder;
        private BlockState Default;
        public string ID;
        public string Data;
        public float Friction = 8;
        public int BreakingTime = 20;
        public BlockRenderType RenderType;
        public bool Transparent = false;
        public bool Cullself = false;

        public Block()
        {
            BlockStateBuilder Builder = new(this);
            RegisterProperties(Builder);
            StateHolder = Builder.Build();
            Default = StateHolder.GetBaseState();
        }

        public virtual void RegisterProperties(BlockStateBuilder Builder)
        {
        }

        public BlockState GetDefaultState()
        {
            return Default;
        }
        public virtual Collider[] GetColliders(BlockState Sate)
        {
            return [new Collider(Vector3d.Zero, Vector3d.One)];
        }
        public virtual void OnInteract(IWorld World,Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }
        public virtual void OnPlaced(IWorld World, Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }
        public virtual void OnBreaked(IWorld World, Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }

    }
    public enum BlockRenderType
    {
        Cube,
        Custom,
    }
}
