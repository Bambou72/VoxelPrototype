namespace VoxelPrototype.API.WorldGenerator
{
    public static class WorldGeneratorRegistry
    {
        private static Dictionary<string, Type> GeneratorTypes = new();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static bool RegisterWorldGenerator(string Name, Type Generator)
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
        public static WorldGenerator CreateWorldGenerator(string name)
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
        public static string[] AllGeneratorsName()
        {
            return GeneratorTypes.Keys.ToArray();
        }
    }
}
