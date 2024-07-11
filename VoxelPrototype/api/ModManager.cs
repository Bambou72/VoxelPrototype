using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Commands;
using VoxelPrototype.api.Items;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.game;
namespace VoxelPrototype.api
{
    public class ModManager
    {
        private static ModManager Instance;
        public static ModManager GetInstance()
        {
            return Instance;
        }
        public Dictionary<string, IModInitializer> ModList = new Dictionary<string, IModInitializer>();
        public Dictionary<string, string> ModPath = new Dictionary<string, string>();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string TempPath = "temp/mods";
        public ModManager()
        {
            if (Instance == null)
            {


                new BlockRegistry();
                new ItemRegistry();
                new CommandRegistry();
                new WorldGeneratorRegistry();
                var VoxelPrototype = new voxelprototype.VoxelPrototype();
                ModList.Add(VoxelPrototype.GetModName(), VoxelPrototype);
                ModPath.Add(VoxelPrototype.GetModName(), null);
                Instance = this;
            }
            else
            {
                throw new InvalidOperationException("You can't instanciate more than 1 instance of singleton");
            }
        }

        public void LoadMods()
        {
            CleanTempFolder();
            Logger.Info("The mod temp folder has been cleaned.");
            string[] Mods = Directory.GetFiles("mods", "*.zip");
            foreach (string filePath in Mods)
            {
                try
                {
                    using (ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Read))
                    {
                        string ExctractPath = TempPath + "/" + Path.GetFileNameWithoutExtension(filePath);
                        zip.ExtractToDirectory(ExctractPath);
                        Assembly ass = AssemblyLoadContext.Default.LoadFromAssemblyPath(ExctractPath + "/" + Path.GetFileNameWithoutExtension(filePath) + ".dll");
                        var modTypes = ass.GetTypes().Where(t => typeof(IModInitializer).IsAssignableFrom(t) && !t.IsInterface);
                        foreach (var modType in modTypes)
                        {
                            var ModInstance = (IModInitializer)Activator.CreateInstance(modType);
                            ModList.Add(ModInstance.GetModName(), ModInstance);
                            ModPath.Add(ModInstance.GetModName(), ExctractPath);
                            Logger.Info("Load a new mod: " + ModInstance.GetModName());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "A mod failed to load.");
                }
            }
        }
        public void PreInit()
        {
            foreach (IModInitializer mod in ModList.Values)
            {
                try
                {
                    mod.PreInit(this);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "{Name} failed to preinit.", mod.GetModName());
                }
            }
            Logger.Info("All mods are preinitialized.");
        }
        public void Init()
        {
            foreach (IModInitializer mod in ModList.Values)
            {
                try
                {
                    mod.Init(this);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "{Name} failed to init.", mod.GetModName());
                }
            }
            Logger.Info("All mods are initialized.");
            BlockRegistry.GetInstance().Finalize();
            CommandRegistry.GetInstance().Finalize();
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
        public void RecursiveDelete(DirectoryInfo baseDir, bool isRootDir)
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
