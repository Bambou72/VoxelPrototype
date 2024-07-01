using LiteNetLib;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.packets;
namespace VoxelPrototype.client
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
            Render.GUI.Console.AddToConsoleHistory(data.Message);
        }
    }
}
