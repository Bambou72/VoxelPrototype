using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void Reload(ResourceManager Manager)
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
