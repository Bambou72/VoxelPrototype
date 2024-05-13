/**
 * Player implementation for client side
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 */
using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.Network.packets;
namespace VoxelPrototype.common.Game.Entities.Player.PlayerManager
{
    internal class ClientPlayerFactory
    {
        internal readonly Dictionary<ushort, Player> PlayerList = new Dictionary<ushort, Player>();
        internal Player LocalPlayer;
        internal bool LocalPlayerExist = false;
        internal const int BufSize = 512;
        internal InputState[] InputBuffer = new InputState[BufSize];
        internal SimulationState[] SimulationBuffer = new SimulationState[BufSize];
        internal SimulationState CurrentPlayerState;
        internal ulong CurrentTick;
        internal ClientPlayerFactory()
        {
            for (int i = 0; i < BufSize; i++)
            {
                InputBuffer[i] = new InputState();
                SimulationBuffer[i] = new SimulationState();
            }
        }
        internal void AddPlayer(ushort clientId, Vector3d position, bool Local)
        {
            Player temp = new Player(position, clientId, Local, true);
            if (Local)
            {
                LocalPlayer = temp;
                LocalPlayerExist = true;
            }
            PlayerList.Add(clientId, temp);
        }
        internal void Dispose()
        {
            PlayerList.Clear();
        }
        internal void Update()
        {
            if (LocalPlayerExist)
            {
                InputState input = LocalPlayer.GetInput(InputSystem.DT, InputSystem.Mouse, InputSystem.Keyboard);
                LocalPlayer.SendControl(input);
                LocalPlayer.UpdateClient(input);
                uint bufferindex = (uint)(CurrentTick % BufSize);
                InputBuffer[bufferindex] = input;
                SimulationBuffer[bufferindex] = new SimulationState { position = LocalPlayer.Position, currentTick = CurrentTick };
                CurrentTick++;
            }
        }
        internal void HandleSpawn(PlayerSpawn data, NetPeer peer)
        {
            AddPlayer(data.ClientID, data.Position, false);
        }
        internal void HandleLocalPlayer(PlayerSpawnLocal data, NetPeer peer)
        {
            AddPlayer(data.ClientID, data.Position, true);
        }
        internal void HandleData(PlayerData data, NetPeer peer)
        {
            if (PlayerList.TryGetValue(data.ClientID, out Player player))
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
                    LocalPlayer.Ghost = data.Ghost;
                    uint IndexBuffer = (uint)(data.ClientTick % BufSize);
                    Vector3d Error = SimulationBuffer[IndexBuffer].position - data.Position;
                    if (Error.LengthSquared > 0.001)
                    {
                        LocalPlayer.Position = data.Position;
                        for (ulong replaying_prediction_id = data.ClientTick + 1; replaying_prediction_id < CurrentTick; replaying_prediction_id++)
                        {
                            uint BufferIndex = (uint)(replaying_prediction_id % BufSize);
                            LocalPlayer.UpdateClient(InputBuffer[BufferIndex]);
                            SimulationBuffer[IndexBuffer] = new SimulationState { position = LocalPlayer.Position, currentTick = replaying_prediction_id };
                        }
                    }
                }
                player.Name = data.Name;
                player.UpdateCollider();
            }
        }
        internal void HandleDeco(PlayerDeconnection data, NetPeer peer)
        {
            PlayerList.Remove(data.ClientID);
        }
    }
}
