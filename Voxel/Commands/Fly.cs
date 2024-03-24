using LiteNetLib;
using VoxelPrototype.common.API.Commands;
using VoxelPrototype.common.Chat;
using VoxelPrototype.common.Game.Entities.Player;
using VoxelPrototype.server;
namespace Voxel.Commands
{
    internal class Fly : ICommand
    {
        public string Name => "fly";
        public void Execute(string[] Arguments, NetPeer peer)
        {
            if (Arguments.Length == 2)
            {
                if (bool.TryParse(Arguments[1], out bool fly))
                {
                    if (Arguments[0] == "@me")
                    {
                        if (Server.TheServer.GetWorld().PlayerFactory.List.TryGetValue((ushort)peer.Id, out Player play))
                        {
                            if (fly)
                            {
                                play.Fly = true;
                                ServerChat.SendServerMessage("Fly on", peer);
                            }
                            else
                            {
                                play.Fly = false;
                                ServerChat.SendServerMessage("Fly off", peer);
                            }
                        }

                    }
                }
            }
        }
    }
}
