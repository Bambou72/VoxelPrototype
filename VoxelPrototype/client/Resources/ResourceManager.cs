﻿using Tomlyn.Model;
using Tomlyn;
using System.IO.Compression;
using VoxelPrototype.api;
using VoxelPrototype.common.Utils;
using OpenTK.Windowing.Common.Input;
using System.Resources;
using VoxelPrototype.client.Resources.Managers;

namespace VoxelPrototype.client.Resources
{
    public class ResourceManager
    {
        const string TempPath = "temp/resourcespacks";
        const string CorePackName = "core.resources";
        List<ResourcesPack> Available = new();
        List<ResourcesPack> Active = new();
        List<IReloadableResourceManager> ResourcesManagers = new();

        public ResourceManager()
        {
            
        }
        public void Init()
        {
            FindResourcesPack();
            ReloadResources();
        }
        public void ReloadPacks()
        {
            FindResourcesPack();
        }

        public void RegisterManager(IReloadableResourceManager manager)
        {
            ResourcesManagers.Add(manager);
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
        internal void FindResourcesPack()
        {
            CleanTempFolder();
            //BaseGame assets
            if (File.Exists(CorePackName))
            {
                ZipArchive zip = ZipFile.Open(CorePackName, ZipArchiveMode.Read);
                zip.ExtractToDirectory("temp/resourcespacks/"+ Path.GetFileNameWithoutExtension(CorePackName));
                zip = null;
                string path = "temp/resourcespacks" + "/" + Path.GetFileNameWithoutExtension(CorePackName);
                TomlTable PackData = Toml.ToModel(File.ReadAllText(path + "/pack.toml"));
                string[] NameSpaces = RelativePath.GetRelativePathsDirectories(path);
                ResourcesPack Pack = new()
                {
                    Name = (string)PackData["Name"],
                    Version = (string)PackData["Version"],
                    Description = (string)PackData["Description"],
                    Namespaces = NameSpaces,
                    Path = path
                };
                Available.Add(Pack);
            }
            //Resourcepacks Folder
            string[] ResourceDirect = Directory.GetDirectories("resourcespacks");
            foreach (string directory in ResourceDirect)
            {
                if (File.Exists(directory + "/pack.toml"))
                {
                    TomlTable PackData = Toml.ToModel(File.ReadAllText(directory + "/pack.toml"));
                    string[] NameSpaces = RelativePath.GetRelativePathsDirectories(directory);
                    ResourcesPack Pack = new()
                    {
                        Name = (string)PackData["Name"],
                        Version = (string)PackData["Version"],
                        Description = (string)PackData["Description"],
                        Namespaces = NameSpaces,
                        Path = directory
                    };
                    Available.Add(Pack);
                }
            }
            string[] ResourceZip = Directory.GetFiles("resourcespacks", "*.zip");
            foreach (string Zip in ResourceZip)
            {
                ZipArchive zip = ZipFile.Open(Zip, ZipArchiveMode.Read);
                zip.ExtractToDirectory("temp/resourcespacks");
                zip = null;
                string path = "temp/resourcespacks" + "/" + Path.GetFileNameWithoutExtension(Zip);
                TomlTable PackData = Toml.ToModel(File.ReadAllText(path + "/pack.toml"));
                string[] NameSpaces = RelativePath.GetRelativePathsDirectories(path);
                ResourcesPack Pack = new()
                {
                    Name = (string)PackData["Name"],
                    Version = (string)PackData["Version"],
                    Description = (string)PackData["Description"],
                    Namespaces = NameSpaces,
                    Path = path
                };
                Available.Add(Pack);
            }
            //Mods
            foreach(string Mod in Client.TheClient.ModManager.ModPath.Values)
            {
                try
                {
                    string path = Mod + "/resources";
                    TomlTable PackData = Toml.ToModel(File.ReadAllText(path + "/pack.toml"));
                    string[] NameSpaces =  RelativePath.GetRelativePathsDirectories(path);
                    ResourcesPack Pack = new()
                    {
                        Name = (string)PackData["Name"],
                        Version = (string)PackData["Version"],
                        Description = (string)PackData["Description"],
                        Namespaces = NameSpaces,
                        Path = path
                    };
                    Available.Add(Pack);
                }catch
                {

                }
                
            }
            ReplacePacks();
            EnsureCoreAndModPackArePresent();
        }
        internal void ReplacePacks()
        {
            for(int i = 0;i< Active.Count;i++)
            {
                bool Find = false;
                foreach(ResourcesPack Pack in Available)
                {
                    if (Pack.Equals(Active[i]))
                    {
                        Active[i] = Pack;
                        Find = true;
                        break;
                    }
                }
                if(!Find)
                {
                    Active.RemoveAt(i);
                }
            }
        }
        internal void EnsureCoreAndModPackArePresent()
        {
            ResourcesPack BasePack = Available.Find(x => x.Name == "Core");
            if(BasePack == null)
            {
                throw new Exception("Hey what a big problem , why did the core ressource pack not found ?");
            }
            if (!Active.Contains(BasePack))
            {
                Active.Add(BasePack);
                Active.MoveToFirstPosition(BasePack);
            }
            foreach(string ModName in Client.TheClient.ModManager.ModList.Keys)
            {
                if(Active.Find(x => x.Name == "Core") == null)
                {
                    var ModPack = Available.Find(x => x.Name == ModName);
                    if(ModPack == null)
                    {
                        throw new Exception($"It lacks the resources of the mod {ModName} that you installed");
                    }
                    Active.Add(ModPack);
                }
            }
        }
        internal void ReloadResources()
        {
            foreach (var manager in ResourcesManagers)
            {
                manager.Reload(this);
            }
        }
        public Resource GetResource(ResourceID ID)
        {
            throw new NotImplementedException();
        }
        public Dictionary<ResourceID,Resource> ListResources(string path,Func<string,bool> Predicate)
        {
            Dictionary<ResourceID,Resource> temp = new();
            for(int i = Active.Count - 1; i >= 0; i--)
            {
                ResourcesPack pack = Active[i];
                foreach(string @namespace in pack.Namespaces)
                {
                    string CurrentPath = pack.Path+"/"+@namespace+"/"+path;
                    string[] ResourcesList = Directory.GetFiles(CurrentPath).Where(Predicate).ToArray();
                    foreach (string Resource in ResourcesList)
                    {
                        ResourceID ID = new ResourceID(@namespace,RelativePath.GetRelativePathFile(pack.Path + "/" + @namespace, Resource));
                        Resource Res = new(new FileStream(Resource,FileMode.Open, FileAccess.Read), Resource);
                        temp[ID] = Res;
                    }
                }
            }
            return temp;
        }
    }
}
