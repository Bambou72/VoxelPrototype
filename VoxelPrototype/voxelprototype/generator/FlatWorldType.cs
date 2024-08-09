using VoxelPrototype.api.worldgeneration;

namespace VoxelPrototype.voxelprototype.generator
{
    internal class FlatWorldType : IWorldType
    {
        public string Name => "Flat";

        public void CustomizableUI()
        {
        }

        public IChunkGenerator GetChunkGenerator(int Seed)
        {
            return new FlatGenerator();
        }
    }
}
