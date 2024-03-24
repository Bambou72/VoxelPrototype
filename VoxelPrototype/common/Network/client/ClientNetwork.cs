/**
 * Client Network Manager
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 * */
using LiteNetLib;
using LiteNetLib.Utils;
using VoxelPrototype.client;
using VoxelPrototype.client.GUI;
using VoxelPrototype.common.Chat;
using VoxelPrototype.common.Network.packets;
namespace VoxelPrototype.common.Network.client
{
    /// <summary>
    /// The network manager for client
    /// </summary>
    public static class ClientNetwork
    {
        static bool Initialized = false;
        static EventBasedNetListener listener;
        internal static NetManager client;
        internal static NetDataWriter message;
        private static NetPacketProcessor packetProcessor;
        internal static NetPeer Server;
        internal static string ServerAPIVersion = "NotConnected";
        internal static string ServerEngineVersion = "NotConnected";
        internal static void Init()
        {
            message = new NetDataWriter();
            packetProcessor = new NetPacketProcessor();
            listener = new EventBasedNetListener();
            client = new(listener);
            client.DisconnectTimeout = 10000;
            //
            //Network
            //
            //
            //API
            //
            packetProcessor.RegisterNestedType<ServerInitialPacket>();
            packetProcessor.RegisterNestedType<ClientChatMessage>();
            packetProcessor.RegisterNestedType<ClientInitialPacket>();
            packetProcessor.SubscribeNetSerializable<ServerInitialPacket>(HandleInitialPacket);
            packetProcessor.SubscribeNetSerializable<ServerChatMessage, NetPeer>(ClientChat.HandleMessage);
            listener.PeerDisconnectedEvent += (server, DisconnectInfo) =>
            {
                client.Stop();
                Client.TheClient.World.Dispose();
                GUIVar.MainMenu = true;
                Initialized = false;
            };
            listener.PeerConnectedEvent += (server) =>
            {
                Client.TheClient.World.Init();
                Server = server;
                //
                //World
                //
                //
                //ChunkManager
                //
                packetProcessor.RegisterNestedType<OneBlockChangeDemand>();
                packetProcessor.SubscribeNetSerializable<ChunkData, NetPeer>(Client.TheClient.World.ChunkManager.HandleChunk);
                packetProcessor.SubscribeNetSerializable<OneBlockChange, NetPeer>(Client.TheClient.World.ChunkManager.HandleChunkUpdate);
                packetProcessor.SubscribeNetSerializable<UnloadChunk, NetPeer>(Client.TheClient.World.ChunkManager.HandleChunkUnload);
                //
                //Player
                //
                packetProcessor.RegisterNestedType<PlayerControl>();
                packetProcessor.SubscribeNetSerializable<PlayerData, NetPeer>(Client.TheClient.World.PlayerFactory.HandleData);
                packetProcessor.SubscribeNetSerializable<PlayerSpawn, NetPeer>(Client.TheClient.World.PlayerFactory.HandleSpawn);
                packetProcessor.SubscribeNetSerializable<PlayerSpawnLocal, NetPeer>(Client.TheClient.World.PlayerFactory.HandleLocalPlayer);
                packetProcessor.SubscribeNetSerializable<PlayerDeconnection, NetPeer>(Client.TheClient.World.PlayerFactory.HandleDeco);
                ClientInitialPacket packet = new ClientInitialPacket { Name = "Debug" };
                SendPacket(packet, DeliveryMethod.ReliableOrdered);
            };
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, Nothing) =>
            {
                packetProcessor.ReadAllPackets(dataReader, fromPeer);
                dataReader.Recycle();
            };
            client.Start();
            Initialized = true;
        }
        /// <summary>
        /// Send a packet to the server
        /// </summary>
        /// <typeparam name="T">A struct who implement INetSerialize</typeparam>
        /// <param name="packet">The packet variable</param>
        /// <param name="deliveryMethod">DeliveryMethod</param>
        public static void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            if (Server != null)
            {
                message.Reset();
                packetProcessor.WriteNetSerializable(message, ref packet);
                Server.Send(message, deliveryMethod);
            }
        }
        internal static void HandleInitialPacket(ServerInitialPacket data)
        {
            ServerEngineVersion = data.EngineVersion;
            ServerAPIVersion = data.APIVersion;
        }
        internal static void Connect(string ip, int port)
        {
            Init();
            client.Connect(ip, port, message);
        }
        internal static void Deconnect()
        {
            client.Stop();
            Client.TheClient.DeInitWorld();
            GUIVar.MainMenu = true;
            Initialized = false;
        }
        internal static void Update()
        {
            if (Initialized)
            {
                client.PollEvents();
            }
        }
    }
}