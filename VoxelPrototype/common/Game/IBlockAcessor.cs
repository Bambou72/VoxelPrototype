using VoxelPrototype.common.API.Blocks.state;

namespace VoxelPrototype.common.Game
{
    public interface IBlockAcessor
    {
        BlockState GetBlock(int x, int y,int z);
        bool IsTransparent(int x,int y,int z);
        bool IsAir(int x,int y,int z);

    }
}
