using LiteNetLib;
using VoxelPrototype.network;
using VoxelPrototype.network.packets;
namespace VoxelPrototype.client.game
{
    internal class ClientChat
    {
        internal ClientChat()
        {
            Client.TheClient.NetworkManager.RegisterHandler<ServerChatMessage>(HandleMessage);
        }
        internal void SendMessage(string message)
        {
            ClientChatMessage packet = new ClientChatMessage() { Message = message };
            Client.TheClient.NetworkManager.SendPacketToServer(packet, DeliveryMethod.ReliableOrdered);
        }
        internal void HandleMessage(NetPeer peer, ServerChatMessage data)
        {
            //ui.Console.AddToConsoleHistory(data.Message);
        }
        internal void Dispose()
        {
            //ui.Console.ClearConsoleHistory();
        }
    }
}
