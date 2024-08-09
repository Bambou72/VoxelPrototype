using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class UnloadChunk : Packet
    {
        public Vector2i[] Positions;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Positions);
        }
        public override  void Deserialize(NetDataReader reader)
        {
            Positions = reader.GetVector2iArray();
        }
        public override Identifier GetID()
        {
            return new Identifier("ChunkUnload");
        }
    }
}
