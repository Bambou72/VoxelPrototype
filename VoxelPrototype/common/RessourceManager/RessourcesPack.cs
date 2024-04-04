using Newtonsoft.Json;
using NLog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Tomlyn;
using Tomlyn.Model;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Text;
using VoxelPrototype.common.Physics;
using VoxelPrototype.common.RessourceManager.data;
namespace VoxelPrototype.common.RessourceManager
{
    internal class RessourcesPack
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal string Name;
        internal string Description;
        internal string Version;
        internal string Path;
        //
        //Block Textures
        //
        internal void LoadBlocksTexture(string[] Paths, RessourcePackManager man)
        {
            foreach (var path in Paths)
            {
                LoadBlocksTexture(path, System.IO.Path.GetFileNameWithoutExtension(path), man);
            }
        }
        private void LoadBlocksTexture(string path, string ModName, RessourcePackManager man)
        {
            path += "/textures/block/";
            string[] blockTextures = Directory.GetFiles(path, "*.png");
            foreach (string filePath in blockTextures)
            {
                Image<Rgba32> Texture = Image.Load<Rgba32>(File.ReadAllBytes(filePath));
                string TextureID = RessourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "block");
                man.loadedTextures.Add(TextureID, Texture);
            }
        }
        //
        //Block colliders
        //
        private Dictionary<string, Collider[]> BlockColliders = new Dictionary<string, Collider[]>();
        internal Collider[] GetBlockCollider(string Id)
        {
            if (BlockColliders.ContainsKey(Id))
            {
                return BlockColliders[Id];
            }
            else
            {
                Logger.Error("Block collider not found : " + Id);
                return null;
            }
        }
        internal void LoadBlockColliders(string[] Paths)
        {
            foreach (var path in Paths)
            {
                LoadBlockColliders(path, System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        void LoadBlockColliders(string path, string ModName)
        {
            path += "/colliders/block";
            string[] JsonModel = Directory.GetFiles(path, "*.json");
            foreach (string filePath in JsonModel)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<ColliderData>(File.ReadAllText(filePath));
                    string ColliderId = RessourcePackManager.GetAssetId(ModName, data.Name, "block");
                    BlockColliders.Add(ColliderId, data.Colliders);
                    Logger.Info("Loaded collider : " + ColliderId);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"A collider can't load : {filePath}");
                }
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
                    string MeshId = RessourcePackManager.GetAssetId(ModName, data.Name, "block");
                    BlockMeshs.Add(MeshId, new BlockMesh(data.Vertex, data.Uv));
                    Logger.Info($"Loaded block mesh : {MeshId}");
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
                    string ShaderId = RessourcePackManager.GetAssetId(ModName, (string)tomlData["Name"]);
                    Shaders.Add(ShaderId, new Shader(filePath + "/vert.glsl", filePath + "/frag.glsl"));
                    Logger.Info($"New shader loaded : {ShaderId}");
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
                    string DataId = RessourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "block");
                    BlocksStateData.Add(DataId, data);
                    Logger.Info($"Loaded block data : {DataId}");
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
                    string MeshId = RessourcePackManager.GetAssetId(ModName, data.Name, "entity");
                    EntitiesMesh.Add(MeshId, new Model(data.Model));
                    Logger.Info($"Loaded entity mesh :  {MeshId}");
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
                    string CubeMapId = RessourcePackManager.GetAssetId(ModName, "Cubemaps", (string)tomlData["Name"]);
                    CubeMapTextures.Add(CubeMapId, CubeMapTexture.LoadCubeMap(faces));
                    Logger.Info($"Loaded cubemap : {CubeMapId}");
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
                    string EntityTextureID = RessourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath), "entity");
                    EntitiesTexture.Add(EntityTextureID, Texture.LoadFromFile(filePath));
                    Logger.Info($"An entity texture loaded : {EntityTextureID}");
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
                    string FontID = RessourcePackManager.GetAssetId(ModName, System.IO.Path.GetFileNameWithoutExtension(filePath));
                    Fonts.Add(FontID, new Font(filePath,40));
                    Logger.Info($"A font loaded : {FontID}");
                //}
                /*catch (Exception ex)
                {
                    Logger.Error(ex, "A font can't load : " + filePath);
                }*/
            }
        }
    }
}
