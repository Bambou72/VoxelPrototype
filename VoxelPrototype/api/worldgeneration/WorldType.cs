namespace VoxelPrototype.api.worldgeneration
{
    public interface IWorldType
    {
        string Name { get; }
        public IChunkGenerator GetChunkGenerator(int Seed);
        public void CustomizableUI();
    }
}
