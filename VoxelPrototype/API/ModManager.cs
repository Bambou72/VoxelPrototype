using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Commands;
using VoxelPrototype.api.Items;
using VoxelPrototype.api.WorldGenerator;
namespace VoxelPrototype.api
{
    public class ModManager
    {
        internal Dictionary<string, IMod> ModList = new Dictionary<string, IMod>();
        internal  Dictionary<string, string> ModPath = new Dictionary<string, string>();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string TempPath = "temp/mods";
        public BlockRegister BlockRegister;
        public ItemRegister ItemRegister;
        public CommandRegister CommandRegister;
        public WorldGeneratorRegistry WorldGeneratorRegistry;
        public ModManager()
        {
            var VoxelPrototype = new  common.Game.VoxelPrototype();
            ModList.Add(VoxelPrototype.Name, VoxelPrototype) ;
            ModPath.Add(VoxelPrototype.Name, null);
            BlockRegister = new BlockRegister();
            ItemRegister = new ItemRegister();
            CommandRegister = new CommandRegister();
            WorldGeneratorRegistry = new WorldGeneratorRegistry();
        }

        internal void LoadMods()
        {
            CleanTempFolder();
            Logger.Info("The mod temp folder has been cleaned.");
            string[] Mods = Directory.GetFiles("mods", "*.modif");
            foreach (string filePath in Mods)
            {
                try
                {
                    using (ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Read))
                    {
                        zip.ExtractToDirectory(TempPath + "/" + Path.GetFileNameWithoutExtension(filePath));
                        string path = TempPath + "/" + Path.GetFileNameWithoutExtension(filePath);
                        Assembly ass = null;
                        ass = Assembly.LoadFrom(path + "/" + Path.GetFileNameWithoutExtension(filePath) + ".dll");
                        if (ass != null)
                        {
                            foreach (Type t in ass.GetTypes())
                            {
                                if (t.GetInterface("VoxelPrototype.api.IMod") != null)
                                {
                                    var temp = (IMod)Activator.CreateInstance(t);
                                    ModList.Add(t.Name, temp);
                                    ModPath.Add(t.Name, path);
                                    Logger.Info("Load a new mod: " + t.Name);
                                }
                                else
                                {
                                    RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "A mod failed to load.");
                }
            }
        }
        internal void PreInit()
        {
            foreach (IMod mod in ModList.Values)
            {
                try
                {
                    mod.PreInit(this);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "{Name} failed to preinit.", mod.Name);
                }
            }
            Logger.Info("All mods are preinitialized.");
        }
        internal  void Init()
        {
            foreach (IMod mod in ModList.Values)
            {
                try
                {
                    mod.Init(this);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "{Name} failed to init.", mod.Name);
                }
            }
            Logger.Info("All mods are initialized.");
        }
        public  void CleanTempFolder()
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
        public  void RecursiveDelete(DirectoryInfo baseDir, bool isRootDir)
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
    }
}
