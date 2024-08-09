using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using VoxelPrototype.utils;
namespace VoxelPrototype.network
{
    public class NetworkManager : INetEventListener
    {
        internal NetManager NetManager;
        internal Dictionary<Identifier, Action<NetPeer, NetDataReader>> PacketHandlers = new();
        internal NetDataWriter Message;
        public int NumberOfSendedPackets;
        public NetworkManager()
        {
            Message = new NetDataWriter();
            NetManager = new(this);
        }
        public void Update()
        {
            NetManager.PollEvents();
            NumberOfSendedPackets = 0;
        }
        public void RegisterHandler<T>(Action<NetPeer, T> handler) where T : Packet, new()
        {
            var packetID = new T().GetID();
            PacketHandlers[packetID] = (peer, reader) =>
            {
                T packet = new T();
                packet.Deserialize(reader);
                handler(peer, packet);
            };
        }
        public void RemoveRegisterHandler<T>() where T : Packet, new()
        {
            var packetID = new T().GetID();
            if( PacketHandlers.ContainsKey(packetID) )
            {
                PacketHandlers.Remove(packetID);
            }
        }
        public void ProcessReceivedData(NetPeer peer, NetPacketReader reader)
        {
            string packetID = reader.GetString();
            if (PacketHandlers.TryGetValue(Identifier.FromString(packetID), out var handler))
            {
                handler(peer, reader);
            }
            else
            {
                Console.WriteLine($"No handler registered for packet ID: {packetID}");
            }
            reader.Recycle();
        }
        public void SendPacket(NetPeer Peer,Packet Packet, DeliveryMethod DeliveryMethod) 
        {
            Message.Reset();
            Message.Put(Packet.GetID().ToString());
            Packet.Serialize(Message);
            Peer.Send(Message, DeliveryMethod);
        }
        public virtual void  OnConnectionRequest(ConnectionRequest request)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            ProcessReceivedData(peer, reader);
        }

        public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPeerConnected(NetPeer peer)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            throw new NotImplementedException();
        }
    }
}
