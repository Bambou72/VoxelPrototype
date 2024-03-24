namespace VoxelPrototype.common.Game.World.Terrain.Storage.Palette
{
    internal static class Utils
    {
        internal static int TreetoOne(int x, int y, int z)
        {
            return x + Section.Size * (y + Section.Size * z);
        }
    }
}
