using LiteNetLib.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct PlayerDeconnection : INetSerializable
    {
        public ushort ClientID;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientID);
        }
        public void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetUShort();
        }
    }
}
