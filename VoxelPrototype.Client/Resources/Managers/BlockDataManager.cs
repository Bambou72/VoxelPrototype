using Newtonsoft.Json;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.utils;
namespace VoxelPrototype.client.Resources.Managers
{
    internal class BlockDataManager : IReloadableResourceManager
    {
        private  Dictionary<ResourceID, BlockStateData> BlocksStateData = new Dictionary<ResourceID, BlockStateData>();
        public BlockStateData GetBlockStateData(ResourceID id)
        {
            if (BlocksStateData.TryGetValue(id, out BlockStateData blockStateData))
            {
                return blockStateData;
            }
            throw new Exception("Can't find block data");
        }
        public void Reload(ResourcesManager Manager)
        {
            Clean();
            var blocksdata= Manager.ListResources("data/block", path => path.EndsWith(".json"));
            foreach (var blockdata in blocksdata)
            {
                TextReader TempTextReader = new StreamReader(blockdata.Value.GetStream());
                var data = JsonConvert.DeserializeObject<BlockStateData>(TempTextReader.ReadToEnd());
                blockdata.Value.Close();
                BlocksStateData.Add(blockdata.Key, data);
            }
        }
        void Clean()
        {
            BlocksStateData.Clear();
        }
    }
    public class BlockStateData
    {
        public Dictionary<string, BlockData> variants = new();
        public BlockData GetBlockData(BlockState State)
        {
            if(State == State.Block.GetDefaultState())
            {
                return variants[""];
            }else if(variants.TryGetValue(State.ToString(),out BlockData Data))
            {
                return Data;
            }
            return default;
        }
    }
    public struct BlockData
    {
        public string model { get; set; }
        public TextureData textures { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
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
