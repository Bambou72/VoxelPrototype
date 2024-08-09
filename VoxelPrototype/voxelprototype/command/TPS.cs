using LiteNetLib;
using VoxelPrototype.api.command;
using VoxelPrototype.server;

namespace VoxelPrototype.voxelprototype.command
{
    internal class TPS : ICommand
    {
        public string Name { get => "tps"; }
        public void Execute(string[] Arguments, NetPeer peer)
        {
            Server.TheServer.World.Chat.SendServerMessage($"TPS:{Server.TheServer.GetTPS().ToString("0.00")}", peer);
        }
    }
}
