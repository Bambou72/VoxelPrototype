﻿using NLog;
using StbImageSharp;
using System.IO.Compression;
using Tomlyn;
using Tomlyn.Model;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Text;
using VoxelPrototype.client.Resources.data;
using VoxelPrototype.client.Utils;
using VoxelPrototype.common.Physics;
namespace VoxelPrototype.client.Resources
{
    public class ResourcePackManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string TempPath = "temp/resourcespacks";
        private static Texture VoxelAtlas;
        private Dictionary<string, float[]> BlockAtlasTexture = new Dictionary<string, float[]>();
        internal Dictionary<string, ImageResult> loadedTextures = new();
        internal Dictionary<string, ResourcesPack> RessourcePacks = new();
        internal Dictionary<string, int> RessourcePacksLevel = new();
        int CurrentPos = 0;
        public void Initialize()
        {
            CleanTempFolder();
            LoadResourcesPacks();
        }
        public void LoadResourcesPacks()
        {
            Logger.Info("Start to load resources packs!");
            loadedTextures.Clear();
            string[] LoadedResourcesPacks = Directory.GetFiles("resourcespacks", "*.zip");
            foreach (string filePath in LoadedResourcesPacks)
            {
                using (ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Read))
                {
                    zip.ExtractToDirectory("temp/resourcespacks");
                    string path = "temp/resourcespacks" + "/" + Path.GetFileNameWithoutExtension(filePath);
                    TomlTable tomlData = Toml.ToModel(File.ReadAllText(path + "/pack.toml"));
                    var ResourcesPack = new ResourcesPack()
                    {
                        Name = (string)tomlData["Name"],
                        Description = (string)tomlData["Description"],
                        Version = (string)tomlData["Version"],
                        Path = path,
                    };
                    RessourcePacks.Add(ResourcesPack.Name, ResourcesPack);
                    if (ResourcesPack.Name == "Voxel")
                    {
                        SetTexturePack("Voxel", 0);
                    }
                    string[] ModAssetsPath = Directory.GetDirectories(ResourcesPack.Path);
                    ResourcesPack.LoadShaders(ModAssetsPath);
                    ResourcesPack.LoadBlocksTexture(ModAssetsPath, this);
                    ResourcesPack.LoadBlockMeshs(ModAssetsPath);
                    ResourcesPack.LoadBlocksData(ModAssetsPath);
                    ResourcesPack.LoadEntitiesMesh(ModAssetsPath);
                    ResourcesPack.LoadEntityTexture(ModAssetsPath);
                    ResourcesPack.LoadCubeMap(ModAssetsPath);
                    ResourcesPack.LoadFont(ModAssetsPath);
                    ResourcesPack.LoadUITexture(ModAssetsPath);
                }
            }
            BuildAtlas();
            Logger.Info("All resources packs loaded!");

        }
        public void SetTexturePack(string Name, int op)
        {
            if (op == 0)
            {
                if (!RessourcePacksLevel.ContainsKey(Name))
                {
                    RessourcePacksLevel.Add(Name, CurrentPos);
                    CurrentPos += 1;
                }
            }
            else if (op == 1)
            {
                if (RessourcePacksLevel.ContainsKey(Name))
                {
                    int Currentpos = RessourcePacksLevel[Name];
                    string SupName = RessourcePacksLevel.FirstOrDefault(x => x.Value == Currentpos - 1).Key;
                    RessourcePacksLevel[Name] = Currentpos - 1;
                    RessourcePacksLevel[SupName] = Currentpos;
                }
            }
            else
            {
                if (RessourcePacksLevel.ContainsKey(Name))
                {
                    int Currentpos = RessourcePacksLevel[Name];
                    string SupName = RessourcePacksLevel.FirstOrDefault(x => x.Value == Currentpos + 1).Key;
                    RessourcePacksLevel[Name] = Currentpos + 1;
                    RessourcePacksLevel[SupName] = Currentpos;
                }
            }
        }
        public void CleanTempFolder()
        {
            if (!Path.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath + "/");
            }
            RecursiveDelete(new DirectoryInfo(TempPath + "/"), true);
            if (!Path.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath + "/");
            }
        }
        public static void RecursiveDelete(DirectoryInfo baseDir, bool isRootDir)
        {
            if (!baseDir.Exists)
                return;
            foreach (var dir in baseDir.EnumerateDirectories()) RecursiveDelete(dir, false);
            foreach (var file in baseDir.GetFiles())
            {
                file.IsReadOnly = false;
                file.Delete();
            }
            if (!isRootDir) baseDir.Delete();
        }
        public void ReloadVoxelAtlas()
        {
            loadedTextures.Clear();
            BlockAtlasTexture.Clear();
            if (RessourcePacksLevel.Count > 0)
            {
                foreach (string pack in RessourcePacksLevel.Keys)
                {
                    var pa = RessourcePacks[pack];
                    pa.LoadBlocksTexture(Directory.GetDirectories(pa.Path), this);
                }
            }
            BuildAtlas();
        }
        public void BuildAtlas()
        {
            int numTextures = loadedTextures.Count;
            int maxTextureWidth = loadedTextures.Values.Max(t => t.Width);
            int maxTextureHeight = loadedTextures.Values.Max(t => t.Height);
            int atlasSize = Math.Max(maxTextureWidth, maxTextureHeight) * (int)Math.Ceiling(Math.Sqrt(numTextures));
            var atlasImage = new byte[atlasSize* atlasSize * 4];
            int currentX = 0;
            int currentY = 0;
            foreach (string key in loadedTextures.Keys)
            {
                var texture = loadedTextures[key];
                if (currentX + texture.Width > atlasSize)
                {
                    currentX = 0;
                    currentY += maxTextureHeight;
                }
                // Create texture coordinates for the current texture
                float left = (float)currentX / atlasSize;
                float right = (float)(currentX + texture.Width) / atlasSize;
                float top = (float)currentY / atlasSize;
                float bottom = (float)(currentY + texture.Height) / atlasSize;
                float[] coordinates = new float[8];
                coordinates[0] = left;
                coordinates[1] = bottom;
                coordinates[2] = left;
                coordinates[3] = top;
                coordinates[4] = right;
                coordinates[5] = top;
                coordinates[6] = right;
                coordinates[7] = bottom;
                // Add texture coordinates to the dictionary with the file name as the key
                BlockAtlasTexture.Add(key, coordinates);
                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        int index = ((currentY + y) * atlasSize + (currentX + x))  *4;

                        for (int c = 0;c<4;c++)
                        {
                            int lindex = ((y * texture.Width + x)) * 4 + c ;
                            atlasImage[index+c] = texture.Data[lindex];
                        }
                    }
                }
                currentX += maxTextureWidth;
            }
            if (!Directory.Exists("debug/atlas"))
            {
                Directory.CreateDirectory("debug/atlas");
            }
            var AtlasImage = new ImageResult
            {
                Data = atlasImage,
                Width = atlasSize,
                Height = atlasSize,
                Comp = ColorComponents.RedGreenBlueAlpha,
            };

            ImageSaver.SaveAsPNG("debug/atlas/block.png",AtlasImage);
            VoxelAtlas = Texture.LoadFromData(AtlasImage);
        }
        public Texture GetAtlas()
        {
            return VoxelAtlas;
        }
        public void RemoveTexturePack(string Name)
        {
            if (RessourcePacksLevel.Count > 1)
            {
                if (RessourcePacksLevel.ContainsKey(Name))
                {
                    for (int i = RessourcePacksLevel[Name] + 1; i < RessourcePacksLevel.Count; i++)
                    {
                        string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                        RessourcePacksLevel[name]--;
                    }
                    RessourcePacksLevel.Remove(Name);
                    CurrentPos--;
                }
            }
            ReloadVoxelAtlas();
        }
        public void ReloadTexturePacks()
        {
            RessourcePacks.Clear();
            CleanTempFolder();
            LoadResourcesPacks();
            ReloadVoxelAtlas();
        }
        public static string GetAssetId(string ModName, string Name, string SubAssetType = "")
        {
            if (SubAssetType == "")
            {
                return ModName + "@" + Name;
            }
            else
            {
                return ModName + "@" + SubAssetType + "/" + Name;
            }
        }
        //
        //GET
        //
        public float[] GetAtlasTexturesCoord(string Id)
        {
            if (BlockAtlasTexture.TryGetValue(Id, out var Texture))
            {
                return Texture;
            }
            else
            {
                Logger.Warn("Block texture don't exist : {name}", Id);
                return BlockAtlasTexture["Voxel@block/unknow"];
            }
        }
        public Shader GetShader(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetShader(Id);
            }
            Logger.Error("Shader not found : " + Id);
            return null;
        }
        public BlockMesh GetBlockMesh(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetBlockMesh(Id);
            }
            Logger.Error("Block mesh not found : " + Id);
            return null;
        }
        public Model GetEntityMesh(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetEntityMesh(Id);
            }
            Logger.Error("Entity mesh not found : " + Id);
            return default;
        }
        public BlockStateData GetBlockStateData(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetBlockStateData(Id);
            }
            Logger.Error("Block data not found : " + Id);
            return new BlockStateData();
        }
        public CubeMapTexture GetCubeMap(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetCubeMap(Id);
            }
            Logger.Error("Cubemap not found : " + Id);
            return null;
        }
        public Texture GetEntityTexture(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetEntityTexture(Id);
            }
            Logger.Error("Entity texture not found : " + Id);
            return null;
        }

        public Font GetFont(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetFont(Id);
            }
            Logger.Error("Font not found : " + Id);
            return null;
        }
        public Texture GetUITexture(string Id)
        {
            for (int i = 0; i < CurrentPos; i++)
            {
                string name = RessourcePacksLevel.FirstOrDefault(x => x.Value == i).Key;
                return RessourcePacks[name].GetUITexture(Id);
            }
            Logger.Error("UI texture not found : " + Id);
            return null;
        }
    }
}