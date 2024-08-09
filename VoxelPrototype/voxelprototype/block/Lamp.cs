using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;
using VoxelPrototype.api.block.state.properties;
using VoxelPrototype.game;

namespace VoxelPrototype.voxelprototype.block
{
    internal class Lamp : Block
    {
        static BooleanProperty LIT = new("Lit");
        public Lamp()
        {
        }
        public override void RegisterProperties(BlockStateBuilder Builder)
        {
            base.RegisterProperties(Builder);
            Builder.Register(LIT);
        }
        public override void OnInteract(IWorld World, Vector3i Pos, BlockState State, bool ServerSide)
        {
            base.OnInteract( World, Pos, State, ServerSide);
            if (State.Get(LIT) == true)
            {
                World.SetBlock(Pos.X,Pos.Y,Pos.Z, State.With(LIT, false));
            }
            else
            {
                World.SetBlock(Pos.X, Pos.Y, Pos.Z, State.With(LIT, true));
            }
        }
    }
}
