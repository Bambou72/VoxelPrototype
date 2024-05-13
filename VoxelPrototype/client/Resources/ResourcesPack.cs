using Newtonsoft.Json;
using NLog;
using StbImageSharp;
using Tomlyn;
using Tomlyn.Model;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Text;
using VoxelPrototype.client.Resources.data;
using VoxelPrototype.common.Physics;
namespace VoxelPrototype.client.Resources
{
    internal class ResourcesPack
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal string Name;
        internal string Description;
        internal string Version;
        internal string Path;
        //
        //Block Textures
        //
        internal void LoadBlocksTexture(string[] Paths, ResourcePackManager man)
        {
            foreach (var path in Paths)
            {
                LoadBlocksTexture(path, System.IO.Path.GetFileNameWithoutExtension(path), man);
            }
        }
        private void LoadBlocksTexture(string path, string ModName, ResourcePackManager man)
        {
            path += "/textures/block/";
            string[] blockTextures = Directory.GetFiles(path, "*.png");
            foreach (string filePath in blockTextures)
            {
                ImageResult Texture;
                using (Stream stream = File.OpenRead(filePath))
                {
                    Texture = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                }
                string TextureID = ResourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "block");
                man.loadedTextures.Add(TextureID, Texture);
            }
        }
        //
        //Block Mesh
        //
        private Dictionary<string, BlockMesh> BlockMeshs = new Dictionary<string, BlockMesh>();
        internal BlockMesh GetBlockMesh(string Id)
        {
            if (BlockMeshs.ContainsKey(Id))
            {
                return BlockMeshs[Id];
            }
            else
            {
                Logger.Error("Block mesh not found : " + Id);
                return null;
            }
        }
        internal void LoadBlockMeshs(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadBlockMeshs(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        internal void LoadBlockMeshs(string path, string ModName)
        {
            path += "/models/block";
            string[] BlocksModel = Directory.GetFiles(path, "*.json");
            foreach (string filePath in BlocksModel)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<BlockMeshData>(File.ReadAllText(filePath));
                    string MeshId = ResourcePackManager.GetAssetId(ModName, data.Name, "block");
                    BlockMeshs.Add(MeshId, new BlockMesh(data.Vertex, data.Uv));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"A block can't load : {filePath}");
                }
            }
        }
        //
        //Shaders
        //
        private static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        internal Shader GetShader(string Id)
        {
            if (Shaders.ContainsKey(Id))
            {
                return Shaders[Id];
            }
            else
            {
                Logger.Error("Shader not found : " + Id);
                return null;
            }
        }
        internal void LoadShaders(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadShaders(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        private void LoadShaders(string path, string ModName)
        {
            path += "/shaders/";
            string[] shaders = Directory.GetDirectories(path);
            foreach (string filePath in shaders)
            {
                TomlTable tomlData = Toml.ToModel(File.ReadAllText(filePath + "/shader.toml"));
                try
                {
                    string ShaderId = ResourcePackManager.GetAssetId(ModName, (string)tomlData["Name"]);
                    Shaders.Add(ShaderId, new Shader(filePath + "/vert.glsl", filePath + "/frag.glsl"));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "A shader can't load : " + filePath);
                }
            }
        }
        //
        //BlockData
        //
        private static Dictionary<string, BlockStateData> BlocksStateData = new Dictionary<string, BlockStateData>();
        internal BlockStateData GetBlockStateData(string Id)
        {
            if (BlocksStateData.ContainsKey(Id))
            {
                return BlocksStateData[Id];
            }
            else
            {
                Logger.Error("Block data not found : " + Id);
                return default;
            }
        }
        internal void LoadBlocksData(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadBlocksData(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        private void LoadBlocksData(string path, string ModName)
        {
            path += "/data/block";
            string[] blocksData = Directory.GetFiles(path, "*.json");
            foreach (string filePath in blocksData)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<BlockStateData>(File.ReadAllText(filePath));
                    string DataId = ResourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "block");
                    BlocksStateData.Add(DataId, data);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"A block data can't load : {filePath}");
                }
            }
        }
        //
        //Entity model
        //
        private static Dictionary<string, Model> EntitiesMesh = new Dictionary<string, Model>();
        internal Model GetEntityMesh(string Id)
        {
            if (EntitiesMesh.ContainsKey(Id))
            {
                return EntitiesMesh[Id];
            }
            else
            {
                Logger.Error("Entity mesh not found : " + Id);
                return default;
            }
        }
        internal void LoadEntitiesMesh(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadEntitiesMesh(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        internal void LoadEntitiesMesh(string path, string ModName)
        {
            path += "/models/entity";
            string[] entitiesMesh = Directory.GetFiles(path, "*.json");
            foreach (string filePath in entitiesMesh)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<MeshData>(File.ReadAllText(filePath));
                    string MeshId = ResourcePackManager.GetAssetId(ModName, data.Name, "entity");
                    EntitiesMesh.Add(MeshId, new Model(data.Model));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An entity model can't load : " + filePath);
                }
            }
        }
        //
        //Cubemap
        //
        private static Dictionary<string, CubeMapTexture> CubeMapTextures = new Dictionary<string, CubeMapTexture>();
        internal CubeMapTexture GetCubeMap(string Id)
        {
            if (CubeMapTextures.ContainsKey(Id))
            {
                return CubeMapTextures[Id];
            }
            else
            {
                Logger.Error("Entity mesh not found : " + Id);
                return null;
            }
        }
        internal void LoadCubeMap(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadCubeMap(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        internal void LoadCubeMap(string path, string ModName)
        {
            path += "/textures/cubemap";
            string[] Skyboxs = Directory.GetDirectories(path);
            foreach (string filePath in Skyboxs)
            {
                TomlTable tomlData = Toml.ToModel(File.ReadAllText(filePath + "/cubemap.toml"));
                try
                {
                    List<string> faces = new List<string>
                    {
                        filePath+"/"+(string)tomlData["Right"],
                        filePath+"/"+(string)tomlData["Left"],
                        filePath+"/"+(string)tomlData["Bottom"],
                        filePath+"/"+(string)tomlData["Top"],
                        filePath+"/"+(string)tomlData["Front"],
                        filePath+"/"+(string)tomlData["Back"]
                    };
                    string CubeMapId = ResourcePackManager.GetAssetId(ModName, "Cubemaps", (string)tomlData["Name"]);
                    CubeMapTextures.Add(CubeMapId, CubeMapTexture.LoadCubeMap(faces));
                }
                catch
                {
                    Logger.Info($"A cubemap can't load : " + (string)tomlData["Name"]);
                }
            }
        }
        //
        //Entity texture
        //
        private static Dictionary<string, Texture> EntitiesTexture = new Dictionary<string, Texture>();
        internal Texture GetEntityTexture(string Id)
        {
            if (EntitiesTexture.ContainsKey(Id))
            {
                return EntitiesTexture[Id];
            }
            else
            {
                Logger.Error("Entity texture not found : " + Id);
                return null;
            }
        }
        internal void LoadEntityTexture(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadEntityTexture(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        internal void LoadEntityTexture(string path, string ModName)
        {
            path += "/textures/entity";
            string[] textures = Directory.GetFiles(path, "*.png");
            foreach (string filePath in textures)
            {
                try
                {
                    string EntityTextureID = ResourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "entity");
                    EntitiesTexture.Add(EntityTextureID, Texture.LoadFromFile(filePath));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Entity texture can't load : " + filePath);
                }
            }
        }
        //
        //Font
        //

        private static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        internal Font GetFont(string Id)
        {
            if (Fonts.ContainsKey(Id))
            {
                return Fonts[Id];
            }
            else
            {
                Logger.Error("Font not found : " + Id);
                return null;
            }
        }
        internal void LoadFont(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadFont(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        private void LoadFont(string path, string ModName)
        {
            path += "/font";
            string[] textures = Directory.GetFiles(path, "*.ttf");
            foreach (string filePath in textures)
            {
                //try
                //{
                string FontID = ResourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath));
                Fonts.Add(FontID, new Font(filePath));
                //}
                /*catch (Exception ex)
                {
                    Logger.Error(ex, "A font can't load : " + filePath);
                }*/
            }
        }
        //
        //UI texture
        //
        private static Dictionary<string, Texture> UITexture = new Dictionary<string, Texture>();
        internal Texture GetUITexture(string Id)
        {
            if (UITexture.ContainsKey(Id))
            {
                return UITexture[Id];
            }
            else
            {
                Logger.Error("UI texture not found : " + Id);
                return null;
            }
        }
        internal void LoadUITexture(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadUITexture(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        internal void LoadUITexture(string path, string ModName)
        {
            path += "/textures/ui";
            string[] textures = Directory.GetFiles(path, "*.png");
            foreach (string filePath in textures)
            {
                try
                {
                    string UITextureID = ResourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "ui");
                    UITexture.Add(UITextureID, Texture.LoadFromFile(filePath));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "UItexture can't load : " + filePath);
                }
            }
        }
    }
}
