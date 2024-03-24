using LiteNetLib.Utils;
namespace VoxelPrototype.common.Network.packets
{
    internal struct ClientChatMessage : INetSerializable
    {
        public string Message;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Message);
        }
        public void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
        }
    }
}
