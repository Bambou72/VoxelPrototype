using LiteNetLib;
using VoxelPrototype.API.Commands;
using VoxelPrototype.server;

namespace VoxelPrototype.common.Base.Commands
{
    internal class TPS : ICommand
    {
        public string Name { get => "tps"; }
        public void Execute(string[] Arguments, NetPeer peer)
        {
           ServerChat.SendServerMessage($"TPS:{Server.TheServer.ServerTimer.GetTPS().ToString("0.00")}",peer);
        }
    }
}
