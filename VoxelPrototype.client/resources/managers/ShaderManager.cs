﻿using System.Text.Json;
using System.Text.Json.Serialization;
using VoxelPrototype.client.rendering;
using VoxelPrototype.client.resources.managers;
namespace VoxelPrototype.client.Resources.Managers
{
    internal class ShaderManager : IReloadableResourceManager
    {
        Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        public void Clean()
        {
            foreach (var shader in Shaders.Values)
            {
                shader.Delete();
            }
            Shaders.Clear();
        }
        public Shader GetShader(string shaderID)
        {
            if(Shaders.TryGetValue(shaderID, out Shader shader))
            {
                return shader;
            }
            throw new Exception("Can't find shader");
        }
        public void Reload(ResourcesManager Manager)
        {
            Clean();
            //Block Mesh
            var shaders = Manager.ListResources("shaders", path => path.EndsWith(".json"));
            foreach (var shader in shaders)
            {
                shader.Value.Open();
                TextReader TempTextReader = new StreamReader(shader.Value.GetStream());
                ShaderJson ShadJson = JsonSerializer.Deserialize<ShaderJson>(TempTextReader.ReadToEnd(),ShaderJsonSerializerContext.Default.ShaderJson);
                shader.Value.Close();
                Shaders.Add(shader.Key, new Shader(Path.GetDirectoryName(shader.Value.GetPath())+"/"+ ShadJson.vertex, Path.GetDirectoryName(shader.Value.GetPath()) + "/" + ShadJson.fragment));
            }
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ShaderJson))]
    internal partial class ShaderJsonSerializerContext : JsonSerializerContext
    {
    }
    public class ShaderJson
    {
        public string vertex { get; set; }
        public string fragment { get; set; }
    }
}
