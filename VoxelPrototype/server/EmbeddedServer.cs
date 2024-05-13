
using SharpFont;
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
            //Resources
            InitialSettings = Settings;
            ServerTimer = new Timer();
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
            ServerTimer.Init();
            float delta;
            float accumulator = 0f;
            float interval = 1f / TICKS_PER_SECOND;
            float alpha;
            while (Running)
            {
                delta = ServerTimer.GetDelta();
                accumulator += delta;
                ServerNetwork.Update();
                while (accumulator >= interval)
                {
                    ServerTimer.UpdateUPS();
                    World.Tick();
                    accumulator -= interval;
                }
                ServerTimer.Update();
                if (ServerTimer.GetTPS() <= 5)
                {
                    Logger.Warn("The server TPS <= 5");
                }
                else if (ServerTimer.GetTPS() <= 10)
                {
                    Logger.Warn("The server TPS <= 10");
                }
                else if (ServerTimer.GetTPS() <= 15)
                {
                    Logger.Warn("The server TPS <= 15");
                }
            }
            World.Dispose();
            ServerNetwork.StopServer();
            TheServer = null;
            Logger.Info("Server closed");
        }
    }
}
