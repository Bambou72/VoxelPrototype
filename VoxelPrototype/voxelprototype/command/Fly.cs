using LiteNetLib;
using VoxelPrototype.api.command;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.server;
using VoxelPrototype.server.game.entity;

namespace VoxelPrototype.voxelprototype.command
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
                        if (Server.TheServer.GetWorld().PlayerFactory.List.TryGetValue((ushort)peer.Id, out ServerPlayer play))
                        {
                            if (fly)
                            {
                                play.Fly = true;
                                Server.TheServer.World.Chat.SendServerMessage("Fly on", peer);
                            }
                            else
                            {
                                play.Fly = false;
                                Server.TheServer.World.Chat.SendServerMessage("Fly off", peer);
                            }
                        }

                    }
                }
            }
        }
    }
}
