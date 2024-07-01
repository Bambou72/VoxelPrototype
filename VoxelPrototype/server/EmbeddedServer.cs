using System.Diagnostics;
using VoxelPrototype.client;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.World;

namespace VoxelPrototype.server
{
    internal class EmbeddedServer : Server
    {
        internal Thread ServerLocalThread;
        public EmbeddedServer(string path, WorldSettings Settings =null ) : base(path,Settings)
        {
            World.ModManager = Client.TheClient.ModManager;
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
