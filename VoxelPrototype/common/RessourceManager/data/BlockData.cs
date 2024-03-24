namespace VoxelPrototype.common.RessourceManager.data
{
    internal struct BlockData
    {
        public string Name { get; set; }
        public TextureData Textures { get; set; }
    }
    public struct TextureData
    {
        public string All { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
        public string[] Textures { get; set; }
    }
}
