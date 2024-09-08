namespace VoxelPrototype.api
{
    public interface IModInitializer
    {
        public void PreInit(ModManager Manager);
        public void Init(ModManager Manager);
        public void DeInit(ModManager Manager);
        public string GetModName();
        public string GetModNamespace();
        public string GetModVersion();
        public string GetModDescription();
        
    }
}
