using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.API.Blocks.Properties;
using VoxelPrototype.common.API.Blocks.state;
using VoxelPrototype.common.API.Blocks.State;

namespace Voxel.Blocks
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
        public override void OnInteract(Vector3i Pos, BlockState State)
        {
            base.OnInteract(Pos, State);
            if(State.Get(new BooleanProperty("Lit")) ==  true)
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(Pos,State.With(new BooleanProperty("Lit"),false));
            }
            else
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(Pos, State.With(new BooleanProperty("Lit"), true));
            }
        }
    }
}
