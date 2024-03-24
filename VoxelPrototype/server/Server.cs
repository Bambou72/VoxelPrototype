using System.Diagnostics;
using VoxelPrototype.common.Game.World;
using VoxelPrototype.common.RessourceManager;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.server
{
    public class Server :IRunnable
    {
        internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal string ServerName;
        internal int ServerPort = 23482;
        //Set if the server is running
        internal volatile bool Running =false;
        //Ressources
        internal RessourcePackManager RessourcePackManager;
        //
        internal ServerWorld World;

        public static Server TheServer;
        internal Stopwatch Stopwatch = new Stopwatch();
        internal int TickCounter = 0;

        public Server()
        {
            if(TheServer == null)
            {
                TheServer = this;
            }
        }
        public ServerWorld GetWorld()
        {
            return World;
        }
        public virtual void Shutdown()
        {
            World.Dispose();
        }
        public virtual void Run()
        {

        }
    }
}
