using VoxelPrototype.api.worldgeneration;
namespace VoxelPrototype.voxelprototype.generator
{
    internal class MandelbrotWorldType : IWorldType
    {
        public string Name => "Mandelbrot";

        public void CustomizableUI()
        {
        }

        public IChunkGenerator GetChunkGenerator(int Seed)
        {
            return new MandelbrotGenerator();
        }
    }
}
