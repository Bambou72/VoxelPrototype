namespace VoxelPrototype.common.RessourceManager.data
{
    public class BlockStateData
    {
        public Dictionary<string,BlockData> variants  =new();
    }
    public struct BlockData
    {
        public TextureData textures { get; set; }
    }
    public struct TextureData
    {
        public string all { get; set; }
        public string top { get; set; }
        public string bottom { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public string front { get; set; }
        public string back { get; set; }
        public string[] textures { get; set; }
    }
}
