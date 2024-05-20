namespace VoxelPrototype.api
{
    public interface IMod
    {
        public string Name { get; }
        public static string NameSpace { get; }
        public string Description { get; }
        public string Version { get; }
        public void Init();
        public void DeInit();
    }
}
