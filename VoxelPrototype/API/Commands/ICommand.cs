using LiteNetLib;
namespace VoxelPrototype.api.Commands
{
    public interface ICommand
    {
        string Name { get; }
        public void Execute(string[] Arguments, NetPeer peer);
    }
}
