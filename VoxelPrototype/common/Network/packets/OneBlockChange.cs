using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.common.Network.packets
{
    internal struct OneBlockChange : INetSerializable
    {
        public Vector2i ChunkPos;
        public Vector3i BlockPos;
        public string BlockID;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ChunkPos);
            writer.Put(BlockPos);
            writer.Put(BlockID);
        }
        public void Deserialize(NetDataReader reader)
        {
            ChunkPos = reader.GetVector2i();
            BlockPos = reader.GetVector3i();
            BlockID = reader.GetString();
        }
    }
}
