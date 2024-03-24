using VoxelPrototype.common.Game.World;

namespace VoxelPrototype.common.Game
{
    public class WorldSettings
    {
        long Seed;
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
        public long GetSeed() 
        { 
            return Seed; 
        }
        public WorldSettings(long Seed,string Generator,string Name)
        {
            this.Seed = Seed;
            this.Generator = Generator;
            this.Name = Name;
        }
        public WorldSettings(string Name,WorldInfo worldInfo)
        {
            new WorldSettings(worldInfo.GetSeed(), worldInfo.GetWorldGenerator(),Name);
        }
    }
}
