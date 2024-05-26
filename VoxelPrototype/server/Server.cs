using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;

namespace VoxelPrototype.server
{
    public class Server : IRunnable
    {
        internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal string ServerName;
        internal int ServerPort = 23482;
        //Set if the server is running
        internal volatile bool Running = false;
        //
        internal ServerWorld World;

        public static Server TheServer;
        internal const int TICKS_PER_SECOND = 20;
        internal Timer ServerTimer;

        public Server()
        {
            if (TheServer == null)
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
