using LiteNetLib.Utils;
using LiteNetLib;
using OpenTK.Mathematics;
using VoxelPrototype.common.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct ChunkData : INetSerializable
    {
        public int importance;
        public Vector2i Pos;
        public byte[] Data;
        public NetPeer peer;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Pos);
            writer.Put(importance);
            writer.Put(Data);
        }
        public void Deserialize(NetDataReader reader)
        {
            Pos = reader.GetVector2i();
            importance = reader.GetInt();
            Data =reader.GetRemainingBytes();
        }
    }
}
