using LiteNetLib;
using LiteNetLib.Utils;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.network.packets;
using VoxelPrototype.server;
using VoxelPrototype.server.game.entity;

namespace VoxelPrototype.network
{
    /// <summary>
    /// The network manager for server
    /// </summary>
    public class ServerNetworkManager : NetworkManager
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ServerNetworkManager() : base()
        {
            InitPackets();
        }
        public void InitPackets()
        {
            RegisterHandler<ClientInitialPacket>(ClientInitialPacket);
        }
        internal void StartServer(int Port)
        {
            NetManager.Start(Port);
        }
        internal void Stop()
        {
            NetManager.Stop();
        }
        /// <summary>
        /// Send packet to all client
        /// </summary>
        /// <param name="packet">The packet</param>
        /// <param name="deliveryMethod">The DeliveryMethod</param>
        public void SendPacketToAll(Packet packet, DeliveryMethod deliveryMethod)
        {
            Message.Reset();
            Message.Put(packet.GetID().ToString());
            packet.Serialize(Message);
            NetManager.SendToAll(Message, deliveryMethod);
            NumberOfSendedPackets++;
        }
        /// <summary>
        /// Send a packet to all client exclude one
        /// </summary>
        /// <param name="packet">The packet</param>
        /// <param name="peer">The excluded peer</param>
        /// <param name="deliveryMethod">The DeliveryMethod</param>
        public void SendPacketToAllWithoutOnePeer(Packet packet, NetPeer peer, DeliveryMethod deliveryMethod)
        {
            Message.Reset();
            Message.Put(packet.GetID().ToString());
            packet.Serialize(Message);
            NetManager.SendToAll(Message, deliveryMethod, peer);
            NumberOfSendedPackets++;
        }
        internal static void ClientInitialPacket(NetPeer peer, ClientInitialPacket data)
        {
            if (Server.TheServer.World.PlayerFactory.List.TryGetValue((ushort)peer.Id, out ServerPlayer play))
            {
                play.Name = data.Name;
            }
        }
        public override void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
        }
        public override void OnPeerConnected(NetPeer peer)
        {
            Logger.Info("A player was connectected: {0}", peer.Address); // Show peer ip
            Server.TheServer.World.PlayerFactory.AddPlayer((ushort)peer.Id, peer);       // Send with reliability
        }
        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info("A player was deconnectected: {0} , for this reason {1}", peer.Address, disconnectInfo.Reason); // Show peer ip
            Server.TheServer.World.PlayerFactory.RemovePlayer((ushort)peer.Id);
        }
    }
}
