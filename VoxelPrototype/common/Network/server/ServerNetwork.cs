using LiteNetLib;
using LiteNetLib.Utils;
using VoxelPrototype.API;
using VoxelPrototype.common.Game.Entities.Player;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.server;
namespace VoxelPrototype.common.Network.server
{
    /// <summary>
    /// The network manager for server
    /// </summary>
    public static class ServerNetwork
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static NetDataWriter message;
        internal static NetManager server;
        internal static NetPacketProcessor PacketProcessor;
        internal static int NumberOfSendedPacket = 0;
        internal static void StartServer(int Port)
        {
            message = new NetDataWriter();
            PacketProcessor = new NetPacketProcessor();
            EventBasedNetListener listener = new EventBasedNetListener();
            server = new NetManager(listener) { DisconnectTimeout = 10000 };
            //
            //SYNC
            //
            //
            //API
            //
            PacketProcessor.RegisterNestedType<ServerInitialPacket>();
            PacketProcessor.RegisterNestedType<ClientInitialPacket>();
            PacketProcessor.SubscribeNetSerializable<ClientInitialPacket, NetPeer>(ClientInitialPacket);
            PacketProcessor.SubscribeNetSerializable<ClientChatMessage, NetPeer>(ServerChat.HandleMessage);
            //
            //World
            //
            //
            //ChunkManager
            //
            PacketProcessor.RegisterNestedType<OneBlockChange>();
            PacketProcessor.RegisterNestedType<ChunkData>();
            PacketProcessor.RegisterNestedType<UnloadChunk>();
            PacketProcessor.SubscribeNetSerializable<OneBlockChangeDemand, NetPeer>(
            Server.TheServer.World.ChunkManager.HandleBlockChange);
            //
            //Player
            //
            PacketProcessor.RegisterNestedType<PlayerData>();
            PacketProcessor.RegisterNestedType<PlayerSpawn>();
            PacketProcessor.RegisterNestedType<PlayerSpawnLocal>();
            PacketProcessor.RegisterNestedType<PlayerDeconnection>();
            PacketProcessor.SubscribeNetSerializable<PlayerControl, NetPeer>(Server.TheServer.World.PlayerFactory.HandleControl);
            listener.ConnectionRequestEvent += request =>
            {
                request.Accept();
            };
            listener.PeerConnectedEvent += peer =>
            {
                Logger.Info("A player was connectected: {0}", peer.Address); // Show peer ip
                Server.TheServer.World.PlayerFactory.AddPlayer((ushort)peer.Id, peer);       // Send with reliability
                ServerInitialPacket packet = new ServerInitialPacket
                {
                    EngineVersion = EngineVersion.Version.ToString(),
                    APIVersion = APIVersion.Version.ToString()
                };
                SendPacket(packet, peer, DeliveryMethod.ReliableOrdered);
            };
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, Nothing) =>
            {
                PacketProcessor.ReadAllPackets(dataReader, fromPeer);
                dataReader.Recycle();
            };
            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Logger.Info("A player was deconnectected: {0} , for this reason {1}", peer.Address, disconnectInfo.Reason); // Show peer ip
                Server.TheServer.World.PlayerFactory.RemovePlayer((ushort)peer.Id);
            };
            server.Start(Port);
        }
        internal static void StopServer()
        {
            server.Stop();
        }
        /// <summary>
        /// Send packet to a specific client
        /// </summary>
        /// <typeparam name="T">A struct who implement INetSerialize</typeparam>
        /// <param name="packet">The packet</param>
        /// <param name="peer">The peer</param>
        /// <param name="deliveryMethod">The DeliveryMethod</param>
        public static void SendPacket<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            if (peer != null)
            {
                message.Reset();
                PacketProcessor.WriteNetSerializable(message, ref packet);
                peer.Send(message, deliveryMethod);
                NumberOfSendedPacket++;
            }
        }
        /// <summary>
        /// Send packet to all client
        /// </summary>
        /// <typeparam name="T">A struct who implement INetSerialize</typeparam>
        /// <param name="packet">The packet</param>
        /// <param name="deliveryMethod">The DeliveryMethod</param>
        public static void SendPacketToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            message.Reset();
            PacketProcessor.WriteNetSerializable(message, ref packet);
            server.SendToAll(message, deliveryMethod);
            NumberOfSendedPacket++;
        }
        /// <summary>
        /// Send a packet to all client exclude one
        /// </summary>
        /// <typeparam name="T">A struct who implement INetSerialize</typeparam>
        /// <param name="packet">The packet</param>
        /// <param name="peer">The excluded peer</param>
        /// <param name="deliveryMethod">The DeliveryMethod</param>
        public static void SendPacketToAllWithoutOnePeer<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            message.Reset();
            PacketProcessor.WriteNetSerializable(message, ref packet);
            server.SendToAll(message, deliveryMethod, peer);
            NumberOfSendedPacket++;
        }
        internal static void ClientInitialPacket(ClientInitialPacket data, NetPeer peer)
        {
            if (Server.TheServer.World.PlayerFactory.List.TryGetValue((ushort)peer.Id, out Player play))
            {
                play.Name = data.Name;
            }
        }
        internal static void Update()
        {
            NumberOfSendedPacket = 0;
            server.PollEvents();
        }
    }
}
