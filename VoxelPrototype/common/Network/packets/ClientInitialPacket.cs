using LiteNetLib.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct ClientInitialPacket : INetSerializable
    {
        public string Name;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Name);
        }
        public void Deserialize(NetDataReader reader)
        {
            Name = reader.GetString();
        }
    }
}
