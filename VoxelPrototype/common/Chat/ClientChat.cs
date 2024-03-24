using LiteNetLib;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.packets;
namespace VoxelPrototype.common.Chat
{
    internal static class ClientChat
    {
        internal static void SendMessage(string message)
        {
            ClientChatMessage packet = new ClientChatMessage() { Message = message };
            ClientNetwork.SendPacket(packet, DeliveryMethod.ReliableOrdered);
        }
        internal static void HandleMessage(ServerChatMessage data, NetPeer peer)
        {
            client.GUI.Console.AddToConsoleHistory(data.Message);
        }
    }
}
