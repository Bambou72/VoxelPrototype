using System.Diagnostics;
using VoxelPrototype.API;
using VoxelPrototype.client;
using VoxelPrototype.common.Game;
using VoxelPrototype.common.Network.server;

namespace VoxelPrototype.server
{
    internal class EmbeddedServer : Server
    {
        internal Thread ServerLocalThread;
        internal WorldSettings InitialSettings;
        internal string Path;
        public EmbeddedServer(WorldSettings Settings, string Path) : base()
        {
            //Ressources
            RessourcePackManager = ClientRessourcePackManager.RessourcePackManager;
            InitialSettings = Settings;
            this.Path = Path;
        }
        public override void Run()
        {
            Running = true;
            ServerLocalThread = new(() => this.ServerLoop(InitialSettings,Path));
            ServerLocalThread.Name = "EmbeddedServer";
            ServerLocalThread.Start();
            

        }
        public void Stop()
        {
            Running=false;

        }
        public bool IsRunning()
        {
            return Running;
        }
        public void ServerLoop(WorldSettings Settings,string Path)
        {
            World = new(Settings, Path);
            ServerNetwork.StartServer(ServerPort);
            Logger.Info("The server has finished initializing, it is now ready at: " + ServerNetwork.server.LocalPort);
            Logger.Info("Server engine version: " + EngineVersion.Version);
            Logger.Info("Server api version: " + APIVersion.Version);
            while (Running)
            {
                if (TickCounter < 20)
                {
                    Stopwatch.Start();
                    ServerNetwork.Update();
                    World.Tick();
                    TickCounter++;
                    Stopwatch.Stop();
                }
                else
                {
                    TickCounter = 0;
                }
                double elapsed = Stopwatch.ElapsedMilliseconds;
                if (elapsed < 50)
                {

                    Thread.Sleep(50 - (int)elapsed);
                }
                else
                {
                    Logger.Warn("Tick can't run at 20tps, current time for one tick :" + elapsed);
                }
                Stopwatch.Reset();
            }
            World.Dispose();
            ServerNetwork.StopServer();
            TheServer = null;
        }
    }
}
