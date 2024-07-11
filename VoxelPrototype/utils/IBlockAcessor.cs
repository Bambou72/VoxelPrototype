using VoxelPrototype.api.Blocks.State;

namespace VoxelPrototype.utils
{
    public interface IBlockAcessor
    {
        BlockState GetBlock(int x, int y, int z);
        void SetBlock(int x, int y, int z, BlockState State);
        bool IsTransparent(int x, int y, int z);
        bool IsAir(int x, int y, int z);

    }
}
