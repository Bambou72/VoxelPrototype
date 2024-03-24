/**
 * Server main process loop
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
/*
using System.Diagnostics;
using VoxelPrototype.common.API;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.RessourceManager;
using VoxelPrototype.server;
namespace VoxelPrototype.common
{
    public static class Server
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Stopwatch stopwatch = new Stopwatch();
        private static int TickCounter = 0;
        internal static RessourcePackManager RessourcePackManager = new();
        internal static string Title = @"
         __      __       _____                  _ _    _____                          
         \ \    / /      |  __ \                | (_)  / ____|                         
          \ \  / /____  _| |__) |__  _ __  _   _| |_  | (___   ___ _ ____   _____ _ __ 
           \ \/ / _ \ \/ /  ___/ _ \| '_ \| | | | | |  \___ \ / _ \ '__\ \ / / _ \ '__|
            \  / (_) >  <| |  | (_) | |_) | |_| | | |  ____) |  __/ |   \ V /  __/ |   
             \/ \___/_/\_\_|   \___/| .__/ \__,_|_|_| |_____/ \___|_|    \_/ \___|_|   
                                    | |                                                
                                    |_|                                                
        ";
        internal static string Author = @"
           ___           ____       __  __                ______             
          / _ )__ __    / __ \__ __/ /_/ /__ __    __    /_  __/__ ___ ___ _ 
         / _  / // /   / /_/ / // / __/ / _ `/ |/|/ /     / / / -_) _ `/  ' \
        /____/\_, /    \____/\_,_/\__/_/\_,_/|__,__/     /_/  \__/\_,_/_/_/_/
             /___/                                                           
        ";
        public static void Main(int port)
        {
            Console.SetWindowSize(100, 25);
            Console.WriteLine(Title);
            Console.WriteLine(Author);
            Logger.Info("The standalone server starts is initialization");
            /*
             _____      _ _   _       _ _           _____                                      
            |_   _|    (_| | (_)     | (_)         /  ___|                                     
              | | _ __  _| |_ _  __ _| |_ _______  \ `--.  ___  __ _ _   _  ___ _ __   ___ ___ 
              | || '_ \| | __| |/ _` | | |_  / _ \  `--. \/ _ \/ _` | | | |/ _ | '_ \ / __/ _ \
             _| || | | | | |_| | (_| | | |/ |  __/ /\__/ |  __| (_| | |_| |  __| | | | (_|  __/
             \___|_| |_|_|\__|_|\__,_|_|_/___\___| \____/ \___|\__, |\__,_|\___|_| |_|\___\___|
                                                                  | |                          
                                                                  |_|              
            *//*
            ModManager.LoadMods();
            Logger.Info("Mods have been Load");
            ModManager.Init();
            Logger.Info("Mods have been Initialize");
            RessourcePackManager.InitializeServer();
            Logger.Info("Ressources have been Initialize");
            ServerWorldManager.InitWorld("world/", File.Exists("world/world.vpw"), new WorldParameter { Description = "Just a world!" });
            Logger.Info("World have been Initialize");
            ServerNetwork.StartServer(port);
            Logger.Info("Network have been Initialized");
            Logger.Info("The server has finished initializing, it is now ready at: " + ServerNetwork.server.LocalPort);
            Logger.Info("Server engine version: " + Version.EngineVersion);
            Logger.Info("Server api version: " + Version.APIVersion);
            while (Console.ReadKey().Key != ConsoleKey.Delete)
            {
                ServerNetwork.Update();
                if (TickCounter < 20)
                {
                    stopwatch.Start();
                    UpdateTick();
                    TickCounter++;
                    stopwatch.Stop();
                }
                else
                {
                    TickCounter = 0;
                }
                double elapsed = stopwatch.ElapsedMilliseconds;
                if (elapsed < 50)
                {
                    Thread.Sleep(50 - (int)elapsed);
                }
                else
                {
                    Logger.Warn("Tick can't run at 20tps, current time for one tick :" + elapsed);
                }
                stopwatch.Reset();
            }
            ServerWorldManager.DeInitWorld();
        }
        public static void MainLocal(int port, string path, bool create, WorldParameter param)
        {
            RessourcePackManager.InitializeServer();
            
        }
        public static void UpdateTick()
        {
            ServerWorldManager.UpdateWorld();
        }
    }
}*/
