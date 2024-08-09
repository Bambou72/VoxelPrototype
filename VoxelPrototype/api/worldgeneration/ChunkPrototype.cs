using OpenTK.Mathematics;
using VoxelPrototype.api.block.state;
namespace VoxelPrototype.api.worldgeneration
{
    public class ChunkPrototype
    {
        public BlockState[] Data = new BlockState[16 * 16 * Const.ChunkHeight *16];
        public void SetBlockState(Vector3i Pos , BlockState State)
        {
            Data[Pos.X + Pos.Y * (Const.ChunkHeight * 16) + Pos.X * 16] = State;
        }
    }
}
