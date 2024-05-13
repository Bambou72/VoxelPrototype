using LiteNetLib;
using VoxelPrototype.API.Commands;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Network.server;
namespace VoxelPrototype.server
{
    public static class ServerChat
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static void HandleMessage(ClientChatMessage data, NetPeer peer)
        {
            if (data.Message[0] == CommandRegister.commandPrefix)
            {
                CommandRegister.ExecuteCommand(data.Message, peer);
            }
            else
            {
                Logger.Info(Server.TheServer.World.PlayerFactory.List[(ushort)peer.Id] + ":" + data.Message);
                ServerChatMessage packet = new() { Message = Server.TheServer.World.PlayerFactory.List[(ushort)peer.Id].Name + ":" + data.Message };
                ServerNetwork.SendPacketToAll(packet, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendServerMessage(string message, NetPeer peer)
        {
            ServerChatMessage packet = new() { Message = "Server:" + message };
            ServerNetwork.SendPacket(packet, peer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendMessage(string message, NetPeer peer)
        {
            ServerChatMessage packet = new() { Message = message };
            ServerNetwork.SendPacket(packet, peer, DeliveryMethod.ReliableOrdered);
        }
    }
}
