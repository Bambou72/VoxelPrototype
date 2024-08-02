using System.IO.Compression;
using VoxelPrototype.utils.collections;
using VoxelPrototype.utils.io;
using VoxelPrototype.utils;
using VoxelPrototype.api;
using System.Text.Json;
using System.IO;
using VoxelPrototype.client.resources.managers;

namespace VoxelPrototype.client.Resources
{
    public class ResourcesManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        const string TempPath = "temp/resourcespacks";
        const string CorePackName = "core.resources";
        List<ResourcesPack> Available = new();
        List<ResourcesPack> Active = new();
        List<IReloadableResourceManager> ResourcesManagers = new();
        List<string> ResourcesPacksPaths = new List<string>{ "resourcespacks" };
        public ResourcesManager(string[]? MoreResourcesPacksPaths)
        {
            if (MoreResourcesPacksPaths != null)
            {
                ResourcesPacksPaths.AddRange(MoreResourcesPacksPaths);
            }
        }
        public void Init()
        {
            FindResourcesPack();
            ReloadResources();
        }
        public void Reload()
        {
            FindResourcesPack();
            ReloadResources();

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
        internal void AddPack(ResourcesPack Pack)
        {
            Available.Add(Pack);
            Logger.Info("Added new resourcespack "+Pack.Name+" "+"Version:"+Pack.Version);
        }
        internal void FindResourcesPack()
        {
            Available.Clear();
            CleanTempFolder();
            //Resourcepacks Folders
            foreach (string resourcespacksfolder in ResourcesPacksPaths)
            {
                Logger.Info("Loading resourcespacks from " + resourcespacksfolder);
                string[] ResourceDirect = Directory.GetDirectories(resourcespacksfolder);
                foreach (string directory in ResourceDirect)
                {
                    if (File.Exists(directory + "/pack.json"))
                    {
                        PackMetadata PackData = JsonSerializer.Deserialize<PackMetadata>(File.ReadAllText(directory + "/pack.json"));
                        string[] NameSpaces = RelativePath.GetRelativePathsDirectories(directory);
                        ResourcesPack Pack = new()
                        {
                            Name = PackData.name,
                            Version = PackData.version,
                            Description = PackData.description,
                            Namespaces = NameSpaces,
                            Path = directory
                        };
                        AddPack(Pack);
                    }
                }
                string[] ResourceZip = Directory.GetFiles("resourcespacks", "*.zip");
                foreach (string Zip in ResourceZip)
                {
                    try
                    {
                        ZipArchive zip = ZipFile.Open(Zip, ZipArchiveMode.Read);
                        zip.ExtractToDirectory("temp/resourcespacks");
                        zip = null;
                        string path = "temp/resourcespacks" + "/" + Path.GetFileNameWithoutExtension(Zip);
                        PackMetadata PackData = JsonSerializer.Deserialize<PackMetadata>(File.ReadAllText(path + "/pack.json"));
                        string[] NameSpaces = RelativePath.GetRelativePathsDirectories(path);
                        ResourcesPack Pack = new()
                        {
                            Name = PackData.name,
                            Version = PackData.version,
                            Description = PackData.description,
                            Namespaces = NameSpaces,
                            Path = path
                        };
                        AddPack(Pack);
                    }
                    catch
                    {
                        Logger.Error("ResourceManager failed to load a resourcespack from a zip file.");
                    }
                }
            }
            //Mods
            foreach(string Mod in ModManager.GetInstance().ModPath.Values)
            {
                try
                {
                    string path = Mod + "/resources";
                    PackMetadata PackData = JsonSerializer.Deserialize<PackMetadata>(File.ReadAllText(path + "/pack.json"));
                    string[] NameSpaces =  RelativePath.GetRelativePathsDirectories(path);
                    ResourcesPack Pack = new()
                    {
                        Name = PackData.name,
                        Version = PackData.version,
                        Description = PackData.description,
                        Namespaces = NameSpaces,
                        Path = path
                    };
                    AddPack(Pack);
                }
                catch
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
            foreach(string ModName in ModManager.GetInstance().ModList.Keys)
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
        public Resource? GetResource(string Location)
        {
            for (int i = 0; i < Active.Count; i++)
            {
                ResourcesPack pack = Active[i];
                if(pack.Namespaces.Contains(ResourceLocationHelper.GetNamespace(Location)))
                {
                    string CurrentPath = pack.Path + "/" + ResourceLocationHelper.GetPath(Location)+ "/" + ResourceLocationHelper.GetPathWithoutLast(Location);
                    foreach (string @namespace in pack.Namespaces)
                    {

                        string[] ResourcesList = Directory.GetFiles(CurrentPath);
                        foreach (string Resource in ResourcesList)
                        {
                            if(ResourceLocationHelper.GetPath(Location) == RelativePath.GetRelativePathFile(pack.Path + "/" + @namespace, Resource))
                            {
                                return  new( Resource);
                            }
                        }
                    }
                }
            }
            return null;
        }
        //
        //First First
        //
        public List<Resource> GetAllResource(string Location)
        {
            List<Resource> resources = new List<Resource>();
            for (int i = 0; i < Active.Count; i++)
            {
                ResourcesPack pack = Active[i];
                if (pack.Namespaces.Contains(ResourceLocationHelper.GetNamespace(Location)))
                {
                    string CurrentPath = pack.Path + "/" + ResourceLocationHelper.GetPath(Location) + "/" + ResourceLocationHelper.GetPathWithoutLast(Location);
                    foreach (string @namespace in pack.Namespaces)
                    {

                        string[] ResourcesList = Directory.GetFiles(CurrentPath);
                        foreach (string Resource in ResourcesList)
                        {
                            if (ResourceLocationHelper.GetPath(Location) == RelativePath.GetRelativePathFile(pack.Path + "/" + @namespace, Resource))
                            {
                                resources.Add(new(Resource));
                            }
                        }
                    }
                }
            }
            return resources;
        }
        public Dictionary<string,Resource> ListResources(string path,Func<string,bool> Predicate)
        {
            Dictionary<string,Resource> temp = new();
            for(int i = Active.Count - 1; i >= 0; i--)
            {
                ResourcesPack pack = Active[i];
                foreach(string @namespace in pack.Namespaces)
                {
                    string CurrentPath = pack.Path+"/"+@namespace+"/"+path;
                    string[] ResourcesList = Directory.GetFiles(CurrentPath).Where(Predicate).ToArray();
                    foreach (string Resource in ResourcesList)
                    {
                        string Location = @namespace+":"+RelativePath.GetRelativePathFile(pack.Path + "/" + @namespace, Resource);
                        Resource Res = new( Resource);
                        temp[Location] = Res;
                    }
                }
            }
            return temp;
        }
        // First Last
        public Dictionary<string, List<Resource>> ListAllResources(string path, Func<string, bool> Predicate)
        {
            Dictionary<string, List<Resource>> temp = new();
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                ResourcesPack pack = Active[i];
                foreach (string @namespace in pack.Namespaces)
                {
                    string CurrentPath = pack.Path + "/" + @namespace + "/" + path;
                    string[] ResourcesList = Directory.GetFiles(CurrentPath).Where(Predicate).ToArray();
                    foreach (string Resource in ResourcesList)
                    {
                        string Location =@namespace+":"+ RelativePath.GetRelativePathFile(pack.Path + "/" + @namespace, Resource);
                        Resource Res = new( Resource);
                        if(!temp.ContainsKey(Location))
                        {
                            temp[Location] = new List<Resource>();
                        }
                        temp[Location].Add(Res);
                    }
                }
            }
            return temp;
        }
    }
    public class PackMetadata
    {
        public string name { get; set; }
        public string version { get; set; }
        public string description { get; set; }
    }
}
