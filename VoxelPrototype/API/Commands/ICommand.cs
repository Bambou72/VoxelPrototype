using LiteNetLib;
namespace VoxelPrototype.API.Commands
{
    public interface ICommand
    {
        string Name { get; }
        public void Execute(string[] Arguments, NetPeer peer);
    }
}
