using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoxelPrototype.api.block.state;
using VoxelPrototype.client.resources.managers;
using VoxelPrototype.utils;
namespace VoxelPrototype.client.Resources.Managers
{
    internal class BlockDataManager : IReloadableResourceManager
    {
        private  Dictionary<string, BlockStateData> BlocksStateData = new Dictionary<string, BlockStateData>();
        public BlockStateData GetBlockStateData(string Location)
        {
            if (BlocksStateData.TryGetValue(Location, out BlockStateData blockStateData))
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
                blockdata.Value.Open();
                TextReader TempTextReader = new StreamReader(blockdata.Value.GetStream());
                var data = JsonSerializer.Deserialize(TempTextReader.ReadToEnd(),BlockDataJsonSerializerContext.Default.BlockStateData);
                blockdata.Value.Close();
                BlocksStateData.Add(blockdata.Key, data);
            }
        }
        void Clean()
        {
            BlocksStateData.Clear();
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(BlockStateData))]
    [JsonSerializable(typeof(BlockData))]
    [JsonSerializable(typeof(TextureData))]
    internal partial class BlockDataJsonSerializerContext : JsonSerializerContext
    {
    }

    public class BlockStateData
    {
        public Dictionary<string, BlockData> variants { get; set; } = new();
        public BlockData GetBlockData(BlockState State)
        {
            if(State == State.Block.GetDefaultState())
            {
                return variants[""];
            }else if(variants.TryGetValue(State.ToString(),out BlockData Data))
            {
                return Data;
            }
            else
            {
                return variants[""];
            }
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
