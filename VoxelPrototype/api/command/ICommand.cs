using LiteNetLib;
namespace VoxelPrototype.api.command
{
    public interface ICommand
    {
        string Name { get; }
        public void Execute(string[] Arguments, NetPeer peer);
    }
}
