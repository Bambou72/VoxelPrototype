namespace VoxelPrototype.api
{
    public interface IMod
    {
        public string Name { get; }
        public static string NameSpace { get; }
        public string Description { get; }
        public string Version { get; }
        public void PreInit(ModManager Manager);
        public void Init(ModManager Manager);
        public void DeInit(ModManager Manager);
    }
}
