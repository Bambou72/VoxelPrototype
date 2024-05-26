/**
 * Player implementation for server side
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.common.Entities.Player;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Network.server;
namespace VoxelPrototype.common.Entities.Player.PlayerManager
{
    public class ServerPlayerFactory
    {
        public Dictionary<ushort, Player> List = new Dictionary<ushort, Player>();
        internal void Dispose()
        {
            List.Clear();
        }
        internal void AddPlayer(ushort clientId, NetPeer peer)
        {
            Player temp = new Player(new Vector3d(0.5, server.Server.TheServer.World.WorldGenerator.GetOriginHeight() + 2, 0.5), clientId);
            foreach (Player otherPlayer in List.Values)
            {
                if (otherPlayer.ClientID != peer.Id)
                {
                    PlayerSpawn packet = new PlayerSpawn { ClientID = otherPlayer.ClientID, Position = otherPlayer.Position };
                    ServerNetwork.SendPacket(packet, peer, DeliveryMethod.ReliableOrdered);
                }
            }
            List.Add(clientId, temp);
            PlayerSpawnLocal packets = new PlayerSpawnLocal { ClientID = temp.ClientID, Position = temp.Position };
            ServerNetwork.SendPacket(packets, peer, DeliveryMethod.ReliableOrdered);
            PlayerSpawn packety = new PlayerSpawn { ClientID = temp.ClientID, Position = temp.Position };
            ServerNetwork.SendPacketToAllWithoutOnePeer(packety, peer, DeliveryMethod.ReliableOrdered);
        }
        internal void RemovePlayer(ushort clientId)
        {
            PlayerDeconnection packet = new PlayerDeconnection { ClientID = clientId };
            ServerNetwork.SendPacketToAll(packet, DeliveryMethod.ReliableOrdered);
            List.Remove(clientId);
        }
        internal void Update()
        {
            foreach (Player player in List.Values)
            {
                player.Update();
            }
        }
        internal void SendData()
        {
            foreach (Player player in List.Values)
            {
                player.SendData();
            }
        }
        internal void HandleControl(PlayerControl data, NetPeer peer)
        {
            if (List.TryGetValue((ushort)peer.Id, out Player player))
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
