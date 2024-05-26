using LiteNetLib.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct ServerInitialPacket : INetSerializable
    {
        public string EngineVersion;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(EngineVersion); 
        }
        public void Deserialize(NetDataReader reader)
        {
            EngineVersion = reader.GetString();
        }
    }
}
