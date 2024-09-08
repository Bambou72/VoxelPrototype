using System.Text.Json;
using System.Text.Json.Serialization;
namespace VoxelPrototype.client
{
    public class Config
    {
        public int WindowWidth { get; set; } = 900;
        public int WindowHeight { get; set; } = 600;
        public bool Fullscreen { get; set; } = false;
        public int RenderDistance { get; set; } = 12;
        public float FOV { get; set; } = 70;
        public int GUIScale { get; set; } = 2;
        public static Config LoadConfig(string Path)
        {
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path,"{}");
            }
            return JsonSerializer.Deserialize(File.ReadAllText(Path), ConfigJsonSerializerContext.Default.Config);
        }
        public static void SaveProperties(string Path ,Config Conf)
        {
            string Test = JsonSerializer.Serialize(Conf, ConfigJsonSerializerContext.Default.Config);
            File.WriteAllText(Path, Test);
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(Config))]
    internal partial class ConfigJsonSerializerContext : JsonSerializerContext
    {
    }
}
