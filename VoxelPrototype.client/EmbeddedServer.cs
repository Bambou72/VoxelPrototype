using VoxelPrototype.game;
using VoxelPrototype.server;

namespace VoxelPrototype.client
{
    internal class EmbeddedServer : Server
    {
        internal Thread ServerLocalThread;
        public EmbeddedServer(string path, WorldSettings Settings = null) : base(path, Settings)
        {
        }
        public void Start()
        {
            Running = true;
            ServerLocalThread = new(Run);
            ServerLocalThread.Name = "EmbeddedServer";

            ServerLocalThread.Start();
        }
    }
}
