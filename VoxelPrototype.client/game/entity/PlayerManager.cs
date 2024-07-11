/**
 * Player implementation for client side
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using SharpFont;
using VoxelPrototype.client;
using VoxelPrototype.network.packets;
namespace VoxelPrototype.client.game.entity
{
    internal class PlayerManager
    {
        internal readonly Dictionary<ushort, ClientPlayer> PlayerList = new Dictionary<ushort, ClientPlayer>();
        internal ClientPlayer LocalPlayer;
        internal bool LocalPlayerExist = false;
        internal const int BufSize = 512;
        internal InputState[] InputBuffer = new InputState[BufSize];
        internal SimulationState[] SimulationBuffer = new SimulationState[BufSize];
        internal SimulationState CurrentPlayerState;
        internal ulong CurrentTick;
        internal PlayerManager()
        {
            InitPackets();
            for (int i = 0; i < BufSize; i++)
            {
                InputBuffer[i] = new InputState();
                SimulationBuffer[i] = new SimulationState();
            }
        }
        internal void AddPlayer(ushort clientId, Vector3d position, bool Local)
        {
            ClientPlayer temp = new ClientPlayer(position, clientId, Local, true);
            if (Local)
            {
                LocalPlayer = temp;
                LocalPlayerExist = true;
            }
            PlayerList.Add(clientId, temp);
        }
        internal void InitPackets()
        {
            Client.TheClient.NetworkManager.RegisterHandler<PlayerData>(HandleData);
            Client.TheClient.NetworkManager.RegisterHandler<PlayerSpawn>(HandleSpawn);
            Client.TheClient.NetworkManager.RegisterHandler<PlayerSpawnLocal>(HandleLocalPlayer);
            Client.TheClient.NetworkManager.RegisterHandler<PlayerDisconnection>(HandleDisco);

        }
        internal void Dispose()
        {
            PlayerList.Clear();
        }
        internal void Update(float DT)
        {
            if (LocalPlayerExist)
            {
                InputState input = LocalPlayer.GetInput(DT);
                LocalPlayer.SendControl(input);
                LocalPlayer.Update(input, Client.TheClient.World);
                uint bufferindex = (uint)(CurrentTick % BufSize);
                InputBuffer[bufferindex] = input;
                SimulationBuffer[bufferindex] = new SimulationState { position = LocalPlayer.Position, currentTick = CurrentTick };
                CurrentTick++;
            }
        }
        internal void HandleSpawn(NetPeer peer, PlayerSpawn data)
        {
            AddPlayer(data.ClientID, data.Position, false);
        }
        internal void HandleLocalPlayer(NetPeer peer, PlayerSpawnLocal data)
        {
            AddPlayer(data.ClientID, data.Position, true);
        }
        internal void HandleData(NetPeer peer, PlayerData data)
        {
            if (PlayerList.TryGetValue(data.ClientID, out ClientPlayer player))
            {
                if (player != LocalPlayer)
                {
                    ulong tickDifference = data.ServerTick - player.LastServerTick;
                    double interpolationFactor = tickDifference / (double)2;
                    interpolationFactor = Math.Max(0.0, Math.Min(1.0, interpolationFactor));
                    player.Position = Vector3d.Lerp(player.Position, data.Position, interpolationFactor);
                    player.Rotation = Vector3.Lerp(player.Rotation, data.Rotation, (float)interpolationFactor);
                    player.LastServerTick = data.ServerTick;
                }
                else
                {
                    LocalPlayer.Fly = data.Fly;
                    LocalPlayer.NoClip = data.Ghost;
                    uint IndexBuffer = (uint)(data.ClientTick % BufSize);
                    Vector3d Error = SimulationBuffer[IndexBuffer].position - data.Position;
                    if (Error.LengthSquared > 0.001)
                    {
                        LocalPlayer.Position = data.Position;
                        for (ulong replaying_prediction_id = data.ClientTick + 1; replaying_prediction_id < CurrentTick; replaying_prediction_id++)
                        {
                            uint BufferIndex = (uint)(replaying_prediction_id % BufSize);
                            LocalPlayer.Update(InputBuffer[BufferIndex], Client.TheClient.World);
                            SimulationBuffer[IndexBuffer] = new SimulationState { position = LocalPlayer.Position, currentTick = replaying_prediction_id };
                        }
                    }
                }
                player.Name = data.Name;
                player.UpdateCollider();
            }
        }
        internal void HandleDisco(NetPeer peer, PlayerDisconnection data)
        {
            PlayerList.Remove(data.ClientID);
        }
    }
}
