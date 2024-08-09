using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.api.block.state;
using VoxelPrototype.network;
using VoxelPrototype.utils;

namespace VoxelPrototype.network.packets
{
    public class OneBlockChange : Packet
    {
        public Vector2i ChunkPos;
        public Vector3i BlockPos;
        public BlockState State;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(ChunkPos);
            writer.Put(BlockPos);
            writer.Put(VBFSerializer.Serialize(State.Serialize()));
        }
        public override void Deserialize(NetDataReader reader)
        {
            ChunkPos = reader.GetVector2i();
            BlockPos = reader.GetVector3i();
            State = new BlockState().Deserialize((VBFCompound)VBFSerializer.Deserialize(reader.GetRemainingBytes()));
        }
        public override Identifier GetID()
        {
            return new Identifier("OneBlockChange");
        }
    }
}
