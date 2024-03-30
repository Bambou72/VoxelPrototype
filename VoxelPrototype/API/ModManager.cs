using System.Reflection;
using System.Runtime.CompilerServices;
namespace VoxelPrototype.API
{
    internal static class ModManager
    {
        static Dictionary<string, IMod> ModList = new Dictionary<string, IMod>();
        internal static List<string> ModAssetFolder = new List<string>();
        static string BaseModFolder = "mods";
        static string BaseTempModFolder = "temp/mods";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static void LoadMods()
        {
            DirectoryInfo di = new DirectoryInfo(BaseTempModFolder);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                file.Delete();
            }
            DirectoryInfo[] subDirectories = di.GetDirectories();
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                subDirectory.Delete(true);
            }
            Logger.Info("The temp folder has been cleaned.");
            string[] Mods = Directory.GetFiles(BaseModFolder, "*.dll");
            foreach (string filePath in Mods)
            {
                try
                {
                    Assembly ass = null;
                    ass = Assembly.LoadFrom(filePath);
                    if (ass != null)
                    {
                        foreach (Type t in ass.GetTypes())
                        {
                            if (t.GetInterface("VoxelPrototype.common.API.IMod") != null)
                            {
                                ModList.Add(t.Namespace, (IMod)Activator.CreateInstance(t));
                                Logger.Info("Load a new mod: " + t.Name);
                            }
                            else
                            {
                                RuntimeHelpers.RunClassConstructor(t.TypeHandle);
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
    }
}
