using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.network.packets;

namespace VoxelPrototype.server.game.entity
{
    public class PlayerManager
    {
        public Dictionary<ushort, ServerPlayer> List = new Dictionary<ushort, ServerPlayer>();

        public PlayerManager()
        {
            InitPackets();
        }
        internal void InitPackets()
        {
            Server.TheServer.NetworkManager.RegisterHandler<PlayerControl>(HandleControl);
        }
        internal void Dispose()
        {
            List.Clear();
        }
        internal void AddPlayer(ushort clientId, NetPeer peer)
        {
            ServerPlayer temp = new ServerPlayer(new Vector3d(0.5, Server.TheServer.World.WorldGenerator.GetOriginHeight() + 2, 0.5), clientId);
            foreach (Player otherPlayer in List.Values)
            {
                if (otherPlayer.ClientID != peer.Id)
                {
                    PlayerSpawn packet = new PlayerSpawn { ClientID = otherPlayer.ClientID, Position = otherPlayer.Position };
                    Server.TheServer.NetworkManager.SendPacket(peer, packet, DeliveryMethod.ReliableOrdered);
                }
            }
            List.Add(clientId, temp);
            PlayerSpawnLocal packets = new PlayerSpawnLocal { ClientID = temp.ClientID, Position = temp.Position };
            Server.TheServer.NetworkManager.SendPacket(peer, packets, DeliveryMethod.ReliableOrdered);
            PlayerSpawn packety = new PlayerSpawn { ClientID = temp.ClientID, Position = temp.Position };
            Server.TheServer.NetworkManager.SendPacketToAllWithoutOnePeer(packety, peer, DeliveryMethod.ReliableOrdered);
        }
        internal void RemovePlayer(ushort clientId)
        {
            PlayerDisconnection packet = new PlayerDisconnection { ClientID = clientId };
            Server.TheServer.NetworkManager.SendPacketToAll(packet, DeliveryMethod.ReliableOrdered);
            List.Remove(clientId);
        }
        internal void Update()
        {
            foreach (ServerPlayer player in List.Values)
            {
                while (player.Controls.Count > 0)
                {
                    PlayerControlsServer current = player.Controls.Dequeue();
                    player.UpdateServer(current, Server.TheServer.World);
                }
            }
        }
        internal void SendData()
        {
            foreach (ServerPlayer player in List.Values)
            {
                player.SendData();
            }
        }
        internal void HandleControl(NetPeer peer, PlayerControl data)
        {
            if (List.TryGetValue((ushort)peer.Id, out ServerPlayer player))
            {
                PlayerControlsServer control = new()
                {
                    forward = data.Forward,
                    backward = data.Backward,
                    right = data.Right,
                    left = data.Left,
                    space = data.Space,
                    shift = data.Shift,
                    control = data.Control,
                    Rotation = data.Rotation,
                    Front = data.Front,
                    Right = data.CRight,
                    Dt = data.dt,
                    ClientTick = data.ClientTick,
                };
                player.Controls.Enqueue(control);
            }
        }
    }
}
