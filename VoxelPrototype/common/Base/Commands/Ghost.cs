using LiteNetLib;
using VoxelPrototype.API.Commands;
using VoxelPrototype.common.Game.Entities.Player;
using VoxelPrototype.server;
namespace VoxelPrototype.common.Base.Commands
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
                        if (Server.TheServer.GetWorld().PlayerFactory.List.TryGetValue((ushort)peer.Id, out Player play))
                        {
                            if (ghost)
                            {
                                play.Ghost = true;
                                ServerChat.SendServerMessage("Ghost on", peer);
                            }
                            else
                            {
                                play.Ghost = false;
                                ServerChat.SendServerMessage("Ghost off", peer);
                            }
                        }
                    }
                }
            }
        }
    }
}
