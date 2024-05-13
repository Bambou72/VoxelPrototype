using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.Commands;
using VoxelPrototype.server;

namespace VoxelPrototype.common.Base.Commands
{
    internal class Set : ICommand
    {
        public string Name => "set";
        public void Execute(string[] Arguments, NetPeer peer)
        {
            if (Arguments.Length == 4)
            {
                Vector3i BlockPos = new Vector3i(int.Parse(Arguments[0]), int.Parse(Arguments[1]), int.Parse(Arguments[2]));
                Server.TheServer.GetWorld().SetBlock(BlockPos.X, BlockPos.Y, BlockPos.Z, BlockRegister.GetBlock(Arguments[3]).GetDefaultState());
            }
        }
    }
}
