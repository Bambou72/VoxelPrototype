/**
 * Client Network Manager
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 * */
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using VoxelPrototype.game;
using VoxelPrototype.network.packets;
namespace VoxelPrototype.network
{
    /// <summary>
    /// The network manager for client
    /// </summary>
    public class ClientNetworkManager : NetworkManager
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetLogger("ClientNetworkManager");

        internal NetPeer Server;
        public ClientNetworkManager() : base()
        {
            NetManager.Start();
        }
        public void SendPacketToServer(Packet Packet, DeliveryMethod DeliveryMethod) 
        {
            SendPacket(Server, Packet, DeliveryMethod);
        }
        public void Connect(string ip, int port)
        {
            NetManager.Connect(ip, port, Message);
        }
        public void Deconnect()
        {
            NetManager.Stop();
        }
        public override void OnPeerConnected(NetPeer peer)
        {
            Server = peer;
            ClientInitialPacket packet = new ClientInitialPacket { Name = "Debug" };
            SendPacketToServer(packet, DeliveryMethod.ReliableOrdered);
        }
        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            /*
            client.Stop();
            Client.TheClient.World.Dispose();
            //GUIVar.MainMenu = true;
            Initialized = false;*/

        }
        public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Error("An network error occured : " + socketError + " from "+ endPoint.Address+":"+endPoint.Port);
        }
    }
}