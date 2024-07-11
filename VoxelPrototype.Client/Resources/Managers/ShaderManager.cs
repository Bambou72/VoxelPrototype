using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomlyn.Model;
using Tomlyn;
using VoxelPrototype.client.Render.Components;
using Newtonsoft.Json;
using static OpenTK.Graphics.OpenGL.GL;
using System.Runtime.InteropServices;
using VoxelPrototype.utils;

namespace VoxelPrototype.client.Resources.Managers
{
    internal class ShaderManager : IReloadableResourceManager
    {
        Dictionary<ResourceID, Shader> Shaders = new Dictionary<ResourceID, Shader>();
        public void Clean()
        {
            foreach (var shader in Shaders.Values)
            {
                shader.Delete();
            }
            Shaders.Clear();
        }
        public Shader GetShader(ResourceID shaderID)
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
            var shaders = Manager.ListResources("shaders", path => path.EndsWith(".toml"));
            foreach (var shader in shaders)
            {
                TextReader TempTextReader = new StreamReader(shader.Value.GetStream());
                TomlTable ShaderData = Toml.ToModel(TempTextReader.ReadToEnd());
                shader.Value.Close();
                Shaders.Add(shader.Key, new Shader(Path.GetDirectoryName(shader.Value.GetPath())+"/"+ShaderData["Vertex"], Path.GetDirectoryName(shader.Value.GetPath()) + "/" + ShaderData["Frag"]));
            }
        }
    }
}
