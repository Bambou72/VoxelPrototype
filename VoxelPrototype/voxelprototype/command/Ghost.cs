using LiteNetLib;
using VoxelPrototype.api.command;
using VoxelPrototype.server;
using VoxelPrototype.server.game.entity;

namespace VoxelPrototype.voxelprototype.command
{
    internal class Ghost : ICommand
    {
        public string Name => "ghost";
        public void Execute(string[] Arguments, NetPeer peer)
        {

            if (Arguments.Length == 2)
            {
                if (bool.TryParse(Arguments[1], out bool ghost))
                {
                    if (Arguments[0] == "@me")
                    {
                        if (Server.TheServer.GetWorld().PlayerFactory.List.TryGetValue((ushort)peer.Id, out ServerPlayer play))
                        {
                            if (ghost)
                            {
                                play.NoClip = true;
                                Server.TheServer.World.Chat.SendServerMessage("Ghost on", peer);
                            }
                            else
                            {
                                play.NoClip = false;
                                Server.TheServer.World.Chat.SendServerMessage("Ghost off", peer);
                            }
                        }
                    }
                }
            }
        }
    }
}
