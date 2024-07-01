using LiteNetLib;
using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.common.Network.packets
{
    public struct ChunkData : INetSerializable
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
            Data = reader.GetRemainingBytes();
        }
    }
}
