using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.common.Network.packets
{
    internal struct UnloadChunk : INetSerializable
    {
        public Vector2i[] Positions;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Positions);
        }
        public void Deserialize(NetDataReader reader)
        {
            Positions = reader.GetVector2iArray();
        }
    }
}
