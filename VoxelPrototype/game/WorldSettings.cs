namespace VoxelPrototype.game
{
    public class WorldSettings
    {
        int Seed;
        string Name;
        string Generator;
        public string GetName()
        {
            return Name;
        }
        public string GetGenerator()
        {
            return Generator;
        }
        public int GetSeed()
        {
            return Seed;
        }
        public WorldSettings(int Seed, string Generator, string Name)
        {
            this.Seed = Seed;
            this.Generator = Generator;
            this.Name = Name;
        }
        public WorldSettings(string Name, WorldInfo worldInfo)
        {
            new WorldSettings(worldInfo.GetSeed(), worldInfo.GetWorldGenerator(), Name);
        }
    }
}
