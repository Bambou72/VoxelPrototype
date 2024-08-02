namespace VoxelPrototype.api.WorldGenerator
{
    public class WorldGeneratorRegistry
    {
        private static WorldGeneratorRegistry Instance;
        public static WorldGeneratorRegistry GetInstance()
        {
            return Instance;
        }
        private Dictionary<string, Type> GeneratorTypes = new();
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

        public bool RegisterWorldGenerator(string Name, Type Generator)
        {
            try
            {
                GeneratorTypes.Add(Name, Generator);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
        public WorldGenerator CreateWorldGenerator(string name)
        {
            if (GeneratorTypes.ContainsKey(name))
            {
                Type type = GeneratorTypes[name];
                if (Activator.CreateInstance(type) is WorldGenerator generator)
                {
                    return generator;
                }
            }
            return null; // Ou lancez une exception, selon votre logique de gestion d'erreurs.
        }
        public string[] AllGeneratorsName()
        {
            return GeneratorTypes.Keys.ToArray();
        }
    }
}
