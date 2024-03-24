using LiteNetLib.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct ServerInitialPacket : INetSerializable
    {
        public string EngineVersion;
        public string APIVersion;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(EngineVersion); writer.Put(APIVersion);
        }
        public void Deserialize(NetDataReader reader)
        {
            EngineVersion = reader.GetString();
            APIVersion = reader.GetString();
        }
    }
}
