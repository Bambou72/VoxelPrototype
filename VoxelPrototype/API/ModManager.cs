using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using VoxelPrototype.common.Base;
namespace VoxelPrototype.API
{
    internal static class ModManager
    {
        static Dictionary<string, IMod> ModList = new Dictionary<string, IMod>();
        static Dictionary<string,string> ModPath = new Dictionary<string,string>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static string TempPath = "temp/mods";
        internal static void LoadMods()
        {
            ModList.Add("Base", new Base());
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
                        ass = Assembly.LoadFrom(path+"/"+Path.GetFileNameWithoutExtension(filePath)+".dll");
                        if (ass != null)
                        {
                            foreach (Type t in ass.GetTypes())
                            {
                                if (t.GetInterface("VoxelPrototype.API.IMod") != null)
                                {
                                    ModList.Add(t.Namespace, (IMod)Activator.CreateInstance(t));
                                    ModPath.Add(t.Namespace,path);
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
        internal static void Init()
        {
            foreach (IMod mod in ModList.Values)
            {
                try
                {
                    mod.Init();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "{Name} failed to load.", mod.Name);
                }
            }
            Logger.Info("All mods are initialized.");
        }
        public static void CleanTempFolder()
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
    }
}
