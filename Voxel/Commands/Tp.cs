using LiteNetLib;
using VoxelPrototype.API.Commands;

namespace Voxel.Commands
{
    internal class Tp : ICommand
    {
        public string Name { get => "tp"; }
        public void Execute(string[] Arguments, NetPeer peer)
        {
            if (Arguments.Length == 2)
            {
                if (float.TryParse(Arguments[0], out float posx) && float.TryParse(Arguments[1], out float posy) && float.TryParse(Arguments[2], out float posz))
                {
                    /*
                    ServerChat.SendServerMessage("You had been teleported to " + posx + " " + posy + " " + posz, peer);
                    if (ServerWorldManager.world.PlayerFactory.List.TryGetValue((ushort)peer.Id, out PlayerServer play))
                    {
                        player.Position  new Vector3(posx, posy, posz);
                        play.Position =;
                    }*/
                }
            }
        }
    }
}
