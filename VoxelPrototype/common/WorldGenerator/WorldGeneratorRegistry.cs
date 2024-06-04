namespace VoxelPrototype.common.WorldGenerator
{
    public class WorldGeneratorRegistry
    {
        private Dictionary<string, Type> GeneratorTypes = new();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
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
