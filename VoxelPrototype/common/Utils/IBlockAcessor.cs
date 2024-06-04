using VoxelPrototype.common.Blocks.State;

namespace VoxelPrototype.common.Utils
{
    public interface IBlockAcessor
    {
        BlockState GetBlock(int x, int y, int z);
        void SetBlock(int x, int y, int z, BlockState State);
        bool IsTransparent(int x, int y, int z);
        bool IsAir(int x, int y, int z);

    }
}
