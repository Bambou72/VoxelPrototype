using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.api.block;
using VoxelPrototype.api.command;
using VoxelPrototype.server;

namespace VoxelPrototype.voxelprototype.command
{
    internal class Set : ICommand
    {
        public string Name => "set";
        public void Execute(string[] Arguments, NetPeer peer)
        {
            if (Arguments.Length == 4)
            {
                Vector3i BlockPos = new Vector3i(int.Parse(Arguments[0]), int.Parse(Arguments[1]), int.Parse(Arguments[2]));
                Server.TheServer.GetWorld().SetBlock(BlockPos.X, BlockPos.Y, BlockPos.Z, BlockRegistry.GetInstance().GetBlock(Arguments[3]).GetDefaultState());
            }
        }
    }
}
