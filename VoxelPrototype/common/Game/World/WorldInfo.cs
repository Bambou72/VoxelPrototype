using VoxelPrototype.VBF;
namespace VoxelPrototype.common.Game.World
{
    public class WorldInfo : IVBFSerializableBinary<WorldInfo>
    {
        internal long Seed;
        internal string WorldGenerator;
        internal string Path {  get; set; }
        internal string Name;
        public WorldInfo()
        { 

        }
        public WorldInfo Deserialize(byte[] data)
        {
            VBFCompound deserializedRoot = (VBFCompound)VBFSerializer.Deserialize(data);
            Seed = deserializedRoot.GetLong("Seed").Value;
            WorldGenerator = deserializedRoot.GetString("WorldGenerator").Value;
            return this;
        }

        public byte[] Serialize()
        {
            VBFCompound root = new VBFCompound();
            root.AddLong("Seed", Seed);
            root.AddString("WorldGenerator", WorldGenerator);
            return VBFSerializer.Serialize(root);
        }
        public long GetSeed()
        {
            return Seed;
        }
        public void SetSeed(long Seed)
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
