using VoxelPrototype.api.worldgeneration;
namespace VoxelPrototype.voxelprototype.generator
{
    internal class ComplexWorldType : IWorldType
    {
        public string Name => "Complex";

        public void CustomizableUI()
        {
            
        }

        public IChunkGenerator GetChunkGenerator(int Seed)
        {
            return new ComplexChunkGenerator(Seed);
        }
    }
}
