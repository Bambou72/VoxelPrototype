using VoxelPrototype.utils;
namespace VoxelPrototype.game
{
    public interface IWorld : IBlockAcessor
    {
        public bool IsClient();
    }
}
