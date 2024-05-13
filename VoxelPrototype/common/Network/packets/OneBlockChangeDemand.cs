using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.API.Blocks.State;
using VoxelPrototype.VBF;

namespace VoxelPrototype.common.Network.packets
{
    internal struct OneBlockChangeDemand : INetSerializable
    {
        public Vector2i ChunkPos;
        public Vector3i BlockPos;
        public BlockState State;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ChunkPos);
            writer.Put(BlockPos);
            writer.Put( VBFSerializer.Serialize( State.Serialize()));
        }
        public void Deserialize(NetDataReader reader)
        {
            ChunkPos = reader.GetVector2i();
            BlockPos = reader.GetVector3i();
            State =  new BlockState().Deserialize( (VBFCompound)VBFSerializer.Deserialize(reader.GetRemainingBytes()));
        }
    }
}
