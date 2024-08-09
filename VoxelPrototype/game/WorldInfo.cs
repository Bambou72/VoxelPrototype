namespace VoxelPrototype.game
{
    public class WorldInfo : IVBFSerializableBinary<WorldInfo>
    {
        public int Seed;
        public string WorldGenerator;
        public string Path { get; set; }
        public string Name;
        public WorldInfo()
        {

        }
        public WorldInfo Deserialize(byte[] data)
        {
            VBFCompound deserializedRoot = (VBFCompound)VBFSerializer.Deserialize(data);
            Seed = deserializedRoot.GetInt("Seed").Value;
            WorldGenerator = deserializedRoot.GetString("Generator").Value;
            return this;
        }

        public byte[] Serialize()
        {
            VBFCompound root = new VBFCompound();
            root.AddLong("Seed", Seed);
            root.AddString("Generator", WorldGenerator);
            byte[] data = VBFSerializer.Serialize(root);
            root = default;
            return data;
        }
        public int GetSeed()
        {
            return Seed;
        }
        public void SetSeed(int Seed)
        {
            this.Seed = Seed;
        }
        public string GetWorldGenerator()
        {
            return WorldGenerator;
        }
        public void SetWorldGenerator(string WorldGenerator)
        {
            this.WorldGenerator = WorldGenerator;
        }
    }
}
