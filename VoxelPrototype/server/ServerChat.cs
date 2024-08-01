using LiteNetLib;
using VoxelPrototype.api.Commands;
using VoxelPrototype.network.packets;
namespace VoxelPrototype.server
{
    public  class ServerChat
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ServerChat()
        {
            Server.TheServer.NetworkManager.RegisterHandler<ClientChatMessage>(HandleMessage);

        }

        internal void HandleMessage( NetPeer peer, ClientChatMessage data)
        {
            if (data.Message[0] == CommandRegistry.GetInstance().commandPrefix)
            {
                CommandRegistry.GetInstance().ExecuteCommand(data.Message, peer);
            }
            else
            {
                Logger.Info(Server.TheServer.World.PlayerFactory.List[(ushort)peer.Id] + ":" + data.Message);
                ServerChatMessage packet = new() { Message = Server.TheServer.World.PlayerFactory.List[(ushort)peer.Id].Name + ":" + data.Message };
                Server.TheServer.NetworkManager.SendPacketToAll(packet, DeliveryMethod.ReliableOrdered);
            }
        }
        public void SendServerMessage(string message, NetPeer peer)
        {
            ServerChatMessage packet = new() { Message = "Server:" + message };
            Server.TheServer.NetworkManager.SendPacket(peer,packet,  DeliveryMethod.ReliableOrdered);
        }
        public void SendMessage(string message, NetPeer peer)
        {
            ServerChatMessage packet = new() { Message = message };
            Server.TheServer.NetworkManager.SendPacket(peer,packet, DeliveryMethod.ReliableOrdered);
        }
    }
}
