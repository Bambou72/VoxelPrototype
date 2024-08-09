namespace VoxelPrototype.api.worldgeneration
{
    public class WorldGeneratorRegistry
    {
        private static WorldGeneratorRegistry Instance;
        public static WorldGeneratorRegistry GetInstance()
        {
            return Instance;
        }
        private Dictionary<string, IWorldType> Generators = new();
        private readonly NLog.Logger Logger = NLog.LogManager.GetLogger("WorldGeneratorRegistry");

        public WorldGeneratorRegistry()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new InvalidOperationException("You can't instanciate more than 1 instance of singleton");
            }

        }

        public bool RegisterWorldType( IWorldType Type)
        {
            try
            {
                Generators.Add(Type.Name, Type);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
        public IWorldType GetWorldType(string Name)
        {
            if (Generators.ContainsKey(Name))
            {
                return Generators[Name];
            }
            return null;
        }
        public string[] AllGeneratorsName()
        {
            return Generators.Keys.ToArray();
        }
    }
}
