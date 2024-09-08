using LiteNetLib;
using VoxelPrototype.network;
using VoxelPrototype.network.packets;
namespace VoxelPrototype.client.game
{
    internal class ClientChat
    {
        internal List<string> consoleHistory = new List<string>();
        const int maxHistorySize = 100;

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
           AddToConsoleHistory(data.Message);
        }
        internal void Dispose()
        {
            consoleHistory.Clear();
        }
        internal  void AddToConsoleHistory(string message)
        {
            consoleHistory.Add(message);
            // Maintain the history size
            if (consoleHistory.Count > maxHistorySize)
            {
                consoleHistory.RemoveAt(0);
            }
        }
    }
}
